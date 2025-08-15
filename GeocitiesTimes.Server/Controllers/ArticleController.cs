using GeocitiesTimes.Server.Clients;
using GeocitiesTimes.Server.Models;
using GeocitiesTimes.Server.Providers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GeocitiesTimes.Server.Controllers;

[ApiController]
[Route("api/articles")]
public class ArticleController(IArticleProvider articleProvider): ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticle(int id)
    {
        var article = await articleProvider.GetArticleFromCacheOrClient(id);
        return new OkObjectResult(article);
    }
}
