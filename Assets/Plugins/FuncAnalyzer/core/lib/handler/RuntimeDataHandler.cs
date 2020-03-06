using System;
using System.Text;
using co.lujun.funcanalyzer.util;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.Profiling;

namespace co.lujun.funcanalyzer.handler
{
    public class RuntimeDataHandler : HandlerImpl
    {
        public override void Inject(ModuleDefinition moduleDefinition, TypeDefinition typeDefinition,
            MethodDefinition methodDefinition, bool enable, Flags flags)
        {
            base.Inject(moduleDefinition, typeDefinition, methodDefinition, enable, flags);

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
            TypeReference startTimeTypeReference = ModuleDefinition.ImportReference(typeof(DateTime));
            MethodDefinition.Body.Variables.Add(new VariableDefinition(startTimeTypeReference));

            TypeReference costTimeMillsTypeReference = ModuleDefinition.ImportReference(typeof(double));
            MethodDefinition.Body.Variables.Add(new VariableDefinition(costTimeMillsTypeReference));

            TypeReference timeSpanTypeReference = ModuleDefinition.ImportReference(typeof(TimeSpan));
            MethodDefinition.Body.Variables.Add(new VariableDefinition(timeSpanTypeReference));

            TypeReference logCostTimeStrTypeReference = ModuleDefinition.ImportReference(typeof(string));
            MethodDefinition.Body.Variables.Add(new VariableDefinition(logCostTimeStrTypeReference));

            // DateTime.Now method reference
            MethodReference nowDateMethodRef = ModuleDefinition.ImportReference(
                typeof(DateTime).GetMethod("get_Now"));

            // Get date time before method execute
            Instruction nowDateInstruction = ILProcessor.Create(OpCodes.Call, nowDateMethodRef);
            ILProcessor.InsertBefore(MethodFirstInstruction, nowDateInstruction);

            // Store the start date time
            Instruction stLocNowDateInstruction = ILProcessor.Create(OpCodes.Stloc, OriginVariablesCount - 4);
            ILProcessor.InsertAfter(nowDateInstruction, stLocNowDateInstruction);

            // Get date time after method executed
            Instruction endDateInstruction = ILProcessor.Create(OpCodes.Call, nowDateMethodRef);
            ILProcessor.InsertBefore(MethodLastInstruction, endDateInstruction);

            // Copy the start date time
            Instruction ldLocStartDateInstruction = ILProcessor.Create(OpCodes.Ldloc, OriginVariablesCount - 4);
            ILProcessor.InsertBefore(MethodLastInstruction, ldLocStartDateInstruction);

            // Get the cost TimeSpan with the start date time and after date time
            MethodReference subtractCostTimeMethodRef = ModuleDefinition.ImportReference(
                typeof(DateTime).GetMethod("op_Subtraction", new[] {typeof(DateTime), typeof(DateTime)}));
            Instruction subtractCostTimeInstruction = ILProcessor.Create(OpCodes.Call, subtractCostTimeMethodRef);
            ILProcessor.InsertBefore(MethodLastInstruction, subtractCostTimeInstruction);

            // Store the time span
            Instruction stLocTimeSpanInstruction = ILProcessor.Create(OpCodes.Stloc, OriginVariablesCount - 2);
            ILProcessor.InsertBefore(MethodLastInstruction, stLocTimeSpanInstruction);

            // Get the time span reference
            Instruction ldLocaTimeSpanInstruction = ILProcessor.Create(OpCodes.Ldloca, OriginVariablesCount - 2);
            ILProcessor.InsertBefore(MethodLastInstruction, ldLocaTimeSpanInstruction);

            // Get the cost time milliseconds with time span
            MethodReference totalMillisecondsMethodRef =
                ModuleDefinition.ImportReference(typeof(TimeSpan).GetMethod("get_TotalMilliseconds"));
            Instruction totalMillisecondsInstruction = ILProcessor.Create(OpCodes.Call, totalMillisecondsMethodRef);
            ILProcessor.InsertBefore(MethodLastInstruction, totalMillisecondsInstruction);

            // Store cost time milliseconds
            Instruction stLocCostTimeInstruction = ILProcessor.Create(OpCodes.Stloc, OriginVariablesCount - 3);
            ILProcessor.InsertBefore(MethodLastInstruction, stLocCostTimeInstruction);

            // Get the cost time milliseconds's reference
            Instruction ldLocCostTimeInstruction = ILProcessor.Create(OpCodes.Ldloca, OriginVariablesCount - 3);
            ILProcessor.InsertBefore(MethodLastInstruction, ldLocCostTimeInstruction);

            // Convert cost time to string
            MethodReference timeToStringMethodRef =
                ModuleDefinition.ImportReference(typeof(double).GetMethod("ToString", new Type[] { }));
            Instruction timeToStringInstruction = ILProcessor.Create(OpCodes.Call, timeToStringMethodRef);
            ILProcessor.InsertBefore(MethodLastInstruction, timeToStringInstruction);

            // Store cost time milliseconds string
            Instruction stLocCostTimeStrInstruction = ILProcessor.Create(OpCodes.Stloc, OriginVariablesCount - 1);
            ILProcessor.InsertBefore(MethodLastInstruction, stLocCostTimeStrInstruction);

            // Push the LogFormat method's string param to Evaluation Stack
            string logFormatStr = new StringBuilder()
                .Append(MethodDefinition.Name)
                .Append(" - ")
                .Append("Execute cost {0} ms")
                .ToString();
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
            Instruction ldLocIndex0ParamInstruction = ILProcessor.Create(OpCodes.Ldloc, OriginVariablesCount - 1);
            ILProcessor.InsertBefore(MethodLastInstruction, ldLocIndex0ParamInstruction);
            Instruction stelemIndex0ParamRefInstruction = ILProcessor.Create(OpCodes.Stelem_Ref);
            ILProcessor.InsertBefore(MethodLastInstruction, stelemIndex0ParamRefInstruction);

            // Print the cost time to the console
            MethodReference logFormatMethodReference = ModuleDefinition.ImportReference(
                typeof(Debug).GetMethod("LogFormat", new[] {typeof(string), typeof(object[])}));
            Instruction logMethodInstruction = ILProcessor.Create(OpCodes.Call, logFormatMethodReference);
            ILProcessor.InsertBefore(MethodLastInstruction, logMethodInstruction);

            // Check enable flag
            InjectCheckEnableCode(ldStrLogFormatStrInstruction, MethodLastInstruction);
        }

