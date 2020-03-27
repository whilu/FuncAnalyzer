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

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace co.lujun.funcanalyzer.imodule
{
    public interface IHandler
    {
        ModuleDefinition ModuleDefinition { get; set; }
        TypeDefinition TypeDefinition { get; set; }
        MethodDefinition MethodDefinition { get; set; }
        ILProcessor ILProcessor { get; set; }
        Instruction MethodFirstInstruction { get; set; }
        Instruction MethodLastInstruction { get; set; }
        int OriginVariablesCount { get; }
        int LocalMethodVariablesCount { get; set; }
        int Flags { get; set; }
        bool Enable { get; set; }
        void Inject(ModuleDefinition moduleDefinition, TypeDefinition typeDefinition, MethodDefinition methodDefinition,
            bool enable, int flags);
        void InjectCheckEnableCode(Instruction insertInstruction, Instruction brInstruction);
    }
}