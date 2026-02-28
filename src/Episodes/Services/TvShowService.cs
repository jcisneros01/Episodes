using Episodes.Clients;
using Episodes.Models;

namespace Episodes.Services;

public class TvShowService : ITvShowService
{
    private readonly ITmdbClient _client;

    public TvShowService(ITmdbClient client)
    {
        _client = client;
    }

    public async Task<TvShowSearchResponse> SearchTvShowsAsync(string query, int? page = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var tmdbSearchTvResponse = await _client.SearchTvShowsAsync(query, page, cancellationToken);
        
        return new TvShowSearchResponse
        {
            Results = tmdbSearchTvResponse.Results.Select(x => new TvSearchResult
            {
                Id = x.Id,
                Name = x.Name,
                PosterPath = x.PosterPath,
                Overview = x.Overview
            }).ToList(),
            Page = tmdbSearchTvResponse.Page,
            TotalPages = tmdbSearchTvResponse.TotalPages,
            TotalResults = tmdbSearchTvResponse.TotalResults
        };
    }

    public async Task<TvShowResponse> GetTvShowAsync(int id, CancellationToken cancellationToken)
    {
        var tvDetailsResponse = await _client.GetTvShowDetailsAsync(id, cancellationToken);
        
        return new TvShowResponse
        {
            Id = tvDetailsResponse.Id,
            Name = tvDetailsResponse.Name,
            PosterPath = tvDetailsResponse.PosterPath,
            Overview = tvDetailsResponse.Overview,
            FirstAirDate = tvDetailsResponse.FirstAirDate,
            InProduction = tvDetailsResponse.InProduction,
            Networks = tvDetailsResponse.Networks.Select(x => x.Name).ToList(),
            Genres = tvDetailsResponse.Genres.Select(x => x.Name).ToList(),
            Status = tvDetailsResponse.Status,
            NumberOfSeasons = tvDetailsResponse.NumberOfSeasons,
            NumberOfEpisodes = tvDetailsResponse.NumberOfEpisodes,
            Seasons = tvDetailsResponse.Seasons.Select(x=> new TVSeasonSummary
            {
                Id = x.Id,
                Name = x.Name,
                SeasonNumber = x.SeasonNumber,
                EpisodeCount = x.EpisodeCount
            }).ToList()
        };
    }
}
