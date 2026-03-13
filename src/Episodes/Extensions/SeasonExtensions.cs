using Episodes.Data;
using Episodes.Models;

namespace Episodes.Extensions;

public static class SeasonExtensions
{
    public static TvSeasonResponse ToTvSeasonResponse(this Season season)
    {
        return new TvSeasonResponse
        {
            Name = season.Name,
            Overview = season.Overview,
            SeasonNumber = season.SeasonNumber,
            EpisodeCount = season.EpisodeCount,
            Episodes = season.Episodes.Select(episode => new EpisodeResponse
            {
                Name = episode.Name,
                Overview = episode.Overview,
                AirDate = episode.AirDate,
                EpisodeNumber = episode.EpisodeNumber
            }).ToList()
        };
    }
}
