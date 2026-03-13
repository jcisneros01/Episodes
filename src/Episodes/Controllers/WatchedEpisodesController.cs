using Episodes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Episodes.Controllers;

[ApiController]
[Authorize]
[Route("api/shows/{showId:int}/episodes/{episodeId:int}")]                                                                                                                     
public class WatchedEpisodesController : ControllerBase
{
    private readonly IWatchedEpisodesService _watchedEpisodesService;

    public WatchedEpisodesController(IWatchedEpisodesService watchedEpisodesService)
    {
        _watchedEpisodesService = watchedEpisodesService;
    }
    
    [HttpPost("watched")] // POST api/shows/{showId}/episodes/{episodeId}/watched
    public async Task<IActionResult> MarkEpisodeWatched(int externalShowId, int episodeId)
    {
        _watchedEpisodesService.MarkEpisodeAsWatched(externalShowId, episodeId);
        
        return NoContent();
    }

    [HttpDelete("watched")] // DELETE api/shows/{showId}/episodes/{episodeId}/watched
    public async Task<IActionResult> MarkEpisodeUnwatched()
    {
        return NoContent();
    }

}