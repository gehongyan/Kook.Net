namespace Kook;

/// <summary>
///     Represents the creation source of a game.
/// </summary>
public enum GameCreationSource
{
    /// <summary>
    ///     Represents that the game was created by the current user.
    /// </summary>
    SelfUser = 1,

    /// <summary>
    ///     Represents that the game was created by the system by default.
    /// </summary>
    System = 2
}
