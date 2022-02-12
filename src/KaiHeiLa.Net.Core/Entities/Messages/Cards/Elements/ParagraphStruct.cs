using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace KaiHeiLa;

/// <summary>
///     区域文本
/// </summary>
/// <remarks>
///     支持分栏结构，将模块分为左右两栏，根据顺序自动排列，支持更自由的文字排版模式，提高可维护性
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ParagraphStruct : IElement
{
    internal ParagraphStruct(int columnCount, ImmutableArray<IElement> fields)
    {
        ColumnCount = columnCount;
        Fields = fields;
    }
    
    public ElementType Type => ElementType.Paragraph;

    public int ColumnCount { get; internal set; }

    public ImmutableArray<IElement> Fields { get; internal set; }

    private string DebuggerDisplay => $"{Type} ({ColumnCount} Columns, {Fields.Length} Fields)";
}