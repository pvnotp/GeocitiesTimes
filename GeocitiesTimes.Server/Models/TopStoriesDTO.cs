namespace GeocitiesTimes.Server.Models
{
    public class TopStoriesDTO
    {
        int PageNum { get; set; }
        int PageSize { get; set; }
        string SearchTerm { get; set; }
    }
}
