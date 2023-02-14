namespace Kook;

/// <summary>
///     Represent a permission object for a role.
/// </summary>
public class RolePermissionOverwrite : IPermissionOverwrite<uint>
{
    /// <summary>
    ///     Gets the identifier for the role this overwrite is targeting.
    /// </summary>
    public uint Target { get; }

    /// <summary>
    ///     Gets the permissions associated with this overwrite entry for a role.
    /// </summary>
    public OverwritePermissions Permissions { get; }

    /// <summary>
    ///     Initializes a new <see cref="RolePermissionOverwrite"/> with provided ID of the role and modified permissions.
    /// </summary>
    public RolePermissionOverwrite(uint targetId, OverwritePermissions permissions)
    {
        Target = targetId;
        Permissions = permissions;
    }
}
