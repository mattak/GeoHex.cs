using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace GeoHex
{
    public static class FileUtil
    {
        public static List<string[]> ParseCsv(string path)
        {
            List<string[]> wordsList = new List<string[]>();

            using (StreamReader reader = new StreamReader(path))
            {
                while (reader.Peek() >= 0)
                {
                    string lineString = reader.ReadLine();
                    if (lineString[0] == '#')
                    {
                        continue;
                    }

                    string[] words = lineString.Split(new Char[] {','});
                    wordsList.Add(words);
                }
            }

            return wordsList;
        }

        public static string GetTestFilePath(string path)
        {
            return System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "../..", path);
        }
    }
}
