namespace Kook;

/// <summary>
///     表示标签的类型。
/// </summary>
public enum TagType
{
    /// <summary>
    ///     用户提及标签。
    /// </summary>
    UserMention,

    /// <summary>
    ///     频道提及标签。
    /// </summary>
    ChannelMention,

    /// <summary>
    ///     角色提及标签。
    /// </summary>
    RoleMention,

    /// <summary>
    ///     全体成员提及标签。
    /// </summary>
    EveryoneMention,

    /// <summary>
    ///     在线成员提及标签。
    /// </summary>
    HereMention,

    /// <summary>
    ///     表情符号标签。
    /// </summary>
    Emoji
}
