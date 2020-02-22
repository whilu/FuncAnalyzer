using Mono.Cecil;

namespace co.lujun.funcanalyzer.imodule
{
    public interface IHandler
    {
        void Inject(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition, Flags flags);
    }
}