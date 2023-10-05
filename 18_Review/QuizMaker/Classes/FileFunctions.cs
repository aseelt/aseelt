using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker.Classes
{
    public class FileFunctions : IDataAccess
    {
        //6. let's create a variable to store the file
        //he's put it in a constant here, it makes it easier to work with in this example
        //instead you'd use the code from word search instead to make it dynamic
        private const string INPUTFILE = "sample-quiz-file.txt";

        //7. we will need to generate a list of questions
        // but we should check if the file path given to us leads to a file that exists
        //(in this example it's hardcoded... but not in other examples where we rely on user input)
        //but that is two things
        //we can break the second thing out from the first in a separate method
        //9. so now, coming back here, we can make the list of questions
        //in order to do that, the logical order is to first check if the file exists
        //(using the method we made), then process the file into a list, then give list as an output
        //processing the file will be multiple steps, so we should do a separate method for it
        //that method should stay here, since it's a function of the file
        // the list doesn't stay here though, the purpose of the filefunction is to process it and return it,
        // not hold on to it, so not a property
        //create the processing method below
        public List<QuizQuestion> GenerateListOfQuestions()
        {
            // validate the file
            if(ValidateFile())
            {
                return LoadUpQuestions();
            }
            else
            {
                return null;
            }
        }
        //8. lets make a method just to check if the file exists
        private bool ValidateFile()
        {
            return File.Exists(INPUTFILE);
        }
        //10. have to open the file to process it line by line
        //each line has different elements in it though, we want those broken out
        //so each line has to have a class for the question
        //so each line should be in a list. We know the method should only live here
        //and be called here, so it's private        
        private List<QuizQuestion> LoadUpQuestions()
        {
            //container for the output so we can use it out of the loops below
            List<QuizQuestion> output = new List<QuizQuestion>();
            // load the file line by line
            //11.try catch because we're loading a file
            try
            {
                using (StreamReader sr = new StreamReader(INPUTFILE))
                {
                    //try, using, while not at the end of the stream
                    while(!sr.EndOfStream)
                    {
                        //12. read the line
                        string line = sr.ReadLine();
                        // generate a question for each line
                        //13. we'll come back to why it's in a try catch here again TODO
                        //24. if the line we're passing is null or empty (e.g. empty line break)
                        //in the active and valid file, it'll throw that exception we've created
                        //this try catch will catch that throw and provide an exception
                        try
                        {
                            //14. we get a singular line out, but it has multiple components
                            //we need to hold those components in one object
                            //so we need to create it's own class, then populate it with
                            //the question, answers, and right answer
                            //time to create a class, then come back here
                            //23. okay now we have a class we can call using its constructor
                            //do so, passing the line we're working with
                            //go to 24 (after 13)
                            QuizQuestion temp = new QuizQuestion(line);
                            //25.then this class question gets added to the output list
                            //26. so now we know how to load our file, and manipulate the data
                            //27. we are ready for the UI
                            // add it to the list
                            output.Add(temp);
                        }
                        catch (Exception)
                        {
                            //44.call the log class, you don't have to instantiate it
                            //call the write method
                            //if the exception is thrown, this action happens and your log gets a new entry
                            //and your program is done. Talk about a web!
                            Log.WriteLog("Couldn't read the line to make a question.");
                        }
                    }
                }
            }
            catch (Exception)
            {
                // TODO Deal with invalid question
            }
            // return the list
            return output;
        }

    }
}
