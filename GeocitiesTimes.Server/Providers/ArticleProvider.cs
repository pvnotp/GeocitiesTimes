using GeocitiesTimes.Server.Clients;
using GeocitiesTimes.Server.Models;
using GeocitiesTimes.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace GeocitiesTimes.Server.Providers
{
    public class ArticleProvider(INewsClient newsClient, ICacheService cacheService) : IArticleProvider
    {

        public async Task<Article> GetArticleFromCacheOrClient(int id)
        {
            if (cacheService.TryGetArticleFromCache(id, out var cachedArticle))
            {
                return cachedArticle;
            }

            var article = await newsClient.GetArticle(id);

            if(article != null)
            {
                cacheService.AddArticleToCache(article);
            }
            else
            {
                throw new KeyNotFoundException($"Did not find article with id: {id}");
            }

            return article;
        }
    }

}
