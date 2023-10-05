using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordSearch.Classes
{
    public class FileFunctions
    {
        //4. start by getting your file. Create a property for where you will store it
        //make the property private - it's only available in here and you can only access it through a method called in here
        //e.g. you could have a method saying .GetInputFile which makes it read only, through the method, not directly
        //5. then create this class's object when you get the file name - use a constructor
        //this requires asking the user to give you the file name, so go back to the UI
        private string InputFile { get; }
        public FileFunctions(string fileIN)
        {
            InputFile = fileIN;
        }
        //10. create a check if valid method. All it does is check if the inputfile, as a parameter here is valid
        //because you're using the property of this class, and it's initialized at the top, you can call it directly
        //in the method, you don't need parameters. 
        //head back to your UI so you can use it
        public bool CheckIfValid()
        {
            return File.Exists(InputFile);
        }
        //20. now we've got our file checked to see if it's valid, what we're looking for, and the case,
        //we can do the meat and potatoes of our action on the file
        public string SearchForWord(string wordToFind,bool isSensitiveToCase)
        {
            //21. lets create a holding variable for our output
            string output = "";
            //22. we want a try catch because we're opening a file
            //then open the file
            //just get used to this code block style, it's used for all text files
            try
            {
                //use the property we have in this file
                using (StreamReader sr = new StreamReader(InputFile))
                {
                    //counter so you can call this later
                    //lives outside the loop
                    int lineNumber = 0;
                    //continue while you are not at the end of the file
                    while(!sr.EndOfStream)
                    {
                        //read the line (don't forget this...)
                        string line = sr.ReadLine();
                        //store that line in a checking variable
                        string lineToCheck = line;
                        //add to the line count
                        lineNumber++;
                        //if the parameter for this method says you're NOT not case sensitive, do this
                        //has to be a double negative if you think through the logic
                        if(!isSensitiveToCase)
                        {
                            //flatten the line you are checking to lowercase
                            //flatten the word you're looking for to lowercase
                            lineToCheck = lineToCheck.ToLower();
                            wordToFind = wordToFind.ToLower();
                        }
                        //and add the original line you're working with to the output
                        //this adds the line whether you're working with lowercase or not (in the above if block)
                        //this needs to run even if the above is true/false, so they're not connected with an else
                        
                        if(lineToCheck.Contains(wordToFind))
                        {
                            output += lineNumber + ") " + line + "\r\n";
                        }
                    }
                }
                //20continued - return the output to the calling method in the UI
                //lets go back to the UI
                return output;
            }
            //22b. need the catch for the exception
            //if you can't open the file, the filefunctions should catch the exception here
            catch (Exception e)
            {
                return output;
            }
        }

    }
}
