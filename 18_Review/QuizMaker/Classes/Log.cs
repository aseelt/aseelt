using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker.Classes
{
    public static class Log
    {
        //41. the log is public and static
        //this allows its use everywhere so you can record different types of errors
        //42. create a method for it
        //can be void, you don't want anything back
        public static void WriteLog(string reason)
        {
            //43. since you're working with writing a file, use a try catch
            try
            {
                //44. write a file named error.txt and append to it (rather than overwrite,
                //which would be a missing second parameter, or the 2nd parameter would be false)
                using (StreamWriter sw = new StreamWriter("error.txt", true))
                {
                    //write the line
                    //lets go back and use this error log function when you try and create a quiz
                    //question but there is no line input
                    //go to filefunctions, in your While(!sr.Endofstream)
                    sw.WriteLine($"{DateTime.UtcNow} {reason} local time {DateTime.Now}");
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
