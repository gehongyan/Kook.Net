namespace Kook;

/// <summary>
///     表示一个服务器特性。
/// </summary>
[Flags]
public enum GuildFeature : uint
{
    /// <summary>
    ///     无特性。
    /// </summary>
    None = 0,

    /// <summary>
    ///     服务器是官方服务器。
    /// </summary>
    Official = 1 << 0,

    /// <summary>
    ///     服务器是合作伙伴服务器。
    /// </summary>
    Partner = 1 << 1,

    /// <summary>
    ///     服务器是重点客户服务器。
    /// </summary>
    KeyAccount = 1 << 2,
}
