using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     用来构建 <see cref="HeaderModule"/> 模块的构建器。
/// </summary>
public record HeaderModuleBuilder : IModuleBuilder
{
    /// <summary>
    ///     标题内容文本的最大长度。
    /// </summary>
    public const int MaxHeaderContentLength = 100;

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Header;

    /// <summary>
    ///     初始化一个 <see cref="HeaderModuleBuilder"/> 类的新实例。
    /// </summary>
    public HeaderModuleBuilder()
    {
    }

    /// <summary>
    ///     初始化一个 <see cref="HeaderModuleBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="text"> 标题文本。 </param>
    public HeaderModuleBuilder(PlainTextElementBuilder text)
    {
        Text = text;
    }

    /// <summary>
    ///     初始化一个 <see cref="HeaderModuleBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="text"> 标题文本。 </param>
    public HeaderModuleBuilder(string text)
    {
        Text = new PlainTextElementBuilder(text);
    }

    /// <summary>
    ///     获取或设置标题文本。
    /// </summary>
    public PlainTextElementBuilder? Text { get; set; }

    /// <summary>
    ///     设置标题文本。
    /// </summary>
    /// <param name="text"> 要设置的标题文本。 </param>
    /// <returns> 当前构建器。 </returns>
    public HeaderModuleBuilder WithText(PlainTextElementBuilder text)
    {
        Text = text;
        return this;
    }

    /// <summary>
    ///     设置标题文本。
    /// </summary>
    /// <param name="text"> 要设置的标题文本。 </param>
    /// <returns> 当前构建器。 </returns>
    public HeaderModuleBuilder WithText(string text)
    {
        Text = new PlainTextElementBuilder(text);
        return this;
    }

    /// <summary>
    ///     设置标题文本。
    /// </summary>
    /// <param name="action"> 一个包含对要设置的标题文本进行配置的操作的委托。 </param>
    /// <returns> 当前构建器。 </returns>
    public HeaderModuleBuilder WithText(Action<PlainTextElementBuilder>? action = null)
    {
        PlainTextElementBuilder text = new();
        action?.Invoke(text);
        Text = text;
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="HeaderModule"/> 对象。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="HeaderModule"/> 对象。 </returns>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Text"/> 为 <c>null</c>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Text"/> 的内容为 <c>null</c>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Text"/> 的内容长度超过了 <see cref="MaxHeaderContentLength"/>。
    /// </exception>
    [MemberNotNull(nameof(Text))]
    public HeaderModule Build()
    {
        if (Text is null)
            throw new ArgumentNullException(nameof(Text), "The header text cannot be null.");

        if (Text.Content is null)
            throw new ArgumentException("The content of the header text cannot be null.", nameof(Text));

        if (Text.Content.Length > MaxHeaderContentLength)
            throw new ArgumentException(
                $"Header content length must be less than or equal to {MaxHeaderContentLength}.",
                nameof(Text));

        return new HeaderModule(Text.Build());
    }

    /// <inheritdoc cref="Kook.HeaderModuleBuilder(System.String)" />
    public static implicit operator HeaderModuleBuilder(string text) => new(text);

    /// <inheritdoc />
    [MemberNotNull(nameof(Text))]
    IModule IModuleBuilder.Build() => Build();
}
