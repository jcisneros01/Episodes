using System.Net;
using System.Text;
using Episodes.Clients;
using Episodes.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Episodes.Tests.UnitTests.Clients;

[TestFixture]
public class TmdbClientTests
{
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public async Task SearchTvShowsAsync_WhenQueryIsNullOrWhitespace_ThrowsArgumentException(string? query)
    {
        // Arrange
        var sut = CreateSut(new StubHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)));

        // Act
        var act = async () => await sut.SearchTvShowsAsync(query!, null);

        // Assert
        var ex = await act.Should().ThrowAsync<ArgumentException>();
        ex.Which.ParamName.Should().Be("query");
    }

    [Test]
    public async Task SearchTvShowsAsync_WhenSuccessful_ReturnsResponse()
    {
        // Arrange
        var json = """
                   {
                     "page": 1,
                     "results": [{ "id": 1399, "name": "Game of Thrones" }],
                     "total_pages": 1,
                     "total_results": 1
                   }
                   """;
        var sut = CreateSut(new StubHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            }));

        // Act
        var result = await sut.SearchTvShowsAsync("game of thrones", null);

        // Assert
        result.Should().NotBeNull();
        result.Page.Should().Be(1);
        result.TotalPages.Should().Be(1);
        result.TotalResults.Should().Be(1);
        result.Results.Should().Contain(x => x.Id == 1399);
    }

    [Test]
    public async Task SearchTvShowsAsync_WhenHttpError_ThrowsException()
    {
        // Arrange
        var sut = CreateSut(new StubHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("""{"error":"bad request"}""")
            }));

        // Act
        var act = async () => await sut.SearchTvShowsAsync("bad", null);

        // Assert
        await act.Should().ThrowAsync<TmdbApiException>();
    }

    [Test]
    public async Task GetTvShowDetailsAsync_WhenSuccessful_ReturnsResponse()
    {
        // Arrange
        var json = """
                   {
                     "id": 1399,
                     "name": "Game of Thrones",
                     "number_of_seasons": 8
                   }
                   """;
        var sut = CreateSut(new StubHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            }));

        // Act
        var result = await sut.GetTvShowDetailsAsync(1399);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1399);
        result.Name.Should().Be("Game of Thrones");
        result.NumberOfSeasons.Should().Be(8);
    }

    [Test]
    public async Task GetTvShowDetailsAsync_WhenHttpError_ThrowsException()
    {
        // Arrange
        var sut = CreateSut(new StubHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("""{"error":"not found"}""")
            }));

        // Act
        var act = async () => await sut.GetTvShowDetailsAsync(999);

        // Assert
        await act.Should().ThrowAsync<TmdbApiException>();
    }

    [Test]
    public async Task GetTvShowSeasonDetailsAsync_WhenSuccessful_ReturnsResponse()
    {
        // Arrange
        var json = """
                   {
                     "id": 1,
                     "name": "Season 1",
                     "season_number": 1,
                     "episodes": []
                   }
                   """;
        var sut = CreateSut(new StubHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            }));

        // Act
        var result = await sut.GetTvShowSeasonDetailsAsync(1399, 1);

        // Assert
        result.Should().NotBeNull();
        result.SeasonId.Should().Be(1);
        result.Name.Should().Be("Season 1");
        result.SeasonNumber.Should().Be(1);
    }

    [Test]
    public async Task GetTvShowSeasonDetailsAsync_WhenHttpError_ThrowsException()
    {
        // Arrange
        var sut = CreateSut(new StubHttpMessageHandler(
            new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("""{"error":"not found"}""")
            }));

        // Act
        var act = async () => await sut.GetTvShowSeasonDetailsAsync(1399, 1);

        // Assert
        await act.Should().ThrowAsync<TmdbApiException>();
    }

    private static TmdbClient CreateSut(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.themoviedb.org") };
        return new TmdbClient(client, NullLogger<TmdbClient>.Instance);
    }
}