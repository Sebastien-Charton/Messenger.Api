namespace Messenger.Api.Infrastructure.Options;

public class JwtOptions
{
    public required string SecurityKey { get; set; }

    public required string Issuer { get; set; }

    public required string Audience { get; set; }

    public required int ExpiryInDays { get; set; }
}
