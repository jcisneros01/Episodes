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

    [HttpGet("{tvShowId:int}")]
    public async Task<IActionResult> GetTvShow(int tvShowId, CancellationToken cancellationToken = default)
    {
        var result = await _tvShowService.GetTvShowAsync(tvShowId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{tvShowId:int}/season/{seasonNumber:int}")]
    public async Task<IActionResult> GetSeasonEpisodes(int tvShowId, int seasonNumber,
        CancellationToken cancellationToken = default)
    {
        var result = await _tvShowService.GetSeasonEpisodesAsync(tvShowId, seasonNumber, cancellationToken);
        return Ok(result);
    }
}