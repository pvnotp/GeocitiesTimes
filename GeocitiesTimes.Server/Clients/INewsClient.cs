using GeocitiesTimes.Server.Models;

namespace GeocitiesTimes.Server.Clients
{
    public interface INewsClient
    {
        public Task<Story?> GetStory(int id);
        public Task<int[]?> GetNewStoryIds();

    }
}
