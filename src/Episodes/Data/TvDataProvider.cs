namespace Episodes.Data;

public class TvDataProvider
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Episode> Episodes { get; set; } = new List<Episode>();

    public virtual ICollection<Season> Seasons { get; set; } = new List<Season>();

    public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
}