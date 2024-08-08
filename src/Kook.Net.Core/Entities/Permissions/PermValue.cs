namespace Kook;

/// <summary>
///     表示一个权限的重写配置的值。
/// </summary>
public enum PermValue
{
    /// <summary>
    ///     允许此权限。
    /// </summary>
    Allow,

    /// <summary>
    ///     禁止此权限。
    /// </summary>
    Deny,

    /// <summary>
    ///     继承此权限。
    /// </summary>
    Inherit
}
