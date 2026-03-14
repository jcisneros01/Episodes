using Episodes.Clients;
using Episodes.Data;
using Episodes.Extensions;
using Episodes.Models;
using Microsoft.EntityFrameworkCore;

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

        return tmdbSearchTvResponse.ToTvShowSearchResponse();
    }

    public async Task<TvShowResponse> GetTvShowAsync(int externalShowId, CancellationToken cancellationToken)
    {
        var existingShow = await _dbContext.Shows
            .Include(show => show.Networks)
            .Include(show => show.Genres)
            .Include(show => show.Seasons)
            .FirstOrDefaultAsync(x => x.ExternalId == externalShowId, cancellationToken: cancellationToken);
        
        // return cached show
        if (existingShow != null)
        {
            return existingShow.ToTvShowResponse();
        }

        // get show from tv data provider
        var tmdbTvDetailsResponse = await _client.GetTvShowDetailsAsync(externalShowId, cancellationToken);

        // cache show from tv data provider
        var newShow = tmdbTvDetailsResponse.ToShow();
        newShow.Genres = await GetGenres(tmdbTvDetailsResponse.Genres);
        newShow.Networks = await GetNetworks(tmdbTvDetailsResponse.Networks);
        _dbContext.Add(newShow);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // concurrent request already cached this show — return from db
            _dbContext.ChangeTracker.Clear();

            var storedShow = await _dbContext.Shows
                .Include(show => show.Networks)
                .Include(show => show.Genres)
                .Include(show => show.Seasons)
                .FirstOrDefaultAsync(x => x.ExternalId == externalShowId, cancellationToken: cancellationToken);
            if (storedShow != null)
            {
                return storedShow.ToTvShowResponse();
            }

            throw;
        }

        return tmdbTvDetailsResponse.ToTvShowResponse();
    }

    private async Task<ICollection<TvNetwork>> GetNetworks(List<TmdbNetwork> networks)
    {
        var tmdbNetworkIds = networks.Select(x => x.Id);

        var existingTvNetworks = await _dbContext.TvNetworks
            .Where(x => tmdbNetworkIds.Contains(x.ExternalId))
            .ToDictionaryAsync(y => y.ExternalId);

        var newTvNetworks = networks
                .Where(x => !existingTvNetworks.ContainsKey(x.Id))
                .Select(y => y.ToTvNetwork())
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
            .Select(y => y.ToGenre())
            .ToList();
        _dbContext.Genres.AddRange(newGenres);

        return existingGenres.Values.Concat(newGenres).ToList();
    }

    public async Task<TvSeasonResponse> GetSeasonEpisodesAsync(int? userId, int externaltvShowId, int seasonNumber,
        CancellationToken cancellationToken)
    {
        var season = await _dbContext.Seasons
            .Include(x => x.Episodes)
            .Include(x => x.Show)
            .FirstOrDefaultAsync(x => x.Show.ExternalId == externaltvShowId && x.SeasonNumber == seasonNumber,
                cancellationToken: cancellationToken);
        
        // show not cached yet — cache it so we have the season to attach episodes to
        if (season == null)
        {
            await GetTvShowAsync(externaltvShowId, cancellationToken);
            season = await _dbContext.Seasons
                .Include(x => x.Episodes)
                .Include(x => x.Show)
                .FirstOrDefaultAsync(x => x.Show.ExternalId == externaltvShowId && x.SeasonNumber == seasonNumber,
                    cancellationToken: cancellationToken);
        }
        
        // return cached episodes
        if (season != null && season.Episodes.Count != 0)
        {
            var tvSeasonResponse = season.ToTvSeasonResponse();
            
            await SetWatchedStatus(userId, tvSeasonResponse.Episodes, cancellationToken);
            
            return tvSeasonResponse;
        }

        // get episodes from tv data provider
        var tvSeasonDetailsResponse =
            await _client.GetTvShowSeasonDetailsAsync(externaltvShowId, seasonNumber, cancellationToken);
        
        await CacheNewEpisodes(tvSeasonDetailsResponse, season, cancellationToken);

        return season!.ToTvSeasonResponse();
    }

    private async Task SetWatchedStatus(int? userId,
        List<EpisodeResponse> episodeResponses,
        CancellationToken cancellationToken)
    {
        var episodeIds = episodeResponses.Select(x => x.Id);
        
        var watchedEpisodes =
            await _dbContext.WatchedEpisodes
                .Where(x => episodeIds.Contains(x.EpisodeId) && x.UserId == userId)
                .ToListAsync(cancellationToken: cancellationToken);

        var watchedEpisodeIds = watchedEpisodes.Select(x => x.EpisodeId).ToHashSet();
        foreach (var episode in episodeResponses)
        {
            episode.IsWatched = watchedEpisodeIds.Contains(episode.Id);
        } 
    }

    private async Task CacheNewEpisodes(TmdbTvSeasonDetailsResponse tvSeasonDetailsResponse, Season? season,
        CancellationToken cancellationToken)
    {
        season!.Overview = tvSeasonDetailsResponse.Overview;
        season.EpisodeCount = tvSeasonDetailsResponse.Episodes.Count;

        var existingEpisodeExternalIds = season.Episodes.Select(e => e.ExternalId).ToHashSet();

        var newEpisodes = tvSeasonDetailsResponse.Episodes
            .Where(x => !existingEpisodeExternalIds.Contains(x.Id))
            .Select(x =>
            {
                var episode = x.ToEpisode();
                episode.SeasonId = season.Id;
                return episode;
            }).ToList();

        foreach (var episode in newEpisodes)
            season.Episodes.Add(episode);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}