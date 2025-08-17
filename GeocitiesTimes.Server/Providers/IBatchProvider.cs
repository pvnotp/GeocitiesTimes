using GeocitiesTimes.Server.Models;
using System.Threading.Tasks;

namespace GeocitiesTimes.Server.Providers
{
    public interface IBatchProvider
    {
        public Task<List<Story[]>> GetTopStories(int pageNum, int pageSize, string? searchTerm);
    }
}
