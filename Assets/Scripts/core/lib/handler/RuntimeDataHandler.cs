using co.lujun.funcanalyzer.imodule;
using Mono.Cecil;

namespace co.lujun.funcanalyzer.module
{
    public class RuntimeDataHandler : IHandler
    {
        public void Inject(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition, Flags flags)
        {
            // including function 'execute time' analyze
            if ((flags & Flags.Time) != 0)
            {

            }

            // including function 'memory data' analyze
            if ((flags & Flags.Memory) != 0)
            {

            }
        }
    }
}