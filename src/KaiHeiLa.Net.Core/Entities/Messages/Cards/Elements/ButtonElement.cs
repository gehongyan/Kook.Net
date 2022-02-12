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
    
    private string DebuggerDisplay => $"{Type}: {Text} ({Click}, {Value}, {Theme})";
}