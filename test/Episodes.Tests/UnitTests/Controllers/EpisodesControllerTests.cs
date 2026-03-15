using System.Security.Claims;
using Episodes.Controllers;
using Episodes.Data;
using Episodes.Models;
using Episodes.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace Episodes.Tests.UnitTests.Controllers;

[TestFixture]
public class EpisodesControllerTests
{
    [SetUp]
    public void SetUp()
    {
        _episodesService = Substitute.For<IEpisodesService>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            Substitute.For<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        _sut = new EpisodesController(_episodesService, _userManager);
    }

    private IEpisodesService _episodesService;
    private UserManager<ApplicationUser> _userManager;
    private EpisodesController _sut;

    private void SetUserId(string userId)
    {
        var claims = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId)
        ], "Test"));

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claims }
        };

        _userManager.GetUserId(Arg.Any<ClaimsPrincipal>()).Returns(userId);
    }

    private void SetNoUserId()
    {
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        _userManager.GetUserId(Arg.Any<ClaimsPrincipal>()).Returns((string?)null);
    }

    [Test]
    public async Task MarkEpisodeWatched_WhenSuccessful_ReturnsNoContent()
    {
        // Arrange
        SetUserId("1");
        _episodesService.MarkEpisodeAsWatched(1, 42, Arg.Any<CancellationToken>())
            .Returns(new MarkEpisodeWatchedResult());

        // Act
        var result = await _sut.MarkEpisodeWatched(42, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        await _episodesService.Received(1).MarkEpisodeAsWatched(1, 42, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task MarkEpisodeWatched_WhenUserIdNotParseable_ReturnsUnauthorized()
    {
        // Arrange
        SetNoUserId();

        // Act
        var result = await _sut.MarkEpisodeWatched(42, CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
        await _episodesService.DidNotReceive()
            .MarkEpisodeAsWatched(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task MarkEpisodeWatched_WhenEpisodeNotFound_ReturnsNotFound()
    {
        // Arrange
        SetUserId("1");
        _episodesService.MarkEpisodeAsWatched(1, 42, Arg.Any<CancellationToken>())
            .Returns(new MarkEpisodeWatchedResult { Error = MarkEpisodeWatchedError.EpisodeNotFound });

        // Act
        var result = await _sut.MarkEpisodeWatched(42, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task MarkEpisodeWatched_WhenAlreadyWatched_ReturnsConflict()
    {
        // Arrange
        SetUserId("1");
        _episodesService.MarkEpisodeAsWatched(1, 42, Arg.Any<CancellationToken>())
            .Returns(new MarkEpisodeWatchedResult { Error = MarkEpisodeWatchedError.AlreadyWatched });

        // Act
        var result = await _sut.MarkEpisodeWatched(42, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ConflictResult>();
    }

    [Test]
    public async Task MarkEpisodeWatched_WhenShowNotInWatchlist_ReturnsUnprocessableEntity()
    {
        // Arrange
        SetUserId("1");
        _episodesService.MarkEpisodeAsWatched(1, 42, Arg.Any<CancellationToken>())
            .Returns(new MarkEpisodeWatchedResult { Error = MarkEpisodeWatchedError.ShowNotInWatchlist });

        // Act
        var result = await _sut.MarkEpisodeWatched(42, CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnprocessableEntityObjectResult>();
    }

    [Test]
    public async Task MarkEpisodeUnwatched_WhenSuccessful_ReturnsNoContent()
    {
        // Arrange
        SetUserId("1");
        _episodesService.UnmarkEpisodeAsWatched(1, 42, Arg.Any<CancellationToken>())
            .Returns(new UnmarkEpisodeWatchedResult());

        // Act
        var result = await _sut.MarkEpisodeUnwatched(42, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        await _episodesService.Received(1).UnmarkEpisodeAsWatched(1, 42, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task MarkEpisodeUnwatched_WhenUserIdNotParseable_ReturnsUnauthorized()
    {
        // Arrange
        SetNoUserId();

        // Act
        var result = await _sut.MarkEpisodeUnwatched(42, CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedResult>();
        await _episodesService.DidNotReceive()
            .UnmarkEpisodeAsWatched(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task MarkEpisodeUnwatched_WhenEpisodeNotWatched_ReturnsNotFound()
    {
        // Arrange
        SetUserId("1");
        _episodesService.UnmarkEpisodeAsWatched(1, 42, Arg.Any<CancellationToken>())
            .Returns(new UnmarkEpisodeWatchedResult { Error = UnmarkEpisodeWatchedError.EpisodeNotWatched });

        // Act
        var result = await _sut.MarkEpisodeUnwatched(42, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
