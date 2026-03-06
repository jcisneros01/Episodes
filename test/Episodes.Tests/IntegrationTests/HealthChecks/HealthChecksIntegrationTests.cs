using System.Net;
using Episodes.Extensions;
using Episodes.Tests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace Episodes.Tests.IntegrationTests.HealthChecks;

[TestFixture]
public class HealthChecksIntegrationTests
{
    [Test]
    public async Task GetHealth_WhenApplicationIsRunning_ReturnsOk()
    {
        await using var factory = new EpisodesWebApplicationFactory();
        using var client = factory.CreateClient();

        var responseMessage = await client.GetAsync("/health");

        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await responseMessage.DeserializeJsonAsync<HealthCheckResponse>();
        response.Status.Should().Be("Healthy");
        response.Checks.Should().ContainKey("self");
        response.Checks["self"].Status.Should().Be("Healthy");
    }

    [Test]
    public async Task GetHealthVerify_WhenDatabaseIsAvailable_ReturnsOk()
    {
        await using var factory = new HealthChecksWebApplicationFactory();
        using var client = factory.CreateClient();
        factory.EnsureDatabaseCreated();

        var responseMessage = await client.GetAsync("/health/verify");

        responseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await responseMessage.DeserializeJsonAsync<HealthCheckResponse>();
        response.Status.Should().Be("Healthy");
        response.Checks.Should().ContainKey("database");
        response.Checks["database"].Status.Should().Be("Healthy");
    }

    [Test]
    public async Task GetHealthVerify_WhenDatabaseIsUnavailable_ReturnsServiceUnavailable()
    {
        await using var factory = new EpisodesWebApplicationFactory();
        using var client = factory.CreateClient();

        var responseMessage = await client.GetAsync("/health/verify");

        responseMessage.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);

        var response = await responseMessage.DeserializeJsonAsync<HealthCheckResponse>();
        response.Status.Should().Be("Unhealthy");
        response.Checks.Should().ContainKey("database");
        response.Checks["database"].Status.Should().Be("Unhealthy");
    }

    private sealed record HealthCheckResponse(
        string Status,
        IReadOnlyDictionary<string, HealthCheckCheck> Checks);

    private sealed record HealthCheckCheck(
        string Status,
        string? Description);
}
