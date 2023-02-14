namespace Kook;

/// <summary>
///     Specifies the style of badges.
/// </summary>
public enum BadgeStyle
{
    /// <summary>
    ///     The badge contains only the guild name.
    /// </summary>
    GuildName = 0,
    /// <summary>
    ///     The badge contains the number of online members in the guild.
    /// </summary>
    OnlineMemberCount = 1,
    /// <summary>
    ///     The badge contains the number of both online and all members in the guild.
    /// </summary>
    OnlineAndTotalMemberCount = 2
}
