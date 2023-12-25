using System.Data.Common;
using Messenger.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Web.IntegrationTests;

public class TestcontainersTestDatabase : ITestDatabase
{
    private readonly PostgreSqlContainer _container;
    private DbConnection _connection = null!;
    private string _connectionString = null!;

    public TestcontainersTestDatabase()
    {
        _container = new PostgreSqlBuilder()
            .WithAutoRemove(true)
            .Build();
    }

    public async Task InitialiseAsync()
    {
        await _container.StartAsync();

        _connectionString = _container.GetConnectionString();

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
        await _container.DisposeAsync();
    }
}
