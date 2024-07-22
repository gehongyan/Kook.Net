namespace Kook;

/// <summary>
///     表示一个为用户设置的频道权限重写设置。
/// </summary>
public class UserPermissionOverwrite : IPermissionOverwrite<IUser>
{
    /// <summary>
    ///     获取此重写所应用的用户。
    /// </summary>
    public IUser Target { get; }

    /// <inheritdoc />
    public OverwritePermissions Permissions { get; }

    /// <summary>
    ///     初始化一个 <see cref="UserPermissionOverwrite"/> 类的新实例。
    /// </summary>
    /// <param name="target"> 用户。 </param>
    /// <param name="permissions"> 用户的权限重写配置。 </param>
    public UserPermissionOverwrite(IUser target, OverwritePermissions permissions)
    {
        Target = target;
        Permissions = permissions;
    }
}
