namespace KaiHeiLa;

/// <summary>
///     普通文本
/// </summary>
public class PlainTextElement : IElement
{
    private string _content;

    internal PlainTextElement(string content, bool emoji = true)
    {
        Content = content;
        Emoji = emoji;
    }

    public ElementType Type => ElementType.PlainText;

    public string Content
    {
        get => _content;
        init
        {
            if (value.Length > 2000)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "最大 2000 个字");
            }
            _content = value;
        }
    }

    public bool Emoji { get; }

    public static implicit operator PlainTextElement(string content) => new(content);
}