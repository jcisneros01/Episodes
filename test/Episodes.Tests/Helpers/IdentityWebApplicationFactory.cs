using System.Data.Common;
using Episodes.Data;
using Episodes.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;

namespace Episodes.Tests.Helpers;

public sealed class IdentityWebApplicationFactory : WebApplicationFactory<Program>
{
    private ITvShowService TvShowService { get; } = Substitute.For<ITvShowService>();

    private SqliteConnection? _connection;

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

            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();

            services.AddSingleton<DbConnection>(_ =>
            {
                _connection = new SqliteConnection("Data Source=:memory:");
                _connection.Open();
                return _connection;
            });

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
                options.UseSqlite(sp.GetRequiredService<DbConnection>()));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();

        return host;
    }
}
