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

        

        // what if it's a dictionary, then I can call the key instead of the array position
        // i would love to but it's not working...
        //public Dictionary<string, Battlefield> Battlefields = new Dictionary<string, Battlefield>();

        // queue for the next player
        // hold this information here, then toggle in the UI using a method below
        //TODO queue
        //public Queue<string> PlayerToggle = new Queue<string>();


        // methods
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

        

        

        
        
    }
}
