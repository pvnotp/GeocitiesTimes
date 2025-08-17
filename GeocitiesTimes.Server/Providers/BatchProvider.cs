using GeocitiesTimes.Server.Clients;
using GeocitiesTimes.Server.Models;
using GeocitiesTimes.Server.Services;
using System;
using System.Diagnostics;

namespace GeocitiesTimes.Server.Providers
{
    public class BatchProvider(INewsClient newsClient, IStoryProvider storyProvider): IBatchProvider
    {
        public async Task<List<Story[]>> GetTopStories(int pageNum, int pageSize, string? searchTerm = null)
        {
            var topStoryIds = await newsClient.GetTopStoryIds();
            var maxStoriesToFetch = Math.Max(pageSize, (pageNum + 2) * pageSize);
            var matches = new List<Story>(capacity: maxStoriesToFetch);
            var batchSize = Math.Min(50, pageNum * pageSize);

            foreach (var idBatch in topStoryIds.Chunk(batchSize))
            { 
                var storyBatch = await storyProvider.GetStoryListFromCacheOrClient(idBatch);

                if (storyBatch is null || storyBatch.Count == 0)
                {
                    continue;
                }

                if (String.IsNullOrEmpty(searchTerm))
                {
                    matches.AddRange(storyBatch);
                    if (matches.Count >= maxStoriesToFetch)
                    {
                        return matches.Chunk(pageSize).ToList();
                    }
                    continue;
                }

                foreach (var story in storyBatch)
                {
                    var title = story?.Title;
                    if (!String.IsNullOrEmpty(title) &&
                        title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    {
                        matches.Add(story!);
                        if (matches.Count >= maxStoriesToFetch)
                        {
                            return matches.Chunk(pageSize).ToList();
                        }
                    }
                }
            }

            return matches.Chunk(pageSize).ToList();

        }
    }
}
