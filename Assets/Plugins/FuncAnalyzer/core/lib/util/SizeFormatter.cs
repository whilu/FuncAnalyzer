using System;

namespace co.lujun.funcanalyzer.util
{
    public class SizeFormatter
    {
        public static string FormatSize(long bytes)
        {
            string[] Suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };

            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return string.Format("{0:n1}{1}", number, Suffixes[counter]);
        }
    }
}