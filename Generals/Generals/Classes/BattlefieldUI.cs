using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generals.Classes
{
    public class BattlefieldUI
    {
        private static string[] xPositionToLetter = { "A", "B", "C", "D", "E", "F", "G", "H" };
        private static string DisplayChooserTop(Battlefield[] battlefields, int playerToggle, int i)
        {
            string topPlayerLine;
            if (playerToggle == 1)
            {
                // if player toggle is 1 get the first player's battlefield
                // for the cell, plus the army's name (the battlefield name), pull the display name

                topPlayerLine = $"x {battlefields[0] /* first player */.Grid[$"{xPositionToLetter[i]}{i + 1}" /* grid position gives that piece */].DisplayName /* what that piece is*/ } x";
            }
            else
            {
                // if it's not the active player, pull the cell, and the battlefield's name
                // but pull the hidden name
                topPlayerLine = $"x {battlefields[0].Grid[$"{xPositionToLetter[i]}{i + 1}"].NameHidden} x";
            }
            return topPlayerLine;
        }
        private static string DisplayChooserBottom(Battlefield[] battlefields, int playerToggle, int i)
        {
            string bottomPlayerLine;
            if (playerToggle == 1)
            {
                bottomPlayerLine = $"x {battlefields[1].Grid[$"{xPositionToLetter[i]}{i + 1}"].NameHidden} x";

            }
            else
            {
                bottomPlayerLine = $"x {battlefields[1].Grid[$"{xPositionToLetter[i]}{i + 1}"].DisplayName} x";
            }
            return bottomPlayerLine;
        }
        string output;
        public static string BattlefieldDisplay(Battlefield[] battlefields, int playerToggle)
        {
            string output = "";
            for (int i = 0; i < 8; i++)
            {
                output += DisplayChooserTop(battlefields, playerToggle, i);
                output += DisplayChooserBottom(battlefields, playerToggle, i);
                output += "\n";
            }
            return output;
        }
    }
}
