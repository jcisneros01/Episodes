using Episodes.Extensions;
using Episodes.Services.Tmdb.Models;

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
    
    public async Task<TmdbSearchTvResponse?> SearchTvShowsAsync(string query, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Query cannot be empty.", nameof(query));
        }

        try
        {
            var requestUri = $"/3/search/tv?query={Uri.EscapeDataString(query)}";    
            
            _logger.LogInformation("Calling TMDb search endpoint: {Url}", requestUri);
            var response = await _client.GetAsync(requestUri, token);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(token);
                _logger.LogError(
                    "TMDb search request returned a {StatusCode} status code for query {Query}. Body: {Body}",
                    (int)response.StatusCode,
                    query,
                    errorBody
                );
                
                response.EnsureSuccessStatusCode();
            }
            
            var content = await response.DeserializeJsonAsync<TmdbSearchTvResponse>(cancellationToken: token);
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TMDb search request failed for query: {Query}", query);
            
            throw;
        }
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