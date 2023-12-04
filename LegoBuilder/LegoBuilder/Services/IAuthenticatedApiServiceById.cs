using LegoBuilder.Models;

namespace LegoBuilder.Services
{
    public interface IAuthenticatedApiServiceById
    {
        public Colour GetColourById(string id);
        public PartCatId GetPartCategoriesById(int id);
        public Part GetPartById(string id);
        public Set GetSetById(string id);
        public SetParts GetSetPartsById(string id);
        public Theme GetThemeById(string id);
    }
}
