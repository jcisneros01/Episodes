using Episodes.Data;
using Episodes.Models;
using Microsoft.EntityFrameworkCore;

namespace Episodes.Services;

public class WatchedEpisodesService : IWatchedEpisodesService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<WatchedEpisodesService> _logger;

    public WatchedEpisodesService(ApplicationDbContext dbContext, ILogger<WatchedEpisodesService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<MarkEpisodeWatchedResult> MarkEpisodeAsWatched(int userId, int episodeId,
        CancellationToken cancellationToken)
    {
        try
        {
            var episodeExists = await _dbContext.Episodes
                .AnyAsync(e => e.Id == episodeId, cancellationToken);
            if (!episodeExists)
            {
                return new MarkEpisodeWatchedResult { Error = MarkEpisodeWatchedError.EpisodeNotFound };
            }

            var alreadyWatched = await _dbContext.WatchedEpisodes
                .AnyAsync(w => w.UserId == userId && w.EpisodeId == episodeId, cancellationToken);
            if (alreadyWatched)
            {
                return new MarkEpisodeWatchedResult { Error = MarkEpisodeWatchedError.AlreadyWatched };
            }
            
            _dbContext.WatchedEpisodes.Add(new WatchedEpisode
            {
                UserId = userId,
                EpisodeId = episodeId,
                WatchedAt = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark episode {EpisodeId} as watched for user {UserId}", episodeId, userId);
            
            throw;
        }

        return new MarkEpisodeWatchedResult();
    }

    public async Task<UnmarkEpisodeWatchedResult> UnmarkEpisodeAsWatched(int userId, int episodeId, CancellationToken cancellationToken)
    {
        try
        {
            var watchedEpisode = await _dbContext.WatchedEpisodes.FirstOrDefaultAsync(
                x => x.UserId == userId && x.EpisodeId == episodeId, cancellationToken);
            if (watchedEpisode == null)
            {
                return new UnmarkEpisodeWatchedResult { Error = UnmarkEpisodeWatchedError.EpisodeNotWatched };
            }

            _dbContext.WatchedEpisodes.Remove(watchedEpisode);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unmark episode {EpisodeId} as watched for user {UserId}", episodeId, userId);
            
            throw;
        }

        return new UnmarkEpisodeWatchedResult();
    }
}