        private void GenerateAnalysisCodeForMemory()
        {
            // Check instruction
            Instruction checkInstruction = null;

            // Log params count
            int logParamsCount = 5;

            // Define 5 variables for data
            for (int i = 0; i < logParamsCount; i++)
            {
                TypeReference memoryRetTypeReference = ModuleDefinition.ImportReference(typeof(string));
                MethodDefinition.Body.Variables.Add(new VariableDefinition(memoryRetTypeReference));
            }

            // Define data method
            MethodReference[] getMemoryMethodReferences = new MethodReference[]
            {
                ModuleDefinition.ImportReference(typeof(Profiler).GetMethod("GetTotalUnusedReservedMemoryLong",
                    new Type[] { })),
                ModuleDefinition.ImportReference(typeof(Profiler).GetMethod("GetTotalAllocatedMemoryLong",
                    new Type[] { })),
                ModuleDefinition.ImportReference(typeof(Profiler).GetMethod("GetTotalReservedMemoryLong",
                    new Type[] { })),
                ModuleDefinition.ImportReference(typeof(GC).GetMethod("Collect", new Type[] { })),
                ModuleDefinition.ImportReference(typeof(Profiler).GetMethod("GetMonoUsedSizeLong", new Type[] { })),
                ModuleDefinition.ImportReference(typeof(Profiler).GetMethod("GetMonoHeapSizeLong", new Type[] { }))
            };

            MethodDefinition formatSizeMethod = null;
            for (int j = 0; j < TypeDefinition.Methods.Count; j++)
            {
                MethodDefinition methodDefinition = TypeDefinition.Methods[j];
                if (methodDefinition.Name.Equals(Analyzer.InjectInjectFlagMethod))
                {
                    formatSizeMethod = methodDefinition;
                    break;
                }
            }

            for (int i = 0; i < getMemoryMethodReferences.Length; i++)
            {

                Instruction methodInstruction = null;
                if (i != 3)
                {
                    methodInstruction = ILProcessor.Create(OpCodes.Ldarg, 0);
                    ILProcessor.InsertBefore(MethodFirstInstruction, methodInstruction);
                }

                if (checkInstruction == null)
                {
                    checkInstruction = methodInstruction;
                }

                ILProcessor.InsertBefore(MethodFirstInstruction,
                    ILProcessor.Create(OpCodes.Call, getMemoryMethodReferences[i]));

                // for GC.Collect method, there is no value be generated
                if (i == 3)
                {
                    continue;
                }

                int localIdxOffset = i < 3 ? i : i - 1;

                // Format memory size
                Instruction formatInstruction = ILProcessor.Create(OpCodes.Call, formatSizeMethod);
                ILProcessor.InsertBefore(MethodFirstInstruction, formatInstruction);

                //TODO test pass delete unuse code
//                MethodReference formatMethodRef = ModuleDefinition.ImportReference(
//                    typeof(SizeFormatter).GetMethod("FormatSize", new Type[]{ typeof(long) }));
//                Instruction formatInstruction = ILProcessor.Create(OpCodes.Call, formatMethodRef);
//                ILProcessor.InsertBefore(MethodFirstInstruction, formatInstruction);

                // Store the formatted memory size
                Instruction stLocDataStrInstruction = ILProcessor.Create(OpCodes.Stloc,
                    OriginVariablesCount - logParamsCount + localIdxOffset);
                ILProcessor.InsertBefore(MethodFirstInstruction, stLocDataStrInstruction);
            }

            string logFormatStr = new StringBuilder()
                .Append(MethodDefinition.Name)
                .Append(" - ")
                .Append("Mono heap size: {0}; Mono used size: {1}; Total reserved memory: {2}; " +
                        "Total allocated memory: {3}; Total unused reserved memory: {4}")
                .ToString();

            Instruction ldStrLogFormatStrInstruction = ILProcessor.Create(OpCodes.Ldstr, logFormatStr);
            ILProcessor.InsertBefore(MethodFirstInstruction, ldStrLogFormatStrInstruction);

            Instruction ldcLogFormatParamsCountInstruction = ILProcessor.Create(OpCodes.Ldc_I4, logParamsCount);
            ILProcessor.InsertBefore(MethodFirstInstruction, ldcLogFormatParamsCountInstruction);

            TypeReference logFormatParamTypeReference = ModuleDefinition.ImportReference(typeof(object));
            Instruction newArrLogFormatParamsInstruction = ILProcessor.Create(OpCodes.Newarr,
                logFormatParamTypeReference);
            ILProcessor.InsertBefore(MethodFirstInstruction, newArrLogFormatParamsInstruction);

            // Copy the stored memory size to array
            for (int i = 0; i < logParamsCount; i++)
            {
                Instruction dupLogFormatParamsArrInstruction = ILProcessor.Create(OpCodes.Dup);
                ILProcessor.InsertBefore(MethodFirstInstruction, dupLogFormatParamsArrInstruction);

                Instruction ldcIndexLogFormatParamsInstruction = ILProcessor.Create(OpCodes.Ldc_I4, i);
                ILProcessor.InsertBefore(MethodFirstInstruction, ldcIndexLogFormatParamsInstruction);
                Instruction ldLocIndexParamInstruction = ILProcessor.Create(OpCodes.Ldloc,
                    OriginVariablesCount - i - 1);
                ILProcessor.InsertBefore(MethodFirstInstruction, ldLocIndexParamInstruction);
                Instruction stelemParamRefInstruction = ILProcessor.Create(OpCodes.Stelem_Ref);
                ILProcessor.InsertBefore(MethodFirstInstruction, stelemParamRefInstruction);
            }

            // Print
            MethodReference logFormatMethodReference = ModuleDefinition.ImportReference(
                typeof(Debug).GetMethod("LogFormat", new[] {typeof(string), typeof(object[])}));
            Instruction logMethodInstruction = ILProcessor.Create(OpCodes.Call, logFormatMethodReference);
            ILProcessor.InsertBefore(MethodFirstInstruction, logMethodInstruction);

            // Check enable flag
            InjectCheckEnableCode(checkInstruction, MethodFirstInstruction);
        }
    }
}