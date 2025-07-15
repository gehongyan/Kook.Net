namespace Kook;

/// <summary>
///     表示一个为用户设置的频道权限重写设置。
/// </summary>
public class UserPermissionOverwrite : IPermissionOverwrite<IUser, ulong>
{
    /// <summary>
    ///     获取此重写所应用的用户。
    /// </summary>
    public IUser? Target { get; }

    /// <summary>
    ///     获取此重写所应用的用户的 ID。
    /// </summary>
    public ulong TargetId { get; }

    /// <inheritdoc />
    public PermissionOverwriteTarget TargetType => PermissionOverwriteTarget.User;

    /// <inheritdoc />
    public OverwritePermissions Permissions { get; }

    internal UserPermissionOverwrite(ulong targetId, OverwritePermissions permissions)
    {
        TargetId = targetId;
        Permissions = permissions;
    }

    internal UserPermissionOverwrite(IUser target, OverwritePermissions permissions)
        : this(target.Id, permissions)
    {
        Target = target;
    }

    internal UserPermissionOverwrite(ulong targetId, IUser? target, OverwritePermissions permissions)
        : this(targetId, permissions)
    {
        Target = target;
    }
}
