namespace Episodes.Models.Tmdb;

public sealed class TmdbTvDetailsResponse
{
    public bool Adult { get; set; }
    public string BackdropPath { get; set; } = string.Empty;

    public List<TmdbCreatedBy> CreatedBy { get; set; } = new();
    public List<int> EpisodeRunTime { get; set; } = new();

    public string FirstAirDate { get; set; } = string.Empty;
    public List<TmdbGenre> Genres { get; set; } = new();
    public string Homepage { get; set; } = string.Empty;

    public int Id { get; set; }
    public bool InProduction { get; set; }

    public List<string> Languages { get; set; } = new();
    public string LastAirDate { get; set; } = string.Empty;

    public TmdbEpisodeSummary? LastEpisodeToAir { get; set; }
    public TmdbEpisodeSummary? NextEpisodeToAir { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<TmdbNetwork> Networks { get; set; } = new();

    public int NumberOfEpisodes { get; set; }
    public int NumberOfSeasons { get; set; }

    public List<string> OriginCountry { get; set; } = new();

    public string OriginalLanguage { get; set; } = string.Empty;
    public string OriginalName { get; set; } = string.Empty;

    public string Overview { get; set; } = string.Empty;

    public double Popularity { get; set; }
    public string PosterPath { get; set; } = string.Empty;

    public List<TmdbProductionCompany> ProductionCompanies { get; set; } = new();
    public List<TmdbProductionCountry> ProductionCountries { get; set; } = new();

    public List<TmdbSeasonSummary> Seasons { get; set; } = new();
    public List<TmdbSpokenLanguage> SpokenLanguages { get; set; } = new();

    public string Status { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public double VoteAverage { get; set; }
    public int VoteCount { get; set; }
}

public sealed class TmdbCreatedBy
{
    public int Id { get; set; }
    public string CreditId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string OriginalName { get; set; } = string.Empty;
    public int Gender { get; set; }
    public string ProfilePath { get; set; } = string.Empty;
}

public sealed class TmdbGenre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class TmdbEpisodeSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;

    public double VoteAverage { get; set; }
    public int VoteCount { get; set; }

    public string AirDate { get; set; } = string.Empty;
    public int EpisodeNumber { get; set; }
    public string EpisodeType { get; set; } = string.Empty;

    public string ProductionCode { get; set; } = string.Empty;
    public int? Runtime { get; set; }

    public int SeasonNumber { get; set; }
    public int ShowId { get; set; }

    public string StillPath { get; set; } = string.Empty;
}

public sealed class TmdbNetwork
{
    public int Id { get; set; }
    public string LogoPath { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string OriginCountry { get; set; } = string.Empty;
}

public sealed class TmdbProductionCompany
{
    public int Id { get; set; }
    public string LogoPath { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string OriginCountry { get; set; } = string.Empty;
}

public sealed class TmdbProductionCountry
{
    public string Iso3166_1 { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public sealed class TmdbSeasonSummary
{
    public string AirDate { get; set; } = string.Empty;
    public int EpisodeCount { get; set; }
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;

    public string PosterPath { get; set; } = string.Empty;
    public int SeasonNumber { get; set; }

    public double VoteAverage { get; set; }
}

public sealed class TmdbSpokenLanguage
{
    public string EnglishName { get; set; } = string.Empty;
    public string Iso639_1 { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}