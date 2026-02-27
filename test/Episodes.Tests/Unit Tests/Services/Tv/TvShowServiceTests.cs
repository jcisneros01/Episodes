using Episodes.Models.Tmdb;
using Episodes.Services.Tmdb;
using Episodes.Services.Tv;
using FluentAssertions;
using Microsoft.Extensions.Logging.Testing;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Episodes.Tests.Unit_Tests.Services.Tv;

[TestFixture]
public class TvShowServiceTests
{
    [SetUp]
    public void SetUp()
    {
        _client = Substitute.For<ITmdbClient>();
        _logger = new FakeLogger<TvShowService>();
        _sut = new TvShowService(_client, _logger);
    }

    private ITmdbClient _client;
    private FakeLogger<TvShowService> _logger;
    private TvShowService _sut;

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public async Task SearchTvShowsAsync_WhenQueryIsNullOrWhitespace_ThrowsArgumentException(string? query)
    {
        // Act
        var act = async () => await _sut.SearchTvShowsAsync(query!);

        // Assert
        var ex = await act.Should().ThrowAsync<ArgumentException>();
        ex.Which.ParamName.Should().Be("query");
    }

    [Test]
    public async Task SearchTvShowsAsync_HappyPath_ReturnsTvShowSearchResponse()
    {
        // Arrange
        _client.SearchTvShowsAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .Returns(new TmdbSearchTvResponse
            {
                Page = 1,
                TotalPages = 1,
                TotalResults = 1,
                Results =
                [
                    new TmdbTvSearchResult
                    {
                        Id = 1396,
                        Name = "Breaking Bad",
                        PosterPath = "/ggFHVNu6YYI5L9pCfOacjizRGt.jpg",
                        Overview = "Walter White's transformation"
                    }
                ]
            });

        // Act
        var result = await _sut.SearchTvShowsAsync("breaking bad", 1);

        // Assert
        result.Results.Should().HaveCount(1);
        result.Page.Should().Be(1);
        result.TotalPages.Should().Be(1);
        result.TotalResults.Should().Be(1);

        var show = result.Results[0];
        show.Id.Should().Be(1396);
        show.Name.Should().Be("Breaking Bad");
        show.PosterPath.Should().Be("/ggFHVNu6YYI5L9pCfOacjizRGt.jpg");
        show.Overview.Should().Be("Walter White's transformation");

        await _client.Received(1).SearchTvShowsAsync("breaking bad", 1, Arg.Any<CancellationToken>());
        _logger.Collector.GetSnapshot().Should().BeEmpty();
    }

    [Test]
    public async Task SearchTvShowsAsync_ErrorOccurs_ThrowsException()
    {
        // Arrange
        var exception = new Exception("TMDb is unavailable");
        _client.SearchTvShowsAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(exception);

        // Act
        var act = async () => await _sut.SearchTvShowsAsync("breaking bad");

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("TMDb is unavailable");
        _logger.Collector.GetSnapshot().Should().ContainSingle(r =>
            r.Level == LogLevel.Error &&
            r.Exception == exception);
    }
}