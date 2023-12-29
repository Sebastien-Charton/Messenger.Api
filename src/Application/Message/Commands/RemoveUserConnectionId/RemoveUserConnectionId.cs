using Messenger.Api.Application.Common.Interfaces;

namespace Messenger.Api.Application.Message.Commands.RemoveUserConnectionId;

public record RemoveUserConnectionIdCommand : IRequest
{
    public string ConnectionId { get; set; } = null!;
}

public class RemoveUserConnectionIdCommandValidator : AbstractValidator<RemoveUserConnectionIdCommand>
{
    public RemoveUserConnectionIdCommandValidator()
    {
        RuleFor(v => v.ConnectionId)
            .NotEmpty();
    }
}

public class RemoveUserConnectionIdCommandHandler : IRequestHandler<RemoveUserConnectionIdCommand>
{
    private readonly ICacheService _cacheService;

    public RemoveUserConnectionIdCommandHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(RemoveUserConnectionIdCommand request, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveAsync(request.ConnectionId);
    }
}
