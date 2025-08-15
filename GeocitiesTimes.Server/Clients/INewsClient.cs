using GeocitiesTimes.Server.Models;

namespace GeocitiesTimes.Server.Clients
{
    public interface INewsClient
    {
        public Task<Article?> GetArticle(int id);
        //public Task<int[]?> GetTopStories();
    }
}
