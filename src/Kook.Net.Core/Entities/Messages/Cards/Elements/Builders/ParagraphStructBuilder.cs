using System.Collections.Immutable;

namespace Kook;

/// <summary>
///     An element builder to build a <see cref="ParagraphStruct"/>.
/// </summary>
public class ParagraphStructBuilder : IElementBuilder, IEquatable<ParagraphStructBuilder>
{
    private int _columnCount;
    private List<IElementBuilder> _fields;

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
        Fields = new List<IElementBuilder>();
    }

    /// <summary>
    ///     Initializes a new <see cref="ParagraphStructBuilder"/> class.
    /// </summary>
    public ParagraphStructBuilder(int columnCount, List<IElementBuilder> fields = null)
    {
        WithColumnCount(columnCount);
        Fields = fields ?? new List<IElementBuilder>();
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
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="value"/> is less than <see cref="MinColumnCount"/> or greater than <see cref="MaxColumnCount"/>.
    /// </exception>
    /// <returns>
    ///     An int that represents the number of columns of the paragraph.
    /// </returns>
    public int ColumnCount
    {
        get => _columnCount;
        set =>
            _columnCount = value switch
            {
                < MinColumnCount => throw new ArgumentException(
                    $"Column must be more than or equal to {MinColumnCount}.", nameof(ColumnCount)),
                > MaxColumnCount => throw new ArgumentException(
                    $"Column must be less than or equal to {MaxColumnCount}.", nameof(ColumnCount)),
                _ => value
            };
    }

    /// <summary>
    ///     Gets or sets the fields of the paragraph.
    /// </summary>
    /// <exception cref="ArgumentNullException" accessor="set">
    ///     The <paramref name="value"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="value"/> contains more than <see cref="MaxFieldCount"/> elements.
    /// </exception>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="value"/> contains an element that is neither a <see cref="PlainTextElementBuilder"/> nor a <see cref="KMarkdownElementBuilder"/>.
    /// </exception>
    /// <returns>
    ///     A <see cref="List{IElementBuilder}"/> that represents the fields of the paragraph.
    /// </returns>
    public List<IElementBuilder> Fields
    {
        get => _fields;
        set
        {
            if (value == null)
                throw new ArgumentNullException(
                    nameof(Fields),
                    "Cannot set an paragraph struct builder's fields collection to null.");

            if (value.Count > MaxFieldCount)
                throw new ArgumentException(
                    $"Field count must be less than or equal to {MaxFieldCount}.",
                    nameof(Fields));

            if (value.Any(field => field is not PlainTextElementBuilder && field is not KMarkdownElementBuilder))
                throw new ArgumentException(
                    "The elements of fields in a paragraph must be PlainTextElementBuilder or KMarkdownElementBuilder.",
                    nameof(Fields));

            _fields = value;
        }
    }

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
    /// <exception cref="ArgumentException">
    ///     The addition operation will result in a field count greater than <see cref="MaxFieldCount"/>.
    /// </exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ParagraphStructBuilder AddField(PlainTextElementBuilder field)
    {
        if (Fields.Count >= MaxFieldCount)
            throw new ArgumentException(
                $"Field count must be less than or equal to {MaxFieldCount}.",
                nameof(field));

        Fields.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds a field to the paragraph.
    /// </summary>
    /// <param name="field">
    ///     A <see cref="KMarkdownElementBuilder"/> that represents the field to add.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     The addition operation will result in a field count greater than <see cref="MaxFieldCount"/>.
    /// </exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ParagraphStructBuilder AddField(KMarkdownElementBuilder field)
    {
        if (Fields.Count >= MaxFieldCount)
            throw new ArgumentException(
                $"Field count must be less than or equal to {MaxFieldCount}.",
                nameof(field));

        Fields.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds a field to the paragraph.
    /// </summary>
    /// <param name="action">
    ///     The action to create a builder of a <see cref="KMarkdownElement"/>, which will be added to the paragraph.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     The addition operation will result in a field count greater than <see cref="MaxFieldCount"/>.
    /// </exception>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ParagraphStructBuilder AddField<T>(Action<T> action = null)
        where T : IElementBuilder, new()
    {
        T field = new();
        action?.Invoke(field);
        switch (field)
        {
            case PlainTextElementBuilder plainText:
                AddField(plainText);
                break;
            case KMarkdownElementBuilder kMarkdown:
                AddField(kMarkdown);
                break;
            default:
                throw new ArgumentException(
                    "The elements of fields in a paragraph must be PlainTextElementBuilder or KMarkdownElementBuilder.",
                    nameof(field));
        }

        return this;
    }

    /// <summary>
    ///     Builds the <see cref="ParagraphStructBuilder"/> into a <see cref="ParagraphStruct"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="ParagraphStruct"/> represents the built element object.
    /// </returns>
    public ParagraphStruct Build() =>
        new(ColumnCount, Fields.Select(f => f.Build()).ToImmutableArray());

    /// <inheritdoc />
    IElement IElementBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="ParagraphStructBuilder"/> is equal to the current <see cref="ParagraphStructBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ParagraphStructBuilder"/> is equal to the current <see cref="ParagraphStructBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(ParagraphStructBuilder left, ParagraphStructBuilder right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="ParagraphStructBuilder"/> is not equal to the current <see cref="ParagraphStructBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ParagraphStructBuilder"/> is not equal to the current <see cref="ParagraphStructBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(ParagraphStructBuilder left, ParagraphStructBuilder right)
        => !(left == right);

    /// <summary>
    ///     Determines whether the specified <see cref="object"/> is equal to the current <see cref="ParagraphStructBuilder"/>.
    /// </summary>
    /// <param name="obj"> The <see cref="object"/> to compare with the current <see cref="ParagraphStructBuilder"/>. </param>
    /// <returns> <c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="ParagraphStructBuilder"/>; otherwise, <c>false</c>. </returns>
    public override bool Equals(object obj)
        => obj is ParagraphStructBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="ParagraphStructBuilder"/> is equal to the current <see cref="ParagraphStructBuilder"/>.</summary>
    /// <param name="paragraphStructBuilder">The <see cref="ParagraphStructBuilder"/> to compare with the current <see cref="ParagraphStructBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ParagraphStructBuilder"/> is equal to the current <see cref="ParagraphStructBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(ParagraphStructBuilder paragraphStructBuilder)
    {
        if (paragraphStructBuilder is null) return false;

        if (Fields.Count != paragraphStructBuilder.Fields.Count) return false;

        for (int i = 0; i < Fields.Count; i++)
            if (Fields[i] != paragraphStructBuilder.Fields[i])
                return false;

        return Type == paragraphStructBuilder.Type
            && ColumnCount == paragraphStructBuilder.ColumnCount;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();
}
