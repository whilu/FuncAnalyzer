using co.lujun.funcanalyzer.attribute;
using UnityEngine;

namespace co.lujun.funcanalyzer.sample
{
    public class FuncAnalyzerSample : MonoBehaviour
    {
        [Analyze(Enable = false)]
        private int AnalyzeDefaultSampleFunction(string msg, int level)
        {
            int i = 8;
            return i;
        }

        [Analyze(AnalyzingFlags = Flags.Args, Enable = false)]
        private void AnalyzeArgsSampleFunction(string msg, int level, object o)
        {
            int i = 8;
        }

        [Analyze(AnalyzingFlags = Flags.Ret, Enable = false)]
        private int AnalyzeRetSampleFunction()
        {
            int i = 8;
            return i;
        }

        [Analyze(AnalyzingFlags = Flags.Time, Enable = true)]
        private void AnalyzeTimeSampleFunction()
        {
            int i = 8;
        }

        [Analyze(AnalyzingFlags = Flags.Memory)]
        private void AnalyzeMemorySampleFunction()
        {
            int i = 8;
        }

        [Analyze(AnalyzingFlags = Flags.Args | Flags.Time, Enable = true)]
        private void AnalyzeArgsAndTimeSampleFunction(string msg, double price)
        {
            int i = 8;
        }

        private void NoAnalyzeSampleFunction()
        {
            int i = 8;
        }

        private void Start()
        {
            AnalyzeDefaultSampleFunction("Hello world!", 22);
            AnalyzeArgsSampleFunction("Hello world!", 22, new object());
            AnalyzeRetSampleFunction();
            AnalyzeTimeSampleFunction();
            AnalyzeMemorySampleFunction();
            AnalyzeArgsAndTimeSampleFunction("你好", 3.222d);
            NoAnalyzeSampleFunction();
        }
    }

}
