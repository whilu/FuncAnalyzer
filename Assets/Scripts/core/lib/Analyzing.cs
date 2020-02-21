using co.lujun.funcanalyzer.imodule;
using co.lujun.funcanalyzer.module;

namespace co.lujun.funcanalyzer
{
    public class Analyzing
    {
        public void Analyze(Flags flags)
        {
            IHandler funcDataHandler = null;
            IHandler runTimeDataHandler = null;

            // default flag, including all analysis in this function
            if ((flags & Flags.Default) != 0)
            {
                flags = Flags.Default | Flags.Args | Flags.Ret | Flags.Time | Flags.Memory;
                funcDataHandler = new FuncDataHandler();
                runTimeDataHandler = new RuntimeDataHandler();
            }
            else if((flags & Flags.Args) != 0 || (flags & Flags.Ret) != 0)
            {
                funcDataHandler = new FuncDataHandler();
            }
            else
            {
                runTimeDataHandler = new RuntimeDataHandler();
            }

            funcDataHandler?.Inject(flags);
            runTimeDataHandler?.Inject(flags);
        }
    }
}


