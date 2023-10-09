using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generals.Exceptions
{
    public class PieceChosenWrongException : Exception
    {
        public PieceChosenWrongException() : base("Piece selection entered is null or empty")
        {
        }
    }
}
