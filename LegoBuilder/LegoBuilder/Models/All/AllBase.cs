using System.Collections.Generic;

namespace LegoBuilder.Models.All
{
    public class AllBase
    {
        public int Count { get; set; }
        public Dictionary<string, string> Added_Updated { get; set; } = new Dictionary<string, string>();
    }
}
