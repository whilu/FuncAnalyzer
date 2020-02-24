using System.IO;
using UnityEditor;
using UnityEngine;

namespace co.lujun.funcanalyzer.util
{
    public class AssemblyFileUtil
    {
        public static void BackupAssemblyFile()
        {
            if (!Directory.Exists(Common.FuncAnalyzerPath))
            {
                Directory.CreateDirectory(Common.FuncAnalyzerPath);
            }

            if (File.Exists(Common.AssemblyPathBakPath))
            {
                File.Delete(Common.AssemblyPathBakPath);
            }

            FileUtil.CopyFileOrDirectory(Common.AssemblyPath, Common.AssemblyPathBakPath);
        }

        public static void ResetBackupAssemblyFile()
        {
            if (!File.Exists(Common.AssemblyPathBakPath))
            {
                Debug.LogFormat("No assembly dll backup found in {0}", Common.AssemblyPathBakPath);
                return;
            }

            if (File.Exists(Common.AssemblyPath))
            {
                File.Delete(Common.AssemblyPath);
            }

            FileUtil.CopyFileOrDirectory(Common.AssemblyPathBakPath, Common.AssemblyPath);
        }
    }
}