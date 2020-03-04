using UnityEditor;

namespace co.lujun.funcanalyzer.editor
{
    [InitializeOnLoad]
    public class AssemblyBuilder : AssetPostprocessor
    {
        static AssemblyBuilder(){}

        [UnityEditor.Callbacks.DidReloadScripts]
        static void ReloadedScripts()
        {
            FuncAnalyzerToolEditor.AutoInject();
        }
    }
}