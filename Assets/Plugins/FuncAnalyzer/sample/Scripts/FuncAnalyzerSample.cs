﻿/*
* Copyright 2020 lujun
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using co.lujun.funcanalyzer.attribute;
using UnityEngine;

namespace co.lujun.funcanalyzer.sample
{
    public class FuncAnalyzerSample : MonoBehaviour
    {
        [Analyze(Enable = false)]
        private int AnalyzeDefaultSampleFunction(string msg, int level)
        {
            int i = 8;
            return i;
        }

        [Analyze(AnalyzingFlags = Flags.Args, Enable = false)]
        private void AnalyzeArgsSampleFunction(string msg, int level, object o)
        {
            int i = 8;
        }

        [Analyze(AnalyzingFlags = Flags.Ret, Enable = false)]
        private int AnalyzeRetSampleFunction()
        {
            int i = 8;
            return i;
        }

        [Analyze(AnalyzingFlags = Flags.Time, Enable = true)]
        private void AnalyzeTimeSampleFunction()
        {
            int i = 8;
        }

        [Analyze(AnalyzingFlags = Flags.Memory)]
        private void AnalyzeMemorySampleFunction()
        {
            int i = 8;
        }

        [Analyze(AnalyzingFlags = Flags.Args | Flags.Time, Enable = true)]
        private void AnalyzeArgsAndTimeSampleFunction(string msg, double price)
        {
            int i = 8;
        }

        private void NoAnalyzeSampleFunction()
        {
            int i = 8;
        }

        private void Start()
        {
            AnalyzeDefaultSampleFunction("Hello world!", 22);
            AnalyzeArgsSampleFunction("Hello world!", 22, new object());
            AnalyzeRetSampleFunction();
            AnalyzeTimeSampleFunction();
            AnalyzeMemorySampleFunction();
            AnalyzeArgsAndTimeSampleFunction("你好", 3.222d);
            NoAnalyzeSampleFunction();
        }
    }

}
