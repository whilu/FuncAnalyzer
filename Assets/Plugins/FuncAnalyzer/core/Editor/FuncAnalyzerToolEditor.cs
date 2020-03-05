using UnityEditor;
using UnityEngine;

namespace co.lujun.funcanalyzer.editor
{
    [InitializeOnLoad]
    public class FuncAnalyzerToolEditor
    {
        private const string Pkg = "co.lujun.funcanalyzer.editor";
        private const string AutoInjectKey = Pkg + ".autoInject";

        private const string BaseMenuLabel = "FuncAnalyzer/";
        private const string InjectAnalysisCodeMenuLabel = BaseMenuLabel + "Inject analysis code";
        private const string AutoInjectMenuLabel = BaseMenuLabel + "Auto inject";


        private static bool _enableAutoInject;
        static FuncAnalyzerToolEditor()
        {
            _enableAutoInject = EditorPrefs.GetBool(AutoInjectKey, true);

            EditorApplication.delayCall += () => { PerformToggleAction(); };
        }

        [MenuItem(InjectAnalysisCodeMenuLabel, false, 1)]
        public static void Inject()
        {
            Analyzer.Instance.Inject("./Library/ScriptAssemblies/Assembly-CSharp.dll", InjectCallback);
            Analyzer.Instance.Inject("./Library/ScriptAssemblies/Assembly-CSharp-firstpass.dll", InjectCallback);
            Analyzer.Instance.Inject("./Library/ScriptAssemblies/Assembly-CSharp-Editor.dll", InjectCallback);
        }

        [MenuItem(AutoInjectMenuLabel, false, 3)]
        static void ToggleAutoBind()
        {
            _enableAutoInject = !_enableAutoInject;
            PerformToggleAction();

            Debug.LogFormat("{0} auto inject", _enableAutoInject ? "Enable" : "Disable");
        }

        static void PerformToggleAction()
        {
            Menu.SetChecked(AutoInjectMenuLabel, _enableAutoInject);
            EditorPrefs.SetBool(AutoInjectKey, _enableAutoInject);
        }

        static void InjectCallback(float progress, string desc)
        {
            if (progress < 1.0f)
            {
                EditorUtility.DisplayProgressBar("FuncAnalyzer Inject Progress", desc, progress);
            }
            else
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static void AutoInject()
        {
            if (_enableAutoInject)
            {
                Inject();
            }
        }
    }
}