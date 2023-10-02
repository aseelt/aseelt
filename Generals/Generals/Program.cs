using Generals.Classes;
using System.Collections.Immutable;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace Generals
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");

            Piece firstPieceFlag = new Piece(-2);
            Piece secondPieceSpy = new Piece(-1);
            Piece thirdPiecePrivate = new Piece(0);
            Piece fourthPieceMajor = new Piece(5);
            Piece fifthPiece5Star = new Piece(12);

            // get the team name
            Console.WriteLine("Welcome to Generals");
            // add more fluff later

            //TODO fix and need to add error checking
            Console.WriteLine("\nPlayer One, enter your army's name: ");
            string playerOne = "x"; //Console.ReadLine();

            //TODO fix and need to add error checking 
            //TODO error checking make sure army starts with a different character
            Console.WriteLine("\nPlayer Two, enter your army's name: ");
            string playerTwo = "y"; //Console.ReadLine();

            //TODO fix and need to add error checking
            Console.WriteLine("\nWhere are your armies meeting? ");
            string field = "a"; // Console.ReadLine();

            // create the armies and battlefield
            // this can be inside the battlefield class
            //Army firstArmy = new Army(playerOne);
            //Army secondArmy = new Army(playerTwo);

            // each player gets their own battlefield
            Battlefield playerOneBattlefield = new Battlefield(playerOne, 1);
            Battlefield playerTwoBattlefield = new Battlefield(playerTwo, 2);
            // array of battlefields
            Battlefield[] battlefields = { playerOneBattlefield, playerTwoBattlefield };

            // assign a piece to a location
            // just a placeholder
            playerOneBattlefield.Grid["A1"] = playerOneBattlefield.MyArmy.pieces[0];
            playerTwoBattlefield.Grid["A1"] = playerTwoBattlefield.MyArmy.pieces[13];

            //TODO show the current player's pieces
            //TODO figure out how to assign player 1 to 1 here, so you have a toggle
            // probably a while loop

            Console.WriteLine($"The great battle between {playerOne} and {playerTwo} is about to begin in {field}!\n");

            // show the battlefield
            // have to figure out how to hide the pieces for the other player
            // do a for loop and add it to an appended string. Duh. Avoids this hot mess


            // let's just say we're on player 1
            int playerToggle = 1;

            
            string topPlayerLine;
            string bottomPlayerLine;
            

            string output = "";


            Console.WriteLine(BattlefieldUI.BattlefieldDisplay(battlefields, playerToggle));
        }
    }
}