using System;
using System.Collections.Generic;
using System.IO;

namespace LegoBuilder.Utilities
{
    public static class CreationUpdateLog
    {
        public static void WriteLog(string action, string item, string text, Dictionary<string, string> results = null)
        {
            try
            {
                DateTime date = DateTime.UtcNow;
                int year = date.Year;
                int month = date.Month;
                int day = date.Day;
                using (StreamWriter sw = new StreamWriter($"Log/CreationUpdateLog_{year}_{month}_{day}.txt", true))
                {
                    sw.WriteLine($"{action} | {item} | {text} | {DateTime.UtcNow}");
                    if (results != null)
                    {
                        foreach (KeyValuePair<string, string> result in results)
                        {
                            sw.WriteLine($"{item} | {result.Key} | {result.Value}");
                        }
                    }
                    sw.WriteLine("--- END OF ACTION ---");
                    sw.WriteLine("");
                    sw.WriteLine("");
                    sw.WriteLine("");
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
