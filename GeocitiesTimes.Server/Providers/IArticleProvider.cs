using GeocitiesTimes.Server.Models;

namespace GeocitiesTimes.Server.Providers
{
    public interface IArticleProvider
    {
        Task<Article> GetArticleFromCacheOrClient(int id);
    }
}
