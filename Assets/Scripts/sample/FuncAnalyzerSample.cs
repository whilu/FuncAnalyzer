using co.lujun.funcanalyzer.attribute;
using UnityEngine;

namespace co.lujun.funcanalyzer.sample
{
    public class FuncAnalyzerSample : MonoBehaviour
    {
//        [Analyze]
//        private int AnalyzeDefaultSampleFunction(string msg, int level)
//        {
//            int i = 8;
//            return i;
//        }

        [Analyze(AnalyzingFlags = Flags.Args)]
        private void AnalyzeArgsSampleFunction(string msg, int level, object o)
        {
            int i = 8;
        }

//        [Analyze(AnalyzingFlags = Flags.Ret)]
//        private int AnalyzeRetSampleFunction()
//        {
//            int i = 8;
//            return i;
//        }

//        [Analyze(AnalyzingFlags = Flags.Time)]
//        private void AnalyzeTimeSampleFunction()
//        {
//            int i = 8;
//        }
//
//        [Analyze(AnalyzingFlags = Flags.Memory)]
//        private void AnalyzeMemorySampleFunction()
//        {
//            int i = 8;
//        }

//        [Analyze(AnalyzingFlags = Flags.Args | Flags.Time)]
//        private void AnalyzeArgsAndTimeSampleFunction()
//        {
//            int i = 8;
//        }

//        private void NoAnalyzeSampleFunction()
//        {
//            int i = 8;
//        }

        private void Start()
        {
//            AnalyzeDefaultSampleFunction("Hello world!", 22);
            AnalyzeArgsSampleFunction("Hello world!", 22, new object());
//            AnalyzeRetSampleFunction();
//            AnalyzeTimeSampleFunction();
//            AnalyzeMemorySampleFunction();
//            AnalyzeArgsAndTimeSampleFunction();
//            NoAnalyzeSampleFunction();
        }

        private string Test(string a)
        {
            int i = 9;
            string b = i + a;
            return b;
        }
    }

}
