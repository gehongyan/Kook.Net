namespace Kook;

/// <summary>
///     Represents the label of a <see cref="IPoke"/>.
/// </summary>
public struct PokeLabel : IEntity<uint>
{
    public uint Id { get; internal set; }
    
    public string Name { get; internal set; }

    internal PokeLabel(uint id, string name)
    {
        Id = id;
        Name = name;
    }

    internal static PokeLabel Create(uint id, string name) => new(id, name);
}