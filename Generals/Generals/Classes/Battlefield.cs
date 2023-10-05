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
        // holds the army and the grid reference

        // properties
        // battlefield name is who owns it, the player name
        // use this in a queue to manage the battlefield toggle
        public string PlayerName { get; set; }

        // array of positions to letters reference
        //TODO decide if you want this public or private
        public string[] xPositionToLetter = { "A", "B", "C", "D", "E", "F", "G", "H" };

        // grid creation for reference
        public string[] GridReference = new string[64];

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
        public List<Piece> Army { get; set; } = new List<Piece>();


        // set up the constructor, it'll create the battlefield
        // public   
        public Battlefield(string name)
        {
            // get the name set up
            PlayerName = name;

            for (int i = 0; i < 8; i++)
            {
                // for each position in the xAxis array
                // add a number to it so it reads A1, A2, A3
                // label for the y axis 
                for (int j = 0; j < 8; j++)
                {
                    string cellName = (xPositionToLetter[i]) + (j + 1);

                    // then add it to the overall battlefield as a key
                    Grid.Add(cellName, new Piece(-3));

                    // add it to the reference array
                    GridReference[(i * 8) + j] = cellName;
                }
            }

            // 1x extra spy and 5x extra privates
            for (int i = 0; i < 5; i++)
            {
                Piece piece = new Piece(0);
                Army.Add(piece);
            }
            Army.Add(new Piece(-1));
            // rest of the army
            for (int i = -2; i < 13; i++)
            {
                Piece piece = new Piece(i);
                Army.Add(piece);
            }
            // reorder the pieces so it makes more sense
            // have to do this manually unfortunately
            Piece holding = new Piece(-3);
            // flag first
            holding = Army[6];
            Army[6] = Army[0];
            Army[0] = holding;
            // spy1 2nd
            holding = Army[5];
            Army[5] = Army[1];
            Army[1] = holding;
            // spy2 3rd
            holding = Army[7];
            Army[7] = Army[2];
            Army[2] = holding;
        }

        // helper method for the top and bottom player lines on the battlefield
        private string DisplayChooserTop(Battlefield[] battlefields, string playerToggle, int h, int i)
        {
            string topPlayerLine;
            if (playerToggle == PlayerName)
            {
                // if player toggle is 1 get the first player's battlefield
                // for the cell, plus the army's name (the battlefield name), pull the display name

                topPlayerLine = $"x {battlefields[0] /* first player */.Grid[$"{xPositionToLetter[i]}{h + 1}" /* grid position gives that piece */].DisplayName /* what that piece is*/ } x";
            }
            else
            {
                // if it's not the active player, pull the cell, and the battlefield's name
                // but pull the hidden name
                topPlayerLine = $"x {battlefields[0].Grid[$"{xPositionToLetter[i]}{h + 1}"].NameHidden} x";
            }
            return topPlayerLine;
        }
        private string DisplayChooserBottom(Battlefield[] battlefields, string playerToggle, int h, int i)
        {
            string bottomPlayerLine;
            if (playerToggle == PlayerName)
            {
                bottomPlayerLine = $"x {battlefields[1].Grid[$"{xPositionToLetter[i]}{h + 1}"].NameHidden} x";

            }
            else
            {
                bottomPlayerLine = $"x {battlefields[1].Grid[$"{xPositionToLetter[i]}{h + 1}"].DisplayName} x";
            }
            return bottomPlayerLine;
        }


        // displays the battlefield for the current player
        // hides the other player's pieces 
        public string BattlefieldDisplay(Battlefield[] battlefields, string playerToggle)
        {
            string output = "";
            for (int h = 0; h < 8; h++)
            {

                for (int i = 0; i < 8; i++)
                {
                    output += $"xX{xPositionToLetter[i]}xxxxxY{h + 1}x";
                }
                output += "\n";
                for (int i = 0; i < 8; i++)
                {
                    output += "x         x";
                }
                output += "\n";
                for (int i = 0; i < 8; i++)
                {
                    output += DisplayChooserTop(battlefields, playerToggle, h, i);
                }
                output += "\n";
                for (int i = 0; i < 8; i++)
                {
                    output += "x         x";
                }
                output += "\n";
                for (int i = 0; i < 8; i++)
                {
                    output += "x---------x";
                }
                output += "\n";
                for (int i = 0; i < 8; i++)
                {
                    output += "x         x";
                }
                output += "\n";
                for (int i = 0; i < 8; i++)
                {
                    output += DisplayChooserBottom(battlefields, playerToggle, h, i);
                }
                output += "\n";
                for (int i = 0; i < 8; i++)
                {
                    output += "x         x";
                }
                output += "\n"; for (int i = 0; i < 8; i++)
                {
                    output += "xxxxxxxxxxx";
                }
                output += "\n";
            }
            return output;
        }

        // need a ToString override so I know what I'm working with
        public override string ToString()
        {
            return $"{PlayerName}";
        }

    }
}
