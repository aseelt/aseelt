using QuizMaker.Classes;
using System;

namespace QuizMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            //0. think about your nouns and verbs
            //your major nouns are your classes
            //your verbs are your methods
            //smaller nouns may become properties
            //start pairing them up
            //then think about the information flows between your nouns, actions, users

            //1. think about where you're getting your data from 
            //any steps to manipulate that data to get it workable in a form you can use
            //then where you get your user input from 
            //then how it's going back to the user

            //2. in this example, we'll have a file function class to process the file input
            //that will give us a list of questions and answers combined
            //we need to process each line in the list into a separate set of questions and answers
            //then we can figure out how to ask the users questions, so use a UI class for that
            //then figure out the methods between them all if any others need to be added
            //4. lets start with an interface (after the below) though, that'll give us scalability in the future

            //3. start with a UI runtime method
            // we can populate this later
            //nothing should be in the main program, it should come from the UI
            UI ui = new UI();
            ui.StartMeUp();
        }
    }
}
