using Generals.Classes;
using System.Collections.Immutable;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace Generals
{
    public class Program
    {
        static void Main(string[] args)
        {
            // start the game
            InitializeUI initialize = new InitializeUI();
            initialize.InitializeUIRuntime();

            // not sure if I need this right now
            PlacementUI placement = new PlacementUI();
            placement.PlacementUIRuntime();

            

            



            
        }
    }
}