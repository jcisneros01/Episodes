namespace Episodes.Data;

public class Show
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly? PremieredDate { get; set; }

    public DateOnly? EndedDate { get; set; }

    public string Status { get; set; } = null!;

    public string? PosterImgLink { get; set; }

    public int ExternalId { get; set; }

    public int DataProviderId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual TvDataProvider DataProvider { get; set; } = null!;

    public virtual ICollection<Season> Seasons { get; set; } = new List<Season>();

    public virtual ICollection<UserShow> UserShows { get; set; } = new List<UserShow>();

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

    public virtual ICollection<TvNetwork> Networks { get; set; } = new List<TvNetwork>();
}