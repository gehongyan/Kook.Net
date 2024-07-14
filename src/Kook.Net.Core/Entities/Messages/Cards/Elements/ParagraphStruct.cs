using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     区域文本结构，可用于 <see cref="IModule"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ParagraphStruct : IElement, IEquatable<ParagraphStruct>, IEquatable<IElement>
{
    internal ParagraphStruct(int? columnCount, ImmutableArray<IElement> fields)
    {
        ColumnCount = columnCount;
        Fields = fields;
    }

    /// <inheritdoc />
    public ElementType Type => ElementType.Paragraph;

    /// <summary>
    ///     获取区域文本的列数。
    /// </summary>
    public int? ColumnCount { get; }

    /// <summary>
    ///     获取区域文本的文本块。
    /// </summary>
    public ImmutableArray<IElement> Fields { get; }

    private string DebuggerDisplay => $"{Type} ({ColumnCount} Columns, {Fields.Length} Fields)";

    /// <summary>
    ///     判定两个 <see cref="ParagraphStruct"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ParagraphStruct"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(ParagraphStruct? left, ParagraphStruct? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="ParagraphStruct"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ParagraphStruct"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(ParagraphStruct? left, ParagraphStruct? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ParagraphStruct paragraphStruct && Equals(paragraphStruct);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] ParagraphStruct? paragraphStruct) =>
        GetHashCode() == paragraphStruct?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int) 2166136261;
            hash = (hash * 16777619) ^ (Type, ColumnCount).GetHashCode();
            foreach (IElement element in Fields)
                hash = (hash * 16777619) ^ element.GetHashCode();
            return hash;
        }
    }

    bool IEquatable<IElement>.Equals([NotNullWhen(true)] IElement? element) =>
        Equals(element as ParagraphStruct);
}
