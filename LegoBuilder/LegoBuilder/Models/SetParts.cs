using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegoBuilder.Models
{
    public class SetParts
    {
        public int Count { get; set; }
        public int Total_Parts { get
            {
                int total = 0;
                foreach(ResultsSetParts item in Results)
                {
                    total += item.Quantity;
                }
                return total;
            } 
        }
        public string Set_Num { get
            {
                return Results.Count == 0 ? null : Results[0].Set_Num; 
            } 
        }
        public Dictionary<string, string> Added_Updated { get; set; } = new Dictionary<string, string>();
        public List<ResultsSetParts> Results { get; set; } = new List<ResultsSetParts>();

    }
}
