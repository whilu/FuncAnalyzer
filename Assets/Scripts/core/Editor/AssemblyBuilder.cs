using System.IO;
using UnityEditor;
using UnityEngine;

namespace co.lujun.funcanalyzer.editor
{
    [InitializeOnLoad]
    public class AssemblyBuilder : AssetPostprocessor
    {
        static AssemblyBuilder(){}

        [UnityEditor.Callbacks.DidReloadScripts]
        static void ReloadedScripts()
        {
//            Debug.Log("ReloadedScripts");
        }


    }
}