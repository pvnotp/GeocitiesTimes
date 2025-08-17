using GeocitiesTimes.Server.Clients;
using GeocitiesTimes.Server.Models;
using GeocitiesTimes.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;

namespace GeocitiesTimes.Server.Providers
{
    public class StoryProvider(INewsClient newsClient, ICacheService cacheService) : IStoryProvider
    {
        private const int _defaultMaxConcurrency = 10;

        public async Task<Story> GetStoryFromCacheOrClient(int id)
        {
            if (cacheService.TryGetArticleFromCache(id, out var cachedArticle))
            {
                return cachedArticle;
            }

            var article = await newsClient.GetStory(id);

            if (article != null)
            {
                cacheService.AddArticleToCache(article);
                return article;
            }

             throw new KeyNotFoundException($"No article found for id {id}");
        }

        public async Task<List<Story>> GetStoryListFromCacheOrClient(int[] storyIds)
        {

            if (storyIds == null || storyIds.Length == 0)
            {
                return new List<Story>();
            }

            using var semaphore = new SemaphoreSlim(_defaultMaxConcurrency, _defaultMaxConcurrency);

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
            var articles = results?.Where(x => x != null).Select(a => a!).ToList();
            return articles ?? new List<Story>();
        }

    }

}
