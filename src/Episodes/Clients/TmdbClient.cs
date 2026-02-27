using Episodes.Extensions;

namespace Episodes.Clients;

public class TmdbClient : ITmdbClient
{
    private readonly HttpClient _client;
    private readonly ILogger<TmdbClient> _logger;

    public TmdbClient(HttpClient client, ILogger<TmdbClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<TmdbSearchTvResponse> SearchTvShowsAsync(string query, int? page,
        CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var requestUri = $"/3/search/tv?query={Uri.EscapeDataString(query)}" +
                         (page.HasValue ? $"&page={page.Value}" : "");

        return await GetAndDeserializeAsync<TmdbSearchTvResponse>(requestUri, token);
    }

    public async Task<TmdbTvDetailsResponse> GetTvShowDetailsAsync(int seriesId, CancellationToken token = default)
    {
        var requestUri = $"/3/tv/{seriesId}";

        return await GetAndDeserializeAsync<TmdbTvDetailsResponse>(requestUri, token);
    }

    public async Task<TmdbTvSeasonDetailsResponse> GetTvShowSeasonDetailsAsync(int seriesId, int seasonNumber,
        CancellationToken token = default)
    {
        var requestUri = $"/3/tv/{seriesId}/season/{seasonNumber}";

        return await GetAndDeserializeAsync<TmdbTvSeasonDetailsResponse>(requestUri, token);
    }

    private async Task<T> GetAndDeserializeAsync<T>(string requestUri, CancellationToken token)
    {
        _logger.LogDebug("Sending GET request to TMDb. Uri={RequestUri}", requestUri);

        using var response = await _client.GetAsync(requestUri, token);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(token);

            _logger.LogWarning(
                "TMDb API returned non-success response. StatusCode={StatusCode} Uri={RequestUri} Body={ResponseBody}",
                (int)response.StatusCode, requestUri, errorBody);

            throw new TmdbApiException(response.StatusCode);
        }

        return await response.DeserializeJsonAsync<T>(token);
    }
}
