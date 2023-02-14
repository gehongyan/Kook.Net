namespace Kook;

/// <summary>
///     Represents a generic poke.
/// </summary>
public interface IPoke : IEntity<uint>
{
    /// <summary>
    ///     Gets the name of the poke.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the description of the poke.
    /// </summary>
    string Description { get; }

    /// <summary>
    ///     Gets how long a user needs to wait before they can use the poke again.
    /// </summary>
    TimeSpan Cooldown { get; }

    /// <summary>
    ///     Gets the categories of the poke.
    /// </summary>
    IReadOnlyCollection<string> Categories { get; }

    /// <summary>
    ///     Gets the label of the poke.
    /// </summary>
    PokeLabel Label { get; }

    /// <summary>
    ///      Gets the icon resources of the poke.
    /// </summary>
    PokeIcon Icon { get; }

    /// <summary>
    ///     Gets the quality of the poke.
    /// </summary>
    PokeQuality Quality { get; }

    /// <summary>
    ///     Gets the resource of the poke.
    /// </summary>
    IPokeResource Resource { get; }

    /// <summary>
    ///     Gets how the poke can be used and displayed in message contexts.
    /// </summary>
    IReadOnlyDictionary<string, string> MessageScenarios { get; }
}
