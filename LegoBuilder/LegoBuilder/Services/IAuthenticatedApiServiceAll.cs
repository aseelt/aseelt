using LegoBuilder.Models;
using LegoBuilder.Models.All;
using System.Collections.Generic;

namespace LegoBuilder.Services
{
    public interface IAuthenticatedApiServiceAll
    {
        public AllColours GetAllColours();
        public AllPartCats GetAllPartCategories();
        public AllParts GetAllParts();
        public AllSets GetAllSets();
        public AllThemes GetAllThemes();
        public SetParts GetAllSetParts(string setNum);
    }
}   
