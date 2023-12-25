using System.Data.Common;
using Messenger.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Web.IntegrationTests;

public class PostgreSqlTestDatabase : ITestDatabase
{
    private readonly string _connectionString = null!;
    private DbConnection _connection = null!;

    public PostgreSqlTestDatabase()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .Build();

        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString);

        _connectionString = connectionString;
    }

    public async Task InitialiseAsync()
    {
        _connection = new NpgsqlConnection(_connectionString);

        await _connection.OpenAsync();

        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        ApplicationDbContext context = new(options);

        await context.Database.MigrateAsync();
    }

    public DbConnection GetConnection()
    {
        return _connection;
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
