using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     An element builder to build a <see cref="ParagraphStruct"/>.
/// </summary>
public class ParagraphStructBuilder : IElementBuilder, IEquatable<ParagraphStructBuilder>, IEquatable<IElementBuilder>
{
    /// <summary>
    ///     Returns the maximum number of fields allowed by Kook.
    /// </summary>
    public const int MaxFieldCount = 50;

    /// <summary>
    ///     Returns the minimum number of columns allowed by Kook.
    /// </summary>
    public const int MinColumnCount = 1;

    /// <summary>
    ///     Returns the maximum number of columns allowed by Kook.
    /// </summary>
    public const int MaxColumnCount = 3;

    /// <summary>
    ///     Initializes a new <see cref="ParagraphStructBuilder"/> class.
    /// </summary>
    public ParagraphStructBuilder()
    {
        Fields = [];
    }

    /// <summary>
    ///     Initializes a new <see cref="ParagraphStructBuilder"/> class.
    /// </summary>
    public ParagraphStructBuilder(int columnCount, IList<IElementBuilder>? fields = null)
    {
        ColumnCount = columnCount;
        Fields = fields ?? [];
    }

    /// <summary>
    ///     Gets the type of the element that this builder builds.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> that represents the type of element that this builder builds.
    /// </returns>
    public ElementType Type => ElementType.Paragraph;

    /// <summary>
    ///     Gets or sets the number of columns of the paragraph.
    /// </summary>
    /// <returns>
    ///     An int that represents the number of columns of the paragraph.
    /// </returns>
    public int ColumnCount { get; set; }

    /// <summary>
    ///     Gets or sets the fields of the paragraph.
    /// </summary>
    /// <returns>
    ///     An <see cref="IList{IElementBuilder}"/> that represents the fields of the paragraph.
    /// </returns>
    public IList<IElementBuilder> Fields { get; set; }

    /// <summary>
    ///     Sets the number of columns of the paragraph.
    /// </summary>
    /// <param name="count">
    ///     An int that represents the number of columns of the paragraph.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ParagraphStructBuilder WithColumnCount(int count)
    {
        ColumnCount = count;
        return this;
    }

    /// <summary>
    ///     Adds a field to the paragraph.
    /// </summary>
    /// <param name="field">
    ///     A <see cref="PlainTextElementBuilder"/> that represents the field to add.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ParagraphStructBuilder AddField(PlainTextElementBuilder field)
    {
        Fields.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds a field to the paragraph.
    /// </summary>
    /// <param name="field">
    ///     A <see cref="KMarkdownElementBuilder"/> that represents the field to add.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ParagraphStructBuilder AddField(KMarkdownElementBuilder field)
    {
        Fields.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds a field to the paragraph.
    /// </summary>
    /// <param name="action">
    ///     The action to create a builder of a <see cref="KMarkdownElement"/>, which will be added to the paragraph.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ParagraphStructBuilder AddField<T>(Action<T>? action = null)
        where T : IElementBuilder, new()
    {
        T field = new();
        action?.Invoke(field);
        Fields.Add(field);
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="ParagraphStructBuilder"/> into a <see cref="ParagraphStruct"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="ParagraphStruct"/> represents the built element object.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <see cref="ColumnCount"/> is less than <see cref="MinColumnCount"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <see cref="ColumnCount"/> is greater than <see cref="MaxColumnCount"/>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     The <see cref="Fields"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     The number of <see cref="Fields"/> is greater than <see cref="MaxFieldCount"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     The <see cref="Fields"/> contain an element that is not a <see cref="PlainTextElementBuilder"/>
    ///     or <see cref="KMarkdownElementBuilder"/>.
    /// </exception>
    public ParagraphStruct Build()
    {
        if (ColumnCount < MinColumnCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(ColumnCount), $"Column must be more than or equal to {MinColumnCount}.");
        }

        if (ColumnCount > MaxColumnCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(ColumnCount), $"Column must be less than or equal to {MaxColumnCount}.");
        }

        if (Fields == null)
        {
            throw new ArgumentNullException(
                nameof(Fields),
                "The fields of a paragraph cannot be null.");
        }

        if (Fields.Count > MaxFieldCount)
        {
            throw new ArgumentException(
                $"Field count must be less than or equal to {MaxFieldCount}.",
                nameof(Fields));
        }

        if (Fields.Any(field => field is not (PlainTextElementBuilder or KMarkdownElementBuilder)))
        {
            throw new ArgumentException(
                "The elements of fields in a paragraph must be PlainTextElementBuilder or KMarkdownElementBuilder.",
                nameof(Fields));
        }

        return new ParagraphStruct(ColumnCount, [..Fields.Select(f => f.Build())]);
    }

    /// <inheritdoc />
    IElement IElementBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="ParagraphStructBuilder"/> is equal to the current <see cref="ParagraphStructBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ParagraphStructBuilder"/> is equal to the current <see cref="ParagraphStructBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(ParagraphStructBuilder? left, ParagraphStructBuilder? right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="ParagraphStructBuilder"/> is not equal to the current <see cref="ParagraphStructBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ParagraphStructBuilder"/> is not equal to the current <see cref="ParagraphStructBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(ParagraphStructBuilder? left, ParagraphStructBuilder? right)
        => !(left == right);

    /// <summary>
    ///     Determines whether the specified <see cref="object"/> is equal to the current <see cref="ParagraphStructBuilder"/>.
    /// </summary>
    /// <param name="obj"> The <see cref="object"/> to compare with the current <see cref="ParagraphStructBuilder"/>. </param>
    /// <returns> <c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="ParagraphStructBuilder"/>; otherwise, <c>false</c>. </returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is ParagraphStructBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="ParagraphStructBuilder"/> is equal to the current <see cref="ParagraphStructBuilder"/>.</summary>
    /// <param name="paragraphStructBuilder">The <see cref="ParagraphStructBuilder"/> to compare with the current <see cref="ParagraphStructBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ParagraphStructBuilder"/> is equal to the current <see cref="ParagraphStructBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)] ParagraphStructBuilder? paragraphStructBuilder)
    {
        if (paragraphStructBuilder is null)
            return false;

        if (Fields.Count != paragraphStructBuilder.Fields.Count)
            return false;

        if (Fields
            .Zip(paragraphStructBuilder.Fields, (x, y) => (x, y))
            .Any(pair => pair.x != pair.y))
            return false;

        return Type == paragraphStructBuilder.Type
            && ColumnCount == paragraphStructBuilder.ColumnCount;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IElementBuilder>.Equals([NotNullWhen(true)] IElementBuilder? elementBuilder) =>
        Equals(elementBuilder as ParagraphStructBuilder);
}
