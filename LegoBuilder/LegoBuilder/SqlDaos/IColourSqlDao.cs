using LegoBuilder.Models;
using LegoBuilder.Models.All;
using System.Collections.Generic;

namespace LegoBuilder.SqlDaos
{
    public interface IColourSqlDao
    {
        public Colour GetColourById(string id);
        public List<Colour> WildcardSearchColours(string searchId);
        public List<Colour> GetAllColours();
        public string DeleteColour(string id);
        public string UnDeleteColour(string id);
        public AllColours UpdateDatabase(AllColours allColours);
    }
}
