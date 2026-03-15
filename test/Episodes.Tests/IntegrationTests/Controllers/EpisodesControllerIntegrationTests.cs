using System.Net;
using Episodes.Models;
using Episodes.Tests.Helpers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Episodes.Tests.IntegrationTests.Controllers;

[TestFixture]
public class EpisodesControllerIntegrationTests
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
    public async Task MarkEpisodeWatched_WhenSuccessful_ReturnsNoContent()
    {
        // Arrange
        _factory.EpisodesService
            .MarkEpisodeAsWatched(EpisodesWebApplicationFactory.TestUserId, 42, Arg.Any<CancellationToken>())
            .Returns(new MarkEpisodeWatchedResult());

        // Act
        var responseMessage = await _client.PostAsync("/api/episodes/42/watched", null);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await _factory.EpisodesService.Received(1)
            .MarkEpisodeAsWatched(EpisodesWebApplicationFactory.TestUserId, 42, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task MarkEpisodeWatched_WhenEpisodeNotFound_ReturnsNotFound()
    {
        // Arrange
        _factory.EpisodesService
            .MarkEpisodeAsWatched(EpisodesWebApplicationFactory.TestUserId, 42, Arg.Any<CancellationToken>())
            .Returns(new MarkEpisodeWatchedResult { Error = MarkEpisodeWatchedError.EpisodeNotFound });

        // Act
        var responseMessage = await _client.PostAsync("/api/episodes/42/watched", null);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task MarkEpisodeWatched_WhenAlreadyWatched_ReturnsConflict()
    {
        // Arrange
        _factory.EpisodesService
            .MarkEpisodeAsWatched(EpisodesWebApplicationFactory.TestUserId, 42, Arg.Any<CancellationToken>())
            .Returns(new MarkEpisodeWatchedResult { Error = MarkEpisodeWatchedError.AlreadyWatched });

        // Act
        var responseMessage = await _client.PostAsync("/api/episodes/42/watched", null);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task MarkEpisodeWatched_WhenShowNotInWatchlist_ReturnsUnprocessableEntity()
    {
        // Arrange
        _factory.EpisodesService
            .MarkEpisodeAsWatched(EpisodesWebApplicationFactory.TestUserId, 42, Arg.Any<CancellationToken>())
            .Returns(new MarkEpisodeWatchedResult { Error = MarkEpisodeWatchedError.ShowNotInWatchlist });

        // Act
        var responseMessage = await _client.PostAsync("/api/episodes/42/watched", null);

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async Task MarkEpisodeUnwatched_WhenSuccessful_ReturnsNoContent()
    {
        // Arrange
        _factory.EpisodesService
            .UnmarkEpisodeAsWatched(EpisodesWebApplicationFactory.TestUserId, 42, Arg.Any<CancellationToken>())
            .Returns(new UnmarkEpisodeWatchedResult());

        // Act
        var responseMessage = await _client.DeleteAsync("/api/episodes/42/watched");

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await _factory.EpisodesService.Received(1)
            .UnmarkEpisodeAsWatched(EpisodesWebApplicationFactory.TestUserId, 42, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task MarkEpisodeUnwatched_WhenEpisodeNotWatched_ReturnsNotFound()
    {
        // Arrange
        _factory.EpisodesService
            .UnmarkEpisodeAsWatched(EpisodesWebApplicationFactory.TestUserId, 42, Arg.Any<CancellationToken>())
            .Returns(new UnmarkEpisodeWatchedResult { Error = UnmarkEpisodeWatchedError.EpisodeNotWatched });

        // Act
        var responseMessage = await _client.DeleteAsync("/api/episodes/42/watched");

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
