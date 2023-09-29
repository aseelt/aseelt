using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generals.Classes
{
    public class Piece
    {
        // properties
        // lets get them on the page first
        // then lets figure out what needs to be get, set, private etc.

        // rank should only be created, it can't be modified after creation
        // we'll need rank for combat, so leave get
        // rank needs to be public for the board to calculate combat
        public int Rank { get; }

        // can derive name using a field
        // needs to be private set. This class can set it, but no one else can
        // but it needs to be public because we'll need the name
        // since it's derived from a dictionary field, simple get works
        // get just returns the dictionary acces
        // no need for set or backing variable
        public string Name 
        { 
            get 
            { 
                return RankToName[Rank]; 
            } 
        }

        // like Name, it's derived based on the Rank
        // like Name, it can't be changed
        // needs to be public, the board will need it to calculate combat
        // but the set can be private and based on an if statement
        public int ForceMultiplier
        {
            get
            {
                if (Rank == -1) // rank of spy, most restrictive
                {
                    return 1;
                }
                else // everything else should be a 1
                {
                    return 1;
                }
                
            }
        } 

        // will need to be public so the board can calculate combat
        // will need to change when peices die
        // initial value is true, all pieces are created alive
        public bool isAlive { get; set; } = true;

        // will need to be public so the board can calculate moves and combat
        // does it need to live here or on the board class?
        // will need to be settable because it'll move
        // no need to put default values, player will place them
        //TODO position will need to be a dictionary, right? let's come back to it
        public string Position { get; set; } = "";

        // this is just the piece class
        // need a class for the overall army

        // constructors
        // pieces must be built with these
        // really all i need is the rank to create the pieces
        // put in some error checking
        // in the team, automate the process of creating the rank
        // so no user can input values and make a custom deck
        public Piece(int rank)
        {
            if(rank >= -2 && rank <= 32) // only create if accurate value supplied for ranks 5* to flag
            {
                Rank = rank;
            }            
        }

        // create dictionary field of Rank providing Name
        // private, outside doesn't need it
        //TODO do I need a critical value for the flag? A child class for it? or should I add that to a rules class
        private Dictionary<int, string> RankToName = new Dictionary<int, string>()
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
        
    }
}
