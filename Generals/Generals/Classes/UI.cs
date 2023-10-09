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

            // get the team name
            Console.WriteLine("Welcome to Generals");
            //TODO add more fluff later

            //TODO fix and need to add error checking
            Console.WriteLine("\nPlayer One, enter your army's name: ");
            string playerOne = "PlayerOneAseel"; //Console.ReadLine();

            //TODO fix and need to add error checking 
            //TODO error checking make sure army starts with a different character
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

            // assign a piece to a location
            // calls the piece's array position, doesn't create a piece
            // just a placeholder
            // call the overall game, then the specific player's battlefield, then the location, then equals to the game, specific army, and then call the piece you want to assign
            //TODO make this a method too: get the piece, assign it to a grid, change the piece's is placed to yes, update list of active pieces and pieces yet to be placed
            Game.Battlefields[0].Grid["A1"] = Game.Battlefields[0].Army[0];
            Game.Battlefields[1].Grid["A1"] = Game.Battlefields[1].Army[5];

            // let's just say we're on player 1
            //TODO figure out how to assign player 1 to 1 here, so you have a toggle
            TogglePlayers();

            // instead use a method in battlefield to pull the piece out to be manipulated
            // then in piece, manipulate the position per placement, movement, or attack
            Console.WriteLine(BattlefieldDisplay());
                        

            Console.WriteLine(SetupPiecesList());
            if (PlayerToggle == 0)
            {
                string locationChosen = SetupAskForLocation($"Please enter a grid location in rows A-C to place your piece: ");
            }
            else
            {
                string locationChosen = SetupAskForLocation($"Please enter a grid location in rows F-H to place your piece: ");
            }
            //int pieceChosen = SetupAskForPiece($"Please enter a number 1-{Game.Battlefields[PlayerToggle].PiecesNotOnBoard.Count} to select a piece: ");

            return false;
        }

        //player toggle lives here, the UI controls this
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


        // gets the value, parses, checks it's a valid piece
        private int SetupAskForPiece(string message)
        {
            bool keepGoing = true;

            do
            {
                //print message
                Console.Write(message);
                try
                {
                    // parse to number
                    // catch if that fails
                    int pieceChosen = int.Parse(Console.ReadLine().Trim());

                    // checks the index position is valid, 
                    // if so, breaks the loop and returns the piece for use
                    if (pieceChosen >= (1 - 1) & pieceChosen <= (Game.Battlefields[PlayerToggle].PiecesNotOnBoard.Count - 1))
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

        private string SetupPiecesList()
        {
            string output = "\nThese pieces have not been placed yet:\n\n";

            for (int i = 0; i < Game.Battlefields[PlayerToggle].PiecesNotOnBoard.Count; i++)
            {
                output += $"{i + 1}. {Game.Battlefields[PlayerToggle].PiecesNotOnBoard[i].GetName()}\n";
            }

            return output;
        }

        private string SetupAskForLocation(string message)
        {
            bool keepGoing = true;

            do
            {
                //print message
                Console.Write(message);
                try
                {
                    string locationChosen = Console.ReadLine().Trim().ToUpper();

                    if (!string.IsNullOrEmpty(locationChosen) && Game.Battlefields[PlayerToggle].GridReference.Contains(locationChosen))
                    { 
                        if (PlayerToggle == 0 && (locationChosen.StartsWith("A") || locationChosen.StartsWith("B") || locationChosen.StartsWith("C")))
                        {
                            return locationChosen;
                            keepGoing = false;
                        }
                        else if (PlayerToggle == 1 && (locationChosen.StartsWith("F") || locationChosen.StartsWith("G") || locationChosen.StartsWith("H")))
                        {
                            return locationChosen;
                            keepGoing = false;
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

        public string SetupPieces(string pieceChosen)
        {
            //place piece
            //originates in UI
            // prints list of pieces yet to place y 
            // ask for a piece y 
            // check if it's valid y 
            // check if it's on the not on board list, if true go ahead y
            // use that variable to select the piece from the list y 
            // ask where they want to put it
            // make sure that's a valid location
            // make sure it's unoccupied
            // make sure it's a-c for player 0, f-h for player 1
            // update the piece's location
            // remove from the list of pieces not on board but alive
            // keep going till the pieces not on board list is empty

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
            return "Which piece would you like to place";
        }
    }
}
