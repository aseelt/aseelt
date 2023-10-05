using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker.Classes
{
    public class QuizQuestion
    {
        //15. we know we're getting a line passed in
        //and that it'll have a question property, answers, and a correct answer number
        //answers should be hidden, we don't want them to be peeked at by other developer's modified code
        //lets make the properties for question, answers, and answer number
        public string Question { get; }
        private List<string> _Answers { get; set; } = new List<string>();
        private int CorrectAnswer { get; set; }
        //16. then we can copy the list into an array, using the toarray method
        //this array is public, so we can print them out
        //next, lets figure out a constructor so we can assign the properties
        public string[] Answers
        {
            get
            {
                return _Answers.ToArray();
            }
        }
        //17. we know we're getting a long string line from the readline in the file
        //with all the elements in it
        public QuizQuestion(string rawQuestion)
        {
            //18. let's make sure it's good data first using string.IsNullOrEmpty
            if(string.IsNullOrEmpty(rawQuestion))
            {
                //19. he's going to throw a custom exception if it is null or empty
                //this won't catch it, it'll just start the error bubble up process
                //have to make that as a custom exception class
                throw new InvalidQuestionException();
            }
            //21. now we know we have a input parameter, lets process it
            //convert it to an array we can work with it
            string[] parts = rawQuestion.Split('|');
            //first element we get back is always the question
            //lets assign that to this class's property
            Question = parts[0];
            // set the answers
            for (int i = 1; i < parts.Length; i++)
            {
                //good habit to trim for spaces
                string answer = parts[i].Trim();
                // check to see if it ends with a *
                if(answer.EndsWith("*"))
                {
                    //if it ends with *, then it's the correct answer
                    //we need to store that value as an integer to identify the correct answer
                    //let's assign that to this class's property
                    CorrectAnswer = i;
                    //then we trim that * off
                    answer = answer.Substring(0, answer.Length - 1);
                }
                //then add it to the answers property of this class
                //22. return to the calling method in functions
                _Answers.Add(answer);
            }
        }
        //38. method to check if the supplied answer parameter (passed from the UI)
        //is correct or not. Which is handy, because the right answer property is here
        // so just return true or false, if the answer matches the correct answer
        //return to the UI and the method calling it
        public bool IsCorrect(int answer)
        {
            return answer == CorrectAnswer;
        }
    }
}
