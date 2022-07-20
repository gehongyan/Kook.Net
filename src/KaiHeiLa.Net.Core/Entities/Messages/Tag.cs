using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     Represents a tag found in <see cref="IMessage"/>.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class Tag<T> : ITag
{
    /// <inheritdoc />
    public TagType Type { get; }
    /// <inheritdoc />
    public int Index { get; }
    /// <inheritdoc />
    public int Length { get; }
    /// <inheritdoc />
    public dynamic Key { get; }
    
    /// <summary>
    ///     Gets the value of the tag.
    /// </summary>
    /// <remarks>
    ///     When <see cref="Type"/> is <see cref="TagType.HereMention"/>,
    ///     this property returns the same entity as <see cref="IGuild.EveryoneRole"/> for convenience.
    ///     because there is no actual entities representing a group of online users.
    /// </remarks>
    public T Value { get; }

    internal Tag(TagType type, int index, int length, dynamic key, T value)
    {
        Type = type;
        Index = index;
        Length = length;
        Key = key;
        Value = value;
    }

    private string DebuggerDisplay => $"{(Type is TagType.HereMention ? "@在线成员" : Value?.ToString()) ?? "null"} ({Type})";
    public override string ToString() => $"{(Type is TagType.HereMention ? "@在线成员" : Value?.ToString()) ?? "null"} ({Type})";

    /// <inheritdoc />
    object ITag.Value => Value;
}