using Episodes.Data;
using Episodes.Models;
using Microsoft.EntityFrameworkCore;

namespace Episodes.Services;

public interface IWatchlistService
{
    Task<IReadOnlyList<WatchlistItem>> GetWatchlistAsync(int userId, CancellationToken cancellationToken = default);
    
    Task<WatchlistItem> AddShowAsync(int userId, int showId, CancellationToken cancellationToken = default);
    
    Task RemoveShowAsync(int userId, int externalShowId, CancellationToken cancellationToken = default);
}

public class WatchlistService : IWatchlistService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<WatchlistService> _logger;

    public WatchlistService(ApplicationDbContext dbContext,  ILogger<WatchlistService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IReadOnlyList<WatchlistItem>> GetWatchlistAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        var userShows = await _dbContext.UserShows
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new WatchlistItem(x.Show.ExternalId, x.Show.Name, x.Show.PosterImgLink, x.Status, x.CreatedAt))
            .ToListAsync(cancellationToken: cancellationToken);
        return userShows;
    }

    public Task<WatchlistItem> AddShowAsync(int userId, int showId, CancellationToken cancellationToken = default)
    {
        // check 
        throw new NotImplementedException();
    }

    public async Task RemoveShowAsync(int userId, int externalShowId, CancellationToken cancellationToken = default)
    {
        var show = await _dbContext.UserShows.FirstOrDefaultAsync(x => x.Show.ExternalId == externalShowId && x.UserId == userId, cancellationToken);
        if (show == null)
        {
            _logger.LogWarning("Unable to remove show {ShowId} for user {userId}. Show was not found", externalShowId, userId);
            return;
        }

        _dbContext.Remove(show);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}