using System;
using WordSearch.Classes;

namespace WordSearch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            UI ui = new UI();
            ui.Run();

            //-1. the general steps of what you want to do
            // in program cs, only call the UI, don't have any code in here typically
            // allows for ease of functionality
            // start with how you're getting the data
            // then figure out the UI

            //A. Ask the user for the file path
            //B. Ask the user for the search string
            //C. Open the file
            //D. Loop through each line in the file
            //E. If the line contains the search string, print it out along with its line number
        }
    }
}
