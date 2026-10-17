using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using StarInventoryAPI.Db;

namespace StarInventoryAPI.Tests.Infrastructure;

/// <summary>
/// Hosts the real API in-memory with an in-memory SQLite database and a locally signed JWT issuer.
/// One instance (and database) is shared by all tests in a class via <see cref="IClassFixture{TFixture}"/>.
/// </summary>
public class StarInventoryApiFactory : WebApplicationFactory<Program>
{
    public static readonly StarLocation[] SeededLocations =
    [
        new() { Id = 1, Name = "Area18" },
        new() { Id = 2, Name = "Port Olisar" },
        new() { Id = 3, Name = "Lorville" },
    ];

    // Kept open for the factory's lifetime; an in-memory SQLite database lives as long as its connection
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("Jwt:Issuer", TestJwt.Issuer);
        builder.UseSetting("Jwt:ClientId", TestJwt.Audience);
        builder.UseSetting("ConnectionStrings:MariaDbConnection", "Server=unused;Database=unused;");

        builder.ConfigureTestServices(services =>
        {
            // Replace the MariaDB registration from Program.cs with SQLite
            services.RemoveAll<DbContextOptions<ItemCacheDb>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ItemCacheDb>>();
            _connection.Open();
            services.AddDbContext<ItemCacheDb>(opt => opt.UseSqlite(_connection));

            // Trust tokens signed by the test key instead of the downloaded JWKS
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                o.TokenValidationParameters.IssuerSigningKey = TestJwt.SigningKey;
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ItemCacheDb>();
        db.Database.EnsureCreated();
        db.StarLocations.AddRange(SeededLocations.Select(l => new StarLocation { Id = l.Id, Name = l.Name }));
        db.SaveChanges();

        return host;
    }

    /// <summary>Creates a client that sends a bearer token for <paramref name="username"/> with the given roles.</summary>
    public HttpClient CreateClientFor(string username, params string[] roles)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestJwt.CreateToken(username, roles));
        return client;
    }

    public async Task WithDbAsync(Func<ItemCacheDb, Task> action)
    {
        using var scope = Services.CreateScope();
        await action(scope.ServiceProvider.GetRequiredService<ItemCacheDb>());
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
