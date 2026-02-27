namespace Episodes.Data;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<UserShow> UserShows { get; set; } = new List<UserShow>();

    public virtual ICollection<WatchedEpisode> WatchedEpisodes { get; set; } = new List<WatchedEpisode>();
}