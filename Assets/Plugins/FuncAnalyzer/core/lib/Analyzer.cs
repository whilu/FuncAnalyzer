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

        public const string InjectInjectFlagMethod = "co_lujun_funcanalyzer_InjectFlag";

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

            string analyzeAttrName = typeof(AnalyzeAttribute).FullName;

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

                bool analyzeAttrExist = false;
                for (int j = 0; j < methods.Count; j++)
                {
                    MethodDefinition methodDefinition = methods[j];
                    for (int l = 0; l < methodDefinition.CustomAttributes.Count; l++)
                    {
                        CustomAttribute attribute = methodDefinition.CustomAttributes[l];

                        if (attribute.Constructor.DeclaringType.FullName.Equals(analyzeAttrName))
                        {
                            analyzeAttrExist = true;
                            break;
                        }
                    }
                }

                // This type has already injected analysis code or don't exist analyze attribute, continue
                if (injectFlagMethodExist || !analyzeAttrExist)
                {
                    continue;
                }

                // Inject flag method for this type first
                InjectFlagMethod(moduleDefinition, typeDefinition);

                // Inject analysis code for specify methods
                for (int j = 0; j < methods.Count; j++)
                {
                    AnalyzeFunc(moduleDefinition, typeDefinition, methods[j]);
                }
            }

            callback(.8f, "Start write assembly...");

            assemblyDefinition.Write(assemblyPath, new WriterParameters(){ WriteSymbols = true });
            assemblyDefinition.Dispose();

            callback(1.0f, "Analysis code injected!");
        }

        private void InjectFlagMethod(ModuleDefinition moduleDefinition, TypeDefinition typeDefinition)
        {
            // New method, with format function
            MethodDefinition injectFlagMethodDefinition = new MethodDefinition(InjectInjectFlagMethod,
                MethodAttributes.Private | MethodAttributes.HideBySig, moduleDefinition.TypeSystem.String);
            typeDefinition.Methods.Add(injectFlagMethodDefinition);

            injectFlagMethodDefinition.Parameters.Add(new ParameterDefinition(moduleDefinition.TypeSystem.Int64));

            injectFlagMethodDefinition.Body.Variables.Add(
                new VariableDefinition(moduleDefinition.ImportReference(typeof(string[]))));
            injectFlagMethodDefinition.Body.Variables.Add(new VariableDefinition(moduleDefinition.TypeSystem.Int32));
            injectFlagMethodDefinition.Body.Variables.Add(
                new VariableDefinition(moduleDefinition.ImportReference(typeof(decimal))));
            injectFlagMethodDefinition.Body.Variables.Add(new VariableDefinition(moduleDefinition.TypeSystem.Boolean));
            injectFlagMethodDefinition.Body.Variables.Add(new VariableDefinition(moduleDefinition.TypeSystem.String));

            ILProcessor ilProcessor = injectFlagMethodDefinition.Body.GetILProcessor();
            ilProcessor.Emit(OpCodes.Ldc_I4, 6);
            ilProcessor.Emit(OpCodes.Newarr, moduleDefinition.TypeSystem.String);

            string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

            for (int i = 0; i < suffixes.Length; i++)
            {
                ilProcessor.Emit(OpCodes.Dup);
                ilProcessor.Emit(OpCodes.Ldc_I4, i);
                ilProcessor.Emit(OpCodes.Ldstr, suffixes[i]);
                ilProcessor.Emit(OpCodes.Stelem_Ref);
            }

            ilProcessor.Emit(OpCodes.Stloc, 0);
            ilProcessor.Emit(OpCodes.Ldc_I4, 0);
            ilProcessor.Emit(OpCodes.Stloc, 1);
            ilProcessor.Emit(OpCodes.Ldarg, 1);

            ilProcessor.Emit(OpCodes.Call,
                moduleDefinition.ImportReference(typeof(decimal).GetMethod("op_Implicit", new Type[] {typeof(long)})));
            ilProcessor.Emit(OpCodes.Stloc, 2);

            Instruction nop_b1 = ilProcessor.Create(OpCodes.Nop);
            Instruction ldloc_2_b2 = ilProcessor.Create(OpCodes.Ldloc, 2);

            ilProcessor.Emit(OpCodes.Br, ldloc_2_b2);

            // Start loop
            ilProcessor.Append(nop_b1);
            ilProcessor.Emit(OpCodes.Ldloc, 2);
            ilProcessor.Emit(OpCodes.Ldc_I4, 1024);
            ilProcessor.Emit(OpCodes.Newobj,
                moduleDefinition.ImportReference(typeof(decimal).GetConstructor(new Type[] {typeof(int)})));
            ilProcessor.Emit(OpCodes.Call,
                moduleDefinition.ImportReference(typeof(decimal).GetMethod("op_Division",
                    new Type[] {typeof(decimal), typeof(decimal)})));
            ilProcessor.Emit(OpCodes.Stloc, 2);
            ilProcessor.Emit(OpCodes.Ldloc, 1);
            ilProcessor.Emit(OpCodes.Ldc_I4, 1);
            ilProcessor.Emit(OpCodes.Add);
            ilProcessor.Emit(OpCodes.Stloc, 1);
            ilProcessor.Emit(OpCodes.Nop);

            ilProcessor.Append(ldloc_2_b2);
            ilProcessor.Emit(OpCodes.Ldc_I4, 1024);
            ilProcessor.Emit(OpCodes.Newobj,
                moduleDefinition.ImportReference(typeof(decimal).GetConstructor(new Type[] {typeof(int)})));
            ilProcessor.Emit(OpCodes.Call,
                moduleDefinition.ImportReference(typeof(decimal).GetMethod("op_Division",
                    new Type[] {typeof(decimal), typeof(decimal)})));
            ilProcessor.Emit(OpCodes.Call,
                moduleDefinition.ImportReference(typeof(Math).GetMethod("Round", new Type[] {typeof(decimal)})));
            ilProcessor.Emit(OpCodes.Ldsfld,
                moduleDefinition.ImportReference(typeof(decimal).GetField("One")));
            ilProcessor.Emit(OpCodes.Call,
                moduleDefinition.ImportReference(typeof(decimal).GetMethod("op_GreaterThanOrEqual",
                    new Type[] {typeof(decimal), typeof(decimal)})));
            ilProcessor.Emit(OpCodes.Stloc, 3);
            ilProcessor.Emit(OpCodes.Ldloc, 3);
            ilProcessor.Emit(OpCodes.Brtrue, nop_b1);

            ilProcessor.Emit(OpCodes.Ldstr, "{0:n1}{1}");
            ilProcessor.Emit(OpCodes.Ldloc, 2);
            ilProcessor.Emit(OpCodes.Box, moduleDefinition.ImportReference(typeof(decimal)));
            ilProcessor.Emit(OpCodes.Ldloc, 0);
            ilProcessor.Emit(OpCodes.Ldloc, 1);
            ilProcessor.Emit(OpCodes.Ldelem_Ref);
            ilProcessor.Emit(OpCodes.Call,
                moduleDefinition.ImportReference(typeof(string).GetMethod("Format",
                    new Type[] {typeof(string), typeof(object), typeof(object)})));
            ilProcessor.Emit(OpCodes.Stloc, 4);
            ilProcessor.Emit(OpCodes.Ldloc, 4);
            ilProcessor.Emit(OpCodes.Ret);
        }

        private void AnalyzeFunc(ModuleDefinition moduleDefinition, TypeDefinition typeDefinition,
            MethodDefinition methodDefinition)
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
                InjectILCode(moduleDefinition, typeDefinition, methodDefinition, enable, flags);
            }
        }

        private void InjectILCode(ModuleDefinition moduleDefinition, TypeDefinition typeDefinition,
            MethodDefinition methodDefinition, bool enable,
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
            funcDataHandler?.Inject(moduleDefinition, typeDefinition, methodDefinition, enable, flags);
            runTimeDataHandler?.Inject(moduleDefinition, typeDefinition, methodDefinition, enable, flags);
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


