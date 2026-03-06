using System.Net;
using System.Text.Json;
using Episodes.Tests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace Episodes.Tests.IntegrationTests.Controllers;

[TestFixture]
public class HealthCheckIntegrationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

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
    public async Task Health_WhenCalled_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task HealthVerify_WhenDatabaseUnavailable_ReturnsServiceUnavailable()
    {
        var response = await _client.GetAsync("/health/verify");

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);

        var content = await response.Content.ReadAsStringAsync();
        var body = JsonSerializer.Deserialize<HealthCheckResponse>(content, JsonOptions);

        body.Should().NotBeNull();
        body!.Status.Should().Be("unhealthy");
        body.Checks.Should().ContainSingle(c => c.Name == "database" && c.Status == "unhealthy");
    }

    private class HealthCheckResponse
    {
        public string Status { get; set; } = string.Empty;
        public List<HealthCheckEntry> Checks { get; set; } = [];
    }

    private class HealthCheckEntry
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
