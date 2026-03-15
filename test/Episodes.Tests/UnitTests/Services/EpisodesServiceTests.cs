using Episodes.Data;
using Episodes.Models;
using Episodes.Services;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Episodes.Tests.UnitTests.Services;

[TestFixture]
public class EpisodesServiceTests
{
    [SetUp]
    public void SetUp()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        _connection.CreateFunction("now", () => DateTime.UtcNow.ToString("o"));

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _dbContext.Database.EnsureCreated();
        _dbContext.TvDataProviders.Add(new TvDataProvider { Id = 1, Name = "tmdb" });
        _dbContext.Users.AddRange(
            new ApplicationUser { Id = 1, UserName = "user1" },
            new ApplicationUser { Id = 2, UserName = "user2" }
        );
        _dbContext.SaveChanges();

        _logger = Substitute.For<ILogger<EpisodesService>>();
        _sut = new EpisodesService(_dbContext, _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }

    private SqliteConnection _connection;
    private ApplicationDbContext _dbContext;
    private ILogger<EpisodesService> _logger;
    private EpisodesService _sut;

    private (Show show, Season season, Episode episode) SeedShowWithEpisode()
    {
        var show = new Show
        {
            Name = "Breaking Bad",
            ExternalId = 1396,
            DataProviderId = 1,
            Status = "Ended",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _dbContext.Shows.Add(show);
        _dbContext.SaveChanges();

        var season = new Season
        {
            ShowId = show.Id,
            Name = "Season 1",
            SeasonNumber = 1,
            DataProviderId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _dbContext.Seasons.Add(season);
        _dbContext.SaveChanges();

        var episode = new Episode
        {
            SeasonId = season.Id,
            Name = "Pilot",
            EpisodeNumber = 1,
            ExternalId = 100,
            DataProviderId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _dbContext.Episodes.Add(episode);
        _dbContext.SaveChanges();

        return (show, season, episode);
    }

    private void AddToWatchlist(int userId, int showId)
    {
        _dbContext.UserShows.Add(new UserShow
        {
            UserId = userId,
            ShowId = showId,
            Status = Enums.UserShowStatus.Current,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        _dbContext.SaveChanges();
    }

    [Test]
    public async Task MarkEpisodeAsWatched_WhenSuccessful_CreatesWatchedEpisode()
    {
        // Arrange
        var (show, _, episode) = SeedShowWithEpisode();
        AddToWatchlist(1, show.Id);

        // Act
        var result = await _sut.MarkEpisodeAsWatched(1, episode.Id, CancellationToken.None);

        // Assert
        result.Error.Should().BeNull();
        var watchedEpisode = await _dbContext.WatchedEpisodes
            .FirstOrDefaultAsync(x => x.UserId == 1 && x.EpisodeId == episode.Id);
        watchedEpisode.Should().NotBeNull();
    }

    [Test]
    public async Task MarkEpisodeAsWatched_WhenEpisodeNotFound_ReturnsEpisodeNotFoundError()
    {
        // Act
        var result = await _sut.MarkEpisodeAsWatched(1, 9999, CancellationToken.None);

        // Assert
        result.Error.Should().Be(MarkEpisodeWatchedError.EpisodeNotFound);
    }

    [Test]
    public async Task MarkEpisodeAsWatched_WhenShowNotInWatchlist_ReturnsShowNotInWatchlistError()
    {
        // Arrange
        var (_, _, episode) = SeedShowWithEpisode();

        // Act
        var result = await _sut.MarkEpisodeAsWatched(1, episode.Id, CancellationToken.None);

        // Assert
        result.Error.Should().Be(MarkEpisodeWatchedError.ShowNotInWatchlist);
    }

    [Test]
    public async Task MarkEpisodeAsWatched_WhenAlreadyWatched_ReturnsAlreadyWatchedError()
    {
        // Arrange
        var (show, _, episode) = SeedShowWithEpisode();
        AddToWatchlist(1, show.Id);
        _dbContext.WatchedEpisodes.Add(new WatchedEpisode
        {
            UserId = 1,
            EpisodeId = episode.Id,
            WatchedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.MarkEpisodeAsWatched(1, episode.Id, CancellationToken.None);

        // Assert
        result.Error.Should().Be(MarkEpisodeWatchedError.AlreadyWatched);
    }

    [Test]
    public async Task UnmarkEpisodeAsWatched_WhenSuccessful_RemovesWatchedEpisode()
    {
        // Arrange
        var (show, _, episode) = SeedShowWithEpisode();
        AddToWatchlist(1, show.Id);
        _dbContext.WatchedEpisodes.Add(new WatchedEpisode
        {
            UserId = 1,
            EpisodeId = episode.Id,
            WatchedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.UnmarkEpisodeAsWatched(1, episode.Id, CancellationToken.None);

        // Assert
        result.Error.Should().BeNull();
        var watchedEpisode = await _dbContext.WatchedEpisodes
            .FirstOrDefaultAsync(x => x.UserId == 1 && x.EpisodeId == episode.Id);
        watchedEpisode.Should().BeNull();
    }

    [Test]
    public async Task UnmarkEpisodeAsWatched_WhenEpisodeNotWatched_ReturnsEpisodeNotWatchedError()
    {
        // Act
        var result = await _sut.UnmarkEpisodeAsWatched(1, 9999, CancellationToken.None);

        // Assert
        result.Error.Should().Be(UnmarkEpisodeWatchedError.EpisodeNotWatched);
    }
}
