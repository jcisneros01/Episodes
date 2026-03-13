using Episodes.Data;

namespace Episodes.Services;

public interface IWatchedEpisodesService
{
    void MarkEpisodeAsWatched(int externalShowId, int episodeId);
}

public class WatchedEpisodesService : IWatchedEpisodesService
{
    private readonly DbContext _dbContext;

    public WatchedEpisodesService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public void MarkEpisodeAsWatched(int externalShowId, int episodeId)
    {
        var show = _dbContext.Shows
            .Include(x=> x.Season)
            .FirstOrDefault(x=> x.ExternalId == externalShowId);
        
    }
}
