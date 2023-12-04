using LegoBuilder.Models;
using LegoBuilder.Models.All;
using System.Collections.Generic;

namespace LegoBuilder.SqlDaos
{
    public interface ISetSqlDao
    {
        public Set GetSetById(string id);
        public List<Set> WildcardSearchSets(string searchId);
        public List<Set> GetAllSets();
        public string DeleteSet(string id);
        public string UnDeleteSet(string id);
        public AllSets UpdateDatabase(AllSets allSets);
        public bool UpdateInstructions();
    }
}
