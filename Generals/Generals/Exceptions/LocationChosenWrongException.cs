using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generals.Exceptions
{
    public class LocationChosenWrongException : Exception
    {
        public LocationChosenWrongException() : base("Location entered was incorrect")
        {
        }
    }
}
