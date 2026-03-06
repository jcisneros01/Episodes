using System.Data.Common;
using Episodes.Data;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Episodes.Tests.Helpers;

public sealed class HealthChecksWebApplicationFactory : EpisodesWebApplicationFactory
{
    private readonly ServiceProvider _entityFrameworkServiceProvider =
        new ServiceCollection().AddEntityFrameworkSqlite().BuildServiceProvider();

    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbConnection>();
            services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();

            services.AddSingleton<DbConnection>(_ =>
            {
                _connection = new SqliteConnection("Data Source=:memory:");
                _connection.Open();
                return _connection;
            });

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
                options.UseSqlite(sp.GetRequiredService<DbConnection>())
                    .UseInternalServiceProvider(_entityFrameworkServiceProvider));
        });
    }

    public void EnsureDatabaseCreated()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing) return;

        _connection?.Dispose();
        _entityFrameworkServiceProvider.Dispose();
    }
}
