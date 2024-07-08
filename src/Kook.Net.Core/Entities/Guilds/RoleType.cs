namespace Kook;

/// <summary>
///     表示角色的类型。
/// </summary>
public enum RoleType : ushort
{
    /// <summary>
    ///     角色是由用户创建的。
    /// </summary>
    UserCreated = 0,

    /// <summary>
    ///     角色是在 Bot 加入服务器时由系统创建的。
    /// </summary>
    BotSpecified = 1,

    /// <summary>
    ///     角色表示服务器助力者。
    /// </summary>
    Booster = 2,

    /// <summary>
    ///     表示角色是默认的 <c>@everyone</c> 全体成员角色。
    /// </summary>
    Everyone = 255
}
