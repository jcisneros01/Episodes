namespace Episodes.Services.Tmdb;

public class TmdbClient : ITmdbClient
{
    private readonly HttpClient _client;
    private readonly ILogger<TmdbClient> _logger;

    public TmdbClient(HttpClient client, ILogger<TmdbClient> logger)
    {
        _client = client;
        _logger = logger;
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