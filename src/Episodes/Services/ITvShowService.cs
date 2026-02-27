using Episodes.Models;

namespace Episodes.Services;

public interface ITvShowService
{
    Task<TvShowSearchResponse> SearchTvShowsAsync(string query, int? page,
        CancellationToken cancellationToken = default);
}