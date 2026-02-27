using System.ComponentModel.DataAnnotations;
using Episodes.Services.Tv;
using Microsoft.AspNetCore.Mvc;

namespace Episodes.Controllers;

[ApiController]
[Route("api/shows")]
public class TvController : ControllerBase
{
    private readonly ITvShowService _tvShowService;
    
    public TvController(ITvShowService tvShowService)
    {
        _tvShowService = tvShowService;
    }

    
    [HttpGet("search")]
    public async Task<IActionResult> SearchTvShows(
        [Required]
        [FromQuery] string query, 
        [FromQuery] int? page,
        CancellationToken cancellationToken = default)
    {
        var result = await _tvShowService.SearchTvShowsAsync(query, page, cancellationToken);
        return Ok(result);
    }
}