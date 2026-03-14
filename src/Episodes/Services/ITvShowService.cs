using Episodes.Models;

namespace Episodes.Services;

public interface ITvShowService
{
    Task<TvShowSearchResponse> SearchTvShowsAsync(string query, int? page,
        CancellationToken cancellationToken = default);

    Task<TvShowResponse?> GetTvShowAsync(int showId, CancellationToken cancellationToken = default);

    Task<TvShowResponse> GetTvShowByExternalIdAsync(int externalShowId, CancellationToken cancellationToken = default);

    Task<TvSeasonResponse> GetSeasonEpisodesAsync(int? userId, int showId, int seasonNumber,
        CancellationToken cancellationToken = default);
}