using GeocitiesTimes.Server.Models;

namespace GeocitiesTimes.Server.Clients
{
    public class NewsClient(HttpClient httpClient) : INewsClient
    {
        private const string _baseURL = "https://hacker-news.firebaseio.com/v0/";

        public async Task<Story?> GetStory(int id)
        {
            return await httpClient.GetFromJsonAsync<Story>($"{_baseURL}item/{id}.json");
        }

        public async Task<int[]?> GetNewStoryIds()
        {
            return await httpClient.GetFromJsonAsync<int[]>($"{_baseURL}topstories.json");
        }
    }
}
