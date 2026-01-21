namespace Episodes.Services.Tmdb.Models;

public class TmdbTvSearchResult
{
    public bool Adult { get; set; }

    public string BackdropPath { get; set; } = string.Empty;

    public List<int> GenreIds { get; set; } = new();

    public int Id { get; set; }

    public List<string> OriginCountry { get; set; } = new();

    public string OriginalLanguage { get; set; } = string.Empty;

    public string OriginalName { get; set; } = string.Empty;

    public string Overview { get; set; } = string.Empty;

    public double Popularity { get; set; }

    public string PosterPath { get; set; } = string.Empty;

    public string FirstAirDate { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public double VoteAverage { get; set; }

    public int VoteCount { get; set; }
}