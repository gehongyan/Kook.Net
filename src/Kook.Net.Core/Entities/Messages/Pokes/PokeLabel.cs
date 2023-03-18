namespace Kook;

/// <summary>
///     Represents the label of a <see cref="IPoke"/>.
/// </summary>
public struct PokeLabel : IEntity<uint>
{
    /// <summary>
    ///     Gets the ID of the  poke action label.
    /// </summary>
    public uint Id { get; internal set; }

    /// <summary>
    ///     Gets the name of the poke action label.
    /// </summary>
    public string Name { get; internal set; }

    internal PokeLabel(uint id, string name)
    {
        Id = id;
        Name = name;
    }

    internal static PokeLabel Create(uint id, string name) => new(id, name);
}
