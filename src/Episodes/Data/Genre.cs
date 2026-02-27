namespace Episodes.Data;

public class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Show> Shows { get; set; } = new List<Show>();
}