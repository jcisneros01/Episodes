using Episodes.Clients;
using Episodes.Data;
using Episodes.Models;

namespace Episodes.Extensions;

public static class TmdbExtensions
{
    public static TvShowSearchResponse ToTvShowSearchResponse(this TmdbSearchTvResponse tmdb)
    {
        return new TvShowSearchResponse
        {
            Results = tmdb.Results.Select(x => new TvSearchResult
            {
                Id = x.Id,
                Name = x.Name,
                PosterPath = x.PosterPath,
                Overview = x.Overview
            }).ToList(),
            Page = tmdb.Page,
            TotalPages = tmdb.TotalPages,
            TotalResults = tmdb.TotalResults
        };
    }

    public static TvShowResponse ToTvShowResponse(this TmdbTvDetailsResponse tmdb)
    {
        return new TvShowResponse
        {
            Id = tmdb.Id,
            Name = tmdb.Name,
            PosterPath = tmdb.PosterPath,
            Overview = tmdb.Overview,
            FirstAirDate = DateOnly.TryParse(tmdb.FirstAirDate, out var date) ? date : null,
            InProduction = tmdb.InProduction,
            Networks = tmdb.Networks.Select(x => x.Name).ToList(),
            Genres = tmdb.Genres.Select(x => x.Name).ToList(),
            Status = tmdb.Status,
            NumberOfSeasons = tmdb.NumberOfSeasons,
            NumberOfEpisodes = tmdb.NumberOfEpisodes,
            Seasons = tmdb.Seasons.Select(x => new TvSeasonSummary
            {
                Id = x.Id,
                Name = x.Name,
                SeasonNumber = x.SeasonNumber,
                EpisodeCount = x.EpisodeCount
            }).ToList()
        };
    }

    public static TvSeasonResponse ToTvSeasonResponse(this TmdbTvSeasonDetailsResponse tmdb)
    {
        return new TvSeasonResponse
        {
            Name = tmdb.Name,
            Overview = tmdb.Overview,
            SeasonNumber = tmdb.SeasonNumber,
            EpisodeCount = tmdb.Episodes.Count,
            Episodes = tmdb.Episodes.Select(x => new EpisodeResponse
            {
                Name = x.Name,
                Overview = x.Overview,
                AirDate = DateOnly.TryParse(x.AirDate, out var airDate) ? airDate : null,
                EpisodeNumber = x.EpisodeNumber
            }).ToList()
        };
    }

    public static Show ToShow(this TmdbTvDetailsResponse tmdb)
    {
        return new Show
        {
            Name = tmdb.Name,
            PremieredDate = DateOnly.TryParse(tmdb.FirstAirDate, out var firstAirDate) ? firstAirDate : null,
            EndedDate = DateOnly.TryParse(tmdb.LastAirDate, out var lastAirDate) ? lastAirDate : null,
            Status = tmdb.Status,
            Overview = tmdb.Overview,
            PosterImgLink = tmdb.PosterPath,
            InProduction = tmdb.InProduction,
            NumberOfSeasons = tmdb.NumberOfSeasons,
            NumberOfEpisodes = tmdb.NumberOfEpisodes,
            ExternalId = tmdb.Id,
            DataProviderId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Seasons = tmdb.Seasons.Select(s => s.ToSeason()).ToList()
        };
    }

    public static Season ToSeason(this TmdbSeasonSummary tmdb)
    {
        return new Season
        {
            SeasonNumber = tmdb.SeasonNumber,
            Name = tmdb.Name,
            Overview = tmdb.Overview,
            AirDate = DateOnly.TryParse(tmdb.AirDate, out var airDate) ? airDate : null,
            PosterImgLink = tmdb.PosterPath,
            EpisodeCount = tmdb.EpisodeCount,
            ExternalId = tmdb.Id,
            DataProviderId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
    }

    public static Episode ToEpisode(this TmdbSeasonEpisode tmdb)
    {
        return new Episode
        {
            EpisodeNumber = tmdb.EpisodeNumber,
            AirDate = DateOnly.TryParse(tmdb.AirDate, out var airDate) ? airDate : null,
            Name = tmdb.Name,
            Overview = tmdb.Overview,
            ExternalId = tmdb.Id,
            DataProviderId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
    }

    public static TvNetwork ToTvNetwork(this TmdbNetwork tmdb)
    {
        return new TvNetwork
        {
            Name = tmdb.Name,
            ExternalId = tmdb.Id,
            LogoImgLink = tmdb.LogoPath,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Genre ToGenre(this TmdbGenre tmdb)
    {
        return new Genre
        {
            Name = tmdb.Name,
            ExternalId = tmdb.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
