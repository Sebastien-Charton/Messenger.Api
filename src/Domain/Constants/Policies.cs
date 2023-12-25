namespace Messenger.Api.Domain.Constants;

public abstract class Policies
{
    public const string CanPurge = nameof(CanPurge);
    public const string IsAdministrator = nameof(IsAdministrator);
    public const string IsUser = nameof(IsUser);
    public const string AllUsers = nameof(AllUsers);
}
