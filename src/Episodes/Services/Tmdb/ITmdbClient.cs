using Episodes.Models.Tmdb;

namespace Episodes.Services.Tmdb;

public interface ITmdbClient
{
    Task<TmdbSearchTvResponse> SearchTvShowsAsync(string query, int? page, CancellationToken token = default);
    Task<TmdbTvDetailsResponse> GetTvShowDetailsAsync(int seriesId, CancellationToken token = default);

    Task<TmdbTvSeasonDetailsResponse> GetTvShowSeasonDetailsAsync(int seriesId, int seasonNumber,
        CancellationToken token = default);
}