using UnityEditor;

namespace co.lujun.funcanalyzer.editor
{
    public class FuncAnalyzerToolMenu
    {
        private const string BaseMenuLabel = "FuncAnalyzer/";
        private const string AssemblyPath = "./Library/ScriptAssemblies/Assembly-CSharp.dll";

        [MenuItem(BaseMenuLabel + "Inject analysis code")]
        public static void Inject()
        {
            Analyzer.Instance.Inject(AssemblyPath);
        }
    }
}