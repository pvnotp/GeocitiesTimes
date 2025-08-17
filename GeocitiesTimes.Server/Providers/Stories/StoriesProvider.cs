using GeocitiesTimes.Server.Clients;
using GeocitiesTimes.Server.Models;
using GeocitiesTimes.Server.Services;

namespace GeocitiesTimes.Server.Providers.Stories
{
    public class StoriesProvider(INewsClient newsClient, ICacheService cacheService) : IStoriesProvider
    {
        public async Task<Story?> GetStoryFromCacheOrClient(int id)
        {
            if (cacheService.TryGetStoryFromCache(id, out var cachedStory))
            {
                return cachedStory;
            }

            var story = await newsClient.GetStory(id);

            if (story != null)
            {
                cacheService.AddStoryToCache(story);
                return story;
            }

            return null; //Null results will be filtered out
        }

        public async Task<List<Story>> GetStoryListFromCacheOrClient(int[] storyIds)
        {

            if (storyIds == null || storyIds.Length == 0)
            {
                return new List<Story>();
            }

            /* The Hacker News API does not have a rate limiter, so we COULD hit it with unlimited 
             * concurrent requests. However, because we are nice people, this method limits
             * concurrency to avoid overloading their generously provided service.
             */

            using var semaphore = new SemaphoreSlim(Constants.DefaultMaxConcurrency, Constants.DefaultMaxConcurrency);

            var tasks = storyIds.Select(async id =>
            {
                // Wait for available slot
                await semaphore.WaitAsync();

                try
                {
                    return await GetStoryFromCacheOrClient(id);
                }
                catch (Exception ex)
                {
                    return null; // Return null instead of throwing to not fail entire batch
                }
                finally
                {
                    semaphore.Release();
                }
            });

            var results = await Task.WhenAll(tasks);

            //Filter out null stories
            var stories = results?.Where(x => x != null).Select(a => a!).ToList();
            return stories ?? new List<Story>();
        }

    }

}
