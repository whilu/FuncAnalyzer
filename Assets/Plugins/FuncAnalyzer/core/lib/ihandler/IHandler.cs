using Mono.Cecil;
using Mono.Cecil.Cil;

namespace co.lujun.funcanalyzer.imodule
{
    public interface IHandler
    {
        ModuleDefinition ModuleDefinition { get; set; }
        MethodDefinition MethodDefinition { get; set; }
        ILProcessor ILProcessor { get; set; }
        Instruction MethodFirstInstruction { get; set; }
        Instruction MethodLastInstruction { get; set; }
        int OriginVariablesCount { get; }
        int LocalMethodVariablesCount { get; set; }
        Flags Flags { get; set; }
        bool Enable { get; set; }
        void Inject(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition, bool enable, Flags flags);
        void InjectCheckEnableCode(Instruction insertInstruction, Instruction brInstruction);
    }
}