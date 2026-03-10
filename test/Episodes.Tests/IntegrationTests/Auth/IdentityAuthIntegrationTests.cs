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
    private static readonly JsonSerializerOptions SnakeCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

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
    public async Task GetCurrentUser_WhenRegisteredAndLoggedIn_ReturnsAuthenticatedProfile()
    {
        // Arrange
        await _client.PostAsJsonAsync("/api/auth/register", new
        {
            email = "user@example.com",
            password = "password1"
        });

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login?useCookies=false", new
        {
            email = "user@example.com",
            password = "password1"
        });

        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(login!.TokenType, login.AccessToken);

        // Act
        var meResponse = await _client.GetAsync("/api/me");

        // Assert
        meResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var me = await meResponse.Content.ReadFromJsonAsync<MeResponse>(SnakeCaseOptions);
        me!.Email.Should().Be("user@example.com");
        me.EmailConfirmed.Should().BeFalse();
    }

    private sealed record LoginResponse(string TokenType, string AccessToken, string RefreshToken);

    private sealed record MeResponse(int Id, string Email, bool EmailConfirmed, DateTime CreatedAt);
}
