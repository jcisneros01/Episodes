using Episodes.Models.Tv;
using Episodes.Services.Tmdb;

namespace Episodes.Services.Tv;

public class TvShowService : ITvShowService
{
    private readonly ITmdbClient _client;

    private readonly ILogger<TvShowService> _logger;

    public TvShowService(ITmdbClient client, ILogger<TvShowService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<TvShowSearchResponse> SearchTvShowsAsync(string query, int? page = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        try
        {
            var tmdbSearchTvResponse = await _client.SearchTvShowsAsync(query, page, cancellationToken);
            return new TvShowSearchResponse
            {
                Results = tmdbSearchTvResponse.Results.Select(x => new TvSearchResult
                {
                    Id = x.Id,
                    Name = x.Name,
                    PosterPath = x.PosterPath,
                    Overview = x.Overview
                }).ToList(),
                Page = tmdbSearchTvResponse.Page,
                TotalPages = tmdbSearchTvResponse.TotalPages,
                TotalResults = tmdbSearchTvResponse.TotalResults
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "TMDb search failed. Query={Query} Page={Page}", query, page);

            throw;
        }
    }
}