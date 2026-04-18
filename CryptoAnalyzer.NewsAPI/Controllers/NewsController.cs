using CryptoAnalyzer.News.Core;
using CryptoAnalyzer.NewsService.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoAnalyzer.NewsService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly ISender _sender;

    public NewsController(ISender sender)
    {
        _sender = sender;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NewsResponse>>> GetNews()
    {
        var response = await _sender.Send(new GetNewsQuery());
        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }
}