using Episodes.Services.Tv;

namespace Episodes.Models.Tv;

public class TvShowSearchResponse
{
    public List<TvSearchResult> Results { get; set; } = new();
    
    public int Page { get; set; }
    
    public int TotalPages { get; set; }

    public int TotalResults { get; set; }
}