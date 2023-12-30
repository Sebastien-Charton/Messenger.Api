using Messenger.Api.Application.Common.Interfaces;

namespace Messenger.Api.Application.Message.Queries.GetUserConnectionId;

public record GetUserConnectionIdQuery : IRequest<string?>
{
    public string UserId { get; set; } = null!;
}

public class GetUserConnectionIdQueryValidator : AbstractValidator<GetUserConnectionIdQuery>
{
    public GetUserConnectionIdQueryValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty();
    }
}

public class GetUserConnectionIdQueryHandler : IRequestHandler<GetUserConnectionIdQuery, string?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cacheService;

    public GetUserConnectionIdQueryHandler(IApplicationDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<string?> Handle(GetUserConnectionIdQuery request, CancellationToken cancellationToken)
    {
        return await _cacheService.GetAsync(request.UserId);
    }
}
