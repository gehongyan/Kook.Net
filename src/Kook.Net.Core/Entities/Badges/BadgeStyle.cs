namespace Kook;

/// <summary>
///     表示徽章的样式。
/// </summary>
public enum BadgeStyle
{
    /// <summary>
    ///     徽章展示含服务器名称。
    /// </summary>
    GuildName = 0,

    /// <summary>
    ///     徽章展示服务器在线成员数量。
    /// </summary>
    OnlineMemberCount = 1,

    /// <summary>
    ///     徽章展示服务器在线成员数量和总成员数量。
    /// </summary>
    OnlineAndTotalMemberCount = 2
}
