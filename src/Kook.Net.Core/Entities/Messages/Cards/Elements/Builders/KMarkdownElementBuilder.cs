using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     An element builder to build a <see cref="KMarkdownElement"/>.
/// </summary>
public class KMarkdownElementBuilder : IElementBuilder, IEquatable<KMarkdownElementBuilder>, IEquatable<IElementBuilder>
{
    /// <summary>
    ///     Gets the maximum KMarkdown length allowed by Kook.
    /// </summary>
    public const int MaxKMarkdownLength = 5000;

    /// <summary>
    ///     Initializes a new instance of the <see cref="KMarkdownElementBuilder"/> class.
    /// </summary>
    public KMarkdownElementBuilder()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="KMarkdownElementBuilder"/> class.
    /// </summary>
    /// <param name="content"></param>
    public KMarkdownElementBuilder(string? content)
    {
        Content = content;
    }

    /// <summary>
    ///     Gets the type of the element that this builder builds.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> that represents the type of element that this builder builds.
    /// </returns>
    public ElementType Type => ElementType.KMarkdown;

    /// <summary>
    ///     Gets or sets the content of a <see cref="KMarkdownElementBuilder"/>.
    /// </summary>
    /// <returns>
    ///     The content of the <see cref="KMarkdownElementBuilder"/>.
    /// </returns>
    public string? Content { get; set; }

    /// <summary>
    ///     Sets the content of a <see cref="KMarkdownElementBuilder"/>.
    /// </summary>
    /// <param name="content">The text to be set as the content.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public KMarkdownElementBuilder WithContent(string content)
    {
        Content = content;
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="KMarkdownElementBuilder"/> into a <see cref="KMarkdownElement"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="KMarkdownElement"/> represents the built element object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     The <see cref="Content"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     The length of <see cref="Content"/> is greater than <see cref="MaxKMarkdownLength"/>.
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
    ///     Initialized a new instance of the <see cref="KMarkdownElementBuilder"/> class
    ///     with the specified content.
    /// </summary>
    /// <param name="content">
    ///     The content of the <see cref="KMarkdownElement"/>.
    /// </param>
    /// <returns>
    ///     A <see cref="KMarkdownElementBuilder"/> object that is initialized with the specified content.
    /// </returns>
    public static implicit operator KMarkdownElementBuilder(string content) => new(content);

    /// <inheritdoc />
    [MemberNotNull(nameof(Content))]
    IElement IElementBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="KMarkdownElementBuilder"/> is equal to the current <see cref="KMarkdownElementBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="KMarkdownElementBuilder"/> is equal to the current <see cref="KMarkdownElementBuilder"/>; otherwise, <c>false</c>.</returns>
    public static bool operator ==(KMarkdownElementBuilder? left, KMarkdownElementBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="KMarkdownElementBuilder"/> is not equal to the current <see cref="KMarkdownElementBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="KMarkdownElementBuilder"/> is not equal to the current <see cref="KMarkdownElementBuilder"/>; otherwise, <c>false</c>.</returns>
    public static bool operator !=(KMarkdownElementBuilder? left, KMarkdownElementBuilder? right) =>
        !(left == right);

    /// <summary>
    ///     Determines whether the specified <see cref="object"/> is equal to the current <see cref="KMarkdownElementBuilder"/>.
    /// </summary>
    /// <param name="obj"> The <see cref="object"/> to compare with the current <see cref="KMarkdownElementBuilder"/>.</param>
    /// <returns> <c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="KMarkdownElementBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is KMarkdownElementBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="KMarkdownElementBuilder"/> is equal to the current <see cref="KMarkdownElementBuilder"/>.</summary>
    /// <param name="kMarkdownElementBuilder">The <see cref="KMarkdownElementBuilder"/> to compare with the current <see cref="KMarkdownElementBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="KMarkdownElementBuilder"/> is equal to the current <see cref="KMarkdownElementBuilder"/>; otherwise, <c>false</c>.</returns>
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
