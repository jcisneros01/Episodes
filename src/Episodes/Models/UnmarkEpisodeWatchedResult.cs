namespace Episodes.Models;

public record UnmarkEpisodeWatchedResult
{
    public UnmarkEpisodeWatchedError? Error { get; init; }
}

public enum UnmarkEpisodeWatchedError
{
    EpisodeNotWatched
}
