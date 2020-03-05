using System;
using System.IO;
using co.lujun.funcanalyzer.attribute;
using co.lujun.funcanalyzer.handler;
using co.lujun.funcanalyzer.imodule;
using Mono.Cecil;
using Mono.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace co.lujun.funcanalyzer
{
    public class Analyzer
    {
        private static Analyzer _instance;
        public static Analyzer Instance
        {
            get
            {
                _instance = _instance ?? new Analyzer();
                return _instance;
            }
        }

        private Analyzer()
        {
        }

        private const string InjectInjectFlagMethod = "co_lujun_funcanalyzer_InjectFlag";

        public void Inject(string assemblyPath, Action<float, string> callback)
        {
            callback(.0f, "Start load assembly...");

            if (Application.isPlaying || EditorApplication.isCompiling)
            {
                string msg = "Application or editor application is busy...";

                callback(1.0f, msg);
                Debug.Log(msg);
                return;
            }

            if (!File.Exists(assemblyPath))
            {
                string msg = string.Format("Can not load file with specify path: {0}", assemblyPath);

                callback(1.0f, msg);
                Debug.LogWarning(msg);
                return;
            }

            ReaderParameters readerParameters = new ReaderParameters(){ ReadSymbols = true };
            AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyPath, readerParameters);

            if (assemblyDefinition == null)
            {
                string msg = string.Format("Can not load assembly from specify path: {0}", assemblyPath);

                callback(1.0f, msg);
                Debug.LogWarning(msg);
                return;
            }

            callback(.3f, "Start inject...");
            ModuleDefinition moduleDefinition = assemblyDefinition.MainModule;

            for (int i = 0; i < moduleDefinition.Types.Count; i++)
            {
                TypeDefinition typeDefinition = moduleDefinition.Types[i];
                Collection<MethodDefinition> methods = typeDefinition.Methods;

                bool injectFlagMethodExist = false;
                for (int j = 0; j < methods.Count; j++)
                {
                    if (methods[j].Name.Equals(InjectInjectFlagMethod))
                    {
                        injectFlagMethodExist = true;
                        break;
                    }
                }

                // This type has already injected analysis code, continue
                if (injectFlagMethodExist)
                {
                    continue;
                }

                // Inject flag method for this type first
                InjectFlagMethod(moduleDefinition, typeDefinition);

                // Inject analysis code for specify methods
                for (int j = 0; j < methods.Count; j++)
                {
                    AnalyzeFunc(moduleDefinition, methods[j]);
                }
            }

            callback(.8f, "Start write assembly...");

            assemblyDefinition.Write(assemblyPath, new WriterParameters(){ WriteSymbols = true });
            assemblyDefinition.Dispose();

            callback(1.0f, "Analysis code injected!");
        }

        private void InjectFlagMethod(ModuleDefinition moduleDefinition, TypeDefinition typeDefinition)
        {
            // New method
            MethodDefinition injectFlgMethodDefinition = new MethodDefinition(InjectInjectFlagMethod,
                MethodAttributes.Private | MethodAttributes.HideBySig, moduleDefinition.TypeSystem.Void);
            typeDefinition.Methods.Add(injectFlgMethodDefinition);
        }

        private void AnalyzeFunc(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition)
        {
            string analyzeAttrName = typeof(AnalyzeAttribute).FullName;
            bool needAnalyze = false;
            bool enable = true;
            Flags flags = Flags.Default;

            for (int i = 0; i < methodDefinition.CustomAttributes.Count; i++)
            {
                CustomAttribute attribute = methodDefinition.CustomAttributes[i];

                if (attribute.Constructor.DeclaringType.FullName.Equals(analyzeAttrName))
                {
                    needAnalyze = true;
                    AnalyzeAttr<Flags>(attribute, "AnalyzingFlags", ref flags);
                    AnalyzeAttr<bool>(attribute, "Enable", ref enable);
                    break;
                }
            }

            if (needAnalyze)
            {
                InjectILCode(moduleDefinition, methodDefinition, enable, flags);
            }
        }

        private void InjectILCode(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition, bool enable,
            Flags flags)
        {
            IHandler funcDataHandler = null;
            IHandler runTimeDataHandler = null;

            if ((flags & Flags.Default) != 0)
            {
                flags = Flags.Default | Flags.Args | Flags.Ret | Flags.Time | Flags.Memory;
                funcDataHandler = new FuncDataHandler();
                runTimeDataHandler = new RuntimeDataHandler();
            }
            else
            {
                if((flags & Flags.Args) != 0 || (flags & Flags.Ret) != 0)
                {
                    funcDataHandler = new FuncDataHandler();
                }

                if((flags & Flags.Time) != 0 || (flags & Flags.Memory) != 0)
                {
                    runTimeDataHandler = new RuntimeDataHandler();
                }
            }

            // inject handler
            funcDataHandler?.Inject(moduleDefinition, methodDefinition, enable, flags);
            runTimeDataHandler?.Inject(moduleDefinition, methodDefinition, enable, flags);
        }

        private void AnalyzeAttr<T>(CustomAttribute attribute, string argName, ref T t)
        {
            for (int i = 0; i < attribute.Properties.Count; i++)
            {
                CustomAttributeNamedArgument attributeNamedArgument = attribute.Properties[i];

                if (attributeNamedArgument.Name.Equals(argName))
                {
                    t = (T) attributeNamedArgument.Argument.Value;
                    break;
                }
            }
        }
    }
}


