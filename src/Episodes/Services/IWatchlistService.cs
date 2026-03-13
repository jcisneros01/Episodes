using Episodes.Models;

namespace Episodes.Services;

public interface IWatchlistService
{
    Task<IReadOnlyList<WatchlistItem>> GetWatchlistAsync(int userId, CancellationToken cancellationToken = default);
    
    Task<AddShowResult> AddShowAsync(int userId, int externalShowId, CancellationToken cancellationToken = default);
    
    Task RemoveShowAsync(int userId, int externalShowId, CancellationToken cancellationToken = default);
}