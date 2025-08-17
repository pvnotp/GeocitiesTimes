using GeocitiesTimes.Server.Models;

namespace GeocitiesTimes.Server.Providers
{
    public interface IStoryProvider
    {
        Task<Story> GetStoryFromCacheOrClient(int id);
        Task<List<Story>> GetStoryListFromCacheOrClient(int[] storyIds);
    }
}
