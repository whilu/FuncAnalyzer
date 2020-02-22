using co.lujun.funcanalyzer.imodule;
using Mono.Cecil;

namespace co.lujun.funcanalyzer.module
{
    public class FuncDataHandler : IHandler
    {
        public void Inject(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition, Flags flags)
        {
            // including function 'args' analyze
            if((flags & Flags.Args) != 0)
            {
                GenerateAnalysisCodeForArgs(moduleDefinition, methodDefinition);
            }

            // including function 'return value' analyze
            if ((flags & Flags.Ret) != 0)
            {
                GenerateAnalysisCodeForRet(moduleDefinition, methodDefinition);
            }
        }

        private void GenerateAnalysisCodeForArgs(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition)
        {

        }


        private void GenerateAnalysisCodeForRet(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition)
        {

        }
    }
}