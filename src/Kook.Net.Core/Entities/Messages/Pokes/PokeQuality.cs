namespace Kook;

/// <summary>
///     表示一个 POKE 的品质。
/// </summary>
public struct PokeQuality : IEntity<uint>
{
    /// <summary>
    ///     获取 POKE 品质的唯一标识符。
    /// </summary>
    public uint Id { get; }

    /// <summary>
    ///     获取 POKE 品质的颜色。
    /// </summary>
    public Color Color { get; }

    /// <summary>
    ///     获取 POKE 品质的资源。
    /// </summary>
    public IReadOnlyDictionary<string, string> Resources { get; }

    internal PokeQuality(uint id, Color color, IReadOnlyDictionary<string, string> resources)
    {
        Id = id;
        Color = color;
        Resources = resources;
    }

    internal static PokeQuality Create(uint id, Color color, IReadOnlyDictionary<string, string> resources) => new(id, color, resources);
}
