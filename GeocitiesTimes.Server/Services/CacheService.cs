using GeocitiesTimes.Server.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace GeocitiesTimes.Server.Services
{
    public class CacheService(IMemoryCache cache) : ICacheService
    {
        public bool TryGetArticleFromCache(int id, out Article? article)
        {
            return cache.TryGetValue(id, out article);
        }

        public void AddArticleToCache(Article article)
        {
            var options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
                Priority = CacheItemPriority.Normal
            };

            cache.Set(article.Id, article, options);

        }
    }
}
