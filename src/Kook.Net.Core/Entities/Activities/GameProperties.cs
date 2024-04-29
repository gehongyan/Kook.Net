namespace Kook;

/// <summary>
///     Properties that are used to modify an <see cref="IGame" /> with the specified changes.
/// </summary>
/// <seealso cref="IGame.ModifyAsync"/>
public class GameProperties
{
    /// <summary>
    ///     Gets or sets the name of the game.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    ///     Gets or sets the icon URL of the game.
    /// </summary>
    public string? IconUrl { get; set; }
}
