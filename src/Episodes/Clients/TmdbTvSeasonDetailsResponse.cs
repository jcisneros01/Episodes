using System.Text.Json.Serialization;

namespace Episodes.Clients;

public sealed class TmdbTvSeasonDetailsResponse
{
    [JsonPropertyName("_id")] public string Id { get; set; } = string.Empty;

    public string AirDate { get; set; } = string.Empty;

    public List<TmdbSeasonEpisode> Episodes { get; set; } = new();

    public string Name { get; set; } = string.Empty;

    public List<TmdbNetwork> Networks { get; set; } = new();

    public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("id")] public int SeasonId { get; set; }

    public string PosterPath { get; set; } = string.Empty;

    public int SeasonNumber { get; set; }

    public double VoteAverage { get; set; }
}

public sealed class TmdbSeasonEpisode
{
    public string AirDate { get; set; } = string.Empty;

    public int EpisodeNumber { get; set; }

    public string EpisodeType { get; set; } = string.Empty;

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Overview { get; set; } = string.Empty;

    public string ProductionCode { get; set; } = string.Empty;

    public int? Runtime { get; set; }

    public int SeasonNumber { get; set; }

    public int ShowId { get; set; }

    public string? StillPath { get; set; }

    public double VoteAverage { get; set; }

    public int VoteCount { get; set; }

    public List<TmdbCrewMember> Crew { get; set; } = new();

    public List<TmdbGuestStar> GuestStars { get; set; } = new();
}

public sealed class TmdbCrewMember
{
    public string Department { get; set; } = string.Empty;

    public string Job { get; set; } = string.Empty;

    public string CreditId { get; set; } = string.Empty;

    public bool Adult { get; set; }

    public int Gender { get; set; }

    public int Id { get; set; }

    public string KnownForDepartment { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string OriginalName { get; set; } = string.Empty;

    public double Popularity { get; set; }

    public string ProfilePath { get; set; } = string.Empty;
}

public sealed class TmdbGuestStar
{
    public string Character { get; set; } = string.Empty;

    public string CreditId { get; set; } = string.Empty;

    public int Order { get; set; }

    public bool Adult { get; set; }

    public int Gender { get; set; }

    public int Id { get; set; }

    public string KnownForDepartment { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string OriginalName { get; set; } = string.Empty;

    public double Popularity { get; set; }

    public string? ProfilePath { get; set; }
}