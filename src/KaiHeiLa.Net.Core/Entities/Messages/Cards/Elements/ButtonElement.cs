using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     普通文本
/// </summary>
/// <remarks>
///     显示文字
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ButtonElement : IElement
{
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

    public IElement Text { get; internal set; }
    // {
    //     get => _text;
    //     internal set
    //     {
    //         if (value is not (PlainTextElement or KMarkdownElement))
    //         {
    //             throw new ArgumentOutOfRangeException(nameof(value),
    //                 $"{Text} 可以为 {nameof(PlainTextElement)}, {nameof(KMarkdownElement)}");
    //         }
    //         _text = value;
    //     }
    // }
    
    private string DebuggerDisplay => $"{Type}: {Text} ({Click}, {Value}, {Theme})";
}