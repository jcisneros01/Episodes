using Episodes.Data;
using Episodes.Enums;
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
public class WatchlistServiceTests
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

        _logger = Substitute.For<ILogger<WatchlistService>>();
        _sut = new WatchlistService(_dbContext, _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }

    private SqliteConnection _connection;
    private ApplicationDbContext _dbContext;
    private ILogger<WatchlistService> _logger;
    private WatchlistService _sut;

    private Show CreateShow(int externalId, string name, string? posterImgLink = "/poster.jpg")
    {
        var show = new Show
        {
            Name = name,
            ExternalId = externalId,
            DataProviderId = 1,
            Status = "Returning Series",
            PosterImgLink = posterImgLink,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _dbContext.Shows.Add(show);
        _dbContext.SaveChanges();
        return show;
    }

    [Test]
    public async Task GetWatchlistAsync_WhenUserHasShows_ReturnsItemsOrderedByCreatedAtDescending()
    {
        // Arrange
        var show1 = CreateShow(1396, "Breaking Bad");
        var show2 = CreateShow(1399, "Game of Thrones");

        _dbContext.UserShows.AddRange(
            new UserShow
            {
                UserId = 1, ShowId = show1.Id, Status = UserShowStatus.Current,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = DateTime.UtcNow
            },
            new UserShow
            {
                UserId = 1, ShowId = show2.Id, Status = UserShowStatus.Current,
                CreatedAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = DateTime.UtcNow
            }
        );
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetWatchlistAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result[0].ShowId.Should().Be(show2.Id);
        result[0].Name.Should().Be("Game of Thrones");
        result[1].ShowId.Should().Be(show1.Id);
        result[1].Name.Should().Be("Breaking Bad");
    }

    [Test]
    public async Task GetWatchlistAsync_WhenNoItems_ReturnsEmptyList()
    {
        // Act
        var result = await _sut.GetWatchlistAsync(1);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetWatchlistAsync_ReturnsOnlyItemsForSpecifiedUser()
    {
        // Arrange
        var show = CreateShow(1396, "Breaking Bad");

        _dbContext.UserShows.AddRange(
            new UserShow
            {
                UserId = 1, ShowId = show.Id, Status = UserShowStatus.Current,
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            },
            new UserShow
            {
                UserId = 2, ShowId = show.Id, Status = UserShowStatus.Current,
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
            }
        );
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetWatchlistAsync(1);

        // Assert
        result.Should().ContainSingle();
        result[0].ShowId.Should().Be(show.Id);
    }

    [Test]
    public async Task AddShowAsync_WhenShowExists_AddsToWatchlistAndReturnsSuccess()
    {
        // Arrange
        var show = CreateShow(1396, "Breaking Bad", "/poster.jpg");

        // Act
        var result = await _sut.AddShowAsync(1, show.Id);

        // Assert
        result.Error.Should().BeNull();
        result.Item.Should().NotBeNull();
        result.Item!.ShowId.Should().Be(show.Id);
        result.Item.Name.Should().Be("Breaking Bad");
        result.Item.PosterImgLink.Should().Be("/poster.jpg");
        result.Item.Status.Should().Be(UserShowStatus.Current);

        var userShow = await _dbContext.UserShows.FirstOrDefaultAsync(x => x.UserId == 1 && x.ShowId == show.Id);
        userShow.Should().NotBeNull();
    }

    [Test]
    public async Task AddShowAsync_WhenShowNotFound_ReturnsShowNotFoundError()
    {
        // Act
        var result = await _sut.AddShowAsync(1, 9999);

        // Assert
        result.Error.Should().Be(AddShowError.ShowNotFound);
        result.Item.Should().BeNull();
    }

    [Test]
    public async Task AddShowAsync_WhenAlreadyOnWatchlist_ReturnsAlreadyOnWatchlistError()
    {
        // Arrange
        var show = CreateShow(1396, "Breaking Bad");
        _dbContext.UserShows.Add(new UserShow
        {
            UserId = 1, ShowId = show.Id, Status = UserShowStatus.Current,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.AddShowAsync(1, show.Id);

        // Assert
        result.Error.Should().Be(AddShowError.AlreadyOnWatchlist);
        result.Item.Should().BeNull();
    }

    [Test]
    public async Task RemoveShowAsync_WhenShowOnWatchlist_RemovesIt()
    {
        // Arrange
        var show = CreateShow(1396, "Breaking Bad");
        _dbContext.UserShows.Add(new UserShow
        {
            UserId = 1, ShowId = show.Id, Status = UserShowStatus.Current,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();

        // Act
        await _sut.RemoveShowAsync(1, show.Id);

        // Assert
        var userShow = await _dbContext.UserShows.FirstOrDefaultAsync(x => x.UserId == 1 && x.ShowId == show.Id);
        userShow.Should().BeNull();
    }

    [Test]
    public async Task RemoveShowAsync_WhenShowNotOnWatchlist_DoesNotThrow()
    {
        // Act & Assert
        var act = async () => await _sut.RemoveShowAsync(1, 9999);

        await act.Should().NotThrowAsync();
    }
}
