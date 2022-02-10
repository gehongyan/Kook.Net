using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     普通文本
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class PlainTextElement : IElement
{
    internal PlainTextElement(string content, bool emoji = true)
    {
        Content = content;
        Emoji = emoji;
    }

    public ElementType Type => ElementType.PlainText;

    public string Content { get; internal set; }
    // {
    //     get => _content;
    //     internal set
    //     {
    //         if (value.Length > 2000)
    //         {
    //             throw new ArgumentOutOfRangeException(nameof(value), "最大 2000 个字");
    //         }
    //         _content = value;
    //     }
    // }

    public bool Emoji { get; internal set; }

    public static implicit operator PlainTextElement(string content) => new(content);
    public override string ToString() => Content;
    private string DebuggerDisplay => $"{Type}: {Content}";
}