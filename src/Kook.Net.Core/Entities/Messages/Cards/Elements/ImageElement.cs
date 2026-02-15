using System.Diagnostics;

namespace Kook;

/// <summary>
///     图片元素，可用于 <see cref="IModule"/> 中。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record ImageElement : IElement
{
    internal ImageElement(string source, string? alternative = null,
        ImageSize? size = null, bool? circle = null, string? fallbackUrl = null)
    {
        Source = source;
        Alternative = alternative;
        Size = size;
        Circle = circle;
        FallbackUrl = fallbackUrl;
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

    /// <summary>
    ///     获取图像的备用 URL。
    /// </summary>
    /// <remarks>
    ///     当位于站外的图片无法成功转存时，KOOK 将使用此备用图片地址作为图片的源。
    /// </remarks>
    public string? FallbackUrl { get; }

    private string DebuggerDisplay => $"{Type}: {Source}";
}
