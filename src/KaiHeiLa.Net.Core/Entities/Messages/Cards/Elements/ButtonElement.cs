namespace KaiHeiLa;

/// <summary>
///     普通文本
/// </summary>
/// <remarks>
///     显示文字
/// </remarks>
public class ButtonElement : IElement
{
    private IElement _text;

    internal ButtonElement(ButtonTheme theme, string value, ButtonClickEventType click, IElement text)
    {
        Theme = theme;
        Value = value;
        Click = click;
        Text = text;
    }

    public ElementType Type => ElementType.Button;

    public ButtonTheme Theme { get; internal set; }

    public string Value { get; internal set; }

    public ButtonClickEventType Click { get; internal set; }

    public IElement Text
    {
        get => _text;
        internal set
        {
            if (value is not (PlainTextElement or KMarkdownElement))
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                    $"{Text} 可以为 {nameof(PlainTextElement)}, {nameof(KMarkdownElement)}");
            }
            _text = value;
        }
    }
}