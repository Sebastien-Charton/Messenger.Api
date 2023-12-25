using System.Net.Http.Headers;
using Messenger.Api.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Web.IntegrationTests;

public abstract class TestingFixture : IAsyncDisposable
{
    protected static readonly string BaseUri = "http://localhost/api/";
    private readonly ITestDatabase _database = null!;
    private readonly CustomWebApplicationFactory _factory = null!;
    private readonly IServiceScopeFactory _scopeFactory = null!;
    protected readonly Guid UserId;
    protected readonly string UserPassword;

    public TestingFixture()
    {
        _database = TestDatabaseFactory.CreateAsync().GetAwaiter().GetResult();

        _factory = new CustomWebApplicationFactory(_database.GetConnection(), ConfigureMocks());

        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();

        ApplicationDbContextInitialiser initialiser =
            _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        ServiceScope = _factory.Services.CreateScope();

        UserId = _factory.DefaultUserId;
        UserPassword = _factory.DefaultUserPassword;

        // Instantiate http client and mock Auth
        HttpClient = _factory.CreateClient();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

        initialiser.SeedAsync().Wait();
    }

    public HttpClient HttpClient { get; }

    protected IServiceScope ServiceScope { get; }

    public async ValueTask DisposeAsync()
    {
        await _database.DisposeAsync();
        await _factory.DisposeAsync();
    }

    protected virtual ServiceDescriptor[] ConfigureMocks()
    {
        return Array.Empty<ServiceDescriptor>();
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        ISender mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    public async Task SendAsync(IBaseRequest request)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        ISender mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        await mediator.Send(request);
    }

    public async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }
}
