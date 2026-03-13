namespace Episodes.Models;

public sealed class TvSeasonResponse
{
    public string? Name { get; set; } = string.Empty;

    public string? Overview { get; set; } = string.Empty;

    public int SeasonNumber { get; set; }

    public List<EpisodeResponse> Episodes { get; set; } = new();
    
    public int EpisodeCount { get; set; }
}

public sealed class EpisodeResponse
{
    public string Name { get; set; } = string.Empty;

    public string? Overview { get; set; } = string.Empty;

    public DateOnly? AirDate { get; set; } 

    public int EpisodeNumber { get; set; }
}