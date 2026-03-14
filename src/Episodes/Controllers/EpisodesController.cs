using Episodes.Data;
using Episodes.Models;
using Episodes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Episodes.Controllers;

[ApiController]
[Authorize]
[Route("api/episodes/{episodeId:int}")]
public class EpisodesController : ControllerBase
{
    private readonly IEpisodesService _episodesService;
    private readonly UserManager<ApplicationUser> _userManager;

    public EpisodesController(IEpisodesService episodesService, UserManager<ApplicationUser> userManager)
    {
        _episodesService = episodesService;
        _userManager = userManager;
    }

    [HttpPost("watched")] 
    public async Task<IActionResult> MarkEpisodeWatched(int episodeId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _episodesService.MarkEpisodeAsWatched(userId.Value, episodeId, cancellationToken);
        if (result.Error != null)
        {
            return result.Error switch
            {
                MarkEpisodeWatchedError.ShowNotInWatchlist => UnprocessableEntity(new
                    { message = "Show must be in your watchlist before marking episodes as watched." }),
                MarkEpisodeWatchedError.EpisodeNotFound => NotFound(),
                MarkEpisodeWatchedError.AlreadyWatched => Conflict(),
                _ => StatusCode(500)
            };
        }

        return NoContent();
    }

    [HttpDelete("watched")] 
    public async Task<IActionResult> MarkEpisodeUnwatched(int episodeId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        
        var result = await _episodesService.UnmarkEpisodeAsWatched(userId.Value, episodeId, cancellationToken);
        if (result.Error != null)
        {
            return result.Error switch
            {
                UnmarkEpisodeWatchedError.EpisodeNotWatched => NotFound(),
                _ => StatusCode(500)
            };
        }

        return NoContent();
    }
    
    private int? GetUserId()
    {
        var userIdString = _userManager.GetUserId(User);
        return int.TryParse(userIdString, out var userId) ? userId : null;
    }
}