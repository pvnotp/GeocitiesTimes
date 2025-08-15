using GeocitiesTimes.Server.Models;

namespace GeocitiesTimes.Server.Services
{
    public interface ICacheService
    {
        public bool TryGetArticleFromCache(int id, out Article article);
        public void AddArticleToCache(Article article);
    }
}
