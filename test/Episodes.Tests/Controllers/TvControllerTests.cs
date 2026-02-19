using Episodes.Controllers;
using Episodes.Models.Tv;
using Episodes.Services.Tv;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Episodes.Tests.Controllers;

[TestFixture]
public class TvControllerTests
{
    private ITvShowService _tvShowService;
    private TvController _sut;

    [SetUp]
    public void SetUp()
    {
        _tvShowService = Substitute.For<ITvShowService>();
        _sut = new TvController(_tvShowService);
    }

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
}
