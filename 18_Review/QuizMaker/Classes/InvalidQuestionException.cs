using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker.Classes
{
    public class InvalidQuestionException : Exception
    {
        //20. make the custom exception
        //don't need to specify parameters, but you can give a custom message
        //make it a child of the top level exception class
        //go back to your quiz question code
        public InvalidQuestionException() : base("Sorry, I can't read the question") { }
    }
}
