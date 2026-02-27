namespace Episodes.Models.Entities;

public class Episode
{
    public int Id { get; set; }

    public int SeasonId { get; set; }

    public int EpisodeNumber { get; set; }

    public string Name { get; set; } = null!;

    public string? Overview { get; set; }

    public DateOnly? AirDate { get; set; }

    public int ExternalId { get; set; }

    public int DataProviderId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual TvDataProvider DataProvider { get; set; } = null!;

    public virtual Season Season { get; set; } = null!;

    public virtual ICollection<WatchedEpisode> WatchedEpisodes { get; set; } = new List<WatchedEpisode>();
}