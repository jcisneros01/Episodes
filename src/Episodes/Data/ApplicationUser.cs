using Microsoft.AspNetCore.Identity;

namespace Episodes.Data;

public sealed class ApplicationUser : IdentityUser<int>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserShow> UserShows { get; set; } = [];

    public ICollection<WatchedEpisode> WatchedEpisodes { get; set; } = [];
}
