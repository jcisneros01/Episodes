using Episodes.Models;

namespace Episodes.Services;

public interface ITvShowService
{
    Task<TvShowSearchResponse> SearchTvShowsAsync(string query, int? page,
        CancellationToken cancellationToken = default);

    Task<TvShowResponse> GetTvShowAsync(int externalShowId, CancellationToken cancellationToken = default);

    Task<TvSeasonResponse> GetSeasonEpisodesAsync(int tvShowId, int seasonNumber,
        CancellationToken cancellationToken = default);
}