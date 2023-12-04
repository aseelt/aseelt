using LegoBuilder.Models;
using LegoBuilder.Models.All;
using System.Collections.Generic;

namespace LegoBuilder.SqlDaos
{
    public interface IPartCatIdSqlDao
    {
        public PartCatId GetPartCatIdById(string id);
        public List<PartCatId> WildcardSearchPartCats(string searchId);
        public List<PartCatId> GetAllPartCatIds();
        public string DeletePartCatIds(string id);
        public string UnDeletePartCatIds(string id);
        public AllPartCats UpdateDatabase(AllPartCats allPartCats);
    }
}
