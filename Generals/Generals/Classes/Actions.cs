using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generals.Classes
{
    public class Actions
    {
        // the array of battlefields lives here
        // what if it's a dictionary, then I can call the key instead of the array position
        public Dictionary<string, Battlefield> Battlefields = new Dictionary<string, Battlefield>();


        // methods
        // create battlefields and add it to the array
        public void CreateBattlefield(string nameOne, string nameTwo)
        {
            Battlefield playerOne = new Battlefield(nameOne);
            Battlefield playerTwo = new Battlefield(nameTwo);
            Battlefields[nameOne] = playerOne;
            Battlefields[nameTwo] = playerTwo;            
        }

        // view the battlefield
        // helper method for the top and bottom player lines on the battlefield
        private string DisplayChooserTop(string playerToggle, int h, int i)
        { 
            string topPlayerLine;
            if (playerToggle == Battlefields[0].BattlefieldName)
            {
                // if player toggle is 1 get the first player's battlefield
                // for the cell, plus the army's name (the battlefield name), pull the display name

                topPlayerLine = $"x {Battlefields[0] /* first player */.Grid[$"{Battlefields[0].xPositionToLetter[i]}{h + 1}" /* grid position gives that piece */].DisplayName /* what that piece is*/ } x";
            }
            else
            {
                // if it's not the active player, pull the cell, and the battlefield's name
                // but pull the hidden name
                topPlayerLine = $"x {Battlefields[0].Grid[$"{Battlefields[0].xPositionToLetter[i]}{h + 1}"].NameHidden} x";
            }
            return topPlayerLine;
        }
        // has to be duplicated like this, it's slighlty different to cover the different combinations
        private string DisplayChooserBottom(string playerToggle, int h, int i)
        {
            string bottomPlayerLine;
            if (playerToggle == Battlefields[1].BattlefieldName)
            {
                bottomPlayerLine = $"x {Battlefields[1].Grid[$"{Battlefields[1].xPositionToLetter[i]}{h + 1}"].NameHidden} x";

            }
            else
            {
                bottomPlayerLine = $"x {Battlefields[1].Grid[$"{Battlefields[1].xPositionToLetter[i]}{h + 1}"].DisplayName} x";
            }
            return bottomPlayerLine;
        }
        // displays the battlefield for the current player
        // hides the other player's pieces 
        public string BattlefieldDisplay(string playerToggle)
        {
            string output = "";
            for (int h = 0; h < 8; h++)
            {

                for (int i = 0; i < 8; i++)
                {
                    output += $"xX{Battlefields[0].xPositionToLetter[i]}xxxxxY{h + 1}x";
                }
                output += "\n";
                for (int i = 0; i < 8; i++)
                {
                    output += "x         x";
                }
                output += "\n";
                for (int i = 0; i < 8; i++)
                {
                    output += DisplayChooserTop(playerToggle, h, i);
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
                    output += DisplayChooserBottom(playerToggle, h, i);
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
    }
}
