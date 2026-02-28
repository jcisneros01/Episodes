using System.Net;
using Episodes.Extensions;
using Episodes.Models;
using Episodes.Tests.Helpers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Episodes.Tests.IntegrationTests.Controllers;

[TestFixture]
public class ShowsControllerIntegrationTests
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
    public async Task SearchTvShows_WhenSuccessful_ReturnsOk()
    {
        // Arrange
        var serviceResponse = new TvShowSearchResponse
        {
            Page = 2,
            TotalPages = 5,
            TotalResults = 50,
            Results =
            [
                new TvSearchResult
                {
                    Id = 1399,
                    Name = "Game of Thrones",
                    PosterPath = "/poster.jpg",
                    Overview = "Dragons and politics."
                }
            ]
        };
        _factory.TvShowService
            .SearchTvShowsAsync("game of thrones", 2, Arg.Any<CancellationToken>())
            .Returns(serviceResponse);

        // Act
        var responseMessage = await _client.GetAsync("/api/shows/search?query=game+of+thrones&page=2");

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await responseMessage.DeserializeJsonAsync<TvShowSearchResponse>();
        response.Should().NotBeNull();
        response.Page.Should().Be(2);
        response.TotalPages.Should().Be(5);
        response.TotalResults.Should().Be(50);
        response.Results.Should().ContainSingle(x => x.Id == 1399 && x.Name == "Game of Thrones");

        await _factory.TvShowService.Received(1)
            .SearchTvShowsAsync("game of thrones", 2, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetTvShow_WhenSuccessful_ReturnsOk()
    {
        // Arrange
        var serviceResponse = new TvShowResponse
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
            Networks = ["AMC"],
            Genres = ["Drama", "Crime"],
            Seasons =
            [
                new TvSeasonSummary { Id = 3572, Name = "Season 1", SeasonNumber = 1, EpisodeCount = 7 }
            ]
        };
        _factory.TvShowService
            .GetTvShowAsync(1396, Arg.Any<CancellationToken>())
            .Returns(serviceResponse);

        // Act
        var responseMessage = await _client.GetAsync("/api/shows/1396");

        // Assert
        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await responseMessage.DeserializeJsonAsync<TvShowResponse>();
        response.Should().NotBeNull();
        response.Id.Should().Be(1396);
        response.Name.Should().Be("Breaking Bad");
        response.PosterPath.Should().Be("/ggFHVNu6YYI5L9pCfOacjizRGt.jpg");
        response.Overview.Should().Be("Walter White's transformation");
        response.FirstAirDate.Should().Be("2008-01-20");
        response.InProduction.Should().BeFalse();
        response.Status.Should().Be("Ended");
        response.NumberOfEpisodes.Should().Be(62);
        response.NumberOfSeasons.Should().Be(5);
        response.Networks.Should().ContainSingle().Which.Should().Be("AMC");
        response.Genres.Should().BeEquivalentTo("Drama", "Crime");
        response.Seasons.Should().ContainSingle(x =>
            x.Id == 3572 && x.Name == "Season 1" && x.SeasonNumber == 1 && x.EpisodeCount == 7);

        await _factory.TvShowService.Received(1)
            .GetTvShowAsync(1396, Arg.Any<CancellationToken>());
    }
}