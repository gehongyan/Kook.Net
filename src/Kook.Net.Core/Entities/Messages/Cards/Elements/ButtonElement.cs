using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     按钮元素，可用于 <see cref="IModule"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ButtonElement : IElement, IEquatable<ButtonElement>, IEquatable<IElement>
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

    /// <summary>
    ///     判定两个 <see cref="ButtonElement"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ButtonElement"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(ButtonElement? left, ButtonElement? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="ButtonElement"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ButtonElement"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(ButtonElement? left, ButtonElement? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ButtonElement buttonElement && Equals(buttonElement);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] ButtonElement? buttonElement) =>
        GetHashCode() == buttonElement?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int) 2166136261;
            hash = (hash * 16777619) ^ (Type, Theme, Value, Click).GetHashCode();
            hash = (hash * 16777619) ^ Text.GetHashCode();
            return hash;
        }
    }

    bool IEquatable<IElement>.Equals([NotNullWhen(true)] IElement? element) =>
        Equals(element as ButtonElement);
}
