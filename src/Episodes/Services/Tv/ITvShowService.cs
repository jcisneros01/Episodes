using Episodes.Models.Tv;

namespace Episodes.Services.Tv;

public interface ITvShowService
{
    Task<TvShowSearchResponse> SearchTvShowsAsync(string query, int? page,
        CancellationToken cancellationToken = default);
}