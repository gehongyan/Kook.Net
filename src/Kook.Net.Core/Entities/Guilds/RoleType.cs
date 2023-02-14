namespace Kook;

/// <summary>
///     Represents the type of a role.
/// </summary>
public enum RoleType : ushort
{
    /// <summary>
    ///     Represents the role was created by a user.
    /// </summary>
    UserCreated = 0,
    /// <summary>
    ///     Represents the role was created by system when a bot joined the guild.
    /// </summary>
    BotSpecified = 1,
    /// <summary>
    ///     Represents the role representing a booster.
    /// </summary>
    Booster = 2,
    /// <summary>
    ///     Represents tht role is the default everyone role.
    /// </summary>
    Everyone = 255
}
