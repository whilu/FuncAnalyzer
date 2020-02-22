using co.lujun.funcanalyzer.attribute;
using co.lujun.funcanalyzer.imodule;
using co.lujun.funcanalyzer.module;
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

        public void Inject(string assemblyPath)
        {
            if (Application.isPlaying || EditorApplication.isCompiling)
            {
                Debug.Log("Application or editor application is busy...");
                return;
            }

            ReaderParameters readerParameters = new ReaderParameters(){ ReadSymbols = true };
            AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyPath, readerParameters);

            if (assemblyDefinition == null)
            {
                Debug.LogFormat("Can not load assembly from specify path: {0}", assemblyPath);
                return;
            }

            ModuleDefinition moduleDefinition = assemblyDefinition.MainModule;

            for (int i = 0; i < moduleDefinition.Types.Count; i++)
            {
                Collection<MethodDefinition> methods = moduleDefinition.Types[i].Methods;

                for (int j = 0; j < methods.Count; j++)
                {
                    AnalyzeFunc(moduleDefinition, methods[j]);
                }
            }

            assemblyDefinition.Write(assemblyPath, new WriterParameters(){ WriteSymbols = true });
            assemblyDefinition.Dispose();
        }

        private void AnalyzeFunc(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition)
        {
            string analyzeAttrName = typeof(AnalyzeAttribute).FullName;
            Flags flags = Flags.Default;

            for (int i = 0; i < methodDefinition.CustomAttributes.Count; i++)
            {
                CustomAttribute attribute = methodDefinition.CustomAttributes[i];

                if (attribute.Constructor.DeclaringType.FullName.Equals(analyzeAttrName))
                {
                    AnalyzeAttr<Flags>(attribute, "AnalyzingFlags", ref flags);
                    break;
                }
            }

            InjectILCode(moduleDefinition, methodDefinition, flags);
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

        private void InjectILCode(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition, Flags flags)
        {
            IHandler funcDataHandler = null;
            IHandler runTimeDataHandler = null;

            if ((flags & Flags.Default) != 0)
            {
                flags = Flags.Default | Flags.Args | Flags.Ret | Flags.Time | Flags.Memory;
                funcDataHandler = new FuncDataHandler();
                runTimeDataHandler = new RuntimeDataHandler();
            }
            else if((flags & Flags.Args) != 0 || (flags & Flags.Ret) != 0)
            {
                funcDataHandler = new FuncDataHandler();
            }
            else
            {
                runTimeDataHandler = new RuntimeDataHandler();
            }

            // inject handler
            funcDataHandler?.Inject(moduleDefinition, methodDefinition, flags);
            runTimeDataHandler?.Inject(moduleDefinition, methodDefinition, flags);
        }
    }
}


