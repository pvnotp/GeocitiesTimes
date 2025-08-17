using GeocitiesTimes.Server.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace GeocitiesTimes.Server.Services
{
    public class CacheService(IMemoryCache cache) : ICacheService
    {
        public bool TryGetArticleFromCache(int id, out Story? story)
        {
            return cache.TryGetValue(id, out story);
        }

        public void AddArticleToCache(Story story)
        {
            var options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
                Priority = CacheItemPriority.Normal
            };

            cache.Set(story.Id, story, options);

        }
    }
}
