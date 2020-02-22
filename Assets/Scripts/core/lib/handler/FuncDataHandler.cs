using Mono.Cecil;

namespace co.lujun.funcanalyzer.handler
{
    public class FuncDataHandler : HandlerImpl
    {
        public override void Inject(ModuleDefinition moduleDefinition, MethodDefinition methodDefinition, Flags flags)
        {
            base.Inject(moduleDefinition, methodDefinition, flags);

            // including function 'args' analyze
            if((flags & Flags.Args) != 0)
            {
                GenerateAnalysisCodeForArgs();
            }

            // including function 'return value' analyze
            if ((flags & Flags.Ret) != 0)
            {
                GenerateAnalysisCodeForRet();
            }
        }

        private void GenerateAnalysisCodeForArgs()
        {

        }


        private void GenerateAnalysisCodeForRet()
        {

        }
    }
}