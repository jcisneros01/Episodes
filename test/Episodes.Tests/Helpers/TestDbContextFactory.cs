using Episodes.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Episodes.Tests.Helpers;

public static class TestDbContextFactory
{
    public static (ApplicationDbContext Context, SqliteConnection Connection) Create()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        connection.CreateFunction("now", () => DateTime.UtcNow.ToString("o"));

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        return (context, connection);
    }
}
