namespace Kook;

/// <summary>
///     表示服务器的加入限制。
/// </summary>
[Flags]
public enum GuildJoinRestrictionTypes
{
    /// <summary>
    ///     没有加入限制。
    /// </summary>
    None = 0,

    /// <summary>
    ///     禁止未实名的用户加入服务器。
    /// </summary>
    DisableUnverified = 1 << 0,

    /// <summary>
    ///     禁止近期有严重违规记录的用户加入服务器。
    /// </summary>
    DisableViolation = 1 << 1,

    /// <summary>
    ///     禁止未实名且近期有严重违规记录的用户加入服务器。
    /// </summary>
    DisableUnverifiedAndViolation = 1 << 2,
}
