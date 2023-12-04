using System;

namespace LegoBuilder.Models
{
    public class UsersSets
    {
        public int Users_Sets_Id { get; set; }
        public int User_Id { get; set; }
        public string Set_Num { get; set; }
        public int Quantity { get; set; }
        public bool Is_Favourite { get; set; }
        public bool Is_Active { get; set; }
        public DateTime LB_Creation_Date { get; set; }
        public DateTime LB_Update_Date { get; set; }

    }
}
