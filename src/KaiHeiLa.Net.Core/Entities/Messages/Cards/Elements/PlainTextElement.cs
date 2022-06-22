using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     A plain text element that can be used in an <see cref="IModule"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class PlainTextElement : IElement
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
    public string Content { get; internal set; }

    /// <summary>
    ///     Gets whether the shortcuts should be translated into emojis.
    /// </summary>
    /// <returns>
    ///     A boolean value that indicates whether the shortcuts should be translated into emojis.
    ///     <c>true</c> if the shortcuts should be translated into emojis;
    ///     <c>false</c> if the text should be displayed as is.
    /// </returns>
    public bool Emoji { get; internal set; }

    public override string ToString() => Content;
    private string DebuggerDisplay => $"{Type}: {Content}";
}