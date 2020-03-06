﻿using System;
using co.lujun.funcanalyzer.attribute;
using co.lujun.funcanalyzer.util;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.Profiling;

namespace co.lujun.funcanalyzer.sample
{
    public class FuncAnalyzerSample : MonoBehaviour
    {
//        [Analyze]
//        private int AnalyzeDefaultSampleFunction(string msg, int level)
//        {
//            int i = 8;
//            return i;
//        }
//
//        [Analyze(AnalyzingFlags = Flags.Args)]
//        private void AnalyzeArgsSampleFunction(string msg, int level, object o)
//        {
//            int i = 8;
//        }
//
//        [Analyze(AnalyzingFlags = Flags.Ret)]
//        private int AnalyzeRetSampleFunction()
//        {
//            int i = 8;
//            return i;
//        }
//
//        [Analyze(AnalyzingFlags = Flags.Time)]
//        private void AnalyzeTimeSampleFunction()
//        {
//            int i = 8;
//        }

        [Analyze(AnalyzingFlags = Flags.Memory, Enable = true)]
        private void AnalyzeMemorySampleFunction()
        {
            int i = 8;
        }

//        [Analyze(AnalyzingFlags = Flags.Args | Flags.Time)]
//        private void AnalyzeArgsAndTimeSampleFunction(string msg, double price)
//        {
//            int i = 8;
//        }
//
//        private void NoAnalyzeSampleFunction()
//        {
//            int i = 8;
//        }

        private void Start()
        {
//            AnalyzeDefaultSampleFunction("Hello world!", 22);
//            AnalyzeArgsSampleFunction("Hello world!", 22, new object());
//            AnalyzeRetSampleFunction();
//            AnalyzeTimeSampleFunction();
            AnalyzeMemorySampleFunction();
            Debug.Log("===hello world");
//            AnalyzeArgsAndTimeSampleFunction("你好", 3.222d);
//            NoAnalyzeSampleFunction();
        }
    }

}