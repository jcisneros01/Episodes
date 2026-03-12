using Episodes.Data;
using Episodes.Models;

namespace Episodes.Extensions;

public static class ShowExtensions
{
    public static TvShowResponse ToTvShowResponse(this Show show)
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
}
