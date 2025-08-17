
using GeocitiesTimes.Server.Models;

namespace GeocitiesTimes.Server.Providers.Pages
{
    public interface IPagesProvider
    {
        public Task<IEnumerable<IEnumerable<Story>>> GetStoryPages(int[] storyIds, int pageNum, int pageSize, string? searchTerm);
    }
}
