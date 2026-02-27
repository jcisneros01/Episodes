namespace Episodes.Data;

public class WatchedEpisode
{
    public int UserId { get; set; }

    public int EpisodeId { get; set; }

    public DateTime WatchedAt { get; set; }

    public virtual Episode Episode { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}