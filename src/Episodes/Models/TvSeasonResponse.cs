namespace Episodes.Models;

public sealed class TvSeasonResponse
{
    public string Name { get; set; } = string.Empty;

    public string Overview { get; set; } = string.Empty;
    
    public int SeasonNumber { get; set; }
    
    public List<Episode> Episodes { get; set; } = new();
}

public sealed class Episode
{
    public string Name { get; set; } = string.Empty;

    public string Overview { get; set; } = string.Empty;
    
    public string AirDate { get; set; } = string.Empty;
    
    public int EpisodeNumber { get; set; }
}