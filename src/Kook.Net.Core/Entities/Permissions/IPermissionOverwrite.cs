namespace Kook;

/// <summary>
///     表示一个通用的频道权限重写设置。
/// </summary>
/// <typeparam name="TTarget"> 权限重写设置所应用的目标的实体类型。 </typeparam>
public interface IPermissionOverwrite<TTarget>
{
    /// <summary>
    ///     获取此重写所应用的目标。
    /// </summary>
    TTarget Target { get; }

    /// <summary>
    ///     获取此重写的权限重写配置。
    /// </summary>
    OverwritePermissions Permissions { get; }
}
