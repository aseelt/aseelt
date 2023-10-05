using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generals.Classes
{
    public class InitializeUI
    {

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
            Actions game = new Actions();
            game.CreateBattlefield(playerOne, playerTwo);

            // assign a piece to a location
            // calls the piece's array position, doesn't create a piece
            // just a placeholder
            // call the overall game, then the specific player's battlefield, then the location, then equals to the game, specific army, and then call the piece you want to assign
            //TODO make this a method too: get the piece, assign it to a grid, change the piece's is placed to yes, update list of active pieces and pieces yet to be placed
            game.Battlefields[0].Grid["A1"] = game.Battlefields[0].Army[0];
            game.Battlefields[1].Grid["A1"] = game.Battlefields[1].Army[5];

            // let's just say we're on player 1
            //TODO figure out how to assign player 1 to 1 here, so you have a toggle
            string playerToggle = playerOne;

            // instead use a method in battlefield to pull the piece out to be manipulated
            // then in piece, manipulate the position per placement, movement, or attack
            Console.WriteLine(game.BattlefieldDisplay(playerToggle));
            //PiecePlacement(battlefields, playerToggle, pieceNumber, positionDesired);

            bool piecePlacementChooser = true;
            do
            {
                try
                {
                    Console.Write("Which piece would you like to place (1-21)? If you would like to see a list of pieces, type 'Pieces'. ");
                    string desiredPiece = Console.ReadLine().ToLower();
                    Piece selectedPiece = new Piece();
                    if (desiredPiece == "pieces")
                    {
                        //for (int i = 0; i < battlefields[playerToggle - 1].MyArmy.Pieces.Count; i++)
                        //{
                        //    Console.WriteLine("Piece Number: " + (i + 1) + ". Piece Name: " + battlefields[playerToggle - 1].MyArmy.Pieces[i].Name);
                        //}
                    }
                    else if (int.Parse(desiredPiece) >= 1 && int.Parse(desiredPiece) <= 21)
                    {
                        bool desiredPlacementChooser = true;                        
                        do
                        {
                            try
                            {
                                Console.Write("Which grid coordinates would you like to place this piece? Please use the format A1-H8. ");
                                string desiredPlacement = Console.ReadLine().ToUpper();
                                //if (battlefields[playerToggle - 1].Grid.ContainsKey(desiredPlacement))
                                //{
                                //    //battlefields[playerToggle - 1].MyArmy.Pieces[int.Parse(desiredPiece) - 1].Position = desiredPlacement;
                                //    Console.WriteLine($"Your piece has been placed at {desiredPlacement} successfully");
                                //    desiredPlacementChooser = false;
                                //    piecePlacementChooser = false;
                                //}
                                //else
                                //{
                                //    Console.WriteLine("You've entered an incorrect set of grid coordinates. Please try again using A1-H8");
                                //}
                                
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("You've entered an incorrect set of grid coordinates. Please try again using A1-H8");
                            }
                        } while (desiredPlacementChooser);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("You've entered an incorrect value. Please enter a number between 1-21, inclusive, or type 'Pieces'.");
                }
                //else
                //{
                //    selectedPiece = battlefields[playerToggle - 1].MyArmy.Pieces.IndexOf(desiredPiece);
                //}
            } while (piecePlacementChooser);
            //Console.WriteLine(playerOneBattlefield.BattlefieldDisplay(battlefields, playerToggle));

            return true;
        }

    }
}
