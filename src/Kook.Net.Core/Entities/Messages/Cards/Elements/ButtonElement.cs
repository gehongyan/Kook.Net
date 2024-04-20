using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     A button element that can be used in an <see cref="IModule"/>.
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
    ///     A <see cref="ButtonTheme"/> value that represents the theme of the button.
    /// </returns>
    public ButtonTheme? Theme { get; }

    /// <summary>
    ///     Gets the value of the button.
    /// </summary>
    /// <returns>
    ///     A string value that represents the value of the button.
    /// </returns>
    public string? Value { get; }

    /// <summary>
    ///     Gets the event type fired when the button is clicked.
    /// </summary>
    /// <returns>
    ///     A <see cref="ButtonClickEventType"/> value that represents the event type fired when the button is clicked.
    /// </returns>
    public ButtonClickEventType? Click { get; }

    /// <summary>
    ///     Gets the text element of the button.
    /// </summary>
    /// <returns>
    ///     An <see cref="IElement"/> value that represents the text element of the button.
    /// </returns>
    public IElement Text { get; }

    private string DebuggerDisplay => $"{Type}: {Text} ({Click}, {Value}, {Theme})";

    /// <summary>
    ///     Determines whether the specified <see cref="ButtonElement"/> is equal to the current <see cref="ButtonElement"/>.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="ButtonElement"/> is equal to the current <see cref="ButtonElement"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(ButtonElement? left, ButtonElement? right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="ButtonElement"/> is not equal to the current <see cref="ButtonElement"/>.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="ButtonElement"/> is not equal to the current <see cref="ButtonElement"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(ButtonElement? left, ButtonElement? right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="ButtonElement"/> is equal to the current <see cref="ButtonElement"/>.</summary>
    /// <remarks>If the object passes is an <see cref="ButtonElement"/>, <see cref="Equals(ButtonElement)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="ButtonElement"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ButtonElement"/> is equal to the current <see cref="ButtonElement"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is ButtonElement buttonElement && Equals(buttonElement);

    /// <summary>Determines whether the specified <see cref="ButtonElement"/> is equal to the current <see cref="ButtonElement"/>.</summary>
    /// <param name="buttonElement">The <see cref="ButtonElement"/> to compare with the current <see cref="ButtonElement"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ButtonElement"/> is equal to the current <see cref="ButtonElement"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)] ButtonElement? buttonElement)
        => GetHashCode() == buttonElement?.GetHashCode();

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
