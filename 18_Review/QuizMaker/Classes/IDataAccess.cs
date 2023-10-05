using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker.Classes
{
    public interface IDataAccess
    {
        //5. create an interface
        //this allows you to easily switch out how you get your data
        //you don't care where you get your data, as long as that class has this property
        //overkill here, but good practice
        List<QuizQuestion> GenerateListOfQuestions(); 
    }
}
