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

        // need a player number
        // don't need to change once set
        public int PlayerNumber { get; }

        // need an x axis and a y axis
        // the piece calls on the dictionary key values
        // movement adjusts the dictionary key values
        // if two pieces have the same key, trigger attack
        // i don't recall how to make this private and provide a copy - it works for now
        // the the values are blank, waiting to be filled
        // all 64 positions in one dictionary
        public Dictionary<string, Piece> Grid = new Dictionary<string, Piece>();

        // hold the army in here
        // needs to be set, we'll be changing the army (alive/dead)
        public Army MyArmy {get; set;}


        // set up the constructor, it'll create the battlefield
        // public  
        // 
        public Battlefield(string name, int playerNumber)
        {
            // get the name set up
            BattlefieldName = name;
            PlayerNumber = playerNumber;
            MyArmy = new Army(name);
            
            for (int i = 0; i < 8; i++)
            {
                // for each position in the xAxis array
                // add a number to it so it reads A1, A2, A3
                // label for the y axis 
                for (int j = 0; j < 8; j++)
                {
                    string cellNamePlayer = (xPositionToLetter[i]) + (j + 1);

                    // then add it to the overall battlefield as a key
                    Grid.Add(cellNamePlayer, new Piece(-3));
                }
            }
        }
             

        // array of positions to letters
        string[] xPositionToLetter = { "A", "B", "C", "D", "E", "F", "G", "H" };

        // need a ToString override so I know what I'm working with
        public override string ToString()
        {
            return $"{BattlefieldName}";
        }

    }
}
