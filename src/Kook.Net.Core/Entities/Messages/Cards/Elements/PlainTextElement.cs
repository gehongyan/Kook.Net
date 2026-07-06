using System.Diagnostics;

namespace Kook;

/// <summary>
///     纯文本元素，可用于 <see cref="IModule"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record PlainTextElement : IElement
{
    internal PlainTextElement(string content, bool? emoji)
    {
        Content = content;
        Emoji = emoji;
    }

    /// <inheritdoc />
    public ElementType Type => ElementType.PlainText;

    /// <summary>
    ///     获取纯文本的内容。
    /// </summary>
    public string Content { get; }

    /// <summary>
    ///     获取 Emoji 表情符号的短代码是否被解析为表情符号。
    /// </summary>
    public bool? Emoji { get; }

    /// <inheritdoc />
    public override string ToString() => Content;

    private string DebuggerDisplay => $"{Type}: {Content}";
}
