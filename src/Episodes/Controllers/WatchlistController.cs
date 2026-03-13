using Episodes.Data;
using Episodes.Models;
using Episodes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Episodes.Controllers;

[ApiController]
[Authorize]
[Route("api/watchlist")]
public sealed class WatchlistController : ControllerBase
{
    private readonly IWatchlistService _watchlistService;
    private readonly UserManager<ApplicationUser> _userManager;

    public WatchlistController(IWatchlistService watchlistService, UserManager<ApplicationUser> userManager)
    {
        _watchlistService = watchlistService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetWatchlist(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var watchlistAsync = await _watchlistService.GetWatchlistAsync(userId.Value, cancellationToken);

        return Ok(watchlistAsync);
    }

    [HttpPost("{showId:int}")]
    public async Task<IActionResult> AddShow(int showId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await _watchlistService.AddShowAsync(userId.Value, showId, cancellationToken);

        return result.Error switch
        {
            AddShowError.ShowNotFound => NotFound(),
            AddShowError.AlreadyOnWatchlist => Conflict(),
            _ => Ok(result.Item)
        };
    }

    [HttpDelete("{showId:int}")]
    public async Task<IActionResult> RemoveShow(int showId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        await _watchlistService.RemoveShowAsync(userId.Value, showId, cancellationToken);
        
        return NoContent();
    }

    private int? GetUserId()
    {
        var userIdString = _userManager.GetUserId(User);
        return int.TryParse(userIdString, out var userId) ? userId : null;
    }
}
