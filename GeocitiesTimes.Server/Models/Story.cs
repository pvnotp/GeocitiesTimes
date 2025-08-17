namespace GeocitiesTimes.Server.Models
{
    public class Story
    {
        public required int Id { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }

        //The Hacker News API includes many more fields, but these are all I needed for my UI.
    }
}
