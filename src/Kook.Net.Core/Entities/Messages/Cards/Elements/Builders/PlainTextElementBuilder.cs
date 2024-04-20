using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     An element builder to build a <see cref="PlainTextElement"/>.
/// </summary>
public class PlainTextElementBuilder : IElementBuilder, IEquatable<PlainTextElementBuilder>, IEquatable<IElementBuilder>
{
    /// <summary>
    ///     Gets the maximum plain text length allowed by Kook.
    /// </summary>
    /// <returns>
    ///     An int that represents the maximum plain text length allowed by Kook.
    /// </returns>
    public const int MaxPlainTextLength = 2000;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PlainTextElementBuilder"/> class.
    /// </summary>
    public PlainTextElementBuilder()
    {
        Content = string.Empty;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PlainTextElementBuilder"/> class.
    /// </summary>
    /// <param name="content"> The content of the <see cref="PlainTextElement"/>.</param>
    /// <param name="emoji"> A boolean value that indicates whether the shortcuts should be translated into emojis.</param>
    public PlainTextElementBuilder(string? content, bool emoji = true)
    {
        Content = content;
        Emoji = emoji;
    }

    /// <summary>
    ///     Gets the type of the element that this builder builds.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> that represents the type of element that this builder builds.
    /// </returns>
    public ElementType Type => ElementType.PlainText;

    /// <summary>
    ///     Gets or sets the content of a <see cref="PlainTextElement"/>.
    /// </summary>
    /// <returns>
    ///     The content of the <see cref="PlainTextElement"/>.
    /// </returns>
    public string? Content { get; set; }

    /// <summary>
    ///     Gets whether the shortcuts should be translated into emojis.
    /// </summary>
    /// <returns>
    ///     A boolean value that indicates whether the shortcuts should be translated into emojis.
    ///     <c>true</c> if the shortcuts should be translated into emojis;
    ///     <c>false</c> if the text should be displayed as is.
    /// </returns>
    public bool Emoji { get; set; } = true;

    /// <summary>
    ///     Sets the content of a <see cref="PlainTextElement"/>.
    /// </summary>
    /// <param name="content">The text to be set as the content.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public PlainTextElementBuilder WithContent(string content)
    {
        Content = content;
        return this;
    }

    /// <summary>
    ///     Sets whether the shortcuts should be translated into emojis.
    /// </summary>
    /// <param name="emoji">
    ///     A boolean value that indicates whether the shortcuts should be translated into emojis.
    ///     <c>true</c> if the shortcuts should be translated into emojis;
    ///     <c>false</c> if the text should be displayed as is.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public PlainTextElementBuilder WithEmoji(bool emoji)
    {
        Emoji = emoji;
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="PlainTextElementBuilder"/> into a <see cref="PlainTextElement"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="PlainTextElement"/> represents the built element object.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The length of the <see cref="Content"/> is greater than <see cref="MaxPlainTextLength"/>.
    /// </exception>
    public PlainTextElement Build()
    {
        if (Content?.Length > MaxPlainTextLength)
            throw new ArgumentException(
                $"Plain text length must be less than or equal to {MaxPlainTextLength}.",
                nameof(Content));

        return new PlainTextElement(Content, Emoji);
    }

    /// <summary>
    ///     Initialized a new instance of the <see cref="PlainTextElementBuilder"/> class
    ///     with the specified content.
    /// </summary>
    /// <param name="content">
    ///     The content of the <see cref="PlainTextElement"/>.
    /// </param>
    /// <returns>
    ///     A <see cref="PlainTextElementBuilder"/> object that is initialized with the specified content.
    /// </returns>
    public static implicit operator PlainTextElementBuilder(string content) => new(content);

    /// <inheritdoc />
    IElement IElementBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="PlainTextElementBuilder"/> is equal to the current <see cref="PlainTextElementBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="PlainTextElementBuilder"/> is equal to the current <see cref="PlainTextElementBuilder"/>; otherwise, <c>false</c>.</returns>
    public static bool operator ==(PlainTextElementBuilder? left, PlainTextElementBuilder? right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="PlainTextElementBuilder"/> is not equal to the current <see cref="PlainTextElementBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="PlainTextElementBuilder"/> is not equal to the current <see cref="PlainTextElementBuilder"/>; otherwise, <c>false</c>.</returns>
    public static bool operator !=(PlainTextElementBuilder? left, PlainTextElementBuilder? right)
        => !(left == right);

    /// <summary>
    ///     Determines whether the specified <see cref="object"/> is equal to the current <see cref="PlainTextElementBuilder"/>.
    /// </summary>
    /// <param name="obj"> The <see cref="object"/> to compare with the current <see cref="PlainTextElementBuilder"/>.</param>
    /// <returns></returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is PlainTextElementBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="PlainTextElementBuilder"/> is equal to the current <see cref="PlainTextElementBuilder"/>.</summary>
    /// <param name="plainTextElementBuilder">The <see cref="PlainTextElementBuilder"/> to compare with the current <see cref="PlainTextElementBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="PlainTextElementBuilder"/> is equal to the current <see cref="PlainTextElementBuilder"/>; otherwise, <c>false</c>.</returns>
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
