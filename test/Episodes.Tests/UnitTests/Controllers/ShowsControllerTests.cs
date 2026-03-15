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
public class ShowsControllerTests
{
    [SetUp]
    public void SetUp()
    {
        _tvShowService = Substitute.For<ITvShowService>();
        var userManager = Substitute.For<UserManager<ApplicationUser>>(
            Substitute.For<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        userManager.GetUserId(Arg.Any<ClaimsPrincipal>()).Returns("1");
        _sut = new ShowsController(_tvShowService, userManager);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, "1")], "Test"))
            }
        };
    }

    private ITvShowService _tvShowService;
    private ShowsController _sut;

    [Test]
    public async Task SearchTvShows_WhenSuccessful_ReturnsOk()
    {
        // Arrange
        var expected = new TvShowSearchResponse
        {
            Page = 1,
            TotalPages = 2,
            TotalResults = 3,
            Results = [new TvSearchResult { Id = 1, Name = "Breaking Bad" }]
        };
        _tvShowService.SearchTvShowsAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        var result = await _sut.SearchTvShows("breaking bad", 1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().Be(expected);
        await _tvShowService.Received(1).SearchTvShowsAsync("breaking bad", 1, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetTvShow_WhenSuccessful_ReturnsOk()
    {
        // Arrange
        var expected = new TvShowResponse
        {
            Id = 1396,
            Name = "Breaking Bad",
            PosterPath = "/ggFHVNu6YYI5L9pCfOacjizRGt.jpg",
            Overview = "Walter White's transformation",
            Status = "Ended"
        };
        _tvShowService.GetTvShowAsync(1396, Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        var result = await _sut.GetTvShow(1396);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().Be(expected);
        await _tvShowService.Received(1).GetTvShowAsync(1396, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetTvShow_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _tvShowService.GetTvShowAsync(9999, Arg.Any<CancellationToken>())
            .Returns((TvShowResponse?)null);

        // Act
        var result = await _sut.GetTvShow(9999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task GetTvShowByExternalId_WhenSuccessful_ReturnsOk()
    {
        // Arrange
        var expected = new TvShowResponse
        {
            Id = 1,
            Name = "Breaking Bad",
            PosterPath = "/ggFHVNu6YYI5L9pCfOacjizRGt.jpg",
            Overview = "Walter White's transformation",
            Status = "Ended"
        };
        _tvShowService.GetTvShowByExternalIdAsync(1396, Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        var result = await _sut.GetTvShowByExternalId(1396);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().Be(expected);
        await _tvShowService.Received(1).GetTvShowByExternalIdAsync(1396, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetSeasonEpisodesAsync_WhenSuccessful_ReturnsOk()
    {
        // Arrange
        var expected = new TvSeasonResponse
        {
            Name = "Season 1",
            Overview = "The first season.",
            SeasonNumber = 1,
            Episodes = [new EpisodeResponse { Name = "Pilot", EpisodeNumber = 1 }]
        };
        _tvShowService.GetSeasonEpisodesAsync(1, 1396, 1, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        var result = await _sut.GetSeasonEpisodes(1396, 1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().Be(expected);
        await _tvShowService.Received(1).GetSeasonEpisodesAsync(1, 1396, 1, cancellationToken: Arg.Any<CancellationToken>());
    }
}