using System.Collections.Generic;

namespace LegoBuilder.Models.All
{
    public class AllParts : AllBase
    {
        public List<Part> Results { get; set; } = new List<Part>();
    }
}
