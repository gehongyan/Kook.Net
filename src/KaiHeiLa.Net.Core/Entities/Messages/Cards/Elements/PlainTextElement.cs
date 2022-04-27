using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     A plain text element that can be used in modules.
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
    ///     A <see cref="string"/> that represents the KMarkdown content of the element.
    /// </returns>
    public string Content { get; internal set; }

    /// <summary>
    ///     //TODO: To be documented.
    /// </summary>
    public bool Emoji { get; internal set; }

    public override string ToString() => Content;
    private string DebuggerDisplay => $"{Type}: {Content}";
}