using System.Threading.Tasks;

namespace LegoBuilder.Services
{
    public interface ILegoUrlGetter
    {
        public Task<int> UrlChecker(string truncatedSetNumber);
    }
}
