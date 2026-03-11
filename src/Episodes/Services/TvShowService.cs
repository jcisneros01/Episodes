using Episodes.Clients;
using Episodes.Data;
using Episodes.Models;
using Microsoft.EntityFrameworkCore;
using Episode = Episodes.Models.Episode;

namespace Episodes.Services;

public class TvShowService : ITvShowService
{
    private readonly ITmdbClient _client;
    private readonly ApplicationDbContext _dbContext;

    public TvShowService(ITmdbClient client, ApplicationDbContext dbContext)
    {
        _client = client;
        _dbContext = dbContext;
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

    public async Task<TvShowResponse> GetTvShowAsync(int externalShowId, CancellationToken cancellationToken)
    {
        var storedShow = await _dbContext.Shows.Include(show => show.Networks).Include(show => show.Genres)
            .Include(show => show.Seasons).FirstOrDefaultAsync(x => x.ExternalId == externalShowId,
                cancellationToken: cancellationToken);
        if (storedShow != null)
        {
            return ShowToTvShowResponse(storedShow);
        }

        var tmdbTvDetailsResponse = await _client.GetTvShowDetailsAsync(externalShowId, cancellationToken); 
        
        var unStoredShow = await TmdbResponseToShow(tmdbTvDetailsResponse);
        _dbContext.Add(unStoredShow);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return TmdbTvDetailsResponseToTvShowResponse(tmdbTvDetailsResponse);
    }

    private static TvShowResponse TmdbTvDetailsResponseToTvShowResponse(TmdbTvDetailsResponse tmdbTvDetailsResponse)
    {
        return new TvShowResponse
        {
            Id = tmdbTvDetailsResponse.Id,
            Name = tmdbTvDetailsResponse.Name,
            PosterPath = tmdbTvDetailsResponse.PosterPath,
            Overview = tmdbTvDetailsResponse.Overview,
            FirstAirDate = DateOnly.TryParse(tmdbTvDetailsResponse.FirstAirDate, out var date) ? date : null,
            InProduction = tmdbTvDetailsResponse.InProduction,
            Networks = tmdbTvDetailsResponse.Networks.Select(x => x.Name).ToList(),
            Genres = tmdbTvDetailsResponse.Genres.Select(x => x.Name).ToList(),
            Status = tmdbTvDetailsResponse.Status,
            NumberOfSeasons = tmdbTvDetailsResponse.NumberOfSeasons,
            NumberOfEpisodes = tmdbTvDetailsResponse.NumberOfEpisodes,
            Seasons = tmdbTvDetailsResponse.Seasons.Select(x => new TvSeasonSummary
            {
                Id = x.Id,
                Name = x.Name,
                SeasonNumber = x.SeasonNumber,
                EpisodeCount = x.EpisodeCount
            }).ToList()
        };
    }

    private static TvShowResponse ShowToTvShowResponse(Show show)
    {
        return new TvShowResponse
        {
            Id = show.Id,
            Name = show.Name,
            PosterPath = show.PosterImgLink,
            Overview = show.Overview,
            FirstAirDate = show.PremieredDate,
            InProduction = show.InProduction,
            Networks = show.Networks.Select(x => x.Name).ToList(),
            Genres = show.Genres.Select(x => x.Name).ToList(),
            Status = show.Status,
            NumberOfSeasons = show.NumberOfSeasons,
            NumberOfEpisodes = show.NumberOfEpisodes,
            Seasons = show.Seasons.Select(x => new TvSeasonSummary
            {
                Id = x.Id,
                Name = x.Name,
                SeasonNumber = x.SeasonNumber,
                EpisodeCount = x.EpisodeCount
            }).ToList()
        };
    }

    private async Task<Show> TmdbResponseToShow(TmdbTvDetailsResponse tmdbTvDetailsResponse)
    {
        var show = new Show
        {
            Name = tmdbTvDetailsResponse.Name,
            PremieredDate = DateOnly.TryParse(tmdbTvDetailsResponse.FirstAirDate, out var firstAirDate) ? firstAirDate : null,
            EndedDate = DateOnly.TryParse(tmdbTvDetailsResponse.LastAirDate, out var lastAirDate) ? lastAirDate : null,
            Status = tmdbTvDetailsResponse.Status,
            Overview = tmdbTvDetailsResponse.Overview,
            PosterImgLink = tmdbTvDetailsResponse.PosterPath,
            InProduction = tmdbTvDetailsResponse.InProduction,
            NumberOfSeasons = tmdbTvDetailsResponse.NumberOfSeasons,
            NumberOfEpisodes = tmdbTvDetailsResponse.NumberOfEpisodes,
            ExternalId = tmdbTvDetailsResponse.Id,
            DataProviderId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Seasons = tmdbTvDetailsResponse.Seasons.Select(s => new Season
            {
                SeasonNumber = s.SeasonNumber,
                Name = s.Name,
                Overview = s.Overview,
                AirDate = DateOnly.TryParse(s.AirDate, out var airDate) ? airDate : null,
                PosterImgLink = s.PosterPath,
                EpisodeCount = s.EpisodeCount,
                ExternalId = s.Id,
                DataProviderId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            }).ToList(),
            Genres = await GetGenres(tmdbTvDetailsResponse.Genres),
            Networks = await GetNetworks(tmdbTvDetailsResponse.Networks)
        };

        return show;
    }

    private async Task<ICollection<TvNetwork>> GetNetworks(List<TmdbNetwork> networks)
    {
        var tmdbNetworkIds = networks.Select(x => x.Id);

        var existingTvNetworks = await _dbContext.TvNetworks
            .Where(x => tmdbNetworkIds.Contains(x.ExternalId))
            .ToDictionaryAsync(y => y.ExternalId);

        var newTvNetworks = networks
            .Where(x => !existingTvNetworks.ContainsKey(x.Id))
            .Select(y => new TvNetwork
            {
                Name = y.Name,
                ExternalId = y.Id,
                LogoImgLink = y.LogoPath,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            })
            .ToList()
            ;
        _dbContext.AddRange(newTvNetworks);

        return existingTvNetworks.Values.Concat(newTvNetworks).ToList();
    }

    private async Task<ICollection<Genre>> GetGenres(List<TmdbGenre> tmdbGenres)
    {
        var tmdbGenreIds = tmdbGenres.Select(x => x.Id);

        var existingGenres = await _dbContext.Genres
            .Where(x => tmdbGenreIds.Contains(x.ExternalId))
            .ToDictionaryAsync(y => y.ExternalId);

        var newGenres = tmdbGenres
            .Where(x => !existingGenres.ContainsKey(x.Id))
            .Select(y => new Genre
            {
                Name = y.Name,
                ExternalId = y.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            })
            .ToList();
        _dbContext.Genres.AddRange(newGenres);

        return existingGenres.Values.Concat(newGenres).ToList();
    }

    public async Task<TvSeasonResponse> GetSeasonEpisodesAsync(int tvShowId, int seasonNumber,
        CancellationToken cancellationToken)
    {
        var tvSeasonDetailsResponse =
            await _client.GetTvShowSeasonDetailsAsync(tvShowId, seasonNumber, cancellationToken);
        return new TvSeasonResponse
        {
            Name = tvSeasonDetailsResponse.Name,
            Overview = tvSeasonDetailsResponse.Overview,
            SeasonNumber = tvSeasonDetailsResponse.SeasonNumber,
            Episodes = tvSeasonDetailsResponse.Episodes.Select(x => new Episode
            {
                Name = x.Name,
                Overview = x.Overview,
                AirDate = x.AirDate,
                EpisodeNumber = x.EpisodeNumber
            }).ToList()
        };
    }
}