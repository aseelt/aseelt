using System.Collections.Generic;

namespace LegoBuilder.Models.All
{
    public class AllPartCats : AllBase
    {
        public List<PartCatId> Results { get; set; } = new List<PartCatId>();
    }
}
