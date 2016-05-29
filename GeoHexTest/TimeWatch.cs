using System;
using System.IO;

namespace GeoHex
{
    public static class TimeWatch
    {
        private static long _currentTime = 0;
        private static long _count = 0;
        private static long _totalTime = 0;

        public static void Reset()
        {
            _currentTime = 0;
            _totalTime = 0;
            _count = 0;
        }

        public static void Resume()
        {
            _currentTime = CurrentTimeMillis();
        }

        public static long Pause(int countup = 1)
        {
            long diff = CurrentTimeMillis() - _currentTime;
            _count += countup;
            _totalTime += diff;
            return diff;
        }

        public static void OutputResult(string tag)
        {
            PrintResult(tag);
            WriteResult(tag);
        }

        public static void PrintResult(string tag)
        {
            Console.WriteLine($"{tag}\t{_count}\t{_totalTime}\t{(double) _totalTime/_count}");
        }

        public static void WriteResult(string tag)
        {
            using (var sw = new StreamWriter(
                "/tmp/geohex_test.tsv",
                true,
                System.Text.Encoding.GetEncoding("utf-8")
                ))
            {
                sw.WriteLine($"{tag}\t{_count}\t{_totalTime}\t{(double) _totalTime/_count}");
                sw.Close();
            }
        }

        private static long CurrentTimeMillis()
        {
            System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            return (long) (System.DateTime.UtcNow - epochStart).TotalMilliseconds;
        }
    }
}
