using Messenger.Api.Application.Common.Interfaces;

namespace Messenger.Api.Application.Message.Commands.SetUserConnectionId;

public record SetUserConnectionIdCommand : IRequest
{
}

public class SetUserConnectionIdCommandValidator : AbstractValidator<SetUserConnectionIdCommand>
{
    public SetUserConnectionIdCommandValidator()
    {
    }
}

public class SetUserConnectionIdCommandHandler : IRequestHandler<SetUserConnectionIdCommand>
{
    private readonly IApplicationDbContext _context;

    public SetUserConnectionIdCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(SetUserConnectionIdCommand request, CancellationToken cancellationToken)
    {
        await Task.Delay(1);
        throw new NotImplementedException();
    }
}
