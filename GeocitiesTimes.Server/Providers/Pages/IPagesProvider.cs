
using GeocitiesTimes.Server.Models;

namespace GeocitiesTimes.Server.Providers.Pages
{
    public interface IPagesProvider
    {
        public Task<List<Story[]>> GetStoryPages(int[]? storyIds, int pageNum, int pageSize, string? searchTerm);
    }
}
