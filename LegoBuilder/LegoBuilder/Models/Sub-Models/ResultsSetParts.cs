using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoBuilder.Models
{
    public class ResultsSetParts
    {
        
        public int Sets_Parts_Id { get; set; }
        public int Id { get; set; }
        public int? Inv_Part_Id { get; set; }
        public Part Part { get; set; } = new Part();
        public Colour Color { get; set; } = new Colour();
        public string Set_Num { get; set; }
        public int Quantity { get; set; }
        public bool Is_Spare { get; set; }
        public string Element_Id { get; set; }
        public bool Is_Active { get; set; }
        public DateTime LB_Creation_Date { get; set; }
        public DateTime LB_Update_Date { get; set; }
    }
}
