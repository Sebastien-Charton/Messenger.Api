namespace Messenger.Api.Application.Common.Interfaces;

public interface IUser
{
    Guid? Id { get; }
    string? Email { get; }
    string? UserName { get; }
}
