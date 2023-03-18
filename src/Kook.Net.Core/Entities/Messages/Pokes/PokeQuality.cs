namespace Kook;

/// <summary>
///     Represents the quality of a <see cref="IPoke"/>.
/// </summary>
public struct PokeQuality : IEntity<uint>
{
    /// <summary>
    ///     Gets the identifier of the <see cref="PokeQuality"/>.
    /// </summary>
    public uint Id { get; internal set; }

    /// <summary>
    ///     Gets the color of the <see cref="PokeQuality"/>.
    /// </summary>
    public Color Color { get; internal set; }

    /// <summary>
    ///     Gets the resources of the <see cref="PokeQuality"/>.
    /// </summary>
    public IReadOnlyDictionary<string, string> Resources { get; internal set; }

    internal PokeQuality(uint id, Color color, IReadOnlyDictionary<string, string> resources)
    {
        Id = id;
        Color = color;
        Resources = resources;
    }

    internal static PokeQuality Create(uint id, Color color, IReadOnlyDictionary<string, string> resources)
        => new PokeQuality(id, color, resources);
}
