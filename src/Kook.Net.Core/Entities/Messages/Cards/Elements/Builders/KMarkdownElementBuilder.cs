using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     An element builder to build a <see cref="KMarkdownElement"/>.
/// </summary>
public sealed class KMarkdownElementBuilder : IElementBuilder, IEquatable<KMarkdownElementBuilder>
{
    private string? _content;

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
    public KMarkdownElementBuilder(string content)
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
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="value"/> cannot be null.
    /// </exception>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="value"/> is greater than <see cref="MaxKMarkdownLength"/>.
    /// </exception>
    /// <returns>
    ///     The content of the <see cref="KMarkdownElementBuilder"/>.
    /// </returns>
    public string? Content
    {
        get => _content;
        set
        {
            if (value is null)
                throw new ArgumentException("The content cannot be null.", nameof(value));

            if (value.Length > MaxKMarkdownLength)
                throw new ArgumentException(
                    $"KMarkdown length must be less than or equal to {MaxKMarkdownLength}.",
                    nameof(Content));

            _content = value;
        }
    }

    /// <summary>
    ///     Sets the content of a <see cref="KMarkdownElementBuilder"/>.
    /// </summary>
    /// <param name="content">The text to be set as the content.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="content"/> cannot be null.
    /// </exception>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="content"/> is greater than <see cref="MaxKMarkdownLength"/>.
    /// </exception>
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
    [MemberNotNull(nameof(Content))]
    public KMarkdownElement Build()
    {
        if (Content is null)
            throw new InvalidOperationException("The content cannot be null.");
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
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="content"/> cannot be null.
    /// </exception>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="content"/> is greater than <see cref="MaxKMarkdownLength"/>.
    /// </exception>
    public static implicit operator KMarkdownElementBuilder(string content) => new(content);

    /// <inheritdoc />
    [MemberNotNull(nameof(Content))]
    IElement IElementBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="KMarkdownElementBuilder"/> is equal to the current <see cref="KMarkdownElementBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="KMarkdownElementBuilder"/> is equal to the current <see cref="KMarkdownElementBuilder"/>; otherwise, <c>false</c>.</returns>
    public static bool operator ==(KMarkdownElementBuilder left, KMarkdownElementBuilder right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="KMarkdownElementBuilder"/> is not equal to the current <see cref="KMarkdownElementBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="KMarkdownElementBuilder"/> is not equal to the current <see cref="KMarkdownElementBuilder"/>; otherwise, <c>false</c>.</returns>
    public static bool operator !=(KMarkdownElementBuilder left, KMarkdownElementBuilder right)
        => !(left == right);

    /// <summary>
    ///     Determines whether the specified <see cref="object"/> is equal to the current <see cref="KMarkdownElementBuilder"/>.
    /// </summary>
    /// <param name="obj"> The <see cref="object"/> to compare with the current <see cref="KMarkdownElementBuilder"/>.</param>
    /// <returns> <c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="KMarkdownElementBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is KMarkdownElementBuilder builder && Equals(builder);

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
}
