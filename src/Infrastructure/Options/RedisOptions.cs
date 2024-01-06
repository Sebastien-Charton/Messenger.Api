namespace Messenger.Api.Infrastructure.Options;

public class RedisOptions
{
    public required string Url { get; set; }
    public required int Port { get; set; }
    public required string ConnectionString { get; set; }
}
