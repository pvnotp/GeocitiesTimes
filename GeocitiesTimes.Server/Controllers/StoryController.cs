using GeocitiesTimes.Server.Clients;
using GeocitiesTimes.Server.Models;
using GeocitiesTimes.Server.Providers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GeocitiesTimes.Server.Controllers;

[ApiController]
[Route("api/articles")]
public class StoryController(IBatchProvider batchProvider): ControllerBase
{

    [HttpGet("topStories")]
    public async Task<IActionResult> GetTopStories(int pageNum, int pageSize, string? searchTerm)
    {
        var topStories = await batchProvider.GetTopStories(pageNum, pageSize, searchTerm);
        return new OkObjectResult(topStories);
    }
}
