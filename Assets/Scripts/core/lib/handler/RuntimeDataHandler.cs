using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace co.lujun.funcanalyzer.handler
{
    public class RuntimeDataHandler : HandlerImpl
    {
        public override void Inject(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition, Flags flags)
        {
            base.Inject(moduleDefinition, methodDefinition, flags);

            // including function 'execute time' analyze
            if ((flags & Flags.Time) != 0)
            {
                GenerateAnalysisCodeForTime();
            }

            // including function 'memory data' analyze
            if ((flags & Flags.Memory) != 0)
            {
                GenerateAnalysisCodeForMemory();
            }
        }

        private void GenerateAnalysisCodeForTime()
        {
            // OriginVariablesCount
            TypeReference startTimeTypeReference = ModuleDefinition.ImportReference(typeof(DateTime));
            MethodDefinition.Body.Variables.Add(new VariableDefinition(startTimeTypeReference));

            // OriginVariablesCount + 1
            TypeReference costTimeMillsTypeReference = ModuleDefinition.ImportReference(typeof(double));
            MethodDefinition.Body.Variables.Add(new VariableDefinition(costTimeMillsTypeReference));

            // OriginVariablesCount + 2
            TypeReference timeSpanTypeReference = ModuleDefinition.ImportReference(typeof(TimeSpan));
            MethodDefinition.Body.Variables.Add(new VariableDefinition(timeSpanTypeReference));

            // OriginVariablesCount + 3
            TypeReference logCostTimeStrTypeReference = ModuleDefinition.ImportReference(typeof(string));
            MethodDefinition.Body.Variables.Add(new VariableDefinition(logCostTimeStrTypeReference));

            // DateTime.Now method reference
            MethodReference nowDateMethodRef = ModuleDefinition.ImportReference(
                typeof(DateTime).GetMethod("get_Now"));

            // Get date time before method execute
            Instruction nowDateInstruction = ILProcessor.Create(OpCodes.Call, nowDateMethodRef);
            ILProcessor.InsertBefore(MethodFirstInstruction, nowDateInstruction);

            // Store the start date time
            Instruction stLocNowDateInstruction = ILProcessor.Create(OpCodes.Stloc, OriginVariablesCount);
            ILProcessor.InsertAfter(nowDateInstruction, stLocNowDateInstruction);

            // Get date time after method executed
            Instruction endDateInstruction = ILProcessor.Create(OpCodes.Call, nowDateMethodRef);
            ILProcessor.InsertBefore(MethodLastInstruction, endDateInstruction);

            // Copy the start date time
            Instruction ldLocStartDateInstruction = ILProcessor.Create(OpCodes.Ldloc, OriginVariablesCount);
            ILProcessor.InsertBefore(MethodLastInstruction, ldLocStartDateInstruction);

            // Get the cost TimeSpan with the start date time and after date time
            MethodReference subtractCostTimeMethodRef = ModuleDefinition.ImportReference(
                typeof(DateTime).GetMethod("op_Subtraction", new[] {typeof(DateTime), typeof(DateTime)}));
            Instruction subtractCostTimeInstruction = ILProcessor.Create(OpCodes.Call, subtractCostTimeMethodRef);
            ILProcessor.InsertBefore(MethodLastInstruction, subtractCostTimeInstruction);

            // Store the time span
            Instruction stLocTimeSpanInstruction = ILProcessor.Create(OpCodes.Stloc, OriginVariablesCount + 2);
            ILProcessor.InsertBefore(MethodLastInstruction, stLocTimeSpanInstruction);

            // Get the time span reference
            Instruction ldLocaTimeSpanInstruction = ILProcessor.Create(OpCodes.Ldloca, OriginVariablesCount + 2);
            ILProcessor.InsertBefore(MethodLastInstruction, ldLocaTimeSpanInstruction);

            // Get the cost time milliseconds with time span
            MethodReference totalMillisecondsMethodRef =
                ModuleDefinition.ImportReference(typeof(TimeSpan).GetMethod("get_TotalMilliseconds"));
            Instruction totalMillisecondsInstruction = ILProcessor.Create(OpCodes.Call, totalMillisecondsMethodRef);
            ILProcessor.InsertBefore(MethodLastInstruction, totalMillisecondsInstruction);

            // Store cost time milliseconds
            Instruction stLocCostTimeInstruction = ILProcessor.Create(OpCodes.Stloc, OriginVariablesCount + 1);
            ILProcessor.InsertBefore(MethodLastInstruction, stLocCostTimeInstruction);

            // Get the cost time milliseconds's reference
            Instruction ldLocCostTimeInstruction = ILProcessor.Create(OpCodes.Ldloca, OriginVariablesCount + 1);
            ILProcessor.InsertBefore(MethodLastInstruction, ldLocCostTimeInstruction);

            // Convert cost time to string
            MethodReference timeToStringMethodRef =
                ModuleDefinition.ImportReference(typeof(double).GetMethod("ToString", new Type[] { }));
            Instruction timeToStringInstruction = ILProcessor.Create(OpCodes.Call, timeToStringMethodRef);
            ILProcessor.InsertBefore(MethodLastInstruction, timeToStringInstruction);

            // Store cost time milliseconds string
            Instruction stLocCostTimeStrInstruction = ILProcessor.Create(OpCodes.Stloc, OriginVariablesCount + 3);
            ILProcessor.InsertBefore(MethodLastInstruction, stLocCostTimeStrInstruction);

            // Push the LogFormat method's string param to Evaluation Stack
            string logFormatStr = MethodDefinition.Name + " execute cost {0} ms";
            Instruction ldStrLogFormatStrInstruction = ILProcessor.Create(OpCodes.Ldstr, logFormatStr);
            ILProcessor.InsertBefore(MethodLastInstruction, ldStrLogFormatStrInstruction);

            // Push the LogFormat method's extra param count to Evaluation Stack
            Instruction ldcLogFormatParamsCountInstruction = ILProcessor.Create(OpCodes.Ldc_I4, 1);
            ILProcessor.InsertBefore(MethodLastInstruction, ldcLogFormatParamsCountInstruction);

            // New array for LogFormat method's extra param
            TypeReference logFormatParamTypeReference = ModuleDefinition.ImportReference(typeof(object));
            Instruction newArrLogFormatParamsInstruction = ILProcessor.Create(OpCodes.Newarr,
                logFormatParamTypeReference);
            ILProcessor.InsertBefore(MethodLastInstruction, newArrLogFormatParamsInstruction);

            // Dup the array
            Instruction dupLogFormatParamsArrInstruction = ILProcessor.Create(OpCodes.Dup);
            ILProcessor.InsertBefore(MethodLastInstruction, dupLogFormatParamsArrInstruction);

            // Copy the cost time milliseconds to the array with specify position
            Instruction ldcIndex0LogFormatParamsInstruction = ILProcessor.Create(OpCodes.Ldc_I4, 0);
            ILProcessor.InsertBefore(MethodLastInstruction, ldcIndex0LogFormatParamsInstruction);
            Instruction ldLocIndex0ParamInstruction = ILProcessor.Create(OpCodes.Ldloc, OriginVariablesCount + 3);
            ILProcessor.InsertBefore(MethodLastInstruction, ldLocIndex0ParamInstruction);
            Instruction stelemIndex0ParamRefInstruction = ILProcessor.Create(OpCodes.Stelem_Ref);
            ILProcessor.InsertBefore(MethodLastInstruction, stelemIndex0ParamRefInstruction);

            // Print the cost time to the console
            MethodReference logFormatMethodReference = ModuleDefinition.ImportReference(
                typeof(Debug).GetMethod("LogFormat", new[] {typeof(string), typeof(object[])}));
            Instruction logMethodInstruction = ILProcessor.Create(OpCodes.Call, logFormatMethodReference);
            ILProcessor.InsertBefore(MethodLastInstruction, logMethodInstruction);
        }

        private void GenerateAnalysisCodeForMemory()
        {

        }
    }
}