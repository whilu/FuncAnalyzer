/*
* Copyright 2020 lujun
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using co.lujun.funcanalyzer.imodule;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace co.lujun.funcanalyzer.handler
{
    public abstract class HandlerImpl : IHandler
    {
        public ModuleDefinition ModuleDefinition { get; set; }

        public TypeDefinition TypeDefinition { get; set; }

        public MethodDefinition MethodDefinition { get; set; }

        public ILProcessor ILProcessor { get; set; }

        public Instruction MethodFirstInstruction { get; set; }

        public Instruction MethodLastInstruction { get; set; }

        public int OriginVariablesCount
        {
            get { return MethodDefinition.Body.Variables.Count; }
        }

        public int LocalMethodVariablesCount { get; set; }

        public Flags Flags { get; set; }

        public bool Enable { get; set; }

        public virtual void Inject(ModuleDefinition moduleDefinition, TypeDefinition typeDefinition,
            MethodDefinition methodDefinition, bool enable, Flags flags)
        {
            ModuleDefinition = moduleDefinition;
            TypeDefinition = typeDefinition;
            MethodDefinition = methodDefinition;
            ILProcessor = methodDefinition.Body.GetILProcessor();
            MethodFirstInstruction = methodDefinition.Body.Instructions[0];
            MethodLastInstruction = methodDefinition.Body.Instructions[methodDefinition.Body.Instructions.Count - 1];
            Flags = flags;
            Enable = enable;
            LocalMethodVariablesCount = MethodDefinition.Body.Variables.Count;
            MethodDefinition.Body.Variables.Add(
                new VariableDefinition(ModuleDefinition.ImportReference(typeof(bool))));
        }

        public void InjectCheckEnableCode(Instruction insertInstruction, Instruction brInstruction)
        {
            // Check is analyze enable
            ILProcessor.InsertBefore(insertInstruction, ILProcessor.Create(OpCodes.Ldc_I4, Enable ? 1 : 0));
            ILProcessor.InsertBefore(insertInstruction, ILProcessor.Create(OpCodes.Stloc, LocalMethodVariablesCount));
            ILProcessor.InsertBefore(insertInstruction, ILProcessor.Create(OpCodes.Ldloc, LocalMethodVariablesCount));
            ILProcessor.InsertBefore(insertInstruction, ILProcessor.Create(OpCodes.Brfalse, brInstruction));
        }
    }
}