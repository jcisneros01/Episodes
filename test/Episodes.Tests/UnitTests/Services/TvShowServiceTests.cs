using Episodes.Clients;
using Episodes.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Episodes.Tests.UnitTests.Services;

[TestFixture]
public class TvShowServiceTests
{
    [SetUp]
    public void SetUp()
    {
        _client = Substitute.For<ITmdbClient>();
        _sut = new TvShowService(_client);
    }

    private ITmdbClient _client;
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
    }

    [Test]
    public async Task GetTvShowAsync_WhenSuccessful_ReturnsTvShowResponse()
    {
        // Arrange
        _client.GetTvShowDetailsAsync(1396, Arg.Any<CancellationToken>())
            .Returns(new TmdbTvDetailsResponse
            {
                Id = 1396,
                Name = "Breaking Bad",
                PosterPath = "/ggFHVNu6YYI5L9pCfOacjizRGt.jpg",
                Overview = "Walter White's transformation",
                FirstAirDate = "2008-01-20",
                InProduction = false,
                Status = "Ended",
                NumberOfEpisodes = 62,
                NumberOfSeasons = 5,
                Networks = [new TmdbNetwork { Name = "AMC" }],
                Genres = [new TmdbGenre { Name = "Drama" }, new TmdbGenre { Name = "Crime" }],
                Seasons =
                [
                    new TmdbSeasonSummary { Id = 3572, Name = "Season 1", SeasonNumber = 1, EpisodeCount = 7 },
                    new TmdbSeasonSummary { Id = 3573, Name = "Season 2", SeasonNumber = 2, EpisodeCount = 13 }
                ]
            });

        // Act
        var result = await _sut.GetTvShowAsync(1396, CancellationToken.None);

        // Assert
        result.Id.Should().Be(1396);
        result.Name.Should().Be("Breaking Bad");
        result.PosterPath.Should().Be("/ggFHVNu6YYI5L9pCfOacjizRGt.jpg");
        result.Overview.Should().Be("Walter White's transformation");
        result.FirstAirDate.Should().Be("2008-01-20");
        result.InProduction.Should().BeFalse();
        result.Status.Should().Be("Ended");
        result.NumberOfEpisodes.Should().Be(62);
        result.NumberOfSeasons.Should().Be(5);
        result.Networks.Should().ContainSingle().Which.Should().Be("AMC");
        result.Genres.Should().BeEquivalentTo(["Drama", "Crime"]);
        result.Seasons.Should().HaveCount(2);
        result.Seasons[0].Should().BeEquivalentTo(new { Id = 3572, Name = "Season 1", SeasonNumber = 1, EpisodeCount = 7 });
        result.Seasons[1].Should().BeEquivalentTo(new { Id = 3573, Name = "Season 2", SeasonNumber = 2, EpisodeCount = 13 });

        await _client.Received(1).GetTvShowDetailsAsync(1396, Arg.Any<CancellationToken>());
    }
}
