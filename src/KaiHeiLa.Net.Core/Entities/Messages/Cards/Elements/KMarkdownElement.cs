using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     A KMarkdown element that can be used in modules.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class KMarkdownElement : IElement
{
    internal KMarkdownElement(string content)
    {
        Content = content;
    }

    /// <summary>
    ///     Gets the type of the element.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> value that represents the theme of the button.
    /// </returns>
    public ElementType Type => ElementType.KMarkdown;

    /// <summary>
    ///     Gets the KMarkdown content of the element.
    /// </summary>
    /// <returns>
    ///     A string that represents the KMarkdown content of the element.
    /// </returns>
    public string Content { get; internal set; }

    public override string ToString() => Content;
    private string DebuggerDisplay => $"{Type}: {Content}";
}