namespace Kook;

/// <summary>
///     用来构建 <see cref="SectionModule"/> 模块的构建器。
/// </summary>
public record SectionModuleBuilder : IModuleBuilder
{
    /// <inheritdoc />
    public ModuleType Type => ModuleType.Section;

    /// <summary>
    ///     初始化一个 <see cref="SectionModuleBuilder"/> 类的新实例。
    /// </summary>
    public SectionModuleBuilder()
    {
    }

    /// <summary>
    ///     初始化一个 <see cref="SectionModuleBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="text"> 文本元素。 </param>
    /// <param name="mode"> 附加内容的显示模式。 </param>
    /// <param name="accessory"> 附加内容元素。 </param>
    public SectionModuleBuilder(IElementBuilder? text,
        SectionAccessoryMode? mode = null, IElementBuilder? accessory = null)
    {
        Text = text;
        Mode = mode;
        Accessory = accessory;
    }

    /// <summary>
    ///     初始化一个 <see cref="SectionModuleBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="text"> 文本元素。 </param>
    /// <param name="isKMarkdown"> 是否为 KMarkdown 格式。 </param>
    /// <param name="mode"> 附加内容的显示模式。 </param>
    /// <param name="accessory"> 附加内容元素。 </param>
    public SectionModuleBuilder(string? text, bool isKMarkdown = false,
        SectionAccessoryMode? mode = null, IElementBuilder? accessory = null)
    {
        WithText(text, isKMarkdown);
        Mode = mode;
        Accessory = accessory;
    }

    /// <summary>
    ///     获取或设置附加内容的显示模式。
    /// </summary>
    public SectionAccessoryMode? Mode { get; set; }

    /// <summary>
    ///     获取或设置模块的文本。
    /// </summary>
    public IElementBuilder? Text { get; set; }

    /// <summary>
    ///     获取或设置模块的附加内容。
    /// </summary>
    public IElementBuilder? Accessory { get; set; }

    /// <summary>
    ///     设置模块的文本。
    /// </summary>
    /// <param name="text"> 要设置的文本。 </param>
    /// <returns> 当前构建器。 </returns>
    public SectionModuleBuilder WithText(PlainTextElementBuilder text)
    {
        Text = text;
        return this;
    }

    /// <summary>
    ///     设置模块的文本。
    /// </summary>
    /// <param name="text"> 要设置的文本。 </param>
    /// <returns> 当前构建器。 </returns>
    public SectionModuleBuilder WithText(KMarkdownElementBuilder text)
    {
        Text = text;
        return this;
    }

    /// <summary>
    ///     设置模块的文本。
    /// </summary>
    /// <param name="text"> 要设置的文本。 </param>
    /// <param name="isKMarkdown"> 是否为 KMarkdown 格式。 </param>
    /// <returns> 当前构建器。 </returns>
    public SectionModuleBuilder WithText(string? text, bool isKMarkdown = false)
    {
        Text = isKMarkdown switch
        {
            false => new PlainTextElementBuilder(text),
            true => new KMarkdownElementBuilder(text)
        };
        return this;
    }

    /// <summary>
    ///     设置模块的文本。
    /// </summary>
    /// <param name="text"> 要设置的文本。 </param>
    /// <returns> 当前构建器。 </returns>
    public SectionModuleBuilder WithText(ParagraphStructBuilder text)
    {
        Text = text;
        return this;
    }

    /// <summary>
    ///     设置模块的文本。
    /// </summary>
    /// <param name="action"> 一个包含对要设置的文本进行配置的操作的委托。 </param>
    /// <typeparam name="T"> 要设置的文本的类型。 </typeparam>
    /// <returns> 当前构建器。 </returns>
    public SectionModuleBuilder WithText<T>(Action<T>? action = null)
        where T : IElementBuilder, new()
    {
        T text = new();
        action?.Invoke(text);
        Text = text;
        return this;
    }

    /// <summary>
    ///     设置模块的附加内容。
    /// </summary>
    /// <param name="accessory"> 要设置的附加内容。 </param>
    /// <returns> 当前构建器。 </returns>
    public SectionModuleBuilder WithAccessory(ImageElementBuilder accessory)
    {
        Accessory = accessory;
        return this;
    }

    /// <summary>
    ///     设置模块的附加内容。
    /// </summary>
    /// <param name="accessory"> 要设置的附加内容。 </param>
    /// <returns> 当前构建器。 </returns>
    public SectionModuleBuilder WithAccessory(ButtonElementBuilder accessory)
    {
        Accessory = accessory;
        return this;
    }

    /// <summary>
    ///     设置模块的附加内容。
    /// </summary>
    /// <param name="action"> 一个包含对要设置的附加内容进行配置的操作的委托。 </param>
    /// <returns> 当前构建器。 </returns>
    public SectionModuleBuilder WithAccessory<T>(Action<T>? action = null)
        where T : IElementBuilder, new()
    {
        T accessory = new();
        action?.Invoke(accessory);
        Accessory = accessory;
        return this;
    }

    /// <summary>
    ///     设置附加内容的显示模式。
    /// </summary>
    /// <param name="mode"> 要设置的附加内容的显示模式。 </param>
    /// <returns> 当前构建器。 </returns>
    public SectionModuleBuilder WithMode(SectionAccessoryMode mode)
    {
        Mode = mode;
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="SectionModule"/> 对象。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="SectionModule"/> 对象。 </returns>
    /// <exception cref="ArgumentException">
    ///     <see cref="Text"/> 不是任何形式的文本元素，包括 <see cref="PlainTextElementBuilder"/>、
    ///     <see cref="KMarkdownElementBuilder"/> 和 <see cref="ParagraphStructBuilder"/>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Accessory"/> 既不是 <see cref="ImageElementBuilder"/> 也不是 <see cref="ButtonElementBuilder"/>。
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     <see cref="ButtonElement"/> 不应位于 <see cref="Text"/> 的左侧。
    /// </exception>
    public SectionModule Build()
    {
        if (Text is not (null or PlainTextElementBuilder or KMarkdownElementBuilder or ParagraphStructBuilder))
            throw new ArgumentException(
                "Section text must be a PlainText element, a KMarkdown element or a Paragraph struct if set.",
                nameof(Text));

        if (Accessory is not (null or ImageElementBuilder or ButtonElementBuilder))
            throw new ArgumentException(
                "Section accessory must be an Image element or a Button element if set.",
                nameof(Accessory));

        if (Mode != SectionAccessoryMode.Right && Accessory is ButtonElementBuilder)
            throw new InvalidOperationException("Button must be placed on the right");

        return new SectionModule(Mode, Text?.Build(), Accessory?.Build());
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();
}
