namespace Episodes.Models;

public record MarkEpisodeWatchedResult
{
    public MarkEpisodeWatchedError? Error { get; init; }
}

public enum MarkEpisodeWatchedError
{
    EpisodeNotFound,
    AlreadyWatched
}
