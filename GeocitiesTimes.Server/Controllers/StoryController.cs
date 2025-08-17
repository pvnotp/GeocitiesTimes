
using GeocitiesTimes.Server.Clients;
using GeocitiesTimes.Server.Models;
using GeocitiesTimes.Server.Providers.Pages;
using Microsoft.AspNetCore.Mvc;
namespace GeocitiesTimes.Server.Controllers;

[ApiController]
[Route("api/stories")]
public class StoryController(IPagesProvider batchProvider, INewsClient newsClient): ControllerBase
{

    [HttpGet("new")]
    public async Task<IActionResult> GetNewStories([FromQuery]NewsRequestDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newStoryIds = await newsClient.GetNewStoryIds();
        var newStories = await batchProvider.GetStoryPages(newStoryIds, dto.PageNum, dto.PageSize, dto.SearchTerm);

        if (newStories.Count == 0)
        {
            return NotFound();
        }

        return Ok(newStories);
    }
}
