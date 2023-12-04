using LegoBuilder.Models;
using LegoBuilder.Models.All;
using System.Collections.Generic;

namespace LegoBuilder.SqlDaos
{
    public interface IPartSqlDao
    {
        public Part GetPartById(string id);
        public List<Part> WildcardSearchParts(string searchId);
        public List<Part> GetAllParts();
        public string DeletePart(string id);
        public string UnDeletePart(string id);
        public AllParts UpdateDatabase(AllParts allParts);
    }
}
