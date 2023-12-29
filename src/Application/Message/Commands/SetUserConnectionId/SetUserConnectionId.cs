using Messenger.Api.Application.Common.Interfaces;

namespace Messenger.Api.Application.Message.Commands.SetUserConnectionId;

public record SetUserConnectionIdCommand : IRequest
{
    public string ConnectionId { get; set; } = null!;
}

public class SetUserConnectionIdCommandValidator : AbstractValidator<SetUserConnectionIdCommand>
{
    public SetUserConnectionIdCommandValidator()
    {
        RuleFor(v => v.ConnectionId)
            .NotEmpty();
    }
}

public class SetUserConnectionIdCommandHandler : IRequestHandler<SetUserConnectionIdCommand>
{
    private readonly ICacheService _cacheService;
    private readonly IUser _user;
    public SetUserConnectionIdCommandHandler(IApplicationDbContext context, ICacheService cacheService, IUser user)
    {
        _cacheService = cacheService;
        _user = user;
    }

    public async Task Handle(SetUserConnectionIdCommand request, CancellationToken cancellationToken)
    {
        await _cacheService.SetAsync(request.ConnectionId, _user.Id.ToString()!);
    }
}
