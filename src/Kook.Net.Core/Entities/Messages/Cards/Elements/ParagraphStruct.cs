using System.Collections.Immutable;
using System.Diagnostics;

namespace Kook;

/// <summary>
///     区域文本结构，可用于 <see cref="IModule"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record ParagraphStruct : IElement
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
}
