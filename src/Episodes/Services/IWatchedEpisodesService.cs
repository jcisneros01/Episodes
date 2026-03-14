using Episodes.Models;

namespace Episodes.Services;

public interface IWatchedEpisodesService
{
    Task<MarkEpisodeWatchedResult> MarkEpisodeAsWatched(int userId, int episodeId, CancellationToken cancellationToken);
    
    Task<UnmarkEpisodeWatchedResult> UnmarkEpisodeAsWatched(int userId, int episodeId, CancellationToken cancellationToken);
}