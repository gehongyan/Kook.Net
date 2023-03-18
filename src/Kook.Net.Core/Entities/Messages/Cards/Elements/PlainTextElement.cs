using System.Diagnostics;

namespace Kook;

/// <summary>
///     A plain text element that can be used in an <see cref="IModule"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class PlainTextElement : IElement, IEquatable<PlainTextElement>
{
    internal PlainTextElement(string content, bool emoji)
    {
        Content = content;
        Emoji = emoji;
    }

    /// <summary>
    ///     Gets the type of the element.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> value that represents the theme of the button.
    /// </returns>
    public ElementType Type => ElementType.PlainText;

    /// <summary>
    ///     Gets the KMarkdown content of the element.
    /// </summary>
    /// <returns>
    ///     A string that represents the KMarkdown content of the element.
    /// </returns>
    public string Content { get; }

    /// <summary>
    ///     Gets whether the shortcuts should be translated into emojis.
    /// </summary>
    /// <returns>
    ///     A boolean value that indicates whether the shortcuts should be translated into emojis.
    ///     <c>true</c> if the shortcuts should be translated into emojis;
    ///     <c>false</c> if the text should be displayed as is.
    /// </returns>
    public bool Emoji { get; }

    /// <inheritdoc />
    public override string ToString() => Content;
    private string DebuggerDisplay => $"{Type}: {Content}";

    /// <summary>
    ///     Determines whether the specified <see cref="PlainTextElement"/> is equal to the current <see cref="PlainTextElement"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="PlainTextElement"/> is equal to the current <see cref="PlainTextElement"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(PlainTextElement left, PlainTextElement right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="PlainTextElement"/> is not equal to the current <see cref="PlainTextElement"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="PlainTextElement"/> is not equal to the current <see cref="PlainTextElement"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(PlainTextElement left, PlainTextElement right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="PlainTextElement"/> is equal to the current <see cref="PlainTextElement"/>.</summary>
    /// <remarks>If the object passes is an <see cref="PlainTextElement"/>, <see cref="Equals(PlainTextElement)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="PlainTextElement"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="PlainTextElement"/> is equal to the current <see cref="PlainTextElement"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is PlainTextElement plainTextElement && Equals(plainTextElement);

    /// <summary>Determines whether the specified <see cref="PlainTextElement"/> is equal to the current <see cref="PlainTextElement"/>.</summary>
    /// <param name="plainTextElement">The <see cref="PlainTextElement"/> to compare with the current <see cref="PlainTextElement"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="PlainTextElement"/> is equal to the current <see cref="PlainTextElement"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(PlainTextElement plainTextElement)
        => GetHashCode() == plainTextElement?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => (Type, Content, Emoji).GetHashCode();
}
