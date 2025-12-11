using Episodes.Config;
using Microsoft.Extensions.Options;

namespace Episodes.Services.Tmdb;

public class TmdbClient : ITmdbClient
{
    private readonly HttpClient _client;
    private readonly TmdbOptions _options;
    private readonly ILogger<TmdbClient> _logger;

    public TmdbClient(IOptions<TmdbOptions> options, HttpClient client, ILogger<TmdbClient> logger)
    {
        _client = client;
        _logger = logger;
        _options = options.Value;
    }
    
    public Task<TmdbSearchTvResponse> SearchTvShowsAsync(string query, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<TmdbTvDetailsResponse> GetTvShowDetailsAsync(int seriesId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<TmdbTvSeasonDetailsResponse> GetTvShowSeasonDetails(int seriesId, int seasonNumber, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}