using Generals.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generals.Classes
{
    public class UI
    {
        // give me my colours
        // found on stack overflow. Someone used the ansi escape sequences to work out the colours
        public string Green = Console.IsOutputRedirected ? "" : "\x1b[92m";
        public string Red = Console.IsOutputRedirected ? "" : "\x1b[91m";
        public string Blue = Console.IsOutputRedirected ? "" : "\x1b[94m";
        public string Grey = Console.IsOutputRedirected ? "" : "\x1b[97m";
        // start game
        public Game Game;

        // player toggle. player0 (playerOne) is false, player1 (playerTwo) is true
        //private int PlayerToggle { get { return Game. } }

        public bool InitializeUIRuntime()
        {
            // make these separate methods
            // this initialize should be the top level runtime
            // you can factor out the asking to a general method that returns a string (readline) from the user
            // the method parameter is the question you want to task

            //TODO add more fluff later
            //TODO ASCII art
            //TODO method to show it in the center of the screen, press any key to continue, wipe
            
            Console.WriteLine("Welcome to the Game of the Generals");

            //TODO give option to see the help menu or to play

            //phase 1, game setup
            Phase1GameSetup();
            
            //Game.TogglePlayers();
            //Console.WriteLine(Game.BattlefieldDisplayGame());

            // assign a piece to a location
            // calls the piece's array position, doesn't create a piece
            // just a placeholder


            // instead use a method in battlefield to pull the piece out to be manipulated
            // then in piece, manipulate the position per placement, movement, or attack
            //BattlefieldDisplay();

            Phase2SetupPieces();
            Console.WriteLine(BattlefieldDisplay(GetPlayerNumberFromBoard()));
            Console.ReadKey();
            Game.TogglePlayers();
            Console.WriteLine(BattlefieldDisplay(GetPlayerNumberFromBoard()));

            //int pieceChosen = SetupAskForPiece($"Please enter a number 1-{Game.Battlefields[PlayerToggle].PiecesNotOnBoard.Count} to select a piece: ");

            return false;
        }

        /// <summary>
        /// Toggles between the two players. Calls the method in Game, it doesn't live here.
        /// </summary>
        /// <returns>Returns 0 for the first player, 1 for the second player</returns>
        private int TogglePlayersInGame()
        {
            return Game.TogglePlayers();
        }

        /// <summary>
        /// Helper method to get the player number, so I don't have to keep calling the Game object
        /// </summary>
        /// <returns>Returns 0 for the first player, 1 for the second player</returns>
        private int GetPlayerNumberFromBoard()
        {
            return Game.GetPlayerNumber();
        }

        /// <summary>
        /// Helper method to get the opposite player number's, so I don't have to keep calling the Game object
        /// </summary>
        /// <returns>If player one selected, returns 1 (player two) and vice versa.</returns>
        private int GetOppositePlayerNumberFromBoard()
        {
            return Game.GetPlayerNumber() == 0 ? 1 : 0;
        }

        /// <summary>
        /// Asks for a string with the entry on the same line.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Returns user input in entered case, trimmed</returns> 
        private string AskForStringEnteredCase(string message)
        {
            Console.Write(message);
            return Console.ReadLine().Trim();
        }
        /// <summary>
        /// Asks for a string with the entry on the same line.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Returns user input in UPPERCASE, trimmed</returns> 
        private string AskForStringUpperCase(string message)
        {
            Console.Write(message);
            return Console.ReadLine().Trim().ToUpper();
        }
        /// <summary>
        /// Asks the user for a single key input which will clear the display.
        /// </summary>
        /// <returns>Always returns true</returns>
        private bool ContinueAndClear(string message)
        {
            Console.Write(message);
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\u001b[2J\u001b[3J"); // found this on reddit. Uses an ansi escape code,
                                                     // the text represents "ESC [3J", which erases the scrollback buffer
                                                     // can go either before or after the clear, but you need the clear otherwise you have black space
            return true;
        }


        /// <summary>
        /// Displays the board / battlefield for that specific player. 
        /// Has to live here otherwise we can't show both sides
        /// </summary>
        /// <param name="playerToggle"></param>
        /// <returns>String battlefield for that player</returns>
        public string BattlefieldDisplay(int playerToggle)
        {
            string output = "\n";
            // taking out the blank spaces for extra space
            // string BlankLine = "x         x";
            string MiddleLine = $"{Grey}+-       -+{Grey}";
            string BottomLine = $"{Grey}+---------+{Grey}";

            for (int h = 0; h < 8; h++)
            {
                //top line, has to be like this 
                for (int i = 0; i < 9; i++)
                {
                    output += $"{Green}{Game.Battlefields[playerToggle].xPositionToLetter[i]}{h + 1}{Green}{Grey}--------+{Grey}";
                }
                output += "\n";

                //output += CreateUILine(BlankLine);

                //player one line, has to be like this
                for (int i = 0; i < 9; i++)
                {
                    output += DisplayChooserTop(h, i, playerToggle);
                }
                output += "\n";

                //output += CreateUILine(BlankLine);
                output += CreateUILine(MiddleLine);
                //output += CreateUILine(BlankLine);

                //player two line, has to be like this
                for (int i = 0; i < 9; i++)
                {
                    output += DisplayChooserBottom(h, i, playerToggle);
                }
                output += "\n";

                //output += CreateUILine(BlankLine);
            }
            output += CreateUILine(BottomLine);
            return output;
        }

        /// <summary>
        /// Helper method for the battlefield display. Calculates Player One's row - both visible and hidden states.
        /// Should only be used in BattlefieldDisplay(), not elsewhere.
        /// </summary>
        /// <param name="h"></param>
        /// <param name="i"></param>
        /// <returns>String of Player One's pieces, by location</returns>
        private string DisplayChooserTop(int h, int i, int playerToggle)
        {
            string topPlayerLine;
            if (playerToggle == 0)
            {
                // if player toggle is 0 get the first player's battlefield
                // for the cell, plus the army's name (the battlefield name), pull the display name

                topPlayerLine = $"{Grey}| {Grey}{Red}{Game.Battlefields[0].Grid[$"{Game.Battlefields[0].xPositionToLetter[i]}{h + 1}" /* grid position gives that piece */].GetShortDisplayName() /* what that piece is*/ }{Red}{Grey} |{Grey}";
            }
            else
            {
                // if it's not the active player, pull the cell, and the battlefield's name
                // but pull the hidden name
                topPlayerLine = $"{Grey}| {Grey}{Red}{Game.Battlefields[0].Grid[$"{Game.Battlefields[1].xPositionToLetter[i]}{h + 1}"].GetHiddenName()}{Red}{Grey} |{Grey}";
            }
            return topPlayerLine;
        }

        /// <summary>
        /// Helper method for the battlefield display. Calculates Player Two's row - both visible and hidden states.
        /// Should only be used in BattlefieldDisplay(), not elsewhere.
        /// Has to be duplicated like this, the code is slightly different to the Top version.
        /// </summary>
        /// <param name="h"></param>
        /// <param name="i"></param>
        /// <returns>String of Player Two's pieces, by location</returns>
        private string DisplayChooserBottom(int h, int i, int playerToggle)
        {
            string bottomPlayerLine;
            if (playerToggle == 0)
            {
                bottomPlayerLine = $"{Grey}| {Grey}{Blue}{Game.Battlefields[1].Grid[$"{Game.Battlefields[0].xPositionToLetter[i]}{h + 1}"].GetHiddenName()}{Blue}{Grey} |{Grey}";

            }
            else
            {
                bottomPlayerLine = $"{Grey}| {Grey}{Blue}{Game.Battlefields[1].Grid[$"{Game.Battlefields[1].xPositionToLetter[i]}{h + 1}"].GetShortDisplayName()}{Blue}{Grey} |{Grey}";
            }
            return bottomPlayerLine;
        }

        /// <summary>
        /// Helper method for the battlefield display. Generates the static lines.
        /// Should only be used in BattlefieldDisplay(), not elsewhere.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>String used by BattlefieldDisplay()</returns>
        private string CreateUILine(string line)
        {
            string output = "";
            for (int i = 0; i < 9; i++)
            {
                output += line;
            }
            output += "\n";
            return output;
        }

        //TODO intro method
        // includes background and instructions

        /// <summary>
        /// Master Method for Phase 0. Provides background on the game and helper instructions
        /// </summary>
        /// <returns>Prints directly to the console a helper menu. Always returns true</returns>
        private bool Phase0Introduction()
        { 
            bool keepGoing = true;
            do
            {
                try
                {
                    string menuSelection = AskForStringUpperCase("\nWelcome to the Help menu for the Game of the Generals!\n" +
                        "\nFor background information on the Game of The Generals, enter (1)." +
                        "\nFor information on the pieces that comprise your army, enter (2)" +
                        "\nFor information on the setup phase, enter (3)" +
                        "\nFor information on moves and combat, enter (4)" +
                        "\nFor information on victory conditions, enter (5)" +
                        "\nTo exit, enter (0)" +
                        "\n\nWhere would you like to navigate to? ");
                    switch (menuSelection)
                    {
                        case "1":
                            MenuOption1Background();
                            break;
                        case "2":
                            MenuOption2Pieces();
                            break;
                        case "3":
                            MenuOption3Setup();
                            break;
                        case "4":
                            MenuOption4MovesAndCombat();
                            break;
                        case "5":
                            MenuOption5Victory();
                            break;
                        case "0":
                            Console.WriteLine();
                            keepGoing = false;
                            break;
                        default:
                            throw new EntryWrongException();
                            break;
                    }
                }
                catch (EntryWrongException ewe)
                {
                    Console.WriteLine(ewe.Message);
                }
                catch (Exception)
                {
                    Console.WriteLine("You have entered an incorrect value. Please try again.\n");
                }

            } while (keepGoing);
            return true;
        }

        //TODO fill these out
        private bool MenuOption1Background()
        { 
            Console.WriteLine("\nOption1");
            return true;
        }
        private bool MenuOption2Pieces()
        {
            Console.WriteLine("\nOption2");
            return true;
        }
        private bool MenuOption3Setup()
        {
            Console.WriteLine("\nOption3");
            return true;
        }
        private bool MenuOption4MovesAndCombat()
        {
            Console.WriteLine("\nOption4");
            return true;
        }
        private bool MenuOption5Victory()
        {
            Console.WriteLine("\nOption5");
            return true;
        }
        /// <summary>
        /// Master Method for Phase 1. Gets the name of the players to build the game.
        /// </summary>
        /// <returns>Always returns true</returns>
        private bool Phase1GameSetup()
        {
            //string playerOne AskForStringEnteredCase("\nPlayer One, enter your army's name: ");
            Console.Write("\nPlayer One, enter your army's name: ");
            string playerOne = "PlayerOneAseel";

            string playerTwo = PlayerTwoSetup(playerOne);
            string field = BattlefieldName(playerOne, playerTwo);

            // can't add colours here, it gets real funky
            Console.WriteLine($"The great battle between {playerOne} and {playerTwo} is about to begin in {field}!\n");

            ContinueAndClear("Press any key to continue.");

            // each player gets their own battlefield
            // construct
            Game = new Game();
            Game.CreateBattlefield(playerOne, playerTwo);

            return true;
        }

        /// <summary>
        /// Returns the name of Player Two, validating it isn't the same as Player One.
        /// Requires Player One's name to perform the validation.
        /// </summary>
        /// <param name="playerOne"></param>
        /// <returns>String of Player Two, trimmed, in entered case.</returns>
        private string PlayerTwoSetup(string playerOne)
        {
            string playerTwo = "";
            bool keepGoingPlayerTwoName = true;
            do
            {
                //string playerTwo AskForStringEnteredCase("\nPlayer Two, enter your army's name: ");
                Console.Write("\nPlayer Two, enter your army's name: ");
                playerTwo = "PlayerTwoBasil";
                string playerOneLower = playerOne.ToLower();
                string playerTwoLower = playerTwo.ToLower();
                if (playerTwoLower == playerOneLower)
                {
                    Console.WriteLine("That name has already been chosen. Please try again.");
                }
                else
                {
                    keepGoingPlayerTwoName = false;
                }
            } while (keepGoingPlayerTwoName);
            return playerTwo;
        }

        /// <summary>
        /// Returns the name of the battlefield, validating it isn't the same as Player One or Player Two.
        /// Requires Player One and Player Two's name to perform the validation.
        /// </summary>
        /// <param name="playerOne"></param>
        /// <param name="playerTwo"></param>
        /// <returns>String, the name of the battlfield.</returns>
        private string BattlefieldName(string playerOne, string playerTwo)
        {
            string field = "";
            bool keepGoingBattlefield = true;
            do
            {
                //string field AskForStringEnteredCase("\nWhere are your armies meeting? ");
                Console.Write("\nWhere are your armies meeting? ");
                field = "a";
                string fieldLower = field.ToLower();
                string playerOneLower = playerOne.ToLower();
                string playerTwoLower = playerTwo.ToLower();
                if (fieldLower == playerTwoLower || fieldLower == playerOneLower)
                {
                    Console.WriteLine("That entry has already been chosen. Please try again.");
                }
                else
                {
                    keepGoingBattlefield = false;
                }
            } while (keepGoingBattlefield);
            return field;
        }

        /// <summary>
        /// Master Method for Phase 2. Sets up initial piece placement, for both Player One and Two
        /// </summary>
        /// <returns>Bool true always...</returns>
        private bool Phase2SetupPieces()
        {
            //place piece
            //originates in UI
            // prints list of pieces yet to place y 
            // game method loops through list for each piece to be placed
            // ask for a piece y 
            // check if it's valid y 
            // check if it's on the not on board list, if true go ahead y
            // use that variable to select the piece from the list y 
            // ask where they want to put it y
            // make sure it's 1-3 for player 0, 6-8 for player 1 y
            // make sure that's a valid location y
            // make sure it's unoccupied y
            // sends instructions to the board
            // board then updates the piece y
            // remove from the list of pieces not on board but alive y
            // piece says "i am now on board"
            // keep going till the pieces not on board list is empty y
            // finally end with "Would you like to move any pieces
            // game method toggle determines which battlefield we are working with

            int setupCount = 0;
            do
            {
                // all pieces placed
                // ask them if they want to change any pieces
                // has to be a separate loop that asks when initial placement is all done
                Console.WriteLine($"{Game.Battlefields[GetPlayerNumberFromBoard()].PlayerName}, it's time to place your pieces on the board.");

                do
                {
                    Console.WriteLine(SetupPiecesList());
                    do
                    {
                        // have to do this for player one and player two. reference their names
                        int piece = SetupAskForPiece();

                        if (piece == 77)
                        {
                            //do the randomized piece assignments on the board
                            //print the battlefield with "hey this is how the pieces are placed" header
                            // side method to pass location and piece number
                            // do that in the board
                            // call the game
                            Console.WriteLine(Game.SetupPiecePlacementRandomizerGame());
                            Console.WriteLine(BattlefieldDisplay(GetPlayerNumberFromBoard()));
                        }
                        else
                        {
                            string location = SetupAskForLocation();
                            Console.WriteLine(Game.SetupPlacePieceGame(piece, location));
                        }

                    } while (Game.Battlefields[GetPlayerNumberFromBoard()].PiecesNotOnBoard.Count > 0);

                    SetupChangePieceLocation();

                } while (Game.Battlefields[GetPlayerNumberFromBoard()].PiecesNotOnBoard.Count > 0);

                // once that is done, change the players
                TogglePlayersInGame();

                //add to the count, when count is 2, loop exits
                setupCount++;


                ContinueAndClear("Your piece placement is complete. Press any key to clear your screen before handing over the console to your opponent." +
                    "\n\nNO PEEKING!");
                ContinueAndClear("This screen has been provided to ensure operational security for your army.\nPlease press any key to continue.");

            } while (setupCount < 2);



            return true;
        }

        /// <summary>
        /// Displays pieces not currently on the board (and alive).
        /// Used only in the setup phase.
        /// </summary>
        /// <returns>String of pieces.</returns>
        private string SetupPiecesList()
        {

            return Game.PiecesToPlace();
        }

        /// <summary>
        /// Ask for a piece to place from the list of peices not yet on the board.
        /// Also has a shortcut to display the list and grid again.
        /// </summary>
        /// <returns>Validated piece number on the list</returns>
        private int SetupAskForPiece()
        {
            do
            {
                try
                {
                    // this is slightly clunky for the player - there's a lot of typing and entry
                    // but it's simpler in the long run
                    //TODO future enhancement - randomize piece placement
                    Console.Write("\n(Type '99' to view the grid) \n(Type '88' to view the list of pieces to place) " +
                        "\n(Type '77' to have the game assign the remaining pieces)" +
                        "\n(Type '00' to view the help menu)" +
                        "\n\nWhich Piece would you like to place? ");

                    int pieceChosen = int.Parse(Console.ReadLine().Trim());

                    // checks if the piece chosen is on the list of peices not on the board
                    // if so, breaks the loop and returns the piece for use
                    if (pieceChosen == 99)
                    {
                        Console.WriteLine(BattlefieldDisplay(GetPlayerNumberFromBoard()));
                    }
                    else if (pieceChosen == 88)
                    {
                        Console.WriteLine(SetupPiecesList());
                    }
                    else if (pieceChosen == 77)
                    {
                        return pieceChosen;
                    }
                    else if (pieceChosen == 00)
                    {
                        Phase0Introduction();
                    }
                    else if (pieceChosen >= (1 - 1) & pieceChosen <= (Game.Battlefields[GetPlayerNumberFromBoard()].PiecesNotOnBoard.Count))
                    {
                        return pieceChosen;
                    }
                    else
                    {
                        throw new PieceChosenWrongException();
                    }
                }
                catch (PieceChosenWrongException pcwe)
                {
                    Console.WriteLine(pcwe.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("You have entered an incorrect value. Please try again.\n");
                }

            } while (Game.Battlefields[GetPlayerNumberFromBoard()].PiecesNotOnBoard.Count != 0);

            return 0;
        }

        /// <summary>
        /// Asks for a location to place a piece during the setup phase.
        /// Restricted to rows that the players are allowed to place pieces.
        /// </summary>
        /// <returns></returns>
        private string SetupAskForLocation()
        {
            bool keepGoing = true;
            do
            {
                try
                {
                    string locationChosen;
                    if (GetPlayerNumberFromBoard() == 0)
                    {
                        locationChosen = AskForStringUpperCase("Please enter a grid location (e.g. A1, B2, C5) in rows 1-3 to place your piece: ");
                    }
                    else
                    {
                        locationChosen = AskForStringUpperCase("Please enter a grid location (e.g. A6, B7, C8) in rows 6-8 to place your piece: ");
                    }

                    if (!string.IsNullOrEmpty(locationChosen) && Game.Battlefields[GetPlayerNumberFromBoard()].GridReference.Contains(locationChosen))
                    {
                        if (GetPlayerNumberFromBoard() == 0 && (locationChosen.EndsWith("1") || locationChosen.EndsWith("2") || locationChosen.EndsWith("3")))
                        {
                            keepGoing = false;
                            return locationChosen;
                        }
                        else if (GetPlayerNumberFromBoard() == 1 && (locationChosen.EndsWith("6") || locationChosen.EndsWith("7") || locationChosen.EndsWith("8")))
                        {
                            keepGoing = false;
                            return locationChosen;
                        }
                        else
                        {
                            throw new LocationChosenWrongException();
                        }
                    }
                    else
                    {
                        throw new LocationChosenWrongException();
                    }
                }
                catch (LocationChosenWrongException lcwe)
                {
                    Console.WriteLine(lcwe.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("You have entered an incorrect value. Please try again.\n");
                }

            } while (keepGoing);

            return "";
        }

        /// <summary>
        /// After the pieces are first placed on the board, players will have the chance to change their deployment
        /// This is the helper method for this process
        /// </summary>
        /// <returns>Always returns false, this breaks the loop in the calling method</returns>
        private bool SetupChangePieceLocation()
        {
            bool keepGoingYesNo = true;
            do
            {
                string changePieceLocationAnswer = AskForStringUpperCase("\nWould you like to change the location of any pieces? (Y/N) ");
                if (changePieceLocationAnswer == "Y")
                {
                    // removes piece, adds it back to the notonboard list
                    // changes the piece's onboard status to false
                    // put blank piece in that square
                    bool keepGoingPiece = true;
                    do
                    {
                        try
                        {
                            //print message 
                            string locationChosen = AskForStringUpperCase("\n(Type '99' to view the grid) " +
                                "\n(Type '88' to view the list of pieces to place)" +
                                "\n(Type '00' to view the help menu)" +
                                "\n\nEnter the grid location of the piece you would like to change. ");

                            // checks if the piece chosen is on the list of peices not on the board
                            // if so, breaks the loop and returns the piece for use
                            if (locationChosen == "99")
                            {
                                Console.WriteLine(BattlefieldDisplay(GetPlayerNumberFromBoard()));
                            }
                            else if (locationChosen == "88")
                            {
                                Console.WriteLine(SetupPiecesList());
                            }
                            else if (Game.Battlefields[GetPlayerNumberFromBoard()].GridReference.Contains(locationChosen))
                            {
                                // throws exception if you try and choose an empty space
                                if (Game.Battlefields[GetPlayerNumberFromBoard()].Grid[locationChosen].GetName() == "Blank")
                                {
                                    throw new PieceChosenWrongException();
                                }
                                // by this point it should be a valid location
                                // change that position's on board status to false
                                // put the piece in that location back on the list
                                // put a blank piece in that location
                                // end the loop 
                                keepGoingPiece = Game.SetupChangePieceLocation(locationChosen);
                            }
                            else
                            {
                                throw new LocationChosenWrongException();
                            }

                        }
                        catch (PieceChosenWrongException pcwe)
                        {
                            Console.WriteLine(pcwe.Message);
                        }
                        catch (LocationChosenWrongException lcwe)
                        {
                            Console.WriteLine(lcwe.Message);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("You have entered an incorrect value. Please try again.\n");
                        }

                    } while (keepGoingPiece);
                }
                else if (changePieceLocationAnswer == "N")
                {
                    keepGoingYesNo = false;
                    return false;
                }
                else
                {
                    Console.WriteLine("You've entered an incorrect value.");
                }
            } while (keepGoingYesNo);

            return false;
        }
        /// <summary>
        /// Master Method for Phase 3. Handles moves, combat, calling on helper methods
        /// </summary>
        /// <returns></returns>
        private bool Phase3MovesAndCombat()
        {
            // check if their ReachedTheOtherSide victory condition is 1 (has to be there for a full turn of the other team)
            //      And flag has to be alive (it can be killed by the opposing team)
            //      if so, break loop and declare victory
            // ask for a location
            //      00 for rules... 
            //      66 to see own killed pieces - has 
            //      ff for forefeit
            // store as current location
            // store prospective location
            // check if position selected is valid
            //      validate here
            // check if position has a selectable piece - handle this at the board level
            //      return true/false
            // if true, ask for a direction
            //      if false, loop back            
            // check if move doesn't send a piece off the board - handle this at the board level
            // check if move doesn't collide with existing piece on team - handle this at the board level
            //      return true/false
            //      if any false, loop back
            //      if both true, store new location, grab piece from current location and continue
            // if all good by here, you have a piece in hand, location to send it to, current location. Then:
            //      check if it collides with piece of other player - handle at game level
            //          if it does, return true, if not, false
            //          if true, attack protocol - handle this at the game level
            //              Ask Y/N if you want to attack
            //                  If Y, attack and resolve - handle at the game level
            //                      For kills, use kill method to kill the piece and remove from board
            //                          Update whichever lists I have going...
            //                      If flag attacking opponent flag, give it an attacking boost
            //                      If flag piece killed, update victory condition to whichever player won
            //                          Break Phase 3 loop
            //                      Log attack, pieces, outcome
            //                  If N, loop back to start and ask for a location
            //          if false, move protocol - handle at the board level
            //              Update desired location with piece
            //              Update current location with blank piece
            //              Log move, pieces, moves
            //                  If player 0 flag reaches row 8, update "ReachedTheOtherSide" victory condition to 1
            //                  If player 1 flag reaches row 1, update "ReachedTheOtherSide" victory condition to 1
            //                      If either player's flag reaches the end and the spaces either side are blank, declare immediate victory
            //                          Need to have an exception that returns ok if the flag reaches the end in a corner and the next space over doesn't exist
            //                  Break loop
            // If those all resolve, clear screen (twice...) and toggle player
            bool victory = false;
            do
            {
                // check if a player has met the conditions for flag reaching the other side victory
                if (Game.DeclareReachedTheOtherSideVictory() != "")
                {
                    victory = true;
                }

                string currentLocation = "";
                string futureLocation = "";

            } while (!victory);

            return true;
        }
        private string AskForLocation()
        {
            bool keepGoing = true;
            do
            {
                try
                {
                    string locationChosen;

                    locationChosen = AskForStringUpperCase("\n(Type '66' to view a list of your casualties)" +
                        "\n(Type 'FF' to forfeit the game)" +
                        "\n(Type '00' to view the help menu)" +
                        "\n\nPlease enter the location of the piece you would like to move: ");

                    if (!string.IsNullOrEmpty(locationChosen) && Game.Battlefields[GetPlayerNumberFromBoard()].GridReference.Contains(locationChosen))
                    {
                        if (locationChosen == "66")
                        {
                            Console.WriteLine(Game.GetListOfDeaths());
                        }
                        else if (locationChosen == "FF")
                        {
                            //TODO forfeit protocol
                        }
                        else if (locationChosen == "00")
                        {
                            Phase0Introduction();
                        }
                        else if (Game.Battlefields[GetPlayerNumberFromBoard()].GridReference.Contains(locationChosen))
                        {
                            // throws exception if you try and choose an empty space
                            if (Game.Battlefields[GetPlayerNumberFromBoard()].Grid[locationChosen].GetName() == "Blank")
                            {
                                throw new PieceChosenWrongException();
                            }
                            // by this point it should be a valid location
                            // change that position's on board status to false
                            // put the piece in that location back on the list
                            // put a blank piece in that location
                            // end the loop 
                            
                            //TODO I WAS HERE BEFORE I GOT SIDE TRACKED
                            //keepGoingPiece = Game.SetupChangePieceLocation(locationChosen);
                        }
                        else
                        {
                            throw new LocationChosenWrongException();
                        }
                    }
                    else
                    {
                        throw new LocationChosenWrongException();
                    }
                }
                catch (LocationChosenWrongException lcwe)
                {
                    Console.WriteLine(lcwe.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("You have entered an incorrect value. Please try again.\n");
                }

            } while (keepGoing);

            return "";
        }

        private bool Phase4Victory()
        {
            // one path for get to the other side victory
            // one path for flag died victory
            // phase 3 has to break the loop to end up getting to this method in the UI
            return true;
        }
    }
}
