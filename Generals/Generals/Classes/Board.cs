using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Generals.Classes
{
    public class Board
    {
        // holds the army and the grid reference

        // properties
        // battlefield name is who owns it, the player name
        // use this in a queue to manage the battlefield toggle
        // no set, once created by constructor it can't be changed
        public string PlayerName { get; }

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
        // derived when the class is created
        // all 64 positions in one dictionary
        public Dictionary<string, Piece> Grid = new Dictionary<string, Piece>();


        // hold the army in here
        // needs to be set, we'll be changing the army (alive/dead)
        // no set, only the constructor can build the army
        // public get, want to access this outside of the class
        public List<Piece> Army { get; } = new List<Piece>();

        // list of pieces NOT on the board (NOT placed AND alive)
        // no set, created at time of board creation
        // public get, we want to access this with the game or the UI
        public List<Piece> PiecesNotOnBoard { get; } = new List<Piece>();

        // list of pieces on the board (placed and alive)
        // private set, only want the board changing this
        // public get, we want to access this with the game or the UI
        public List<Piece> PiecesOnBoard { get; private set; } = new List<Piece>();

        // list of pieces killed
        // private set, only want the board changing this
        // public get, we want to access this with the game or the UI
        public List<Piece> PiecesKilled { get; private set; } = new List<Piece>();

        // set up the constructor, it'll create the battlefield
        // public   
        public Board(string name)
        {
            // get the name set up
            PlayerName = name;
            Grid = BuildGrid();

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

            PiecesNotOnBoard = new List<Piece>(Army);
        }

        private Dictionary<string, Piece> BuildGrid()
        {
            Dictionary<string, Piece> grid = new Dictionary<string, Piece>();
            for (int i = 0; i < 8; i++)
            {
                // for each position in the xAxis array
                // add a number to it so it reads A1, A2, A3
                // label for the y axis 
                for (int j = 0; j < 8; j++)
                {
                    string cellName = (xPositionToLetter[i]) + (j + 1);

                    // then add it to the overall battlefield as a key
                    grid.Add(cellName, new Piece(-3));

                    // add it to the reference array
                    GridReference[(i * 8) + j] = cellName;
                }
            }
            return grid;
        }

        public string PlacePiece(string key)
        {
            //place piece
            //originates in game
            //game method asks for all pieces in the army list to be placed 
            //game method loops through list for each piece to be placed
            //sends instructions to the board
            //board then updates the piece
            //piece says "i am now on board"
            //finally end with "Would you like to move any pieces
            //do I need a list of pieces on the board? Probably...
            //game method toggle determines which battlefield we are working with
            //toggle false - only place pieces in a, b, c rows
            //toggle true - only place pieces in f, g, h rows
            return "";
        }


        // move piece
        //      Action works on the battlefield
        //      Battlefield works on the piece to udpate its position
        //      validate if it's an actual legal move
        //          if it's one of the reference positions
        //          so method has to return true/false if it worked
        //      check if the cell being moved to is occupied by the other team
        //          if so, check if you want to attack
        //          if yes, attack
        //                  how does this work? 
        //                  what if we do an array of 0, 1, 2, 3 -
        //                      0, are you sure you want to attack,
        //                      1, attack made - result,
        //                      2, position moved,
        //                      3, position is invalid
        //          attack kills one or both pieces
        // then move
        // replace prior piece
        // get the input from the actions
        // validate here if it's true, then call the method for the piece to move the piece
        // if moved, return true, if not move false
        // actions/ui says if move was successful or not based on what's returned here
        // check if the move results in an attack (separate method)


        // need a ToString override so I know what I'm working with
        public override string ToString()
        {
            return $"{PlayerName}";
        }

    }
}
