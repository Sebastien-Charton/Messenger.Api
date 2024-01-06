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
    private readonly IUser _user;
    public RemoveUserConnectionIdCommandHandler(ICacheService cacheService, IUser user)
    {
        _cacheService = cacheService;
        _user = user;
    }

    public async Task Handle(RemoveUserConnectionIdCommand request, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveAsync(_user.Id.ToString()!);
    }
}
