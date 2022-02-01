namespace KaiHeiLa;

/// <summary>
///     KMarkdown
/// </summary>
/// <summary>
///     显示文字
/// </summary>
public class KMarkdownElement : IElement
{
    private string _content;

    internal KMarkdownElement(string content)
    {
        Content = content;
    }

    public ElementType Type => ElementType.KMarkdown;

    public string Content
    {
        get => _content;
        internal set
        {
            if (_content.Length > 5000)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "最大 5000 个字");
            }
            _content = value;
        }
    }

    public static implicit operator KMarkdownElement(string content) => new(content);
}