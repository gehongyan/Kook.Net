namespace Kook;

/// <summary>
///     表示一个服务器认证的类型。
/// </summary>
public enum GuildCertificationType
{
    /// <summary>
    ///     官方认证。
    /// </summary>
    Official = 1,

    /// <summary>
    ///     合作伙伴认证。
    /// </summary>
    Partner = 2,

    /// <summary>
    ///     推荐认证。
    /// </summary>
    Recommended = 4,

    /// <summary>
    ///     个人认证。
    /// </summary>
    Personal = 6

    // TODO: To be investigated
}
