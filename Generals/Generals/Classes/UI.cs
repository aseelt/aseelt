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
        // start game
        public Game Game;

        // player toggle. player0 (playerOne) is false, player1 (playerTwo) is true
        private int PlayerToggle { get; set; } = 0;

        public bool InitializeUIRuntime()
        {
            // make these separate methods
            // this initialize should be the top level runtime
            // you can factor out the asking to a general method that returns a string (readline) from the user
            // the method parameter is the question you want to task

            //phase 1, game setup
            Phase1();
            

            // assign a piece to a location
            // calls the piece's array position, doesn't create a piece
            // just a placeholder
            //Game.Battlefields[0].Grid["A1"] = Game.Battlefields[0].Army[0];
            //Game.Battlefields[1].Grid["A1"] = Game.Battlefields[1].Army[5];

            //TogglePlayers();

            // instead use a method in battlefield to pull the piece out to be manipulated
            // then in piece, manipulate the position per placement, movement, or attack
            Console.WriteLine(BattlefieldDisplay());

            SetupPieces();

            //int pieceChosen = SetupAskForPiece($"Please enter a number 1-{Game.Battlefields[PlayerToggle].PiecesNotOnBoard.Count} to select a piece: ");

            return false;
        }

        private bool Phase1()
        {
            // get the team name
            Console.WriteLine("Welcome to Generals");
            //TODO add more fluff later

            //TODO fix and need to add error checking
            Console.WriteLine("\nPlayer One, enter your army's name: ");
            string playerOne = "PlayerOneAseel"; //Console.ReadLine();

            //TODO fix and need to add error checking, make sure it's not the same as player one
            Console.WriteLine("\nPlayer Two, enter your army's name: ");
            string playerTwo = "PlayerTwoBasil"; //Console.ReadLine();

            //TODO fix and need to add error checking
            Console.WriteLine("\nWhere are your armies meeting? ");
            string field = "a"; // Console.ReadLine();

            Console.WriteLine($"The great battle between {playerOne} and {playerTwo} is about to begin in {field}!\n");

            // each player gets their own battlefield
            // construct
            Game = new Game();
            Game.CreateBattlefield(playerOne, playerTwo);
            
            return true;
        }

        //player toggle lives here, the UI controls this
        private int TogglePlayers()
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


        // view the battlefield - lives here as part of the UI
        // helper method for the top and bottom player lines on the battlefield
        private string DisplayChooserTop(int h, int i)
        {
            string topPlayerLine;
            if (PlayerToggle == 0)
            //if (PlayerToggle.Peek() == Battlefields[playerToggle].PlayerName)
            {
                // if player toggle is 1 get the first player's battlefield
                // for the cell, plus the army's name (the battlefield name), pull the display name

                topPlayerLine = $"x {Game.Battlefields[0] /* first player */.Grid[$"{Game.Battlefields[0].xPositionToLetter[i]}{h + 1}" /* grid position gives that piece */].GetShortDisplayName() /* what that piece is*/ } x";
            }
            else
            {
                // if it's not the active player, pull the cell, and the battlefield's name
                // but pull the hidden name
                topPlayerLine = $"x {Game.Battlefields[0].Grid[$"{Game.Battlefields[0].xPositionToLetter[i]}{h + 1}"].GetHiddenName()} x";
            }
            return topPlayerLine;
        }

        // has to be duplicated like this, it's slighlty different to cover the different combinations
        // and it returns another string 
        private string DisplayChooserBottom(int h, int i)
        {
            string bottomPlayerLine;
            //for(int i = 0; i < 8; i++)
            //{

            //}
            if (PlayerToggle == 0)
            {
                bottomPlayerLine = $"x {Game.Battlefields[1].Grid[$"{Game.Battlefields[1].xPositionToLetter[i]}{h + 1}"].GetHiddenName()} x";

            }
            else
            {
                bottomPlayerLine = $"x {Game.Battlefields[1].Grid[$"{Game.Battlefields[1].xPositionToLetter[i]}{h + 1}"].GetShortDisplayName()} x";
            }
            return bottomPlayerLine;
        }

        // displays the battlefield for the current player
        // hides the other player's pieces 
        private string BattlefieldDisplay()
        {
            string output = "";
            const string BlankLine = "x         x";
            const string MiddleLine = "x---------x";
            const string BottomLine = "xxxxxxxxxxx";

            for (int h = 0; h < 8; h++)
            {
                //top line, has to be like this
                for (int i = 0; i < 8; i++)
                {
                    output += $"xX{Game.Battlefields[0].xPositionToLetter[i]}xxxxxY{h + 1}x";
                }
                output += "\n";

                output += CreateUILine(BlankLine);

                //player one line, has to be like this
                for (int i = 0; i < 8; i++)
                {
                    output += DisplayChooserTop(h, i);
                }
                output += "\n";

                output += CreateUILine(BlankLine);
                output += CreateUILine(MiddleLine);
                output += CreateUILine(BlankLine);

                //player two line, has to be like this
                for (int i = 0; i < 8; i++)
                {
                    output += DisplayChooserBottom(h, i);
                }
                output += "\n";

                output += CreateUILine(BlankLine);
                output += CreateUILine(BottomLine);
            }
            return output;
        }

        // helper method for the battlefield
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

        private bool SetupPiecesList()
        {

            string output = "\nThese pieces have not been placed yet:\n\n";

            for (int i = 0; i < Game.Battlefields[PlayerToggle].PiecesNotOnBoard.Count; i++)
            {
                output += $"{i + 1}. {Game.Battlefields[PlayerToggle].PiecesNotOnBoard[i].GetName()}\n";
            }
            Console.WriteLine(output);

            return true;
        }
        // gets the value, parses, checks it's a valid piece
        private int SetupAskForPiece()
        {

            bool keepGoing = true;
            do
            {
                try
                {
                    //print message
                    Console.Write("Which piece would you like to place? (Type 99 to view the grid) ");
                    // parse to number
                    // catch if that fails
                    int pieceChosen = int.Parse(Console.ReadLine().Trim());

                    // checks if the piece chosen is on the list of peices not on the board
                    // if so, breaks the loop and returns the piece for use
                    if (pieceChosen == 99)
                    {
                        Console.WriteLine(BattlefieldDisplay());
                    }
                    else if (pieceChosen >= (1 - 1) & pieceChosen <= (Game.Battlefields[PlayerToggle].PiecesNotOnBoard.Count - 1))
                    {
                        keepGoing = false;
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

            } while (keepGoing);

            return 0;
        }



        private string SetupAskForLocation()
        {

            bool keepGoing = true;
            do
            {
                try
                {
                    if (PlayerToggle == 0)
                    {
                        Console.Write($"Please enter a grid location in rows 1-3 to place your piece: ");
                    }
                    else
                    {
                        Console.Write($"Please enter a grid location in rows 6-8 to place your piece: ");
                    }
                    string locationChosen = Console.ReadLine().Trim().ToUpper();

                    if (!string.IsNullOrEmpty(locationChosen) && Game.Battlefields[PlayerToggle].GridReference.Contains(locationChosen))
                    {
                        if (PlayerToggle == 0 && (locationChosen.EndsWith("1") || locationChosen.EndsWith("2") || locationChosen.EndsWith("3")))
                        {
                            keepGoing = false;
                            return locationChosen;
                        }
                        else if (PlayerToggle == 1 && (locationChosen.EndsWith("6") || locationChosen.EndsWith("7") || locationChosen.StartsWith("8")))
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

        private bool PlacePiece(string location, int piece)
        {
            // check the location to see if it doesn't contain a piece other than ""
            // if no, place
            // if yes, return error

            if (Game.Battlefields[PlayerToggle].Grid[location].GetName() == "Blank")
            {
                // this code is a hot mess
                // for the grid location, if blank
                // then place that peice's not on board location there
                Game.Battlefields[PlayerToggle].Grid[location] = Game.Battlefields[PlayerToggle].PiecesNotOnBoard[piece - 1];

                // update the piece to say its on the board
                Game.Battlefields[PlayerToggle].Grid[location].IsPlaced();
                Game.Battlefields[PlayerToggle].PiecesNotOnBoard.RemoveAt(piece - 1);
                return true;
            }
            else
            {
                Console.WriteLine("A piece already exists at this location. Please try again.");
                return false;
            }

        }

        public bool SetupPieces()
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

            int piecesLeftToPlace = Game.Battlefields[PlayerToggle].PiecesNotOnBoard.Count;

            do
            {
                // hAVE TO do this for player one and player two. reference their names
                SetupPiecesList();
                int piece = SetupAskForPiece();
                string location = SetupAskForLocation();
                PlacePiece(location, piece);
            } while (piecesLeftToPlace > 0);

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
            return true;
        }
    }
}
