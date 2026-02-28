using Episodes.Models;

namespace Episodes.Services;

public interface ITvShowService
{
    Task<TvShowSearchResponse> SearchTvShowsAsync(string query, int? page,
        CancellationToken cancellationToken = default);

    Task<TvShowResponse> GetTvShowAsync(int id, CancellationToken cancellationToken = default);

    Task<TvSeasonResponse> GetSeasonEpisodesAsync(int tvShowId, int seasonNumber,
        CancellationToken cancellationToken = default);
}