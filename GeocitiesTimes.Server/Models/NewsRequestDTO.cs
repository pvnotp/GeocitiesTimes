using System.ComponentModel.DataAnnotations;

namespace GeocitiesTimes.Server.Models
{
    public class NewsRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
        public int PageNum { get; set; }

        [Range(1, 101, ErrorMessage = "Page size must be between 1 and 100.")]
        public int PageSize { get; set; }

        [MinLength(2, ErrorMessage = "Search term must be at least 2 characters")]
        public string? SearchTerm { get; set; }
    }
}
