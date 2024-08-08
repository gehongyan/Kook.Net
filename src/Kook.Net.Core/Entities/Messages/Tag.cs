using System.Diagnostics;

namespace Kook;

/// <inheritdoc />
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Tag<TKey, TValue> : ITag
    where TKey : IEquatable<TKey>
    where TValue : IEntity<TKey>
{
    /// <inheritdoc />
    public TagType Type { get; }

    /// <inheritdoc />
    public int Index { get; }

    /// <inheritdoc />
    public int Length { get; }

    /// <inheritdoc cref="P:Kook.ITag.Key" />
    public TKey Key { get; }

    /// <inheritdoc cref="P:Kook.ITag.Key" />
    public TValue? Value { get; }

    internal Tag(TagType type, int index, int length, TKey key, TValue? value)
    {
        Type = type;
        Index = index;
        Length = length;
        Key = key;
        Value = value;
    }

    private string DebuggerDisplay => $"{(Type is TagType.HereMention ? "@在线成员" : Value?.ToString()) ?? "null"} ({Type})";

    /// <inheritdoc />
    public override string ToString() => DebuggerDisplay;

    /// <inheritdoc />
    object ITag.Key => Key;

    /// <inheritdoc />
    object? ITag.Value => Value;
}
