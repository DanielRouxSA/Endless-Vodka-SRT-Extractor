using System;

namespace EndlessVodkaSRTExtractor
{
    public static class CustomConsole
    {
        public static void WriteLineWithBreak(string value)
        {
            Console.WriteLine(value);
            Console.WriteLine();
        }
    }
}
