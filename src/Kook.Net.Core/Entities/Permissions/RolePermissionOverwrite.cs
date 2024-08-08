namespace Kook;

/// <summary>
///     表示一个为角色设置的频道权限重写设置。
/// </summary>
public class RolePermissionOverwrite : IPermissionOverwrite<uint>
{
    /// <summary>
    ///     获取此重写所应用的角色的 ID。
    /// </summary>
    public uint Target { get; }

    /// <inheritdoc />
    public OverwritePermissions Permissions { get; }

    /// <summary>
    ///     初始化一个 <see cref="RolePermissionOverwrite"/> 类的新实例。
    /// </summary>
    /// <param name="targetId"> 角色的 ID。 </param>
    /// <param name="permissions"> 角色的权限重写配置。 </param>
    public RolePermissionOverwrite(uint targetId, OverwritePermissions permissions)
    {
        Target = targetId;
        Permissions = permissions;
    }
}
