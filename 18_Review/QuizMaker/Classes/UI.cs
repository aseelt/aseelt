using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker.Classes
{
    public class UI
    {
        //28. in the UI, we'll need to create objects to use (plus they can be handy for holding variables0
        // this generates the object for me to use
        private FileFunctions fileFunctions = new FileFunctions();
        private List<QuizQuestion> questions = new List<QuizQuestion>();
        //29. we'll also need to store the number of answers
        private int NumberOfCorrect { get; set; }

        //30. we can create the active program
        public void StartMeUp()
        {
            // greet the user
            Console.WriteLine("Welcome to QuizMaster 2023!");
            //31. this is similar to the word search program and asks for an input, returns the output, in one line
            //we can just copy that code
            // get their name
            string victim = AskForString("What is your name?");
            Console.WriteLine($"Welcome {victim}, to the game.");

            //32. once we have the user's input, we are ready to start loading information in
            //load the questions into the active list we have here
            //to do that, you call the file functions class (responsible for loading the file)
            //and ask it to generate the list of questions using the file you have
            //that'll run the GenerateListOfQuestions method,
            //which will first check if the file exists in that location
            //then run the LoadUpQuestions method
            //which will open the file and go through it line by line
            //which will create an object for each question
            //which will get placed in the output variable of the LoadUpQuestions method, returning it
            //which will go to GenerateListOfQuestions, which will return the list here in the UI
            //which will get stored in the questions variable listed in row 14 here

            // load up the questions
            // TODO should this be in the constructor
            questions = fileFunctions.GenerateListOfQuestions();

            //33. now we have our information we need here in this file
            //then we can do the quiz portion, and Take the Quiz. The quiz does these actions
            //so we can create a method for the quiz portion, rather than do it in this start method directly
            //create a new method
            // ask question
            // get answer
            // rinse and repeat
            TakeQuiz();
            // we are done
            //40. you've looped through each of the questions, then the code continues here
            //so you print out the total number of answers right
            //you also have an exception thrown. Where does that message go?
            //in a log file. That's made in a separate class
            //create the log class and go there
            Console.WriteLine($"{victim} you scored {NumberOfCorrect} out of {questions.Count}");

        }
        //31. see other note about 31
        public string AskForString(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }
        //36. ask for integer
        //pass in the question you want printed
        //but the number could be input as a word which will break your string>int parse
        //so need a try catch
        public int AskForInteger(string message)
        {
            Console.Write(message);
            try
            {
                return int.Parse(Console.ReadLine());
            }
            //here, Tom is saying don't give an error, just give the answer a definite wrong
            //since there is no "answer" positioned -1
            //return to the answer a question portion below
            catch (Exception)
            {
                return -1;
            }
        }
        //34. we have our list of questions here, so we don't need to put them in a parameter for this method
        private bool TakeQuiz()
        {
            //we want to iterate through the questions in this UI class because we're printing them to the screen
            foreach (QuizQuestion item in questions)
            {
                //Pull the question, write it to the screen
                Console.WriteLine(item.Question);
                //pull each of the answers and write it to the screen
                for (int i = 0; i < item.Answers.Length; i++)
                {
                    Console.WriteLine($"{i + 1}) {item.Answers[i]}");
                }
                //35. ask for an answer
                //it's different method than asking a question and getting a string response
                //you're getting an integer response
                //but the process is about the same for a string ask/answer
                //create a separate method for it though
                int userAnswer = AskForInteger("What is your answer?");

                //37. you can't access the answer directly since you can't peek it
                //and you shouldn't peek it, that'll allow other developers to build in cheats
                //what you can do is the action to say "I am guessing this answer, am I right or wrong?"
                //that doesn't reveal the answer, it just checks if it's right or wrong
                //but that behvaiour shouldn't live here. it's a property of the question
                //so make a method there that handles this

                //39. The method only returns true or false
                //remember, console.writeline doesn't belong with the question, it only belongs with the UI
                //so here process what the console says when you get true (right), or wrong)
                //if right, add a number to the number correct property in this UI
                //then all your displaying and checking is correct
                //return to the method above
                if(item.IsCorrect(userAnswer))
                {
                    NumberOfCorrect++;
                    Console.WriteLine("You are RIGHT!!");
                }
                else
                {
                    Console.WriteLine("You are WROOOONG!");
                }
            }
            // A bit Jankey
            return true;
        }
        // display score
    }
}
