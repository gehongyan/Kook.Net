using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace KaiHeiLa;

/// <summary>
///     A paragraph struct that can be used in modules.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ParagraphStruct : IElement
{
    internal ParagraphStruct(int columnCount, ImmutableArray<IElement> fields)
    {
        ColumnCount = columnCount;
        Fields = fields;
    }
    
    /// <summary>
    ///     Gets the type of the element.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> value that represents the theme of the button.
    /// </returns>
    public ElementType Type => ElementType.Paragraph;

    /// <summary>
    ///     Gets the number of columns in the paragraph.
    /// </summary>
    /// <returns>
    ///     An <see cref="int"/> value that represents the number of columns in the paragraph.
    /// </returns>
    public int ColumnCount { get; internal set; }

    /// <summary>
    ///     Gets the fields in the paragraph.
    /// </summary>
    /// <returns>
    ///     An <see cref="ImmutableArray{IElement}"/> array that contains the fields in the paragraph.
    /// </returns>
    public ImmutableArray<IElement> Fields { get; internal set; }

    private string DebuggerDisplay => $"{Type} ({ColumnCount} Columns, {Fields.Length} Fields)";
}