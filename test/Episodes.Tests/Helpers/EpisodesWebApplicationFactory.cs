using Episodes.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
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
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test",
                ["Tmdb:BaseUrl"] = "https://api.themoviedb.org",
                ["Tmdb:ApiToken"] = "test-token"
            });
        });

        builder.ConfigureTestServices(services => { services.AddSingleton(TvShowService); });
    }
}