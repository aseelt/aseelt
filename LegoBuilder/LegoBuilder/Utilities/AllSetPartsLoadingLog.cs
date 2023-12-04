using LegoBuilder.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LegoBuilder.Utilities
{
    public static class AllSetPartsLoadingLog
    {
        // just need a barebones file of what's loaded or not
        public static void WriteLog(string setNumber)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter($"Log/AllSetPartsLoadingLog.txt", true))
                {
                    sw.WriteLine($"{setNumber}");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error occurred writing {setNumber} to the file, check to see if it was loaded");
            }
        }

        // for when a part is missing, log an error, and skip and keep going with the load
        public static void PartLoadError(ResultsSetParts setPart)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter($"Log/AllSetPartsLoading_PartLoadError.txt", true))
                {
                    sw.WriteLine($"{setPart.Part.Part_Num}, {setPart.Part.Name}, belonging to {setPart.Set_Num} was not loaded");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error occurred writing {setPart.Part.Part_Num}, {setPart.Part.Name}, belonging to {setPart.Set_Num} to the file, check to see if it was loaded");
            }
        }
        public static List<string> GetLog()
        {
            List<string> output = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader($"Log/AllSetPartsLoadingLog.txt", true))
                {
                    while (!sr.EndOfStream)
                    {
                        output.Add(sr.ReadLine());
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error occurred reading the file, you need to restart this process");
            }
            return output;
        }
    }
}
