using System.ComponentModel.DataAnnotations;
using Episodes.Data;
using Episodes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Episodes.Controllers;

[ApiController]
[Authorize]
[Route("api/shows")]
public class ShowsController : ControllerBase
{
    private readonly ITvShowService _tvShowService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ShowsController(ITvShowService tvShowService, UserManager<ApplicationUser> userManager)
    {
        _tvShowService = tvShowService;
        _userManager = userManager;
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

    [HttpGet("{showId:int}")]
    public async Task<IActionResult> GetTvShow(int showId, CancellationToken cancellationToken = default)
    {
        var result = await _tvShowService.GetTvShowAsync(showId, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("external/{externalId:int}")]
    public async Task<IActionResult> GetTvShowByExternalId(int externalId, CancellationToken cancellationToken = default)
    {
        var result = await _tvShowService.GetTvShowByExternalIdAsync(externalId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{showId:int}/season/{seasonNumber:int}")]
    public async Task<IActionResult> GetSeasonEpisodes(int showId, int seasonNumber,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _tvShowService.GetSeasonEpisodesAsync(userId, showId, seasonNumber, cancellationToken);
        return Ok(result);
    }
    
    private int? GetUserId()
    {
        var userIdString = _userManager.GetUserId(User);
        return int.TryParse(userIdString, out var userId) ? userId : null;
    }
}