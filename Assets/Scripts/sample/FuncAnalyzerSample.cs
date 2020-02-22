using co.lujun.funcanalyzer.attribute;
using UnityEngine;

namespace co.lujun.funcanalyzer.sample
{

    public class FuncAnalyzerSample : MonoBehaviour
    {
        [Analyze]
        private void AnalyzeDefaultSampleFunction()
        {
            int i = 8;
        }

        [Analyze(AnalyzingFlags = Flags.Args)]
        private void AnalyzeArgsSampleFunction()
        {
            int i = 8;
        }

        [Analyze(AnalyzingFlags = Flags.Ret)]
        private void AnalyzeRetSampleFunction()
        {
            int i = 8;
        }

        [Analyze(AnalyzingFlags = Flags.Time)]
        private void AnalyzeTimeSampleFunction()
        {
            int i = 8;
        }

        [Analyze(AnalyzingFlags = Flags.Memory)]
        private void AnalyzeMemorySampleFunction()
        {
            int i = 8;
        }

        [Analyze(AnalyzingFlags = Flags.Args | Flags.Time)]
        private void AnalyzeArgsAndTimeSampleFunction()
        {
            int i = 8;
        }

        private void NoAnalyzeSampleFunction()
        {
            int i = 8;
        }
    }

}
