using LegoBuilder.Models;
using LegoBuilder.Models.All;
using System.Collections.Generic;

namespace LegoBuilder.SqlDaos
{
    public interface IUserSetsSqlDao
    {
        public UserFullInfo GetFullInfoByUsername(string username);
        public UserFullInfo WildcardSearchAllFieldsUsersSets(string username, string searchId);
        public UserFullInfo WildcardSearchByFieldUsersSets(string username, string setSearch = "", string partSearch = "", string colourSearch = "");
        public List<UserSetInfo> GetSetOnlyInfoByUsername(string username);
        public List<UserPartColourInfo> GetPartAndColourInfoByUsername(string username);
        public List<UserPartInfo> GetPartOnlyInfoByUsername(string username);
        public List<UserColourInfo> GetColourOnlyInfoByUsername(string username);
        public List<UserPartCatInfo> GetPartCategoriesInfoByUsername(string username);
        public List<UserThemeInfo> GetThemesInfoByUsername(string username);
        public UserTotalPartInfo GetTotalPartInfoByUsername(string username);
        public UsersSets AddNewSet(string username, string setNumber, int quantity);
        public string ChangeSetQuantity(string username, string setNumber, int quantity);
        public string FavouriteUsersSet(string username, string setNumber);
        public string UnFavouriteUsersSet(string username, string setNumber);
    }
}
