namespace KaiHeiLa;

public interface IGame : IEntity<int>, IDeletable
{
    string Name { get; }
    GameType Type { get; }
    string Options { get; }
    bool KmHookAdmin { get; }
    IReadOnlyCollection<string> ProcessNames { get; }
    IReadOnlyCollection<string> ProductNames { get; }
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