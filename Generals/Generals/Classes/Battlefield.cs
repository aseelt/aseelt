using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Generals.Classes
{
    public class Battlefield
    {
        // properties
        // battlefield name
        public string BattlefieldName { get; set; }

        // need an x axis and a y axis
        // the piece calls on the dictionary key values
        // movement adjusts the dictionary key values
        // if two pieces have the same key, trigger attack
        // but can make this private and provide arrays for use in program.cs
        // the the values are blank, waiting to be filled
        // all 64 positions in one dictionary
        Dictionary<string, Piece> battlefield = new Dictionary<string, Piece>();

        // set up the constructor, it'll create the battlefield
        // public  
        // 
        public Battlefield(string name)
        {
            // get the name set up
            BattlefieldName = name;
                        
            for (int i = 0; i < 8; i++)
            {
                // for each position in the xAxis array
                // add a number to it so it reads A1, A2, A3
                // label for the y axis 
                for (int j = 0; j < 8; j++)
                {
                    string cellName = (i + 1) + ", " + (j + 1);
                    // then add it to the overall battlefield as a key
                    battlefield.Add(cellName, null);
                }
            }
        }

        // need a ToString override so I know what I'm working with
        public override string ToString()
        {
            return $"{BattlefieldName}";
        }

    }
}
