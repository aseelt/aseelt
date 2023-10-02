using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generals.Classes
{
    internal class Useless
    {
        /*
         *      xxxxxxxxxxx
         *      x         x
         *      x 1234567 x
         *      x         x
         *      x---------x
         *      x         x
         *      x ABCDEFG x
         *      x         x
         *      xxxxxxxxxxx 
         * 
         *  // if it's player 1, hide player 2 abcdefg
         *  // if it's player 2, show their pieces
         *  // if a square is null, show "       " (7 spaces)
         *  
         *  so... if key is null, show "      " y
         *  pull army's first character of name
         *  if player one, show only shortnames of own pieces, ____ out others
         *  if player two, the reverse
         *  How do I show the code block without it getting crazy
         *  access dictionary key by typing battlefield["1, 1"] y
         *  Use a method... called "piece" in battlefield
         * 
         * 
         *  add a name property called "masked name"
         *  
         *  // copy dictionary with dictionary<key, value> newDic = new dictionar<key, value>(oldDic)
         */





        //for (int i = 0; i< 8; i++)
        //    {
        // method needs to be called in this
        // store this in top player line
        // do i need another for bottom player line?
        // if playerToggle = 1, use this way to calculate the row (displayName)
        // if not, use the namehidden
        // $"x {playerOneBattlefield.Grid[$"{xPositionToLetter[i]}{i + 1}" + currentPlayer].DisplayName} x";
        // else, the reverse 
        //  $"x {playerTwoBattlefield.Grid[$"{xPositionToLetter[i]}{i + 1}" + currentPlayer].NameHidden} x";

        //if (playerToggle == 2)
        //{
        //    // if player toggle is 1 get the first player's battlefield
        //    // for the cell, plus the army's name (the battlefield name), pull the display name

        //    topPlayerLine = $"x {battlefields[0] /* first player */.Grid[$"{xPositionToLetter[i]}{i + 1}" /* grid position gives that piece */].DisplayName /* what that piece is*/ } x";
        //    bottomPlayerLine = $"x {battlefields[1].Grid[$"{xPositionToLetter[i]}{i + 1}"].NameHidden} x";

        //}
        //else
        //{
        //    // if it's not the active player, pull the cell, and the battlefield's name
        //    // but pull the hidden name
        //    topPlayerLine = $"x {battlefields[0].Grid[$"{xPositionToLetter[i]}{i + 1}"].NameHidden} x";
        //    bottomPlayerLine = $"x {battlefields[1].Grid[$"{xPositionToLetter[i]}{i + 1}"].DisplayName} x";
        //}

        //string topLine = $"xX{xPositionToLetter[i]}xxxxxY{i + 1}x";
        //string spacerLine = "x         x";
        ////topPlayerLine = DisplayChooser(playerToggle, i);
        //string midLine = "x---------x";
        ////bottomPlayerLine = DisplayChooser(playerToggle, i); // needs to be blank if other player is chosen
        //string bottomLine = "xxxxxxxxxxx";

        ////create the cell
        //Console.Write(topLine);
        //Console.Write(spacerLine);
        //Console.Write(topPlayerLine);
        //Console.Write(spacerLine);
        //Console.Write(midLine);
        //Console.Write(spacerLine);
        //Console.Write(bottomPlayerLine);
        //Console.Write(spacerLine);
        //Console.Write(bottomLine);
        //}






    }
}
