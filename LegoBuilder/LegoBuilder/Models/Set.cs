using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoBuilder.Models
{
    public class Set
    {
        public int Set_Id { get; set; }
        public string Set_Num { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int Theme_Id { get; set; }
        public string Theme_Name { get; set; }
        public int Num_Parts { get; set; }
        public string Set_Img_Url { get; set; }
        public string Set_Url { get; set; }
        public DateTime Last_Modified_Dt { get; set; }
        public bool Is_Active { get; set; }
        public DateTime LB_Creation_Date { get; set; }
        public DateTime LB_Update_Date { get; set; }
        public string Instructions_Url { get; set; }
    }
}
