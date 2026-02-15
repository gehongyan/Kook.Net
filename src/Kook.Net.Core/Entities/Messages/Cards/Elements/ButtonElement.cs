using System.Diagnostics;

namespace Kook;

/// <summary>
///     按钮元素，可用于 <see cref="IModule"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record ButtonElement : IElement
{
    internal ButtonElement(ButtonTheme? theme, string? value, ButtonClickEventType? click, IElement text)
    {
        Theme = theme;
        Value = value;
        Click = click;
        Text = text;
    }

    /// <inheritdoc />
    public ElementType Type => ElementType.Button;

    /// <summary>
    ///     获取按钮的主题。
    /// </summary>
    public ButtonTheme? Theme { get; }

    /// <summary>
    ///     获取按钮的值。
    /// </summary>
    public string? Value { get; }

    /// <summary>
    ///     获取按钮被点击时触发的事件类型。
    /// </summary>
    public ButtonClickEventType? Click { get; }

    /// <summary>
    ///     获取按钮的文本元素。
    /// </summary>
    public IElement Text { get; }

    private string DebuggerDisplay => $"{Type}: {Text} ({Click}, {Value}, {Theme})";
}
