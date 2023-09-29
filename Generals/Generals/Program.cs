using Generals.Classes;
using System.Collections.Immutable;
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
            Army firstArmy = new Army(playerOne);
            Army secondArmy = new Army(playerTwo);

            // each player gets their own battlefield
            Battlefield playerOneBattlefield = new Battlefield(playerOne);
            Battlefield playerTwoBattlefield = new Battlefield(playerTwo);

            // assign a piece to a location
            playerOneBattlefield.Grid["1,1x"] = firstArmy.pieces[0];

            // show the current player's pieces
            //TODO figure out how to assign player 1 to 1 here, so you have a toggle
            // probably a while loop

            Console.WriteLine($"The great battle between {playerOne} and {playerTwo} is about to begin on {field}!\n");

            // show the battlefield
            // have to figure out how to hide the pieces for the other player

            for (int j = 0; j < 8; j++)
            {

                Console.WriteLine($"X{1}xxxxxxx{j + 1}YX{2}xxxxxxx{j + 1}YX{3}xxxxxxx{j + 1}YX{4}xxxxxxx{j + 1}YX{5}xxxxxxx{j + 1}YX{6}xxxxxxx{j + 1}YX{7}xxxxxxx{j + 1}YX{8}xxxxxxx{j + 1}Y");
                Console.WriteLine($"x         xx         xx         xx         xx         xx         xx         xx         x");
                Console.WriteLine($"x {playerOneBattlefield.Grid[$"1,{j + 1}" + playerOne].DisplayName} xx {playerOneBattlefield.Grid[$"2,{j + 1}" + playerOne].DisplayName} xx {playerOneBattlefield.Grid[$"3,{j + 1}" + playerOne].DisplayName} xx {playerOneBattlefield.Grid[$"4,{j + 1}" + playerOne].DisplayName} xx {playerOneBattlefield.Grid[$"5,{j + 1}" + playerOne].DisplayName} xx {playerOneBattlefield.Grid[$"6,{j + 1}" + playerOne].DisplayName} xx {playerOneBattlefield.Grid[$"7,{j + 1}" + playerOne].DisplayName} xx {playerOneBattlefield.Grid[$"8,{j + 1}" + playerOne].DisplayName} x");
                Console.WriteLine($"x         xx         xx         xx         xx         xx         xx         xx         x");
                Console.WriteLine($"x---------xx---------xx---------xx---------xx---------xx---------xx---------xx---------x");
                Console.WriteLine($"x         xx         xx         xx         xx         xx         xx         xx         x");
                Console.WriteLine($"x {playerTwoBattlefield.Grid[$"1,{j + 1}" + playerTwo].DisplayName} xx {playerTwoBattlefield.Grid[$"2,{j + 1}" + playerTwo].DisplayName} xx {playerTwoBattlefield.Grid[$"3,{j + 1}" + playerTwo].DisplayName} xx {playerTwoBattlefield.Grid[$"4,{j + 1}" + playerTwo].DisplayName} xx {playerTwoBattlefield.Grid[$"5,{j + 1}" + playerTwo].DisplayName} xx {playerTwoBattlefield.Grid[$"6,{j + 1}" + playerTwo].DisplayName} xx {playerTwoBattlefield.Grid[$"7,{j + 1}" + playerTwo].DisplayName} xx {playerTwoBattlefield.Grid[$"8,{j + 1}" + playerTwo].DisplayName} x");
                Console.WriteLine($"x         xx         xx         xx         xx         xx         xx         xx         x");
                Console.WriteLine($"xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            }


        }
    }
}