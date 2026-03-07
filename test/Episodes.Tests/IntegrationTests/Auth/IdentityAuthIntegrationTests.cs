using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Episodes.Tests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace Episodes.Tests.IntegrationTests.Auth;

[TestFixture]
public sealed class IdentityAuthIntegrationTests
{
    private IdentityWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new IdentityWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task RegisterLoginAndGetCurrentUser_returnsBearerTokenAndAuthenticatedProfile()
    {
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "user@example.com",
            password = "password1"
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login?useCookies=false", new
        {
            email = "user@example.com",
            password = "password1"
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var loginPayload = await JsonDocument.ParseAsync(await loginResponse.Content.ReadAsStreamAsync());
        var accessToken = loginPayload.RootElement.GetProperty("accessToken").GetString();
        var refreshToken = loginPayload.RootElement.GetProperty("refreshToken").GetString();
        var tokenType = loginPayload.RootElement.GetProperty("tokenType").GetString();

        accessToken.Should().NotBeNullOrWhiteSpace();
        refreshToken.Should().NotBeNullOrWhiteSpace();
        tokenType.Should().Be("Bearer");

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);

        var meResponse = await _client.GetAsync("/api/me");

        meResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var mePayload = await JsonDocument.ParseAsync(await meResponse.Content.ReadAsStreamAsync());
        mePayload.RootElement.GetProperty("email").GetString().Should().Be("user@example.com");
        mePayload.RootElement.GetProperty("email_confirmed").GetBoolean().Should().BeFalse();
    }
}
