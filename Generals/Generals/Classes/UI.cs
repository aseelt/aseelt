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
        public string Yellow = Console.IsOutputRedirected ? "" : "\x1b[93m";
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
            Console.WriteLine("Welcome to Generals");


            //phase 1, game setup
            Phase1GameSetup();
            //Game.Battlefields[0].Grid["A1"] = Game.Battlefields[0].Army[0];
            //Game.Battlefields[1].Grid["A1"] = Game.Battlefields[1].Army[5];
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
        /// Asks for a string with the entry on the same line.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Returns user input in lowercase, trimmed</returns> 
        private string AskForString(string message)
        {
            Console.Write(message);
            return Console.ReadLine().Trim().ToLower();
        }
        /// <summary>
        /// Asks the user for a single key input which will clear the display.
        /// </summary>
        /// <returns>Always returns true</returns>
        private bool ContinueAndClear()
        {
            Console.Write("Press any key to continue.");
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
                for (int i = 0; i < 8; i++)
                {
                    output += $"{Yellow}{Game.Battlefields[playerToggle].xPositionToLetter[i]}{h + 1}{Yellow}{Grey}--------+{Grey}";
                }
                output += "\n";

                //output += CreateUILine(BlankLine);

                //player one line, has to be like this
                for (int i = 0; i < 8; i++)
                {
                    output += DisplayChooserTop(h, i, playerToggle);
                }
                output += "\n";

                //output += CreateUILine(BlankLine);
                output += CreateUILine(MiddleLine);
                //output += CreateUILine(BlankLine);

                //player two line, has to be like this
                for (int i = 0; i < 8; i++)
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
            for (int i = 0; i < 8; i++)
            {
                output += line;
            }
            output += "\n";
            return output;
        }

        //TODO intro method
        // includes background and instructions

        private bool Phase1GameSetup()
        {
            //string playerOne AskForString("\nPlayer One, enter your army's name: ");
            Console.Write("\nPlayer One, enter your army's name: ");
            string playerOne = "PlayerOneAseel"; //Console.ReadLine().Trim().ToLower();

            string playerTwo = PlayerTwoSetup(playerOne);
            string field = BattlefieldName(playerOne, playerTwo);

            // can't add colours here, it gets real funky
            Console.WriteLine($"The great battle between {playerOne} and {playerTwo} is about to begin in {field}!\n");

            ContinueAndClear();

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
        /// <returns>String of Player Two, trimmed and in lowercase.</returns>
        private string PlayerTwoSetup(string playerOne)
        {
            string playerTwo = "";
            bool keepGoingPLayerTwoName = true;
            do
            {
                //string playerTwo AskForString("\nPlayer Two, enter your army's name: ");
                Console.Write("\nPlayer Two, enter your army's name: ");
                playerTwo = "PlayerTwoBasil"; //Console.ReadLine().Trim().ToLower();
                if (playerTwo == playerOne)
                {
                    Console.WriteLine("That name has already been chosen. Please try again.");
                }
                else
                {
                    keepGoingPLayerTwoName = false;
                }
            } while (keepGoingPLayerTwoName);
            return playerTwo;
        }

        /// <summary>
        /// Returns the name of the battlefield, validating it isn't the same as Player One or Player Two.
        /// Requires Player One and Player Two's name to perform the validation.
        /// </summary>
        /// <param name="playerOne"></param>
        /// <param name="playerTwo"></param>
        /// <returns></returns>
        private string BattlefieldName(string playerOne, string playerTwo)
        {
            string field = "";
            bool keepGoingBattlefield = true;
            do
            {
                //string field AskForString("\nWhere are your armies meeting? ");
                Console.Write("\nWhere are your armies meeting? ");
                field = "a"; //Console.ReadLine().Trim().ToLower();
                if (field == playerTwo || field == playerOne)
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
        /// Displays pieces not currently on the board (and alive).
        /// Used only in the setup phase.
        /// </summary>
        /// <returns>String of pieces.</returns>
        private string SetupPiecesList()
        {

            string output = $"\nThese {Game.Battlefields[GetPlayerNumberFromBoard()].PiecesNotOnBoard.Count} pieces have not been placed yet:\n\n";

            for (int i = 0; i < Game.Battlefields[GetPlayerNumberFromBoard()].PiecesNotOnBoard.Count; i++)
            {
                output += $"{i + 1}. {Game.Battlefields[GetPlayerNumberFromBoard()].PiecesNotOnBoard[i].GetName()}\n";
            }
            return output;
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
                        "\nWhich Piece would you like to place? ");

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
                    else if (pieceChosen >= (1 - 1) & pieceChosen <= (Game.Battlefields[GetPlayerNumberFromBoard()].PiecesNotOnBoard.Count))
                    {
                        return pieceChosen;
                    }
                    else
                    {
                        throw new PieceChosenWrongException();
                    }
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
                        //!Game.Battlefields[PlayerToggle].GridReference.Contains(pieceAndLocation[1]) 
                        Console.Write("Please enter a grid location (e.g. A1, B2, C5) in rows 1-3 to place your piece: ");
                        locationChosen = Console.ReadLine().Trim().ToUpper();
                    }
                    else
                    {
                        Console.Write($"Please enter a grid location (e.g. A6, B7, C8) in rows 6-8 to place your piece: ");
                        locationChosen = Console.ReadLine().Trim().ToUpper();
                    }

                    if (!string.IsNullOrEmpty(locationChosen) && Game.Battlefields[GetPlayerNumberFromBoard()].GridReference.Contains(locationChosen))
                    {
                        if (GetPlayerNumberFromBoard() == 0 && (locationChosen.EndsWith("1") || locationChosen.EndsWith("2") || locationChosen.EndsWith("3")))
                        {
                            keepGoing = false;
                            return locationChosen;
                        }
                        else if (GetPlayerNumberFromBoard() == 1 && (locationChosen.EndsWith("6") || locationChosen.EndsWith("7") || locationChosen.StartsWith("8")))
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
                catch (Exception e)
                {
                    Console.WriteLine("You have entered an incorrect value. Please try again.\n");
                }

            } while (keepGoing);

            return "";
        }

        // should this live on the board?
        // returns string of piece placed or not
        // this would have to call on the game, the game would have to say which player's board to use
        // then the board of that player would do the actual move
        // so this should be a call of that method here
        // simplifies this hot mess of a code
        // helper methods above do the piece validation
        // but then asking for the piece and location... is that handled here or on the board?

        private bool SetupChangePieceLocation()
        {
            bool keepGoingYesNo = true;
            do
            {
                Console.Write("\nWould you like to change the location of any pieces? (Y/N) ");
                string changePieceLocationAnswer = Console.ReadLine().ToUpper().Trim();
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
                            Console.Write("\n(Type '99' to view the grid) \n(Type '88' to view the list of pieces to place)" +
                                "\nEnter the grid location of the piece you would like to change. ");
                            // parse to number
                            // catch if that fails
                            string locationChosen = Console.ReadLine().Trim().ToUpper();

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
                                if (Game.Battlefields[GetPlayerNumberFromBoard()].Grid[locationChosen].GetName() == "Blank")
                                {
                                    throw new PieceChosenWrongException();
                                }
                                // by this point it should be a valid location
                                // change that position's on board status to false
                                // put the piece in that location back on the list
                                // put a blank piece in that location
                                // end the loop 
                                Game.Battlefields[GetPlayerNumberFromBoard()].Grid[locationChosen].RemoveFromBoard();
                                Piece holdingPiece = Game.Battlefields[GetPlayerNumberFromBoard()].Grid[locationChosen];
                                Game.Battlefields[GetPlayerNumberFromBoard()].PiecesNotOnBoard.Add(holdingPiece);
                                Game.Battlefields[GetPlayerNumberFromBoard()].Grid[locationChosen] = new Piece(-3);
                                keepGoingPiece = false;
                            }
                            else
                            {
                                throw new LocationChosenWrongException();
                            }

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

        private bool Phase2SetupPieces()
        {
            //place piece
            //originates in UI
            // prints list of pieces yet to place y 
            // ask for a piece y 
            // check if it's valid y 
            // check if it's on the not on board list, if true go ahead y
            // use that variable to select the piece from the list y 
            // ask where they want to put it y
            // make sure it's a-c for player 0, f-h for player 1 y
            // make sure that's a valid location y
            // make sure it's unoccupied y
            // update the piece's location y
            // remove from the list of pieces not on board but alive y
            // keep going till the pieces not on board list is empty y

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

                Console.WriteLine("Your piece placement is complete. Please hand over to your opponent." +
                    "\n\nNO PEEKING!");

                ContinueAndClear();
            } while (setupCount < 2);


            //game method asks for all pieces in the army list to be placed 
            //game method loops through list for each piece to be placed
            //sends instructions to the board
            //board then updates the piece
            //piece says "i am now on board"
            //finally end with "Would you like to move any pieces
            //do I need a list of pieces on the board? Probably...
            //game method toggle determines which battlefield we are working with
            //toggle false - only place pieces in 1, 2, 3 rows
            //toggle true - only place pieces in 6, 7, 8 rows
            return true;
        }
    }
}
