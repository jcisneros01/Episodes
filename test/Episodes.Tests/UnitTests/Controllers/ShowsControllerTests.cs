using Episodes.Controllers;
using Episodes.Models;
using Episodes.Services;
using FluentAssertions;
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
        _sut = new ShowsController(_tvShowService);
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
    public async Task GetSeasonEpisodes_WhenSuccessful_ReturnsOk()
    {
        // Arrange
        var expected = new TvSeasonResponse
        {
            Name = "Season 1",
            Overview = "The first season.",
            SeasonNumber = 1,
            Episodes = [new Episode { Name = "Pilot", EpisodeNumber = 1 }]
        };
        _tvShowService.GetSeasonEpisodes(1396, 1, Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        var result = await _sut.GetSeasonEpisodes(1396, 1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().Be(expected);
        await _tvShowService.Received(1).GetSeasonEpisodes(1396, 1, Arg.Any<CancellationToken>());
    }
}