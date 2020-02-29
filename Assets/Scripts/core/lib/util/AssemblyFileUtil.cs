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

            if (File.Exists(Common.AssemblyBakPath))
            {
                File.Delete(Common.AssemblyBakPath);
            }

            if (File.Exists(Common.AssemblyPdbBakPath))
            {
                File.Delete(Common.AssemblyPdbBakPath);
            }

            FileUtil.CopyFileOrDirectory(Common.AssemblyPath, Common.AssemblyBakPath);
            FileUtil.CopyFileOrDirectory(Common.AssemblyPdbPath, Common.AssemblyPdbBakPath);
        }
    }
}