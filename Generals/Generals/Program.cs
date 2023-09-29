using Generals.Classes;

namespace Generals
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");

            Piece firstPieceFlag = new Piece(-2);
            Piece secondPieceSpy = new Piece(-1);
            Piece thirdPiecePrivate = new Piece(0);
            Piece fourthPieceMajor = new Piece(5);
            Piece fifthPiece5Star = new Piece(12);

            Dictionary<int, string> RankToName = new Dictionary<int, string>()
        {
            { -2, "Flag" },
            { -1, "Spy" },
            { 0, "Private" },
            { 1, "Sergeant" },
            { 2, "2nd Lieutenant" },
            { 3, "1st Lieutenant" },
            { 4, "Captain" },
            { 5, "Major" },
            { 6, "Lieutenant Colonel" },
            { 7, "Colonel" },
            { 8, "* General" },
            { 9, "** General" },
            { 10, "*** General" },
            { 11, "**** General" },
            { 12, "***** General" }
        };

            string name = "";
            int rank = -2;
            name = RankToName[rank];
        }
    }
}