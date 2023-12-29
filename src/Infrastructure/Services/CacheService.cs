using Messenger.Api.Application.Common.Interfaces;
using StackExchange.Redis;

namespace Messenger.Api.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;

    public CacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }
    
    public async Task GetAsync(string key)
    {
        await _redis.GetDatabase().StringGetAsync(key);
    }
    
    public async Task SetAsync(string key, string value, TimeSpan? timeToLive = null)
    {
        await _redis.GetDatabase().StringSetAsync(key, value, timeToLive);
    }
    
    public async Task UpdateAsync(string key, string value, TimeSpan? timeToLive = null)
    {
        await _redis.GetDatabase().StringSetAsync(key, value, timeToLive);
    }
    public async Task RemoveAsync(string key)
    {
        await _redis.GetDatabase().KeyDeleteAsync(key);
    }
}
