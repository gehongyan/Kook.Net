namespace Kook;

/// <summary>
///     Represent a permission object for a user.
/// </summary>
public class UserPermissionOverwrite : IPermissionOverwrite<IUser>
{
    /// <summary>
    ///     Gets the user this overwrite is targeting.
    /// </summary>
    public IUser Target { get; }

    /// <summary>
    ///     Gets the permissions associated with this overwrite entry for a user.
    /// </summary>
    public OverwritePermissions Permissions { get; }

    /// <summary>
    ///     Initializes a new <see cref="UserPermissionOverwrite"/> with provided user information and modified permissions.
    /// </summary>
    public UserPermissionOverwrite(IUser target, OverwritePermissions permissions)
    {
        Target = target;
        Permissions = permissions;
    }
}
