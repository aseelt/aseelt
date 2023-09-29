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
        // i don't recall how to make this private and provide a copy - it works for now
        // the the values are blank, waiting to be filled
        // all 64 positions in one dictionary
        public Dictionary<string, Piece> Grid = new Dictionary<string, Piece>();



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
                    string cellNamePlayer = (i + 1) + "," + (j + 1) + name;

                    // then add it to the overall battlefield as a key
                    Grid.Add(cellNamePlayer, new Piece(-3));
                }
            }
        }

        // method to populate the battlefield square
        // takes the input of the grid position and finds the value
        //public string ShowGridPiece(int currentPlayer, string gridPosition)
        //{


        //    if (currentPlayer == 1)
        //    {
        //        return $"xxxxxxxxxxx\nx         x\nx {Grid[gridPosition + BattlefieldName].DisplayName} x\nx         x\nx---------x\nx         x\nx {Grid[gridPosition].DisplayName} x\nx         x\nxxxxxxxxxxx";
        //    }
        //    else
        //    {
        //        return $"xxxxxxxxxxx\nx         x\nx {Grid[gridPosition].DisplayName} x\nx         x\nx---------x\nx         x\nx {Grid[gridPosition].DisplayName} x\nx         x\nxxxxxxxxxxx";
        //    }

        //    if (Grid[gridPosition] == null)
        //    {
        //        return "       ";
        //    }
        //}

        // need a ToString override so I know what I'm working with
        public override string ToString()
        {
            return $"{BattlefieldName}";
        }

    }
}
