using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     用来构建 <see cref="KMarkdownElement"/> 元素的构建器。
/// </summary>
public class KMarkdownElementBuilder : IElementBuilder, IEquatable<KMarkdownElementBuilder>, IEquatable<IElementBuilder>
{
    /// <summary>
    ///     KMarkdown 文本的最大长度。
    /// </summary>
    public const int MaxKMarkdownLength = 5000;

    /// <summary>
    ///     初始化一个 <see cref="KMarkdownElementBuilder"/> 类的新实例。
    /// </summary>
    public KMarkdownElementBuilder()
    {
    }

    /// <summary>
    ///     初始化一个 <see cref="KMarkdownElementBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="content"> KMarkdown 文本内容。 </param>
    public KMarkdownElementBuilder(string? content)
    {
        Content = content;
    }

    /// <inheritdoc />
    public ElementType Type => ElementType.KMarkdown;

    /// <summary>
    ///     获取或设置 KMarkdown 的文本内容。
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    ///     设置 KMarkdown 的文本内容，值将被设置到 <see cref="Content"/> 属性上。
    /// </summary>
    /// <param name="content"> KMarkdown 的文本内容。 </param>
    /// <returns> 当前构建器。 </returns>
    public KMarkdownElementBuilder WithContent(string content)
    {
        Content = content;
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="KMarkdownElement"/>。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="KMarkdownElement"/> 对象。 </returns>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Content"/> 为 <c>null</c>。
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Content"/> 的长度超过了 <see cref="MaxKMarkdownLength"/>。
    /// </exception>
    [MemberNotNull(nameof(Content))]
    public KMarkdownElement Build()
    {
        if (Content == null)
            throw new ArgumentNullException(nameof(Content), $"The {nameof(Content)} cannot be null.");

        if (Content.Length > MaxKMarkdownLength)
            throw new ArgumentException(
                $"KMarkdown length must be less than or equal to {MaxKMarkdownLength}.",
                nameof(Content));

        return new KMarkdownElement(Content);
    }

    /// <summary>
    ///     使用指定的 KMarkdown 文本内容初始化一个新的 <see cref="KMarkdownElementBuilder"/> 类的实例。
    /// </summary>
    /// <param name="content"> KMarkdown 文本内容。 </param>
    /// <returns> 一个使用指定的 KMarkdown 文本内容初始化的 <see cref="KMarkdownElementBuilder"/> 类的实例。 </returns>
    public static implicit operator KMarkdownElementBuilder(string content) => new(content);

    /// <inheritdoc />
    [MemberNotNull(nameof(Content))]
    IElement IElementBuilder.Build() => Build();

    /// <summary>
    ///     判定两个 <see cref="KMarkdownElementBuilder"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="KMarkdownElementBuilder"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。</returns>
    public static bool operator ==(KMarkdownElementBuilder? left, KMarkdownElementBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="KMarkdownElementBuilder"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="KMarkdownElementBuilder"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。</returns>
    public static bool operator !=(KMarkdownElementBuilder? left, KMarkdownElementBuilder? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is KMarkdownElementBuilder builder && Equals(builder);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] KMarkdownElementBuilder? kMarkdownElementBuilder)
    {
        if (kMarkdownElementBuilder is null)
            return false;
        return Type == kMarkdownElementBuilder.Type
            && Content == kMarkdownElementBuilder.Content;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IElementBuilder>.Equals([NotNullWhen(true)] IElementBuilder? elementBuilder) =>
        Equals(elementBuilder as KMarkdownElementBuilder);
}
