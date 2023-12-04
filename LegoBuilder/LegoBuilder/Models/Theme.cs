using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoBuilder.Models
{
    public class Theme
    {
        public int Themes_Id { get; set; }
        public int Id { get; set; }
        public int? Parent_Id { get; set; }
        public string Name { get; set; }
        public bool Is_Active { get; set; }
        public DateTime LB_Creation_Date { get; set; }
        public DateTime LB_Update_Date { get; set; }
    }
}
