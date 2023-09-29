using Generals.Classes;
using System.Collections.Immutable;

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
            string playerOne = Console.ReadLine();

            //TODO fix and need to add error checking 
            //TODO error checking make sure army starts with a different character
            Console.WriteLine("\nPlayer Two, enter your army's name: ");
            string playerTwo = Console.ReadLine();

            //TODO fix and need to add error checking
            Console.WriteLine("\nWhere are your armies meeting? ");
            string field = Console.ReadLine();

            // create the armies and battlefield
            Army firstArmy = new Army(playerOne);
            Army secondArmy = new Army(playerTwo);
            Battlefield warzone = new Battlefield(field);

            // assign a piece to a location
            warzone.Grid["1, 1"] = firstArmy.pieces[0];

            // show the current player's pieces
            //TODO figure out how to assign player 1 to 1 here, so you have a toggle
            // probably a while loop
            int currentPlayer = 2;


            Console.WriteLine($"The great battle between {playerOne} and {playerTwo} is about to begin on {field}!\n");

            // show the battlefield
            Console.WriteLine($"{warzone.ShowGridPiece(2,"1, 1")}");
        }
    }
}