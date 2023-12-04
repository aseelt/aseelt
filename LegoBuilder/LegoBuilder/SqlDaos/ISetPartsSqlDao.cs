using LegoBuilder.Models;
using LegoBuilder.Models.All;
using System.Collections.Generic;

namespace LegoBuilder.SqlDaos
{
    public interface ISetPartsSqlDao
    {
        public SetParts GetSetPartsBySetNum(string setNum);
        public string GetRandomSetNumber();
        public List<FullSetInfo> WildcardSearchAllFieldsSetParts(string searchId);
        public List<FullSetInfo> WildcardSearchByFieldSetParts(string setSearch = "", string partSearch = "", string colourSearch = "");
        public FullSetInfo GetFullSetInfo(string inputSetNum);
        public SetParts UpdateDatabase(SetParts setPart);
    }
}
