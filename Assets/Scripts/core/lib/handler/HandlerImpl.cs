using co.lujun.funcanalyzer.imodule;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace co.lujun.funcanalyzer.handler
{
    public abstract class HandlerImpl : IHandler
    {
        public ModuleDefinition ModuleDefinition { get; set; }

        public MethodDefinition MethodDefinition { get; set; }

        public ILProcessor ILProcessor { get; set; }

        public Instruction MethodFirstInstruction { get; set; }

        public Instruction MethodLastInstruction { get; set; }

        public int OriginVariablesCount { get; set; }

        public Flags Flags { get; set; }

        public virtual void Inject(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition, Flags flags)
        {
            ModuleDefinition = moduleDefinition;
            MethodDefinition = methodDefinition;
            ILProcessor = methodDefinition.Body.GetILProcessor();
            OriginVariablesCount = methodDefinition.Body.Variables.Count;
            MethodFirstInstruction = methodDefinition.Body.Instructions[0];
            MethodLastInstruction = methodDefinition.Body.Instructions[methodDefinition.Body.Instructions.Count - 1];
            Flags = flags;
        }
    }
}