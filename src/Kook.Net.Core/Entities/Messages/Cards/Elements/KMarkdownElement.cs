using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     KMarkdown 文本元素，可用于 <see cref="IModule"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class KMarkdownElement : IElement, IEquatable<KMarkdownElement>, IEquatable<IElement>
{
    internal KMarkdownElement(string content)
    {
        Content = content;
    }

    /// <inheritdoc />
    public ElementType Type => ElementType.KMarkdown;

    /// <summary>
    ///     获取 KMarkdown 文本的内容。
    /// </summary>
    public string Content { get; }

    /// <inheritdoc />
    public override string ToString() => Content;

    private string DebuggerDisplay => $"{Type}: {Content}";

    /// <summary>
    ///     判定两个 <see cref="KMarkdownElement"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="KMarkdownElement"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(KMarkdownElement? left, KMarkdownElement? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="KMarkdownElement"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="KMarkdownElement"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(KMarkdownElement? left, KMarkdownElement? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is KMarkdownElement kMarkdownElement && Equals(kMarkdownElement);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] KMarkdownElement? kMarkdownElement) =>
        GetHashCode() == kMarkdownElement?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode() =>
        (Type, Content).GetHashCode();

    bool IEquatable<IElement>.Equals([NotNullWhen(true)] IElement? element) =>
        Equals(element as KMarkdownElement);
}
