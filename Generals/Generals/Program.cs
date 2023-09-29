using Generals.Classes;

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
            Console.WriteLine("\nPlayer Two, enter your army's name: ");
            string playerTwo = Console.ReadLine();

            // create the armies
            Army firstArmy = new Army(playerOne);
            Army secondArmy = new Army(playerTwo);

            Console.WriteLine($"The great battle between {playerOne} and {playerTwo} is about to begin!");

        }
    }
}