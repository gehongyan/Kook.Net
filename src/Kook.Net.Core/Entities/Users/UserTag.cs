using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     表示一个用户的标签。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class UserTag : IEquatable<UserTag>
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

    #region IEquatable

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] UserTag? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Text == other.Text;
    }

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;

        return Equals((UserTag)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode() => Text != null ? Text.GetHashCode() : 0;

    #endregion
}
