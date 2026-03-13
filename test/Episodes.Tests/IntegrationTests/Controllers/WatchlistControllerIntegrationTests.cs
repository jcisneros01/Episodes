using System.Net;
using Episodes.Enums;
using Episodes.Extensions;
using Episodes.Models;
using Episodes.Tests.Helpers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Episodes.Tests.IntegrationTests.Controllers;

[TestFixture]
public class WatchlistControllerIntegrationTests
{
    [SetUp]
    public void SetUp()
    {
        _factory = new EpisodesWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private EpisodesWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [Test]
    public async Task GetWatchlist_WhenSuccessful_ReturnsOk()
    {
        // Arrange
        var serviceResponse = new List<WatchlistItem>
        {
            new(1396, "Breaking Bad", "/poster.jpg", UserShowStatus.Current, new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc)),
            new(1399, "Game of Thrones", "/got.jpg", UserShowStatus.Current,
                new DateTime(2024, 2, 20, 0, 0, 0, DateTimeKind.Utc))
        };
        _factory.WatchlistService
            .GetWatchlistAsync(EpisodesWebApplicationFactory.TestUserId, Arg.Any<CancellationToken>())
            .Returns(serviceResponse);

        // Act
        var responseMessage = await _client.GetAsync("/api/watchlist");

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await responseMessage.DeserializeJsonAsync<List<WatchlistItem>>();
        response.Should().HaveCount(2);
        response[0].ShowId.Should().Be(1396);
        response[0].Name.Should().Be("Breaking Bad");
        response[1].ShowId.Should().Be(1399);
        response[1].Name.Should().Be("Game of Thrones");

        await _factory.WatchlistService.Received(1)
            .GetWatchlistAsync(EpisodesWebApplicationFactory.TestUserId, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task AddShow_WhenSuccessful_ReturnsOk()
    {
        // Arrange
        var watchlistItem =
            new WatchlistItem(1396, "Breaking Bad", "/poster.jpg", UserShowStatus.Current, DateTime.UtcNow);
        _factory.WatchlistService
            .AddShowAsync(EpisodesWebApplicationFactory.TestUserId, 1396, Arg.Any<CancellationToken>())
            .Returns(new AddShowResult { Item = watchlistItem });

        // Act
        var responseMessage = await _client.PostAsync("/api/watchlist/1396", null);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await responseMessage.DeserializeJsonAsync<WatchlistItem>();
        response.Should().NotBeNull();
        response.ShowId.Should().Be(1396);
        response.Name.Should().Be("Breaking Bad");

        await _factory.WatchlistService.Received(1)
            .AddShowAsync(EpisodesWebApplicationFactory.TestUserId, 1396, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task AddShow_WhenShowNotFound_ReturnsNotFound()
    {
        // Arrange
        _factory.WatchlistService
            .AddShowAsync(EpisodesWebApplicationFactory.TestUserId, 9999, Arg.Any<CancellationToken>())
            .Returns(new AddShowResult { Error = AddShowError.ShowNotFound });

        // Act
        var responseMessage = await _client.PostAsync("/api/watchlist/9999", null);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task AddShow_WhenAlreadyOnWatchlist_ReturnsConflict()
    {
        // Arrange
        _factory.WatchlistService
            .AddShowAsync(EpisodesWebApplicationFactory.TestUserId, 1396, Arg.Any<CancellationToken>())
            .Returns(new AddShowResult { Error = AddShowError.AlreadyOnWatchlist });

        // Act
        var responseMessage = await _client.PostAsync("/api/watchlist/1396", null);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task RemoveShow_WhenSuccessful_ReturnsNoContent()
    {
        // Act
        var responseMessage = await _client.DeleteAsync("/api/watchlist/1396");

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await _factory.WatchlistService.Received(1)
            .RemoveShowAsync(EpisodesWebApplicationFactory.TestUserId, 1396, Arg.Any<CancellationToken>());
    }
}
