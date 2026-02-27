using Episodes.Clients;
using Episodes.Models;

namespace Episodes.Services;

public class TvShowService : ITvShowService
{
    private readonly ITmdbClient _client;

    public TvShowService(ITmdbClient client)
    {
        _client = client;
    }

    public async Task<TvShowSearchResponse> SearchTvShowsAsync(string query, int? page = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

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
}
