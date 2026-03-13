using Episodes.Data;
using Episodes.Enums;
using Episodes.Models;
using Microsoft.EntityFrameworkCore;

namespace Episodes.Services;

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

    public async Task<AddShowResult> AddShowAsync(int userId, int externalShowId,
        CancellationToken cancellationToken = default)
    {
        var show = await _dbContext.Shows.FirstOrDefaultAsync(x => x.ExternalId == externalShowId,
            cancellationToken: cancellationToken);
        if (show == null)
        {
            return new AddShowResult { Error = AddShowError.ShowNotFound };
        }

        var alreadyAdded = await _dbContext.UserShows
            .AnyAsync(x => x.UserId == userId && x.ShowId == show.Id,
                cancellationToken: cancellationToken);
        if (alreadyAdded)
        {
            return new AddShowResult { Error = AddShowError.AlreadyOnWatchlist };
        }

        var newUserShow = new UserShow
        {
            UserId = userId,
            ShowId = show.Id,
            Status = UserShowStatus.Current,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _dbContext.Add(newUserShow);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AddShowResult
        {
            Item = new WatchlistItem(externalShowId, show.Name, show.PosterImgLink, newUserShow.Status, newUserShow.CreatedAt)
        };
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