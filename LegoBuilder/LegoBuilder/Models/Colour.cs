using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoBuilder.Models
{
    public class Colour
    {
        public int Colours_Id { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string RGB { get; set; }
        public bool Is_Trans { get; set; } 
        public bool Is_Active { get; set; } 
        public DateTime LB_Creation_Date { get; set; }
        public DateTime LB_Update_Date { get; set; }

        public override string ToString()
        {
            return $"Colour ID = {Id}";
        }
    }
}
