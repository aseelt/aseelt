using LegoBuilder.Models;
using LegoBuilder.Models.All;
using System.Collections.Generic;

namespace LegoBuilder.SqlDaos
{
    public interface IThemeSqlDao
    {
        public Theme GetThemeById(string id);
        public List<Theme> WildcardSearchThemes(string searchId);
        public List<Theme> GetAllThemes();
        public string DeleteTheme(string id);
        public string UnDeleteTheme(string id);
        public AllThemes UpdateDatabase(AllThemes allThemes);
    }
}
