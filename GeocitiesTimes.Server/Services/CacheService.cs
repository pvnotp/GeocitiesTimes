using GeocitiesTimes.Server.Models;
using Microsoft.Extensions.Caching.Memory;

namespace GeocitiesTimes.Server.Services
{
    public class CacheService(IMemoryCache cache) : ICacheService
    {
        public bool TryGetStoryFromCache(int id, out Story story)
        {
            var status = cache.TryGetValue(id, out story);

            if (story == null)
            {
                return false;
            }

            return status;
        }


        public void AddStoryToCache(Story story)
        {
            /* Currently this application only processes a maximum of 500 stories, 
             * so we can afford to add the entire collection to the cache.  If the requirements
             * of this application change to include more items, time-based eviction
             * policies can easily be applied here, and the maximum number of items in cache,
             * as defined in Program.cs, can be unset.
             */

            var options = new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.Normal,
                Size = 1
            };

            cache.Set(story.Id, story, options);
        }
    }
}
