using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Generals.Classes
{
    public class Army
    {
        // property

        // need the army name
        // public, we'll need it to assign to pieces and during moves
        // can't change it after creation
        public string MyArmy { get; }


        // army list property holds a list of the pieces
        // needs to be a list, I will be pushing/pulling off it out of order
        // it should be in here
        // this is my list
        // give a copy to use
        public List<Piece> pieces { get; set; } = new List<Piece>();

        // constructor
        public Army(string name)
        {
            MyArmy = name;
            // 1x extra spy and 5x extra privates
            for (int i = 0; i < 5; i++)
            {
                Piece piece = new Piece(0);
                pieces.Add(piece);
            }
            pieces.Add(new Piece(-1));
            // rest of the army
            for (int i = -2; i < 13; i++)
            {
                Piece piece = new Piece(i);
                pieces.Add(piece);
            }
        }

        // need a ToString override so I know what I'm working with
        public override string ToString()
        {
            return $"{MyArmy}";
        }

    }
}
