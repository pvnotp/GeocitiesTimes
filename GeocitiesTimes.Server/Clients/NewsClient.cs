using GeocitiesTimes.Server.Models;
using System.Net.Http;
using static System.Net.WebRequestMethods;

namespace GeocitiesTimes.Server.Clients
{
    public class NewsClient(HttpClient httpClient) : INewsClient
    {
        private const string _baseURL = "https://hacker-news.firebaseio.com/v0/";
        public async Task<Article?> GetArticle(int id)
        {
            return await httpClient.GetFromJsonAsync<Article>($"{_baseURL}item/{id}.json");
        }

        //public async Task<int[]?> GetTopStories()
        //{
        //    return await httpClient.GetFromJsonAsync<int[]>("topstories.json");
        //}
    }
}
