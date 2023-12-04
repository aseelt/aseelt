using System;
using System.Collections.Generic;

namespace LegoBuilder.Models
{
    public class UserBase
    {
        public string Username { get; set; }
        public bool User_Active { get; set; }
    }
    public class UserFullInfo : UserBase
    {
        public List<FullSetInfo> Sets { get; set; } = new List<FullSetInfo>();
    }
    // this is used in the full object
    public class FullSetInfo
    {
        public int Sets_Parts_Id { get; set; }
        public string Set_Num { get; set; }
        public bool Is_Favourite { get; set; }
        public bool Is_Active { get; set; }
        public DateTime LB_Creation_Date { get; set; }
        public DateTime LB_Update_Date { get; set; }

        public string Set_Name { get; set; }
        public int Set_Quantity { get; set; }
        public int Year_Released { get; set; }
        public int Total_Num_Parts { get; set; }
        public string Set_Img_Url { get; set; }
        public DateTime Set_RB_Last_Modified { get; set; }
        public string Instructions_Url { get; set; }
        public string Set_Theme { get; set; }
        public int Part_Count
        {
            get
            {
                return Parts.Count;
            }
        }

        public List<FullPartInfo> Parts { get; set; } = new List<FullPartInfo>();
        public override string ToString()
        {
            return Set_Num + " - " + Set_Name;
        }
    }
    // this is used in the full object
    public class FullPartInfo
    {
        public string Part_Num { get; set; }
        public string Element_Id { get; set; }
        public int Part_Quantity { get; set; }
        public string Part_Name { get; set; }
        public string Part_Cat_Name { get; set; }
        public string Part_Url { get; set; }
        public string Part_Img_Url { get; set; }

        public string Colour { get; set; }
        public string RGB { get; set; }
        public bool Is_Trans { get; set; }
        public override string ToString()
        {
            return Part_Num + " - " + Part_Name;
        }
    }

    // these are used for the more specific returned objects
    public class UserThemeInfo : UserBase
    {
        public string Set_Theme { get; set; }
        public int Set_Quantity { get; set; }
        public int Total_Num_Parts { get; set; }
        public override string ToString()
        {
            return Set_Theme;
        }
    }
    public class UserSetInfo : UserThemeInfo
    {
        public string Set_Num { get; set; }
        public bool Is_Favourite { get; set; }
        public bool Is_Active { get; set; }
        public DateTime LB_Creation_Date { get; set; }
        public DateTime LB_Update_Date { get; set; }
        public string Set_Name { get; set; }
        public int Year_Released { get; set; }
        public string Set_Img_Url { get; set; }
        public DateTime Set_RB_Last_Modified { get; set; }
        public string Instructions_Url { get; set; }
        public override string ToString()
        {
            return Set_Num + " - " + Set_Name;
        }
    }

    public class UserTotalPartInfo : UserBase
    {
        public int Part_Quantity { get; set; }
        public override string ToString()
        {
            return "Total Parts: " + Part_Quantity;
        }
    }

    public class UserPartCatInfo : UserTotalPartInfo
    {
        public string Part_Cat_Name { get; set; }
        public override string ToString()
        {
            return Part_Cat_Name;
        }
    }
    public class UserColourInfo : UserPartCatInfo
    {
        public string Colour { get; set; }
        public string RGB { get; set; }
        public bool Is_Trans { get; set; }
        public override string ToString()
        {
            return Colour;
        }
    }

    public class UserPartInfo : UserColourInfo
    {
        public string Part_Num { get; set; }
        public string Part_Name { get; set; }
        public string Part_Url { get; set; }
        public string Part_Img_Url { get; set; }
        public override string ToString()
        {
            return Part_Num + " - " + Part_Name;
        }
    }

    public class UserPartColourInfo : UserPartInfo 
    {
        public string Element_Id { get; set; }
        public override string ToString()
        {
            return Part_Num + " - " + Part_Name + " - " + Colour;
        }
    }
    
}
