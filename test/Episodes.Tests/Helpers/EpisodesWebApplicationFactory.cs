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
                ["ConnectionStrings:DefaultConnection"] =
                    "Host=127.0.0.1;Port=1;Database=test;Username=test;Password=test;Timeout=1;Command Timeout=1",
                ["Tmdb:BaseUrl"] = "https://api.themoviedb.org",
                ["Tmdb:ApiToken"] = "test-token"
            });
        });

        builder.ConfigureTestServices(services => { services.AddSingleton(TvShowService); });
    }
}
