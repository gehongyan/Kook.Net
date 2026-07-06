using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     用来构建 <see cref="ButtonElement"/> 元素的构建器。
/// </summary>
public record ButtonElementBuilder : IElementBuilder
{
    /// <summary>
    ///     按钮文本的最大长度。
    /// </summary>
    public const int MaxButtonTextLength = 40;

    /// <summary>
    ///     初始化一个 <see cref="ButtonElementBuilder"/> 类的新实例。
    /// </summary>
    public ButtonElementBuilder()
    {
    }

    /// <summary>
    ///     初始化一个 <see cref="ButtonElementBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="text"> 按钮的文本。 </param>
    /// <param name="theme"> 按钮的主题。 </param>
    /// <param name="value"> 按钮的值。 </param>
    /// <param name="click"> 按钮的点击事件类型。 </param>
    /// <remarks>
    ///     如果 <paramref name="click"/> 设置为 <see cref="Kook.ButtonClickEventType.ReturnValue"/>，
    ///     则在用户点击按钮时，KOOK 会通过网关下发按钮点击事件，并携带 <paramref name="value" /> 的值。<br />
    ///     如果 <paramref name="click"/> 设置为 <see cref="Kook.ButtonClickEventType.Link"/>，
    ///     则在用户点击按钮时，KOOK 会将用户重定向到 <paramref name="value" /> 指定的 URL。
    /// </remarks>
    public ButtonElementBuilder(string text, ButtonTheme theme = ButtonTheme.Primary,
        string? value = null, ButtonClickEventType click = ButtonClickEventType.None)
    {
        Text = new PlainTextElementBuilder(text);
        Theme = theme;
        Value = value;
        Click = click;
    }

    /// <inheritdoc />
    public ElementType Type => ElementType.Button;

    /// <summary>
    ///     获取或设置按钮的主题。
    /// </summary>
    public ButtonTheme Theme { get; set; }

    /// <summary>
    ///     获取或设置按钮的值。
    /// </summary>
    /// <remarks>
    ///     如果 <see cref="Click"/> 设置为 <see cref="Kook.ButtonClickEventType.ReturnValue"/>，
    ///     则在用户点击按钮时，KOOK 会通过网关下发按钮点击事件，并携带此属性的值。<br />
    ///     如果 <see cref="Click"/> 设置为 <see cref="Kook.ButtonClickEventType.Link"/>，
    ///     则在用户点击按钮时，KOOK 会将用户重定向到此属性指定的 URL。
    /// </remarks>
    public string? Value { get; set; }

    /// <summary>
    ///     获取或设置按钮被点击时触发的事件类型。
    /// </summary>
    /// <remarks>
    ///     如果此属性设置为 <see cref="Kook.ButtonClickEventType.ReturnValue"/>，
    ///     则在用户点击按钮时，KOOK 会通过网关下发按钮点击事件，并携带 <see cref="Value"/> 的值。<br />
    ///     如果此属性设置为 <see cref="Kook.ButtonClickEventType.Link"/>，
    ///     则在用户点击按钮时，KOOK 会将用户重定向到 <see cref="Value"/> 指定的 URL。
    /// </remarks>
    public ButtonClickEventType Click { get; set; }

    /// <summary>
    ///     获取或设置按钮的文本元素。
    /// </summary>
    /// <remarks>
    ///     此属性只接受 <see cref="PlainTextElementBuilder"/> 或 <see cref="KMarkdownElementBuilder"/>。
    /// </remarks>
    public IElementBuilder? Text { get; set; }

    /// <summary>
    ///     设置按钮的主题，值将被设置到 <see cref="Theme"/> 属性上。
    /// </summary>
    /// <param name="theme"> 按钮的主题。 </param>
    /// <returns> 当前构建器。 </returns>
    public ButtonElementBuilder WithTheme(ButtonTheme theme)
    {
        Theme = theme;
        return this;
    }

    /// <summary>
    ///     设置按钮的值，值将被设置到 <see cref="Value"/> 属性上。
    /// </summary>
    /// <param name="value"> 按钮的值。 </param>
    /// <returns> 当前构建器。 </returns>
    public ButtonElementBuilder WithValue(string? value)
    {
        Value = value;
        return this;
    }

