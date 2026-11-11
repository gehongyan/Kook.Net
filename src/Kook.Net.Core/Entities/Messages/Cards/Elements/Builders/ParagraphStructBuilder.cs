namespace Kook;

/// <summary>
///     用来构建 <see cref="ParagraphStruct"/> 元素的构建器。
/// </summary>
public record ParagraphStructBuilder : IElementBuilder
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
    ///     添加一个文本块到区域文本。
    /// </summary>
    /// <param name="action">
    ///     用于创建一个 <see cref="KMarkdownElement"/> 的构建器的操作，该构建器将被添加到区域文本。
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
}
