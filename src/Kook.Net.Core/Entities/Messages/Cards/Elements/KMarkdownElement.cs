using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     A KMarkdown element that can be used in an <see cref="IModule"/>.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class KMarkdownElement : IElement, IEquatable<KMarkdownElement>, IEquatable<IElement>
{
    internal KMarkdownElement(string content)
    {
        Content = content;
    }

    /// <summary>
    ///     Gets the type of the element.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> value that represents the theme of the button.
    /// </returns>
    public ElementType Type => ElementType.KMarkdown;

    /// <summary>
    ///     Gets the KMarkdown content of the element.
    /// </summary>
    /// <returns>
    ///     A string that represents the KMarkdown content of the element.
    /// </returns>
    public string Content { get; }

    /// <inheritdoc />
    public override string ToString() => Content;

    private string DebuggerDisplay => $"{Type}: {Content}";

    /// <summary>
    ///     Determines whether the specified <see cref="KMarkdownElement"/> is equal to the current <see cref="KMarkdownElement"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="KMarkdownElement"/> is equal to the current <see cref="KMarkdownElement"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(KMarkdownElement? left, KMarkdownElement? right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="KMarkdownElement"/> is not equal to the current <see cref="KMarkdownElement"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="KMarkdownElement"/> is not equal to the current <see cref="KMarkdownElement"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(KMarkdownElement? left, KMarkdownElement? right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="KMarkdownElement"/> is equal to the current <see cref="KMarkdownElement"/>.</summary>
    /// <remarks>If the object passes is an <see cref="KMarkdownElement"/>, <see cref="Equals(KMarkdownElement)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="KMarkdownElement"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="KMarkdownElement"/> is equal to the current <see cref="KMarkdownElement"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is KMarkdownElement kMarkdownElement && Equals(kMarkdownElement);

    /// <summary>Determines whether the specified <see cref="KMarkdownElement"/> is equal to the current <see cref="KMarkdownElement"/>.</summary>
    /// <param name="kMarkdownElement">The <see cref="KMarkdownElement"/> to compare with the current <see cref="KMarkdownElement"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="KMarkdownElement"/> is equal to the current <see cref="KMarkdownElement"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)] KMarkdownElement? kMarkdownElement)
        => GetHashCode() == kMarkdownElement?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => (Type, Content).GetHashCode();

    bool IEquatable<IElement>.Equals([NotNullWhen(true)] IElement? element) =>
        Equals(element as KMarkdownElement);
}
