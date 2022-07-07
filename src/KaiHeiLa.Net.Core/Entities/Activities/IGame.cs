namespace KaiHeiLa;

/// <summary>
///     Represents a generic game object.
/// </summary>
public interface IGame : IActivity, IEntity<int>, IDeletable
{
    /// <summary>
    ///     Gets the name of the game.
    /// </summary>
    /// <returns>
    ///     A string containing the name of the game.
    /// </returns>
    string Name { get; }
    /// <summary>
    ///     Gets the type of the game.
    /// </summary>
    /// <returns>
    ///     The type of the game.
    /// </returns>
    GameType GameType { get; }
    /// <summary>
    ///     Gets the additional information about the game.
    /// </summary>
    /// <returns>
    ///     A string containing the additional information about the game.
    /// </returns>
    string Options { get; }
    /// <summary>
    ///     Gets whether the KaiHeiLa client needs administrator privileges to detect the game.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the KaiHeiLa client needs administrator privileges to detect the game; otherwise, <c>false</c>.
    /// </returns>
    bool RequireAdminPrivilege { get; }
    /// <summary>
    ///     Gets the process names of the game.
    /// </summary>
    /// <returns>
    ///     An <see cref="IReadOnlyCollection{String}"/> containing the process names of the game.
    /// </returns>
    IReadOnlyCollection<string> ProcessNames { get; }
    /// <summary>
    ///     Gets the product names of the game.
    /// </summary>
    /// <returns>
    ///     An <see cref="IReadOnlyCollection{String}"/> containing the product names of the game.
    /// </returns>
    IReadOnlyCollection<string> ProductNames { get; }
    /// <summary>
    ///     Gets the URL of the game's icon.
    /// </summary>
    /// <returns>
    ///     A string representing the URL of the game's icon.
    /// </returns>
    string Icon { get; }

    /// <summary>
    ///     Modifies this game.
    /// </summary>
    /// <remarks>
    ///     This method modifies this game with the specified properties. To see an example of this
    ///     method and what properties are available, please refer to <see cref="GameProperties"/>.
    /// </remarks>
    /// <param name="func">A delegate containing the properties to modify the game with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous modification operation.
    /// </returns>
    Task<IGame> ModifyAsync(Action<GameProperties> func, RequestOptions options = null);
}