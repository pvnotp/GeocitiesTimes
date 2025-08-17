using GeocitiesTimes.Server.Models;

namespace GeocitiesTimes.Server.Services
{
    public interface ICacheService
    {
        public bool TryGetArticleFromCache(int id, out Story article);
        public void AddArticleToCache(Story article);
    }
}
