namespace Kook;

/// <summary>
///     表示服务器行为限制的条件类型。
/// </summary>
public enum GuildRestrictionConditionType
{
    /// <summary>
    ///     注册不满指定的时长。
    /// </summary>
    RegistrationDuration,

    /// <summary>
    ///     非大陆用户。
    /// </summary>
    Overseas,

    /// <summary>
    ///     近期有严重违规记录。
    /// </summary>
    Violation,

    /// <summary>
    ///     非实名用户。
    /// </summary>
    Unverified,
}