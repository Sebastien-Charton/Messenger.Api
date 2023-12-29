namespace Messenger.Api.Application.Common.Interfaces;

public interface ICacheService
{
    Task GetAsync(string key);
    Task SetAsync(string key, string value, TimeSpan? timeToLive = null);
    Task UpdateAsync(string key, string value, TimeSpan? timeToLive = null);
    Task RemoveAsync(string key);
}
