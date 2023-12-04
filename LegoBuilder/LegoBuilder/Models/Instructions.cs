using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoBuilder.Models
{
    public class Instructions
    {
        public string Set_Id { get; set; }
        private string Url { get; set; }
        public bool Is_Active { get; set; }
        public DateTime LB_Creation_Date { get; set; }
        public DateTime LB_Update_Date { get; set; }
    }
}
