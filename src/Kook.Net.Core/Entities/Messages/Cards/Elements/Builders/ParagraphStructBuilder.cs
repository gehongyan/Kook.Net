using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     用来构建 <see cref="ParagraphStruct"/> 元素的构建器。
/// </summary>
public class ParagraphStructBuilder : IElementBuilder, IEquatable<ParagraphStructBuilder>, IEquatable<IElementBuilder>
{
    /// <summary>
    ///     区域文本内文本块的最大数量。
    /// </summary>
    public const int MaxFieldCount = 50;

    /// <summary>
    ///     区域文本的最小列数。
    /// </summary>
    public const int MinColumnCount = 1;

    /// <summary>
    ///     区域文本的最大列数。
    /// </summary>
    public const int MaxColumnCount = 3;

    /// <summary>
    ///     初始化一个 <see cref="ParagraphStructBuilder"/> 类的新实例。
    /// </summary>
    public ParagraphStructBuilder()
    {
        Fields = [];
    }

    /// <summary>
    ///     初始化一个 <see cref="ParagraphStructBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="columnCount"> 区域文本的列数。 </param>
    /// <param name="fields"> 区域文本的文本块。 </param>
    public ParagraphStructBuilder(int columnCount, IList<IElementBuilder>? fields = null)
    {
        ColumnCount = columnCount;
        Fields = fields ?? [];
    }

    /// <inheritdoc />
    public ElementType Type => ElementType.Paragraph;

    /// <summary>
    ///     获取或设置区域文本的列数。
    /// </summary>
    /// <remarks>
    ///     默认值为 <see cref="MinColumnCount"/>。
    /// </remarks>
    public int ColumnCount { get; set; } = MinColumnCount;

    /// <summary>
    ///     获取或设置区域文本的文本块。
    /// </summary>
    public IList<IElementBuilder> Fields { get; set; }

    /// <summary>
    ///     设置区域文本的列数。
    /// </summary>
    /// <param name="count"> 区域文本的列数。 </param>
    /// <returns> 当前构建器。 </returns>
    public ParagraphStructBuilder WithColumnCount(int count)
    {
        ColumnCount = count;
        return this;
    }

    /// <summary>
    ///     添加一个纯文本文本块到区域文本。
    /// </summary>
    /// <param name="field"> 要添加的纯文本文本块。 </param>
    /// <returns> 当前构建器。 </returns>
    public ParagraphStructBuilder AddField(PlainTextElementBuilder field)
    {
        Fields.Add(field);
        return this;
    }

    /// <summary>
    ///     添加一个 KMarkdown 文本块到区域文本。
    /// </summary>
    /// <param name="field"> 要添加的 KMarkdown 文本块。 </param>
    /// <returns> 当前构建器。 </returns>
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
    /// <returns> 当前构建器。 </returns>
    public ParagraphStructBuilder AddField<T>(Action<T>? action = null)
        where T : IElementBuilder, new()
    {
        T field = new();
        action?.Invoke(field);
        Fields.Add(field);
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="ParagraphStruct"/>。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="ParagraphStruct"/> 对象。 </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="ColumnCount"/> 不足 <see cref="MinColumnCount"/>。
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="ColumnCount"/> 超出了 <see cref="MaxColumnCount"/>。
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Fields"/> 为 <c>null</c>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Fields"/> 的数量超出了 <see cref="MaxFieldCount"/>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Fields"/> 包含了既不是 <see cref="PlainTextElementBuilder"/> 也不是 <see cref="KMarkdownElementBuilder"/> 的元素。
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
    ///     判定两个 <see cref="ParagraphStructBuilder"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ParagraphStructBuilder"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(ParagraphStructBuilder? left, ParagraphStructBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="ParagraphStructBuilder"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ParagraphStructBuilder"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(ParagraphStructBuilder? left, ParagraphStructBuilder? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ParagraphStructBuilder builder && Equals(builder);

    /// <inheritdoc />
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
