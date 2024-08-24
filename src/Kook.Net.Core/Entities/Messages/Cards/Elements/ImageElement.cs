using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     图片元素，可用于 <see cref="IModule"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ImageElement : IElement, IEquatable<ImageElement>, IEquatable<IElement>
{
    internal ImageElement(string source, string? alternative = null, ImageSize? size = null, bool? circle = null)
    {
        Source = source;
        Alternative = alternative;
        Size = size;
        Circle = circle;
    }

    /// <inheritdoc />
    public ElementType Type => ElementType.Image;

    /// <summary>
    ///     获取图像的源。
    /// </summary>
    public string Source { get; }

    /// <summary>
    ///     获取图像的替代文本。
    /// </summary>
    public string? Alternative { get; }

    /// <summary>
    ///     获取图像的大小。
    /// </summary>
    public ImageSize? Size { get; }

    /// <summary>
    ///     获取图片是否渲染为圆形。
    /// </summary>
    public bool? Circle { get; }

    private string DebuggerDisplay => $"{Type}: {Source}";

    /// <summary>
    ///     判定两个 <see cref="ImageElement"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ImageElement"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(ImageElement? left, ImageElement? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="ImageElement"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="ImageElement"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(ImageElement? left, ImageElement? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ImageElement imageElement && Equals(imageElement);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] ImageElement? imageElement) =>
        GetHashCode() == imageElement?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode() =>
        (Type, Source, Alternative, Size, Circle).GetHashCode();

    bool IEquatable<IElement>.Equals([NotNullWhen(true)] IElement? element) =>
        Equals(element as ImageElement);
}
