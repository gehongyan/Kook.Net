namespace Kook;

/// <summary>
///     Represents a generic poke resource.
/// </summary>
public interface IPokeResource
{
    /// <summary>
    ///     Gets the type of the poke resource.
    /// </summary>
    PokeResourceType Type { get; }
}