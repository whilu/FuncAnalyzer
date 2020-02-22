using System;
using co.lujun.funcanalyzer.imodule;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace co.lujun.funcanalyzer.module
{
    public class RuntimeDataHandler : IHandler
    {
        public void Inject(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition, Flags flags)
        {
            // including function 'execute time' analyze
            if ((flags & Flags.Time) != 0)
            {
                GenerateAnalysisCodeForTime(moduleDefinition, methodDefinition);
            }

            // including function 'memory data' analyze
            if ((flags & Flags.Memory) != 0)
            {
                GenerateAnalysisCodeForMemory(moduleDefinition, methodDefinition);
            }
        }

        private void GenerateAnalysisCodeForTime(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition)
        {
            // method processor
            ILProcessor ilProcessor = methodDefinition.Body.GetILProcessor();

            // first instruction in the method
            Instruction firstInstruction = methodDefinition.Body.Instructions[0];
            // last instruction in the method
            Instruction lastInstruction =
                methodDefinition.Body.Instructions[methodDefinition.Body.Instructions.Count - 1];

            // origin variables count
            int originVariablesCount = methodDefinition.Body.Variables.Count;

            // originVariablesCount
            TypeReference startTimeTypeReference = moduleDefinition.ImportReference(typeof(DateTime));
            methodDefinition.Body.Variables.Add(new VariableDefinition(startTimeTypeReference));

            // originVariablesCount + 1
            TypeReference costTimeMillsTypeReference = moduleDefinition.ImportReference(typeof(double));
            methodDefinition.Body.Variables.Add(new VariableDefinition(costTimeMillsTypeReference));

            // originVariablesCount + 2
            TypeReference timeSpanTypeReference = moduleDefinition.ImportReference(typeof(TimeSpan));
            methodDefinition.Body.Variables.Add(new VariableDefinition(timeSpanTypeReference));

            // originVariablesCount + 3
            TypeReference logCostTimeStrTypeReference = moduleDefinition.ImportReference(typeof(string));
            methodDefinition.Body.Variables.Add(new VariableDefinition(logCostTimeStrTypeReference));

            // DateTime.Now method reference
            MethodReference nowDateMethodRef = moduleDefinition.ImportReference(
                typeof(DateTime).GetMethod("get_Now"));

            // Get date time before method execute
            Instruction nowDateInstruction = ilProcessor.Create(OpCodes.Call, nowDateMethodRef);
            ilProcessor.InsertBefore(firstInstruction, nowDateInstruction);

            // Store the start date time
            Instruction stLocNowDateInstruction = ilProcessor.Create(OpCodes.Stloc, originVariablesCount);
            ilProcessor.InsertAfter(nowDateInstruction, stLocNowDateInstruction);

            // Get date time after method executed
            Instruction endDateInstruction = ilProcessor.Create(OpCodes.Call, nowDateMethodRef);
            ilProcessor.InsertBefore(lastInstruction, endDateInstruction);

            // Copy the start date time
            Instruction ldLocStartDateInstruction = ilProcessor.Create(OpCodes.Ldloc, originVariablesCount);
            ilProcessor.InsertBefore(lastInstruction, ldLocStartDateInstruction);

            // Get the cost TimeSpan with the start date time and after date time
            MethodReference subtractCostTimeMethodRef = moduleDefinition.ImportReference(
                typeof(DateTime).GetMethod("op_Subtraction", new[] {typeof(DateTime), typeof(DateTime)}));
            Instruction subtractCostTimeInstruction = ilProcessor.Create(OpCodes.Call, subtractCostTimeMethodRef);
            ilProcessor.InsertBefore(lastInstruction, subtractCostTimeInstruction);

            // Store the time span
            Instruction stLocTimeSpanInstruction = ilProcessor.Create(OpCodes.Stloc, originVariablesCount + 2);
            ilProcessor.InsertBefore(lastInstruction, stLocTimeSpanInstruction);

            // Get the time span reference
            Instruction ldLocaTimeSpanInstruction = ilProcessor.Create(OpCodes.Ldloca, originVariablesCount + 2);
            ilProcessor.InsertBefore(lastInstruction, ldLocaTimeSpanInstruction);

            // Get the cost time milliseconds with time span
            MethodReference totalMillisecondsMethodRef =
                moduleDefinition.ImportReference(typeof(TimeSpan).GetMethod("get_TotalMilliseconds"));
            Instruction totalMillisecondsInstruction = ilProcessor.Create(OpCodes.Call, totalMillisecondsMethodRef);
            ilProcessor.InsertBefore(lastInstruction, totalMillisecondsInstruction);

            // Store cost time milliseconds
            Instruction stLocCostTimeInstruction = ilProcessor.Create(OpCodes.Stloc, originVariablesCount + 1);
            ilProcessor.InsertBefore(lastInstruction, stLocCostTimeInstruction);

            // Get the cost time milliseconds's reference
            Instruction ldLocCostTimeInstruction = ilProcessor.Create(OpCodes.Ldloca, originVariablesCount + 1);
            ilProcessor.InsertBefore(lastInstruction, ldLocCostTimeInstruction);

            // Convert cost time to string
            MethodReference timeToStringMethodRef =
                moduleDefinition.ImportReference(typeof(double).GetMethod("ToString", new Type[] { }));
            Instruction timeToStringInstruction = ilProcessor.Create(OpCodes.Call, timeToStringMethodRef);
            ilProcessor.InsertBefore(lastInstruction, timeToStringInstruction);

            // Store cost time milliseconds string
            Instruction stLocCostTimeStrInstruction = ilProcessor.Create(OpCodes.Stloc, originVariablesCount + 3);
            ilProcessor.InsertBefore(lastInstruction, stLocCostTimeStrInstruction);

            // Push the LogFormat method's string param to Evaluation Stack
            string logFormatStr = methodDefinition.Name + " execute cost {0} ms";
            Instruction ldStrLogFormatStrInstruction = ilProcessor.Create(OpCodes.Ldstr, logFormatStr);
            ilProcessor.InsertBefore(lastInstruction, ldStrLogFormatStrInstruction);

            // Push the LogFormat method's extra param count to Evaluation Stack
            Instruction ldcLogFormatParamsCountInstruction = ilProcessor.Create(OpCodes.Ldc_I4, 1);
            ilProcessor.InsertBefore(lastInstruction, ldcLogFormatParamsCountInstruction);

            // New array for LogFormat method's extra param
            TypeReference logFormatParamTypeReference = moduleDefinition.ImportReference(typeof(object));
            Instruction newArrLogFormatParamsInstruction = ilProcessor.Create(OpCodes.Newarr,
                logFormatParamTypeReference);
            ilProcessor.InsertBefore(lastInstruction, newArrLogFormatParamsInstruction);

            // Dup the array
            Instruction dupLogFormatParamsArrInstruction = ilProcessor.Create(OpCodes.Dup);
            ilProcessor.InsertBefore(lastInstruction, dupLogFormatParamsArrInstruction);

            // Copy the cost time milliseconds to the array with specify position
            Instruction ldcIndex0LogFormatParamsInstruction = ilProcessor.Create(OpCodes.Ldc_I4, 0);
            ilProcessor.InsertBefore(lastInstruction, ldcIndex0LogFormatParamsInstruction);
            Instruction ldLocIndex0ParamInstruction = ilProcessor.Create(OpCodes.Ldloc, originVariablesCount + 3);
            ilProcessor.InsertBefore(lastInstruction, ldLocIndex0ParamInstruction);
            Instruction stelemIndex0ParamRefInstruction = ilProcessor.Create(OpCodes.Stelem_Ref);
            ilProcessor.InsertBefore(lastInstruction, stelemIndex0ParamRefInstruction);

            // Print the cost time to the console
            MethodReference logFormatMethodReference = moduleDefinition.ImportReference(
                typeof(Debug).GetMethod("LogFormat", new[] {typeof(string), typeof(object[])}));
            Instruction logMethodInstruction = ilProcessor.Create(OpCodes.Call, logFormatMethodReference);
            ilProcessor.InsertBefore(lastInstruction, logMethodInstruction);
        }

        private void GenerateAnalysisCodeForMemory(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition)
        {

        }
    }
}