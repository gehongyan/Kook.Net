using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     纯文本元素，可用于 <see cref="IModule"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class PlainTextElement : IElement, IEquatable<PlainTextElement>, IEquatable<IElement>
{
    internal PlainTextElement(string content, bool? emoji)
    {
        Content = content;
        Emoji = emoji;
    }

    /// <inheritdoc />
    public ElementType Type => ElementType.PlainText;

    /// <summary>
    ///     获取纯文本的内容。
    /// </summary>
    public string Content { get; }

    /// <summary>
    ///     获取 Emoji 表情符号的短代码是否被解析为表情符号。
    /// </summary>
    public bool? Emoji { get; }

    /// <inheritdoc />
    public override string ToString() => Content;

    private string DebuggerDisplay => $"{Type}: {Content}";

    /// <summary>
    ///     判定两个 <see cref="PlainTextElement"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="PlainTextElement"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(PlainTextElement? left, PlainTextElement? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="PlainTextElement"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="PlainTextElement"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(PlainTextElement? left, PlainTextElement? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is PlainTextElement plainTextElement && Equals(plainTextElement);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] PlainTextElement? plainTextElement) =>
        GetHashCode() == plainTextElement?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode() =>
        (Type, Content, Emoji).GetHashCode();

    bool IEquatable<IElement>.Equals([NotNullWhen(true)] IElement? element) =>
        Equals(element as PlainTextElement);
}
