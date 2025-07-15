namespace Kook;

/// <summary>
///     表示一个通用的频道权限重写设置。
/// </summary>
/// <typeparam name="TTarget"> 权限重写设置所应用的目标的实体类型。 </typeparam>
/// <typeparam name="TId"> 权限重写设置所应用的目标的实体的 ID 的类型。 </typeparam>
public interface IPermissionOverwrite<TTarget, TId>
    where TTarget : IEntity<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    ///     获取此重写所应用的目标的唯一标识符。
    /// </summary>
    TId TargetId { get; }

    /// <summary>
    ///     获取此重写所应用的目标。
    /// </summary>
    TTarget? Target { get; }

    /// <summary>
    ///     获取此重写所应用的目标的类型。
    /// </summary>
    PermissionOverwriteTarget TargetType { get; }

    /// <summary>
    ///     获取此重写的权限重写配置。
    /// </summary>
    OverwritePermissions Permissions { get; }
}
