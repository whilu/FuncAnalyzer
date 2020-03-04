using UnityEditor;
using UnityEngine;

namespace co.lujun.funcanalyzer.editor
{
    [InitializeOnLoad]
    public class FuncAnalyzerToolEditor
    {
        private const string Pkg = "co.lujun.funcanalyzer.editor";
        private const string AutoInjectKey = Pkg + ".autoInject";
        private const string EnableAnalysisKey = Pkg + ".enableAnalysis";

        private const string BaseMenuLabel = "FuncAnalyzer/";
        private const string InjectAnalysisCodeMenuLabel = BaseMenuLabel + "Inject analysis code";
        private const string EnableAnalysisMenuLabel = BaseMenuLabel + "Enable/Disable analysis";
        private const string AutoInjectMenuLabel = BaseMenuLabel + "Auto inject";


        private static bool _enableAutoInject, _enableAnalysis;
        static FuncAnalyzerToolEditor()
        {
            _enableAutoInject = EditorPrefs.GetBool(AutoInjectKey, true);
            _enableAnalysis = EditorPrefs.GetBool(EnableAnalysisKey, true);

            EditorApplication.delayCall += () => { PerformToggleAction(); };
        }

        [MenuItem(InjectAnalysisCodeMenuLabel)]
        public static void Inject()
        {
            Analyzer.Instance.Inject("./Library/ScriptAssemblies/Assembly-CSharp.dll",
                _enableAnalysis, InjectCallback);
            Analyzer.Instance.Inject("./Library/ScriptAssemblies/Assembly-CSharp-firstpass.dll",
                _enableAnalysis, InjectCallback);
            Analyzer.Instance.Inject("./Library/ScriptAssemblies/Assembly-CSharp-Editor.dll",
                _enableAnalysis, InjectCallback);
        }

        [MenuItem(EnableAnalysisMenuLabel)]
        static void ToggleEnableAnalysis()
        {
            _enableAnalysis = !_enableAnalysis;
            PerformToggleAction();

            Debug.LogFormat("{0} analysis", _enableAnalysis ? "Enable" : "Disable");
        }

        [MenuItem(AutoInjectMenuLabel)]
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

            Menu.SetChecked(EnableAnalysisMenuLabel, _enableAnalysis);
            EditorPrefs.SetBool(EnableAnalysisKey, _enableAnalysis);
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