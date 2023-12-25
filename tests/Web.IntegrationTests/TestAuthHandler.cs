using System.Security.Claims;
using System.Text.Encodings.Web;
using Messenger.Api.Domain.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Web.IntegrationTests;

public class TestAuthHandlerOptions : AuthenticationSchemeOptions
{
    public Guid DefaultUserId { get; set; }
}

public class TestAuthHandler : AuthenticationHandler<TestAuthHandlerOptions>
{
    public const string UserId = "UserId";

    public const string AuthenticationScheme = "Test";
    private readonly Guid _defaultUserId;

    public TestAuthHandler(
        IOptionsMonitor<TestAuthHandlerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
        _defaultUserId = options.CurrentValue.DefaultUserId;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim> { new(ClaimTypes.Name, "Test user") };

        // Extract User ID from the request headers if it exists,
        // otherwise use the default User ID from the options.
        if (Context.Request.Headers.TryGetValue(UserId, out var userId))
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId[0]!));
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, _defaultUserId.ToString()));
        }

        claims.Add(new Claim(ClaimTypes.Role, Roles.Administrator));

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
