using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Kook;

/// <summary>
///     A paragraph struct that can be used in modules.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ParagraphStruct : IElement, IEquatable<ParagraphStruct>
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
    ///     An int value that represents the number of columns in the paragraph.
    /// </returns>
    public int ColumnCount { get; }

    /// <summary>
    ///     Gets the fields in the paragraph.
    /// </summary>
    /// <returns>
    ///     An <see cref="ImmutableArray{IElement}"/> array that contains the fields in the paragraph.
    /// </returns>
    public ImmutableArray<IElement> Fields { get; }

    private string DebuggerDisplay => $"{Type} ({ColumnCount} Columns, {Fields.Length} Fields)";
    public static bool operator ==(ParagraphStruct left, ParagraphStruct right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(ParagraphStruct left, ParagraphStruct right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="ParagraphStruct"/> is equal to the current <see cref="ParagraphStruct"/>.</summary>
    /// <remarks>If the object passes is an <see cref="ParagraphStruct"/>, <see cref="Equals(ParagraphStruct)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="ParagraphStruct"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ParagraphStruct"/> is equal to the current <see cref="ParagraphStruct"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is ParagraphStruct paragraphStruct && Equals(paragraphStruct);

    /// <summary>Determines whether the specified <see cref="ParagraphStruct"/> is equal to the current <see cref="ParagraphStruct"/>.</summary>
    /// <param name="paragraphStruct">The <see cref="ParagraphStruct"/> to compare with the current <see cref="ParagraphStruct"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ParagraphStruct"/> is equal to the current <see cref="ParagraphStruct"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(ParagraphStruct paragraphStruct)
        => GetHashCode() == paragraphStruct?.GetHashCode();

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
}