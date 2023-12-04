using LegoBuilder.Exceptions;
using LegoBuilder.Models.All;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace LegoBuilder.SqlDaos
{
    public class BaseSqlDao
    {
        public string DefaultRole => _DefaultRole;
        private const string _DefaultRole = "User";

        public string AdminRole => _AdminRole;
        private const string _AdminRole = "Overlord";

        public bool Inactive => _Inactive;
        private const bool _Inactive = false;

        public bool Active => _Active;
        private const bool _Active = true;

        protected string LegoInstructionsUrl = "https://www.lego.com/en-us/service/buildinginstructions/";

        protected readonly string ConnectionString;
        public BaseSqlDao(string dbConnectionString)
        {
            ConnectionString = dbConnectionString;
        }

        protected bool CheckString(string input, string message = "")
        {
            if (string.IsNullOrEmpty(input))
            {
               throw new IncorrectEntryException((message == "" ? "Please enter in a valid value" : message));
            }
            return true;
        }

        protected bool CheckCount(int count, string message = "")
        {
            if (count == 0)
            {
                throw new IncorrectEntryException((message == "" ? "Please enter in a valid value" : message));
            }
            return true;
        }
        protected string SetNumChecker(string setNum)
        {
            int counter = 0;
            foreach(char character in setNum)
            {
                if (character.ToString() == "-")
                {
                    counter++;
                }
            }
            if (counter == 0)
            {
                // if a hyphen is not included
                // default to looking for the first edition of the set
                // it might not be 100% accurate but it's at least something
                return setNum + "-1";
            }
            else
            {
                // if a hypen is included, process it through
                // assumes they know what they are doing
                return setNum;
            } 
        }
        protected string SetNumTruncater(string setNum)
        {
            int counter = 0;
            foreach (char character in setNum)
            {
                if (character.ToString() == "-")
                {
                    counter++;
                }
            }
            string setNumTruncated = null;
            if (setNum.IndexOf("-") == setNum.Length - 2 && counter == 1)
            {
                setNumTruncated = setNum.Substring(0, setNum.Length - 2);
            }
            else if(setNum.IndexOf("-") == setNum.Length - 3 && counter == 1)
            {
                setNumTruncated = setNum.Substring(0, setNum.Length - 3);
            }
            return setNumTruncated;
        }

        protected string SearchStringWildcardAdder(string query)
        {
            return string.IsNullOrEmpty(query) ? "" : "%" + query + "%";

        }
    }
}
