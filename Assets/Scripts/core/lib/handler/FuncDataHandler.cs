using System;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace co.lujun.funcanalyzer.handler
{
    public class FuncDataHandler : HandlerImpl
    {
        public override void Inject(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition, Flags flags)
        {
            base.Inject(moduleDefinition, methodDefinition, flags);

            // including function 'args' analyze
            if((flags & Flags.Args) != 0)
            {
                GenerateAnalysisCodeForArgs();
            }

            // including function 'return value' analyze
            if ((flags & Flags.Ret) != 0)
            {
                GenerateAnalysisCodeForRet();
            }
        }

        private void GenerateAnalysisCodeForArgs()
        {
            StringBuilder builder = new StringBuilder().Append(MethodDefinition.Name).Append(" - ");

            if (MethodDefinition.HasParameters)
            {
                int paramtersCount = MethodDefinition.Parameters.Count;

                for (int i = 0; i < paramtersCount; i++)
                {
                    ParameterDefinition parameter = MethodDefinition.Parameters[i];
                    TypeReference paramTypeReference = parameter.ParameterType;

                    MethodDefinition.Body.Variables.Add(new VariableDefinition(
                        ModuleDefinition.ImportReference(typeof(string)))
                    );

                    int ldArgIdx = MethodDefinition.IsStatic ? i : i + 1;

                    // Ldarg arg with specify position
                    Instruction ldArgInstruction = ILProcessor.Create(OpCodes.Ldarg, ldArgIdx);
                    ILProcessor.InsertBefore(MethodFirstInstruction, ldArgInstruction);

                    // TODO need to store to record frame, and pop from record frame with ref

                    // Get type with type name
                    Type paramType = Type.GetType(paramTypeReference.FullName);
                    if (paramType == null)
                    {
                        paramType = Type.GetType(Assembly.CreateQualifiedName(paramTypeReference.Module.Assembly.FullName,
                            paramTypeReference.FullName));
                    }

                    // ToString
                    MethodReference toStringMethodRef = ModuleDefinition.ImportReference(paramType.GetMethod("ToString",
                        new Type[] { }));
                    Instruction toStringInstruction = ILProcessor.Create(OpCodes.Call, toStringMethodRef);
                    ILProcessor.InsertBefore(MethodLastInstruction, toStringInstruction);

                    // Store
                    Instruction stLocArgStrStrInstruction = ILProcessor.Create(OpCodes.Stloc, OriginVariablesCount - 1);
                    ILProcessor.InsertBefore(MethodLastInstruction, stLocArgStrStrInstruction);

                    // eg: string msg(value: hello world), int level(value: 8)
                    builder.Append(paramTypeReference.Name).Append(" ");
                    builder.Append(parameter.Name).Append("(value: ");
                    builder.Append("{").Append(i).Append("}").Append("), ");
                }

                builder.Remove(builder.Length - 2, 2);
            }
            else
            {
                builder.Append("Method has no parameters");
            }
        }


        private void GenerateAnalysisCodeForRet()
        {
            StringBuilder builder = new StringBuilder().Append(MethodDefinition.Name).Append(" - ");
            TypeReference retTypeReference = MethodDefinition.ReturnType;
            string retTypeName = retTypeReference.Name;
            bool voidReturn = retTypeName.Equals("Void");
            int logFormatParamsCount = voidReturn ? 0 : 1;

            if (!voidReturn)
            {
                builder.Append(string.Format("Return '{0}' ", retTypeName)).Append("(value: {0})");

                MethodDefinition.Body.Variables.Add(new VariableDefinition(retTypeReference));

                TypeReference logRetValueTypeReference = ModuleDefinition.ImportReference(typeof(string));
                MethodDefinition.Body.Variables.Add(new VariableDefinition(logRetValueTypeReference));

                // Dup the return value
                Instruction dupRetValueInstruction = ILProcessor.Create(OpCodes.Dup);
                ILProcessor.InsertBefore(MethodLastInstruction, dupRetValueInstruction);

                // Push the clone return value to Record Frame
                Instruction stLocRetValueInstruction = ILProcessor.Create(OpCodes.Stloc, OriginVariablesCount - 2);
                ILProcessor.InsertBefore(MethodLastInstruction, stLocRetValueInstruction);
                Instruction ldlocaRetValueRefInstruction = ILProcessor.Create(retTypeReference.IsValueType ? OpCodes.Ldloca : OpCodes.Ldloc, OriginVariablesCount - 2);
                ILProcessor.InsertBefore(MethodLastInstruction, ldlocaRetValueRefInstruction);

                // Get type with type name
                Type retType = Type.GetType(retTypeReference.FullName);
                if (retType == null)
                {
                    retType = Type.GetType(Assembly.CreateQualifiedName(retTypeReference.Module.Assembly.FullName,
                        retTypeReference.FullName));
                }

                // Convert return value to string
                MethodReference toStringMethodRef = ModuleDefinition.ImportReference(retType.GetMethod("ToString",
                    new Type[] { }));
                Instruction toStringInstruction = ILProcessor.Create(OpCodes.Call, toStringMethodRef);
                ILProcessor.InsertBefore(MethodLastInstruction, toStringInstruction);

                // Store return value
                Instruction stLocRetValueStrInstruction = ILProcessor.Create(OpCodes.Stloc, OriginVariablesCount - 1);
                ILProcessor.InsertBefore(MethodLastInstruction, stLocRetValueStrInstruction);
            }
            else
            {
                builder.Append("Return 'Void'");
            }

            // Push the LogFormat method's string param to Evaluation Stack
            Instruction ldStrLogFormatStrInstruction = ILProcessor.Create(OpCodes.Ldstr, builder.ToString());
            ILProcessor.InsertBefore(MethodLastInstruction, ldStrLogFormatStrInstruction);

            // Push the LogFormat method's extra param count to Evaluation Stack
            Instruction ldcLogFormatParamsCountInstruction = ILProcessor.Create(OpCodes.Ldc_I4, logFormatParamsCount);
            ILProcessor.InsertBefore(MethodLastInstruction, ldcLogFormatParamsCountInstruction);

            // New array for LogFormat method's extra param
            TypeReference logFormatParamTypeReference = ModuleDefinition.ImportReference(typeof(object));
            Instruction newArrLogFormatParamsInstruction = ILProcessor.Create(OpCodes.Newarr,
                logFormatParamTypeReference);
            ILProcessor.InsertBefore(MethodLastInstruction, newArrLogFormatParamsInstruction);

            if (!voidReturn)
            {
                Instruction dupLogFormatParamsArrInstruction = ILProcessor.Create(OpCodes.Dup);
                ILProcessor.InsertBefore(MethodLastInstruction, dupLogFormatParamsArrInstruction);

                Instruction ldcIndex0LogFormatParamsInstruction = ILProcessor.Create(OpCodes.Ldc_I4, 0);
                ILProcessor.InsertBefore(MethodLastInstruction, ldcIndex0LogFormatParamsInstruction);
                Instruction ldLocIndex0ParamInstruction = ILProcessor.Create(OpCodes.Ldloc, OriginVariablesCount - 1);
                ILProcessor.InsertBefore(MethodLastInstruction, ldLocIndex0ParamInstruction);
                Instruction stelemIndex0ParamRefInstruction = ILProcessor.Create(OpCodes.Stelem_Ref);
                ILProcessor.InsertBefore(MethodLastInstruction, stelemIndex0ParamRefInstruction);
            }

            // Print
            MethodReference logFormatMethodReference = ModuleDefinition.ImportReference(
                typeof(Debug).GetMethod("LogFormat", new[] {typeof(string), typeof(object[])}));
            Instruction logMethodInstruction = ILProcessor.Create(OpCodes.Call, logFormatMethodReference);
            ILProcessor.InsertBefore(MethodLastInstruction, logMethodInstruction);
        }
    }
}