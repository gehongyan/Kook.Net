using System.Diagnostics;

namespace Kook;

/// <summary>
///     KMarkdown 文本元素，可用于 <see cref="IModule"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record KMarkdownElement : IElement
{
    internal KMarkdownElement(string content)
    {
        Content = content;
    }

    /// <inheritdoc />
    public ElementType Type => ElementType.KMarkdown;

    /// <summary>
    ///     获取 KMarkdown 文本的内容。
    /// </summary>
    public string Content { get; }

    /// <inheritdoc />
    public override string ToString() => Content;

    private string DebuggerDisplay => $"{Type}: {Content}";
}
