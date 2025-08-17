
using GeocitiesTimes.Server.Clients;
using GeocitiesTimes.Server.Models;
using GeocitiesTimes.Server.Providers.Pages;
using Microsoft.AspNetCore.Mvc;
namespace GeocitiesTimes.Server.Controllers;

[ApiController]
[Route("stories")]
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
        if (newStoryIds == null || newStoryIds.Count() == 0)
        {
            //If no new stories could be fetched from the API, something is wrong.
            return NotFound("Could not retrieve new stories");
        }

        var newStories = await batchProvider.GetStoryPages(newStoryIds, dto.PageNum, dto.PageSize, dto.SearchTerm);
        if (newStories.Count() == 0)
        {
            //No results were found, but that's okay.
            return Ok(new List<List<Story>>());
        }



        return Ok(newStories);
    }
}
