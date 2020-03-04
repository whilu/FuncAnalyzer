using co.lujun.funcanalyzer.imodule;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace co.lujun.funcanalyzer.handler
{
    public abstract class HandlerImpl : IHandler
    {
        public ModuleDefinition ModuleDefinition { get; set; }

        public MethodDefinition MethodDefinition { get; set; }

        public MethodDefinition InjectFlagMethodDefinition { get; set; }

        public ILProcessor ILProcessor { get; set; }

        public Instruction MethodFirstInstruction { get; set; }

        public Instruction MethodLastInstruction { get; set; }

        public int OriginVariablesCount
        {
            get { return MethodDefinition.Body.Variables.Count; }
        }

        public Flags Flags { get; set; }

        public virtual void Inject(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition,
            MethodDefinition injectFlagMethodDefinition, Flags flags)
        {
            ModuleDefinition = moduleDefinition;
            MethodDefinition = methodDefinition;
            InjectFlagMethodDefinition = injectFlagMethodDefinition;
            ILProcessor = methodDefinition.Body.GetILProcessor();
            MethodFirstInstruction = methodDefinition.Body.Instructions[0];
            MethodLastInstruction = methodDefinition.Body.Instructions[methodDefinition.Body.Instructions.Count - 1];
            Flags = flags;
        }
    }
}