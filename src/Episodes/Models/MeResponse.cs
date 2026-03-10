namespace Episodes.Models;

public sealed record MeResponse(
    int Id,
    string Email,
    bool EmailConfirmed,
    DateTime CreatedAt);