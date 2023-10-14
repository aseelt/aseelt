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
        public string[] xPositionToLetter = { "A", "B", "C", "D", "E", "F", "G", "H", "I" };

        // grid creation for reference
        public string[] GridReference = new string[72];

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

        // victory counters
        private int VictoryCount { get; set; } = 0;

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

        //methods

        /// <summary>
        /// Builds the grid for play
        /// </summary>
        /// <returns>Returns a dictionary grid of the field of play</returns>
        private Dictionary<string, Piece> BuildGrid()
        {
            Dictionary<string, Piece> grid = new Dictionary<string, Piece>();
            for (int i = 0; i < 9; i++)
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


        /// <summary>
        /// Provides the list of pieces not yet on the board
        /// </summary>
        /// <returns>String of pieces not yet on the board</returns>
        public string PiecesToPlace()
        {
            string output = $"\nThese {PiecesNotOnBoard.Count} pieces have not been placed yet:\n\n";

            for (int i = 0; i < PiecesNotOnBoard.Count; i++)
            {
                output += $"{i + 1}. {PiecesNotOnBoard[i].GetName()}\n";
            }
            return output;
        }

        /// <summary>
        /// Places the piece on the location of the board specified, checking to make sure it is blank
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public string SetupPlacePieceBoard(int piece, string location)
        {
            // check the location to see if it doesn't contain a piece other than ""
            // if no, place
            // if yes, return error

            if (Grid[location].GetName() == "Blank")
            {
                // this code is a hot mess
                // for the grid location, if blank
                // then place that peice's not on board location there
                Grid[location] = PiecesNotOnBoard[piece - 1];

                // update the piece to say its on the board
                Grid[location].IsPlaced();

                //need to store this string before removal
                string output = $"\nYour {PiecesNotOnBoard[piece - 1].GetName()} has been placed on {location}.";
                PiecesNotOnBoard.RemoveAt(piece - 1);

                return output;
            }
            else
            {
                return "A piece already exists at this location. Please try again.";
            }

        }
        /// <summary>
        /// Randomly assigns locations to the remaining pieces not yet on the board
        /// </summary>
        /// <param name="playerToggle"></param>
        /// <returns>String message if successful, by piece</returns>
        public string SetupPiecePlacementRandomizerBoard(int playerToggle)
        {
            // shuffle the pieces not on board list
            // go through each one by one
            // get a random location based on the player number
            // make sure the locations available are blank
            // store this in an array to make sure there are no dupes
            // if the player toggle is 0, grid references are 0,1,2 x9
            // if the player toggle is 1, grid references are 5,6,7 x9

            string piecePlacementMessage = "The following pieces have randomly been placed on the board. Please adjust as necessary.\n";
             
            List<string> randomizedBlankLocations = new List<string>(LocationRandomizer(playerToggle));

            // then assign locations to the pieces not on the board
            // this needs to be a while
            while (PiecesNotOnBoard.Count > 0)
            {
                // go through the pieces not on board
                // find it's place in the shuffled piece's list, and return that number 
                
                // need a 1 because setup place piece board takes user input of actual value and changes it to zero based indexing
                piecePlacementMessage += SetupPlacePieceBoard(1, randomizedBlankLocations[0]);
                
                // remove from shuffledPieces and blankLocations lists so we don't have conflicts
                randomizedBlankLocations.RemoveAt(0);        
            }
            return piecePlacementMessage;
        }
        /// <summary>
        /// Helper method to randomize loacations to place pieces on if player does not want to manually
        /// </summary>
        /// <param name="gridReference"></param>
        /// <param name="playerToggle"></param>
        /// <returns>List of randomized, valid, locations, for the current player</returns>
        private List<string> LocationRandomizer(int playerToggle)
        {
            // based this method on the above            
            List<string> selectedLocations = new List<string>();
            List<string> randomizedSelectedLocations = new List<string>();
            Random rnd = new Random();

            // holding numbers to start our cycle
            int i;
            int j;
            int k;

            //if player toggle is 0, want positions 0, 1, 2 and 9x multiples of it
            if (playerToggle == 0) 
            {
                // pull the blank locations from the grid and put them in the list
                //pull the first 24 positions out of the grid reference
                // have to do this 3 times...
                i = 0;
                j = 1;
                k = 2;

                LocationChooserHelper(selectedLocations,i);
                LocationChooserHelper(selectedLocations,j);
                LocationChooserHelper(selectedLocations,k); 
            }
            else //otherwise, want positions 5, 6, 7 and 8x multiples of it
            {
                i = 5;
                j = 6;
                k = 7;

                LocationChooserHelper(selectedLocations, i);
                LocationChooserHelper(selectedLocations, j);
                LocationChooserHelper(selectedLocations, k);
            }

            // when the selected locations list of blanks is built, randomize it 
            while (selectedLocations.Count > 0)
            {
                int index = rnd.Next(0, selectedLocations.Count); // give me a random number between 0 and the end of the list
                randomizedSelectedLocations.Add(selectedLocations[index]); //add the old list at that random position to the new list, one by one
                selectedLocations.RemoveAt(index); //remove the old list item so it's not pulled in again
            }
            return randomizedSelectedLocations;
        }
        /// <summary>
        /// Helper method for the Location Randomizer method. Obtains the list of valid positions for that player
        /// </summary>
        /// <param name="selectedLocations"></param>
        /// <param name="counter"></param>
        /// <returns>List of valid locations for that player's starting lineup.</returns>
        private List<string> LocationChooserHelper(List<string> selectedLocations, int counter)
        {
            // pull the blank locations from the grid and put them in the list
            //pull the first 27 positions out of the grid reference 
            for (int i = counter; i < 72; i += 8)
            {
                // pull the location
                string location = GridReference[i];
                // check if it's empty
                if (Grid[location].GetRank() == -3)
                {
                    //if empty, add it to the selected locations list
                    selectedLocations.Add(location); 
                }
            }
            return selectedLocations;
        }

        public bool SetupChangePieceLocation(string locationChosen)
        {
            // change the status of the piece to not on board
            // put it in a holding piece just for this method
            // add it back to the board's PiecesNotOnBoard list
            // replace that current spot with a blank piece
            Grid[locationChosen].RemoveFromBoard();
            Piece holdingPiece = Grid[locationChosen];
            PiecesNotOnBoard.Add(holdingPiece);
            Grid[locationChosen] = new Piece(-3);

            //return false when done so the SetupChangePieceLocation loop breaks in UI
            return false;
        }
        /// <summary>
        /// List of deaths in a player's army
        /// </summary>
        /// <returns>String of pieces no longer alive and removed from the board</returns>
        public string ListOfDeaths()
        {
            string output = $"\nYou have lost the following {PiecesKilled.Count} pieces:\n\n";

            for (int i = 0; i < PiecesKilled.Count; i++)
            {
                output += $"{i + 1}. {PiecesKilled[i].GetName()}\n";
            }
            return output;
        }


        /// <summary>
        /// Increments the victory counter for each player.
        /// Flag needs to reach the opposite end of the board and stay alive for one turn
        /// before victory can be declared.
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool IncrementVictoryCounter()
        {
            VictoryCount++;
            
            return true;
        }

        /// <summary>
        /// Method that declares this team's victory
        /// Flag needs to reach the opposite end of the board and stay alive for one turn
        /// before victory can be declared.
        /// </summary>
        /// <returns>Returns true if victory is to be declared</returns>
        public bool DeclareReachedOtherSideVictory()
        {
            if (Army[0].GetLifeStatus() == true && VictoryCount == 1)
            {
                return true;
            }
            return false;
        }

        // need a ToString override so I know what I'm working with
        public override string ToString()
        {
            return $"{PlayerName}";
        }

    }
}
