namespace Episodes.Models.Tmdb;

public sealed class TmdbSearchTvResponse
{
    public int Page { get; set; }

    public List<TmdbTvSearchResult> Results { get; set; } = new();

    public int TotalPages { get; set; }

    public int TotalResults { get; set; }
}