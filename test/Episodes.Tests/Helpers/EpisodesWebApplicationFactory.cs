using System.Security.Claims;
using System.Text.Encodings.Web;
using Episodes.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Episodes.Tests.Helpers;

public class EpisodesWebApplicationFactory : WebApplicationFactory<Program>
{
    public ITvShowService TvShowService { get; } = Substitute.For<ITvShowService>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    "Host=127.0.0.1;Port=1;Database=test;Username=test;Password=test;Timeout=1;Command Timeout=1",
                ["Tmdb:BaseUrl"] = "https://api.themoviedb.org",
                ["Tmdb:ApiToken"] = "test-token"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(TvShowService);

            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Test", _ => { });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder("Test")
                    .RequireAuthenticatedUser()
                    .Build();
            });
        });
    }

    private sealed class FakeAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new Claim(ClaimTypes.Email, "test@example.com")
            ], "Test");

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