    /// <summary>
    ///     设置按钮被点击时触发的事件类型，值将被设置到 <see cref="Click"/> 属性上。
    /// </summary>
    /// <param name="click"> 按钮的点击事件类型。 </param>
    /// <returns> 当前构建器。 </returns>
    public ButtonElementBuilder WithClick(ButtonClickEventType click)
    {
        Click = click;
        return this;
    }

    /// <summary>
    ///     设置按钮的文本。
    /// </summary>
    /// <param name="text"> 按钮的文本。 </param>
    /// <returns> 当前构建器。 </returns>
    public ButtonElementBuilder WithText(PlainTextElementBuilder text)
    {
        Text = text;
        return this;
    }

    /// <summary>
    ///     设置按钮的文本。
    /// </summary>
    /// <param name="text"> 按钮的文本。 </param>
    /// <returns> 当前构建器。 </returns>
    public ButtonElementBuilder WithText(KMarkdownElementBuilder text)
    {
        Text = text;
        return this;
    }

    /// <summary>
    ///     设置按钮的文本。
    /// </summary>
    /// <param name="action">
    ///     一个包含对新创建的文本元素构建器进行配置的操作的委托，委托的入参类型必须是 <see cref="PlainTextElementBuilder"/>
    ///     或 <see cref="KMarkdownElementBuilder"/>。
    /// </param>
    /// <returns> 当前构建器。 </returns>
    public ButtonElementBuilder WithText<T>(Action<T> action)
        where T : IElementBuilder, new()
    {
        T text = new();
        action.Invoke(text);
        Text = text;
        return this;
    }

    /// <summary>
    ///     设置按钮的文本。
    /// </summary>
    /// <param name="text">
    ///     按钮的文本。
    /// </param>
    /// <param name="isKMarkdown">
    ///     文本是否为 KMarkdown 格式；如果为 <c>true</c>，则文本将被设置为 <see cref="KMarkdownElement"/>；如果为
    ///     <c>false</c>，则文本将被设置为 <see cref="PlainTextElement"/>。
    /// </param>
    /// <returns> 当前构建器。 </returns>
    public ButtonElementBuilder WithText(string text, bool isKMarkdown = false)
    {
        Text = isKMarkdown switch
        {
            false => new PlainTextElementBuilder(text),
            true => new KMarkdownElementBuilder(text)
        };
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="ButtonElement"/>。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="ButtonElement"/> 对象。 </returns>
    /// <exception cref="ArgumentException">
    ///     <see cref="Text"/> 既不是 <see cref="PlainTextElementBuilder"/> 也不是 <see cref="KMarkdownElementBuilder"/>。
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Text"/> 为 <c>null</c>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Text"/> 为空字符串。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     文本的长度超过了 <see cref="MaxButtonTextLength"/>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Click"/> 为 <see cref="Kook.ButtonClickEventType.Link"/> 但 <see cref="Value"/> 为 <c>null</c> 或空。
    /// </exception>
    /// <exception cref="UriFormatException">
    ///     <see cref="Value"/> 不是有效的 URL。
    /// </exception>
    [MemberNotNull(nameof(Text))]
    public ButtonElement Build()
    {
        string? text = Text switch
        {
            PlainTextElementBuilder plainText => plainText.Content,
            KMarkdownElementBuilder kMarkdown => kMarkdown.Content,
            _ => throw new ArgumentException(
                $"The text of a button must be a {nameof(PlainTextElementBuilder)} or a {nameof(KMarkdownElementBuilder)}.",
                nameof(Text))
        };

        if (text is null)
            throw new ArgumentNullException(nameof(Text), "The text of a button cannot be null.");

        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("The text of a button cannot be empty.", nameof(Text));

        if (text.Length > MaxButtonTextLength)
        {
            throw new ArgumentException(
                $"The length of button text must be less than or equal to {MaxButtonTextLength}.",
                nameof(Text));
        }

        if (Click == ButtonClickEventType.Link)
        {
            if (Value is null || string.IsNullOrEmpty(Value))
                throw new ArgumentException("The value of a button with a link event type cannot be null or empty.",
                    nameof(Value));
            UrlValidation.Validate(Value);
        }

        return new ButtonElement(Theme, Value, Click, Text.Build());
    }

    /// <inheritdoc />
    [MemberNotNull(nameof(Text))]
    IElement IElementBuilder.Build() => Build();
}
