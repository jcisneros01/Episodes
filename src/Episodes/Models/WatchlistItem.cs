using Episodes.Enums;

namespace Episodes.Models;

public sealed record WatchlistItem(
    int ShowId,
    string Name,
    string? PosterImgLink,
    UserShowStatus Status,
    DateTime AddedAt);
