using System;

namespace co.lujun.funcanalyzer.attribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class AnalyzeAttribute : Attribute
    {
        public Flags AnalyzingFlags { get; set; }
        public bool Enable { get; set; }

        public AnalyzeAttribute()
        {
            AnalyzingFlags = Flags.Default;
            Enable = true;
        }
    }
}