using System.Text;
using Messenger.Api.Application.Common.Interfaces;
using Messenger.Api.Domain.Constants;
using Messenger.Api.Infrastructure.Data;
using Messenger.Api.Infrastructure.Data.Interceptors;
using Messenger.Api.Infrastructure.Options;
using Messenger.Api.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace Messenger.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");
        
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();
        
        // Inject options
        
        services
            .AddOptionsWithValidateOnStart<JwtOptions>()
            .Configure(jwtOptions => configuration.Bind(nameof(JwtOptions), jwtOptions))
            .Validate(x => x.Audience is not null, $"{nameof(JwtOptions.Audience)} is null")
            .Validate(x => x.Issuer is not null, $"{nameof(JwtOptions.Issuer)} is null")
            .Validate(x => x.SecurityKey is not null, $"{nameof(JwtOptions.SecurityKey)} is null")
            .Validate(x => x.ExpiryInDays > 0, $"{nameof(JwtOptions.ExpiryInDays)} is less than 1")
            .ValidateOnStart();
        
        services
            .AddOptionsWithValidateOnStart<RedisOptions>()
            .Configure(redisOptions => configuration.Bind(nameof(RedisOptions), redisOptions))
            .Validate(x => x.Url is not null, $"{nameof(RedisOptions.Url)} is null")
            .Validate(x => x.ConnectionString is not null, $"{nameof(RedisOptions.ConnectionString)} is null")
            .Validate(x => x.Port > 0, $"{nameof(RedisOptions.Port)} is null")
            .ValidateOnStart();
        
        // Build service provider, all options should be injected and validated by now
        var serviceProvider = services.BuildServiceProvider();
        
        var redisOptions = serviceProvider.GetRequiredService<IOptions<RedisOptions>>();
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>();

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(
            new ConfigurationOptions()
            {
                EndPoints = {redisOptions.Value.ConnectionString}
            }));

        services.AddScoped<ICacheService, CacheService>();

        // Authentication

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Value.SecurityKey)),
                    ValidIssuer = jwtOptions.Value.Issuer,
                    ValidAudience = jwtOptions.Value.Audience
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/test-chat")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorizationBuilder();

        // Authorization
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.IsUser, new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(Roles.User)
                .Build());

            options.AddPolicy(Policies.AllUsers, new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(Roles.User, Roles.Administrator)
                .Build());

            options.AddPolicy(Policies.IsAdministrator, new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(Roles.Administrator)
                .Build());

            options
                .AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator));
        });
        return services;
    }
}
