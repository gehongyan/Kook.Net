using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     普通文本
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class PlainTextElement : IElement
{
    internal PlainTextElement(string content, bool emoji)
    {
        Content = content;
        Emoji = emoji;
    }

    public ElementType Type => ElementType.PlainText;

    public string Content { get; internal set; }

    public bool Emoji { get; internal set; }

    public override string ToString() => Content;
    private string DebuggerDisplay => $"{Type}: {Content}";
}