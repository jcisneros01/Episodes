using System.Net;
using Episodes.Extensions;
using Episodes.Models.Tv;
using Episodes.Tests.Helpers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Episodes.Tests.Integration_Tests.Controllers;

[TestFixture]
public class TvControllerIntegrationTests
{
    private EpisodesWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

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
}
