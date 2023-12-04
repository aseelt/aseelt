using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoBuilder.Models
{
    public class Part
    {
        public int Parts_Id { get; set; }
        public string Part_Num { get; set; }
        public string Name { get; set; }
        public int Part_Cat_Id { get; set; }
        public string Category_Name { get; set; }
        public int? Year_From { get; set; }
        public int? Year_To { get; set; }
        public string Part_Url { get; set; }
        public string Part_Img_Url { get; set; }
        public bool Is_Active { get; set; }
        public DateTime LB_Creation_Date { get; set; }
        public DateTime LB_Update_Date { get; set; }
    }
}
