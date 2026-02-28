using System.ComponentModel.DataAnnotations;
using Episodes.Services;
using Microsoft.AspNetCore.Mvc;

namespace Episodes.Controllers;

[ApiController]
[Route("api/shows")]
public class ShowsController : ControllerBase
{
    private readonly ITvShowService _tvShowService;

    public ShowsController(ITvShowService tvShowService)
    {
        _tvShowService = tvShowService;
    }


    [HttpGet("search")]
    public async Task<IActionResult> SearchTvShows(
        [Required] [FromQuery] string query,
        [FromQuery] int? page,
        CancellationToken cancellationToken = default)
    {
        var result = await _tvShowService.SearchTvShowsAsync(query, page, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTvShow(int id, CancellationToken cancellationToken = default)
    {
        var result = await _tvShowService.GetTvShowAsync(id, cancellationToken);
        return Ok(result);
    }
}