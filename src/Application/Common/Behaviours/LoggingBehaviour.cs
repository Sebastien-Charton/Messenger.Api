using Messenger.Api.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Messenger.Api.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IUser _user;

    public LoggingBehaviour(ILogger<TRequest> logger, IUser user)
    {
        _logger = logger;
        _user = user;
    }

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        _logger.LogInformation("Messenger.Api Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, _user.Id, _user.UserName, request);
        return Task.CompletedTask;
    }
}
