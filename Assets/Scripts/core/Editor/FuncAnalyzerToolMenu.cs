using System.IO;
using co.lujun.funcanalyzer.util;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

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
            // TODO how to deal unity assembly cache?
            AssemblyFileUtil.ResetBackupAssemblyFile();

            string className = "FuncAnalyzer_tmp_cl_" + (int)(Random.value * 10);
            string classPath = "Assets/" + className + ".cs";

            if(!File.Exists(classPath)){
                using (StreamWriter outfile = new StreamWriter(classPath))
                {
                    outfile.WriteLine("namespace co.lujun.funcanalyzer{");
                    outfile.WriteLine("public class " + className + "{}}");
                }
            }
            else
            {
                File.Delete(classPath);
            }
            AssetDatabase.Refresh();

            Debug.Log("Analysis code have been removed!");
        }

        [MenuItem(BaseMenuLabel + "Auto inject")]
        public static void SwitchAutoInject()
        {

        }
    }
}