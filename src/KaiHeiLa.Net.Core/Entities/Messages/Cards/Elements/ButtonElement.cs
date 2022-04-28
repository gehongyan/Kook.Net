using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     A button element that can be used in a module.
/// </summary>
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

    /// <summary>
    ///     Gets the theme of the button.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> value that represents the theme of the button.
    /// </returns>
    public ElementType Type => ElementType.Button;

    /// <summary>
    ///     Gets the theme of the button.
    /// </summary>
    /// <returns>
    ///     An <see cref="ButtonTheme"/> value that represents the theme of the button.
    /// </returns>
    public ButtonTheme Theme { get; internal set; }

    /// <summary>
    ///     Gets the value of the button.
    /// </summary>
    /// <returns>
    ///     A string value that represents the value of the button.
    /// </returns>
    public string Value { get; internal set; }

    /// <summary>
    ///     Gets the event type fired when the button is clicked.
    /// </summary>
    /// <returns>
    ///     A <see cref="ButtonClickEventType"/> value that represents the event type fired when the button is clicked.
    /// </returns>
    public ButtonClickEventType Click { get; internal set; }

    /// <summary>
    ///     Gets the text element of the button.
    /// </summary>
    /// <returns>
    ///     An <see cref="IElement"/> value that represents the text element of the button.
    /// </returns>
    public IElement Text { get; internal set; }
    
    private string DebuggerDisplay => $"{Type}: {Text} ({Click}, {Value}, {Theme})";
}