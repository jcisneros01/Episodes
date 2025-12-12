using Episodes.Services.Tmdb.Models;

namespace Episodes.Services.Tmdb;

public interface ITmdbClient
{
    Task<TmdbSearchTvResponse?> SearchTvShowsAsync(string query, CancellationToken token = default);
    Task<TmdbTvDetailsResponse> GetTvShowDetailsAsync(int seriesId, CancellationToken token = default);
    Task<TmdbTvSeasonDetailsResponse> GetTvShowSeasonDetails(int seriesId, int seasonNumber, CancellationToken token = default);
}