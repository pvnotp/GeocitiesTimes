using GeocitiesTimes.Server.Models;

namespace GeocitiesTimes.Server.Providers.Stories
{
    public interface IStoriesProvider
    {
        Task<Story?> GetStoryFromCacheOrClient(int id);
        Task<List<Story>> GetStoryListFromCacheOrClient(int[] storyIds);
    }
}
