using co.lujun.funcanalyzer.util;
using UnityEditor;

namespace co.lujun.funcanalyzer.editor
{
    public class FuncAnalyzerToolMenu
    {
        private const string BaseMenuLabel = "FuncAnalyzer/";

        [MenuItem(BaseMenuLabel + "Inject analysis code")]
        public static void Inject()
        {
            AssemblyFileUtil.BackupAssemblyFile();
            Analyzer.Instance.Inject(Common.AssemblyPath);
        }

        [MenuItem(BaseMenuLabel + "Remove analysis code")]
        public static void RemoveAnalysisCode()
        {
            Analyzer.Instance.ResetAnalysisCode(Common.AssemblyPath, Common.AssemblyBakPath);
            AssetDatabase.Refresh();
        }

        [MenuItem(BaseMenuLabel + "Auto inject")]
        public static void SwitchAutoInject()
        {

        }
    }
}