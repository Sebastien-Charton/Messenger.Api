using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Messenger.Api.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger;
    private readonly Stopwatch _timer;

    public PerformanceBehaviour(
        ILogger<TRequest> logger)
    {
        _timer = new Stopwatch();

        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        TResponse response = await next();

        _timer.Stop();

        long elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            string requestName = typeof(TRequest).Name;
            string? userName = string.Empty;

            if (!string.IsNullOrEmpty(userName))
            {
                userName = "Performance";
            }

            _logger.LogWarning(
                "Messenger.Api Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserName} {@Request}",
                requestName, elapsedMilliseconds, userName, request);
        }

        return response;
    }
}
