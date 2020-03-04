using System;
using System.IO;
using co.lujun.funcanalyzer.attribute;
using co.lujun.funcanalyzer.handler;
using co.lujun.funcanalyzer.imodule;
using Mono.Cecil;
using Mono.Cecil.Cil;
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

        private const string ExecuteInjectInjectFlagMethod = "co_lujun_funcanalyzer_ExecuteInjectFlag";

        public void Inject(string assemblyPath, bool enable, Action<float, string> callback)
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

                MethodDefinition injectFlagMethodDefinition = null;
                for (int j = 0; j < methods.Count; j++)
                {
                    if (methods[j].Name.Equals(ExecuteInjectInjectFlagMethod))
                    {
                        injectFlagMethodDefinition = methods[j];
                        break;
                    }
                }

                // This type has already injected analysis code, just update enable state
                if (injectFlagMethodDefinition != null)
                {
                    SetFlagMethod(injectFlagMethodDefinition, enable);
                }
                else
                {
                    // Inject flag method for this type first
                    injectFlagMethodDefinition = InjectFlagMethod(moduleDefinition, typeDefinition, enable);

                    // Inject analysis code for specify methods
                    for (int j = 0; j < methods.Count; j++)
                    {
                        AnalyzeFunc(moduleDefinition, methods[j], injectFlagMethodDefinition);
                    }
                }
            }

            callback(.8f, "Start write assembly...");

            assemblyDefinition.Write(assemblyPath, new WriterParameters(){ WriteSymbols = true });
            assemblyDefinition.Dispose();

            callback(1.0f, "Analysis code injected!");
        }

        private MethodDefinition InjectFlagMethod(ModuleDefinition moduleDefinition, TypeDefinition typeDefinition,
            bool enable)
        {
            // New method
            MethodDefinition injectFlgMethodDefinition = new MethodDefinition(ExecuteInjectInjectFlagMethod,
                MethodAttributes.Private | MethodAttributes.HideBySig, moduleDefinition.TypeSystem.Boolean);
            typeDefinition.Methods.Add(injectFlgMethodDefinition);

            // Add local variable with bool type
            VariableDefinition boolVariableDefinition =
                new VariableDefinition(moduleDefinition.ImportReference(typeof(bool)));
            injectFlgMethodDefinition.Body.Variables.Add(boolVariableDefinition);

            // Set return value code
            SetFlagMethod(injectFlgMethodDefinition, enable);

            return injectFlgMethodDefinition;
        }

        private void SetFlagMethod(MethodDefinition methodDefinition, bool enable)
        {
            ILProcessor ilProcessor = methodDefinition.Body.GetILProcessor();
            methodDefinition.Body.Instructions.Clear();

            ilProcessor.Emit(OpCodes.Ldc_I4, enable ? 1 : 0);
            ilProcessor.Emit(OpCodes.Stloc, 0);
            ilProcessor.Emit(OpCodes.Ldloc, 0);
            ilProcessor.Emit(OpCodes.Ret);
        }

        private void AnalyzeFunc(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition,
            MethodDefinition injectFlagMethodDefinition)
        {
            string analyzeAttrName = typeof(AnalyzeAttribute).FullName;
            bool needAnalyze = false;
            Flags flags = Flags.Default;

            for (int i = 0; i < methodDefinition.CustomAttributes.Count; i++)
            {
                CustomAttribute attribute = methodDefinition.CustomAttributes[i];

                if (attribute.Constructor.DeclaringType.FullName.Equals(analyzeAttrName))
                {
                    needAnalyze = true;
                    AnalyzeAttr<Flags>(attribute, "AnalyzingFlags", ref flags);
                    break;
                }
            }

            if (needAnalyze)
            {
                InjectILCode(moduleDefinition, methodDefinition, injectFlagMethodDefinition, flags);
            }
        }

        private void InjectILCode(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition,
            MethodDefinition injectFlagMethodDefinition, Flags flags)
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
            funcDataHandler?.Inject(moduleDefinition, methodDefinition, injectFlagMethodDefinition, flags);
            runTimeDataHandler?.Inject(moduleDefinition, methodDefinition, injectFlagMethodDefinition, flags);
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


