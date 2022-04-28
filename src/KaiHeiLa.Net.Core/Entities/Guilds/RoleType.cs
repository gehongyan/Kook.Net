namespace KaiHeiLa;

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
    ///     Represents the role represents a booster.
    /// </summary>
    Booster = 2,
    /// <summary>
    ///     Represents tht role is the default everyone role.
    /// </summary>
    Everyone = 255
}