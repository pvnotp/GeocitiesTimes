using GeocitiesTimes.Server.Models;

namespace GeocitiesTimes.Server.Providers.Stories
{
    public interface IStoriesProvider
    {
        Task<Story?> GetStoryFromCacheOrClient(int id);
        Task<IEnumerable<Story>> GetStoryListFromCacheOrClient(int[] storyIds);
    }
}
