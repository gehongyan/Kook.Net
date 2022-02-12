using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     KMarkdown
/// </summary>
/// <summary>
///     显示文字
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class KMarkdownElement : IElement
{
    internal KMarkdownElement(string content)
    {
        Content = content;
    }

    public ElementType Type => ElementType.KMarkdown;

    public string Content { get; internal set; }

    public override string ToString() => Content;
    private string DebuggerDisplay => $"{Type}: {Content}";
}