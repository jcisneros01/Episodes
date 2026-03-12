namespace Episodes.Models;

public record AddShowResult
{
    public bool Success { get; init; }
    public WatchlistItem? Item { get; init; }
    public AddShowError? Error { get; init; }
}

public enum AddShowError
{
    ShowNotFound,
    AlreadyOnWatchlist
}