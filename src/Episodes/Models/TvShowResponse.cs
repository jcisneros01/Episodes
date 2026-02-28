namespace Episodes.Models;

public class TvShowResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string PosterPath { get; set; } = string.Empty;

    public string Overview { get; set; } = string.Empty;

    public string FirstAirDate { get; set; } = string.Empty;
    
    public bool InProduction { get; set; }
    
    public List<string> Networks { get; set; } = new();
    
    public List<string> Genres { get; set; } = new();

    public string Status { get; set; } = string.Empty;
    
    public List<TVSeasonSummary> Seasons { get; set; } = new();
    
    public int NumberOfEpisodes { get; set; }
    
    public int NumberOfSeasons { get; set; }

}



public sealed class TVSeasonSummary
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int SeasonNumber { get; set; }
    
    public int EpisodeCount { get; set; }
}