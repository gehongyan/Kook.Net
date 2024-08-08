namespace Kook;

/// <summary>
///     表示一个 POKE 的标签。
/// </summary>
public struct PokeLabel : IEntity<uint>
{
    /// <summary>
    ///     获取 POKE 的标签的唯一标识符。
    /// </summary>
    public uint Id { get; }

    /// <summary>
    ///     获取 POKE 的标签的名称。
    /// </summary>
    public string Name { get; }

    internal PokeLabel(uint id, string name)
    {
        Id = id;
        Name = name;
    }

    internal static PokeLabel Create(uint id, string name) => new(id, name);
}
