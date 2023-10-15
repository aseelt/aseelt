﻿using Generals.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generals.Classes
{
    public class Game
    {
        // the array of battlefields lives here
        // no it doesn't, the array should live in the UI
        // but need one
        public Board[] Battlefields { get; set; } = new Board[2];

        // player toggle. player0 (playerOne) is false, player1 (playerTwo) is true
        private int PlayerToggle { get; set; } = 0;

        // victor property
        // which player has won
        public int Victor { get; private set; } = -1;

        // Constructor
        // create battlefields and add it to the dictionary with the keys
        // add the players to the queue
        public bool CreateBattlefield(string nameOne, string nameTwo)
        {
            Board playerOne = new Board(nameOne);
            Board playerTwo = new Board(nameTwo);
            Battlefields[0] = playerOne;
            Battlefields[1] = playerTwo;
            //PlayerToggle.Enqueue(nameOne);
            //PlayerToggle.Enqueue(nameTwo);
            return true;
        }

        // methods



        /// <summary>
        /// Returns the player number
        /// </summary>
        /// <returns>Returns Int. 0 is Player 1, 1 is Player 2</returns>
        public int GetPlayerNumber()
        {
            return PlayerToggle;
        }

        /// <summary>
        /// Helper method to get the opposite player number's, so I don't have to keep calling the Game object
        /// </summary>
        /// <returns>If player one selected, returns 1 (player two) and vice versa.</returns>
        public int GetOppositePlayerNumber()
        {
            return PlayerToggle == 0 ? 1 : 0;             
        }

        /// <summary>
        /// Toggles between the two players.
        /// </summary>
        /// <returns>Returns 0 for the first player, 1 for the second player</returns>
        public int TogglePlayers()
        {
            if (PlayerToggle == 0)
            {
                PlayerToggle = 1;
                return 1;
            }
            else
            {
                PlayerToggle = 0;
                return 0;
            }
        }

        public string PiecesToPlace()
        {
            return Battlefields[PlayerToggle].PiecesToPlace();
        }

        //this needs to live here as a go between. The game tracks which player is currently playing
        /// <summary>
        /// Instructs the correct player board to place a piece based on the user's scrubbed input
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="location"></param>
        /// <returns>String outcome (succesful/not).</returns>
        public string SetupPlacePieceGame(int piece, string location)
        {
            return Battlefields[PlayerToggle].SetupPlacePieceBoard(piece, location);
        }

        /// <summary>
        /// Instructs the board to randomly assign the remaining pieces.
        /// Needs to live here as the game controls which player is active
        /// </summary>
        /// <returns>Returns string of successful placements</returns>
        public string SetupPiecePlacementRandomizerGame()
        {
            return Battlefields[PlayerToggle].SetupPiecePlacementRandomizerBoard(PlayerToggle);
        }

        /// <summary>
        /// Instructs the board to change the locations of pieces at the request of the player
        /// Only available during setup phase
        /// Needs to live here as the game controls which player is active
        /// </summary>
        /// <param name="locationChosen"></param>
        /// <returns>Returns string of successful placements</returns>
        public bool SetupChangePieceLocation(string locationChosen)
        {
            return Battlefields[PlayerToggle].SetupChangePieceLocation(locationChosen);
        }

        /// <summary>
        /// Gets a list of deaths from the board, based on the current player
        /// </summary>
        /// <returns>String list of deaths</returns>
        public string GetListOfDeaths()
        {
            return Battlefields[PlayerToggle].ListOfDeaths();
        }

        /// <summary>
        /// Calls the board to confirm the location selected has a valid piece to action
        /// </summary>
        /// <param name="location"></param>
        /// <returns>The same location given, if valid, otherwise bubbles up an exception</returns>
        public string ConfirmLocationHasValidPiece(string location)
        {
            return Battlefields[PlayerToggle].ConfirmLocationHasValidPiece(location);
        }

        /// <summary>
        /// Calls the board to confirm the location selected has a valid piece to action
        /// and that the move won't bump into an existing piece
        /// </summary>
        /// <param name="currentLocation"></param>
        /// <param name="direction"></param>
        /// <returns>The same location given, if valid, otherwise bubbles up an exception</returns>
        public string ConfirmMoveIsValid(string currentLocation, string direction)
        {
            return Battlefields[PlayerToggle].ConfirmMoveIsValid(currentLocation, direction);
        }

        public string MakeMove(string currentLocation, string futureLocation)
        {
            // this completes the action
            string pieceMoved = Battlefields[PlayerToggle].BoardExecuteMove(currentLocation, futureLocation);

            // now work only with futureLocation as that is the new location of the piece
            string returnText = CheckFlagImmediateVictory(pieceMoved, futureLocation);
            return returnText;
        }

        /// <summary>
        /// Checks for immediate victory in the event a flag gets to the final row and doesn't have neighbours
        /// </summary>
        /// <param name="pieceMoved"></param>
        /// <param name="location"></param>
        /// <returns>String "Immediate" if victory, "Wait" if the Flag reached the end, "Other" for other pieces. Updates the victor to the player number, if immediate</returns>
        private string CheckFlagImmediateVictory(string pieceMoved, string location)
        {
            // checking immediate victory conditions
            if (pieceMoved == "Flag" && CheckFlagLocation())
            {
                // if both of these are true, then the flag is now at the final row
                // won't work if the flag isn't at the  

                // check if the pieces next to flag are empty
                string victoryCheck = Battlefields[GetOppositePlayerNumber()].CheckFlagNeighbours(location);

                if (victoryCheck == "Immediate")
                {
                    // if blank neighbours, immediate victory
                    Victor = PlayerToggle;
                    return "Immediate";
                }
                else
                {
                    // if has one neighbour, wait
                    // increment the counter
                    Battlefields[PlayerToggle].IncrementVictoryCounter();
                    return "Wait";
                }
            }
            // if other piece moved, don't really care
            return "Other";
        }

        /// <summary>
        /// Checks the location of the flag to see if it's at the final row for that player
        /// </summary>
        /// <returns>True if flag for that player is at the end</returns>
        private bool CheckFlagLocation()
        {
            // board sends back the last position
            if (PlayerToggle == 0)
            {
                string lastCharOfFlagLocation = Battlefields[PlayerToggle].BoardCheckFlagLocation();

                // for player one, check if their flag is in the end row, 8
                if (lastCharOfFlagLocation == "8")
                {
                    return true;
                }
            }
            if (PlayerToggle == 1)
            {
                string lastCharOfFlagLocation = Battlefields[PlayerToggle].BoardCheckFlagLocation();

                // for player two, check if their flag is in the end row, 1
                if (lastCharOfFlagLocation == "1")
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckFlagWaitingVictory()
        {
            // if the flag is in the right position, the increment has been upped to 1
            if (CheckFlagLocation() && Battlefields[PlayerToggle].DeclareReachedOtherSideVictory())
            {
                //declare a victor
                Victor = PlayerToggle;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Forfeit protocol. When called, it updates the Victor property with the winning player's index number
        /// </summary>
        /// <returns>Always returns true.</returns>
        public bool ForfeitProtocol()
        {
            // if the player toggle is zero, meaning player one, they are forfeiting
            // so return "Two"
            if (PlayerToggle == 0)
            {
                Victor = 1;
            }
            else
            {
                Victor = 0;
            }

            return true;
        }
    }
}
