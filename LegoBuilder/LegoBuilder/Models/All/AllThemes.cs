using System.Collections.Generic;

namespace LegoBuilder.Models.All
{
    public class AllThemes : AllBase
    {
        public List<Theme> Results { get; set; } = new List<Theme>();
    }
}
