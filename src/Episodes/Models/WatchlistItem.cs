namespace Episodes.Models;

public sealed record WatchlistItem(
    int ShowId,
    string Name,
    string? PosterImgLink,
    string Status,
    DateTime AddedAt);
