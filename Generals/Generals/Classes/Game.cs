using Generals.Exceptions;
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

        public string GetListOfDeaths()
        {
            return Battlefields[PlayerToggle].
        }

        /// <summary>
        /// Breaks the Phase 3 loop and declares victory if a team's flag reaches the other side
        /// Checks both player's battlefields for victory
        /// </summary>
        /// <returns>String of which player has won</returns>
        public string DeclareReachedTheOtherSideVictory()
        {
            // has to be the player's turn, their flag has to be alive, and the playerOneVictoryCount has to be 1
            if (PlayerToggle == 0 && Battlefields[PlayerToggle].DeclareReachedOtherSideVictory() == true)
            {
                return "One";
            }
            else if (PlayerToggle == 1 && Battlefields[PlayerToggle].DeclareReachedOtherSideVictory() == true)
            {
                return "Two";
            }
            return "";
        }
    }
}
