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
        Flags Flags { get; set; }
        void Inject(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition, Flags flags);
    }
}