using Episodes.Extensions;
using Episodes.Models.Tmdb;

namespace Episodes.Services.Tmdb;

public class TmdbClient : ITmdbClient
{
    private readonly HttpClient _client;

    public TmdbClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<TmdbSearchTvResponse> SearchTvShowsAsync(string query, int? page,
        CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        
        var requestUri = $"/3/search/tv?query={Uri.EscapeDataString(query)}" + (page.HasValue ? $"&page={page.Value}" : "");
        
        return await GetAndDeserializeAsync<TmdbSearchTvResponse>(requestUri, token);
    }

    public async Task<TmdbTvDetailsResponse> GetTvShowDetailsAsync(int seriesId, CancellationToken token = default)
    {
        var requestUri = $"/3/tv/{seriesId}";    
        
        return await GetAndDeserializeAsync<TmdbTvDetailsResponse>(requestUri, token);
    }

    public async Task<TmdbTvSeasonDetailsResponse> GetTvShowSeasonDetailsAsync(int seriesId, int seasonNumber, CancellationToken token = default)
    {
        var requestUri = $"/3/tv/{seriesId}/season/{seasonNumber}";    
        
        return await GetAndDeserializeAsync<TmdbTvSeasonDetailsResponse>(requestUri, token);
    }
    
    private async Task<T> GetAndDeserializeAsync<T>(string requestUri, CancellationToken token)
    {
        using var response = await _client.GetAsync(requestUri, token);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(token);

            throw new TmdbApiException((int)response.StatusCode, requestUri, errorBody);
        }
        
        return await response.DeserializeJsonAsync<T>(cancellationToken: token);
    }
}