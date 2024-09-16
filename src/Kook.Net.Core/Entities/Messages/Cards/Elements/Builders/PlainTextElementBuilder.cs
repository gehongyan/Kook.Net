using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     用来构建 <see cref="PlainTextElement"/> 元素的构建器。
/// </summary>
public class PlainTextElementBuilder : IElementBuilder, IEquatable<PlainTextElementBuilder>, IEquatable<IElementBuilder>
{
    /// <summary>
    ///     纯文本的最大长度。
    /// </summary>
    public const int MaxPlainTextLength = 2000;

    /// <summary>
    ///     初始化一个 <see cref="PlainTextElementBuilder"/> 类的新实例。
    /// </summary>
    public PlainTextElementBuilder()
    {
        Content = string.Empty;
    }

    /// <summary>
    ///     初始化一个 <see cref="PlainTextElementBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="content"> 纯文本的内容。 </param>
    /// <param name="emoji"> 是否将 Emoji 表情符号的短代码解析为表情符号。 </param>
    /// <seealso cref="Kook.Emoji"/>
    public PlainTextElementBuilder(string? content, bool emoji = true)
    {
        Content = content;
        Emoji = emoji;
    }

    /// <inheritdoc />
    public ElementType Type => ElementType.PlainText;

    /// <summary>
    ///     获取或设置纯文本的文本内容。
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    ///     获取或设置 Emoji 表情符号的短代码是否应被解析为表情符号。
    /// </summary>
    /// <seealso cref="Kook.Emoji"/>
    public bool Emoji { get; set; } = true;

    /// <summary>
    ///     设置纯文本的文本内容。
    /// </summary>
    /// <param name="content"> 纯文本的文本内容。 </param>
    /// <returns> 当前构建器。 </returns>
    public PlainTextElementBuilder WithContent(string content)
    {
        Content = content;
        return this;
    }

    /// <summary>
    ///     设置 Emoji 表情符号的短代码是否应被解析为表情符号。
    /// </summary>
    /// <param name="emoji"> Emoji 表情符号的短代码是否应被解析为表情符号。 </param>
    /// <returns> 当前构建器。 </returns>
    /// <seealso cref="Kook.Emoji"/>
    public PlainTextElementBuilder WithEmoji(bool emoji)
    {
        Emoji = emoji;
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="PlainTextElement"/>。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="PlainTextElement"/> 对象。 </returns>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Content"/> 为 <c>null</c>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Content"/> 的长度超过了 <see cref="MaxPlainTextLength"/>。
    /// </exception>
    [MemberNotNull(nameof(Content))]
    public PlainTextElement Build()
    {
        if (Content == null)
            throw new ArgumentNullException(nameof(Content), $"The {nameof(Content)} cannot be null.");

        if (Content.Length > MaxPlainTextLength)
            throw new ArgumentException(
                $"Plain text length must be less than or equal to {MaxPlainTextLength}.",
                nameof(Content));

        return new PlainTextElement(Content, Emoji);
    }

    /// <summary>
    ///     使用指定的纯文本内容初始化一个新的 <see cref="PlainTextElementBuilder"/> 类的实例。
    /// </summary>
    /// <param name="content"> 纯文本内容。 </param>
    /// <returns> 一个使用指定的纯文本内容初始化的 <see cref="PlainTextElementBuilder"/> 类的实例。 </returns>
    public static implicit operator PlainTextElementBuilder(string content) => new(content);

    /// <inheritdoc />
    [MemberNotNull(nameof(Content))]
    IElement IElementBuilder.Build() => Build();

    /// <summary>
    ///     判定两个 <see cref="PlainTextElementBuilder"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="PlainTextElementBuilder"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(PlainTextElementBuilder? left, PlainTextElementBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="PlainTextElementBuilder"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="PlainTextElementBuilder"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(PlainTextElementBuilder? left, PlainTextElementBuilder? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is PlainTextElementBuilder builder && Equals(builder);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] PlainTextElementBuilder? plainTextElementBuilder)
    {
        if (plainTextElementBuilder is null) return false;

        return Type == plainTextElementBuilder.Type
            && Content == plainTextElementBuilder.Content
            && Emoji == plainTextElementBuilder.Emoji;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IElementBuilder>.Equals([NotNullWhen(true)] IElementBuilder? elementBuilder) =>
        Equals(elementBuilder as PlainTextElementBuilder);
}
