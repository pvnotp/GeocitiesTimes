using GeocitiesTimes.Server.Models;

namespace GeocitiesTimes.Server.Services
{
    public interface ICacheService
    {
        public bool TryGetStoryFromCache(int id, out Story article);
        public void AddStoryToCache(Story article);
    }
}
