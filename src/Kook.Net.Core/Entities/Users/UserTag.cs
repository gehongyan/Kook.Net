using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     表示一个用户的标签。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record UserTag
{
    /// <summary>
    ///     获取此用户标签的颜色。
    /// </summary>
    public Color Color { get; }

    /// <summary>
    ///     获取此用户标签的背景色。
    /// </summary>
    public AlphaColor BackgroundColor { get; }

    /// <summary>
    ///     获取此用户标签的文本。
    /// </summary>
    public string Text { get; }

    private UserTag(Color color, AlphaColor backgroundColor, string text)
    {
        Color = color;
        BackgroundColor = backgroundColor;
        Text = text;
    }

    internal static UserTag Create(Color color, AlphaColor backgroundColor, string text)
    {
        return new UserTag(color, backgroundColor, text);
    }

    private string DebuggerDisplay => Text;
}
