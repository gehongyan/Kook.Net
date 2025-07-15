namespace Kook;

/// <summary>
///     表示一个为角色设置的频道权限重写设置。
/// </summary>
public class RolePermissionOverwrite : IPermissionOverwrite<IRole, uint>
{
    /// <summary>
    ///     获取此重写所应用的角色的。
    /// </summary>
    public IRole? Target { get; }

    /// <summary>
    ///     获取此重写所应用的角色的 ID。
    /// </summary>
    public uint TargetId { get; }

    /// <inheritdoc />
    public PermissionOverwriteTarget TargetType => PermissionOverwriteTarget.Role;

    /// <inheritdoc />
    public OverwritePermissions Permissions { get; }

    internal RolePermissionOverwrite(uint targetId, OverwritePermissions permissions)
    {
        TargetId = targetId;
        Permissions = permissions;
    }

    internal RolePermissionOverwrite(IRole target, OverwritePermissions permissions)
        : this(target.Id, permissions)
    {
        Target = target;
    }

    internal RolePermissionOverwrite(uint targetId, IRole? target, OverwritePermissions permissions)
        : this(targetId, permissions)
    {
        Target = target;
    }
}
