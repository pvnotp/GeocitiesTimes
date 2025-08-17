using GeocitiesTimes.Server.Clients;
using GeocitiesTimes.Server.Models;
using GeocitiesTimes.Server.Providers.Stories;

namespace GeocitiesTimes.Server.Providers.Pages
{
    public class PagesProvider(IStoriesProvider storyProvider): IPagesProvider
    {
        public async Task<List<Story[]>> GetStoryPages(int[]? storyIds, int pageNum, int pageSize, string? searchTerm = null)
        {

            if (storyIds == null || storyIds.Length == 0)
            {
                return new List<Story[]>();
            }

            /* At maximum, we will fetch only the stories leading up to the requested page
             * (plus two extra, so that the UI doesn't have to make a call every time the user
             * advances the page, and can anticipate the final page).  This prevents us from 
             * having to fetch the full set of results in a single call, leading to a quicker 
             * response time.
             */
            var maxStoriesToFetch = Math.Max(pageSize, (pageNum + 2) * pageSize);
            var matches = new List<Story>(capacity: maxStoriesToFetch);
            
            /* We fetch stories in batches rather than retrieving the full set of results at once.
             * This allows us to exit early when we have enough results to display.
             */
            var batchSize = Math.Min(Constants.MaxBatchSize, pageNum * pageSize);
            foreach (var idBatch in storyIds.Chunk(batchSize))
            { 
                var storyBatch = await storyProvider.GetStoryListFromCacheOrClient(idBatch);

                if (storyBatch is null || storyBatch.Count == 0)
                {
                    continue;
                }

                //If there's no search term, return all results.
                if (string.IsNullOrEmpty(searchTerm))
                {
                    matches.AddRange(storyBatch);

                    //Exit early if we have enough.
                    if (matches.Count >= maxStoriesToFetch)
                    {
                        return matches.Chunk(pageSize).ToList();
                    }
                    continue;
                }

                //If there is a search term, filter the batch accordingly.
                foreach (var story in storyBatch)
                {
                    var title = story?.Title;
                    if (!string.IsNullOrEmpty(title) &&
                        title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    {
                        matches.Add(story!);

                        //Exit early if we have enough.
                        if (matches.Count >= maxStoriesToFetch)
                        {
                            return matches.Chunk(pageSize).ToList();
                        }
                    }
                }
            }

            //If there weren't enough results to fill three full pages, just return what we have.
            return matches.Chunk(pageSize).ToList();

        }
    }
}
