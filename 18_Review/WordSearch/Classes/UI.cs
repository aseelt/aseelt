using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordSearch.Classes
{
    public class UI
    {
        //0. you want to separate which class interacts with which entity
        //so the user only interacts with the UI, a person shouldn't speak to the filefunctions class directly
        //and likewise, the filefunctions shouldn't speak to a user directly
        //similarly, the user shouldn't speak to the file directly, that's filefunctions job. The file only 
        //interacts with file functions, its own class/object, directly
        //this is better encapsulation

        //1. everything has to go in a method, property or constructor, you can't code directly into the class
        //2. break up methods into as small an action as possible. If it's longer than 10-15 lines,
        //think about what you can pull out and make a reusable building block
        //so there will be one major block where your program happens, and several helper methods below it
        //3. start with getting your file, and that starts with your file function class
        //NOT HERE...

        /// <summary>
        /// This is our application, the business logic
        /// </summary>
        /// <returns></returns>
        /// <returns></returns>
        public bool Run()
        {
            //6. ask the user to give you the filename. First make somewhere to store it (in the object you just created).
            //declare it so you can access its methods. It can be null for now
            FileFunctions fileFunctions = null;
            //24b. asking if we want to go again
            do
            {
                
                do
                {
                    //7. ask for the file name. Before, it was done by console.writeline(question),
                    //variable = console.readline, but we'll change that soon
                    string fileName = AskForString("Please enter a file name (feel free to include path): ");
                    //8. initialize the variable holding your filefunctions object
                    fileFunctions = new FileFunctions(fileName);
                    //9. you want to do this while true... but is there an easier way to check if the file is valid
                    //rather than declaring a holding keepGoing and returning that false when you have a valid file?
                    //Yes. Create a method to check if the file is valid
                    //This method is related to the file, which lives in the filefunctions class, so the method should go there
                    // then be called here.
                    // head back to the file functions class.
                    //11. with the method created, use it (with a ! so it returns false) in your do while loop
                } while (!fileFunctions.CheckIfValid());

                //12. ask for your search word
                //although... we're asking another question and storing a text string input
                //which is basically what we did when asking for the file path
                //so we can create a method just for asking questions and storing string answers
                //since that's a user interface type action, it belongs here
                //this is better encapsulation and polymorphism - bits of the code are simpler and hidden,
                //and the code can handle different objects/cases
                //15. we can use our new condensed ask a question method and get a response string back method
                string searchWord = AskForString("Please enter a word: ");

                //16. let's ask if it's case sensitive
                //it either is or isnt, which is a yes or no, which is a bool
                //except you can't use your regular ask a question method because that returns string
                //but we can follow the ask a question method example and make a bool variant of it
                //overkill here, but we still good practice in case we have another yes or no question
                //lets go add the method here, because it's a user interface type action
                //18. now we have our method ready, lets ask the question
                bool isCaseSensitive = AskForYesOrNo("Should this search be case sensitive? ");

                //19. now we have all of our inputs, we know how to go process our data
                //so lets go back to filefunctions, because that's where the file is
                string screenOutput = fileFunctions.SearchForWord(searchWord, isCaseSensitive);
                //23. now we can print the output out
                Console.WriteLine(screenOutput);

                //24. now we've gone through it once, we can if they want to go through again
                //this is another yes or no question, like 16, so we can reuse the method
                //ask if we want to go again, and use that method to return true/false back to the start
                //if not, program over

                //basically 10 lines of code for the main bulk of the UI Run method and asking for information
                //everything else lives in helper methods here or in the file functions class
            } while (AskForYesOrNo("Do you want to do another search?"));

            //25. this program ends here, so we don't have to return true, but tom hates void return methods
            //unless they are logs
            //so have to put something here otherwise it's a red squiggly
            return true;
        }
        //13. create a method to ask a question and store the return the typed in answer
        //now you can ask a new question (as the parameter) repeatedly and get the answer
        //if you notice, Tom has the same method twice here. 
        //the only difference between asking for a file and a search word is how specific they are
        //because of the text inside the writeline portion
        //so instead, you can make thew writeline portion an input parameter,
        //and then give the method the text you want to be asked each time you call the method
        //so lets make a simpler method that works for both
        public string AskForFile()
        {
            Console.Write("Please enter a file name (feel free to include path): ");
            return Console.ReadLine();
        }

        public string AskForSearchWord()
        {
            Console.Write("Please enter a word: ");
            return Console.ReadLine();
        }
        //14. you can pass any message in to this and it'll write that message to the console writeline,
        //accept the input, and then return that input as a string output back
        //lets go back to our main body
        /// <summary>
        /// Generic method that returns a string from the user
        /// </summary>
        /// <param name="message">Question to ask the user</param>
        /// <returns></returns>
        public string AskForString(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }

        public bool AskForAnotherWord()
        {
            Console.WriteLine("Do you want to search for another word?");
            string response = Console.ReadLine();
            return response.ToLower().StartsWith("y");
        }
        //17. here, we pass in a parameter of the message we want to display
        //and read the response and store that
        //except we have to process that response into a yes or no, bool
        //we could do an if statement
        //or we could just flatten the input to lower case
        //and check if it starts with a y, since most answers (yes, y, yeah, ya)
        //in the affirmative start with y (it'll still work if someone types yuck or yacht...)
        //startswith returns bool, which gives us our output. If it's a y, returns true, otherwise false
        //lets go back up
        public bool AskForYesOrNo(string message)
        {
            Console.Write(message);
            string response = Console.ReadLine();
            return response.ToLower().StartsWith("y");
        }
    }
}
