using System;

namespace co.lujun.funcanalyzer.util
{
    public class SizeFormatter
    {
        private static readonly string[] Suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

        public static string FormatSize(long bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, Suffixes[counter]);
        }
    }
}