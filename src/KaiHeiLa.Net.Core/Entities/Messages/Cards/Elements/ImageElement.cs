using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     图片
/// </summary>
/// <remarks>
///     显示图片
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class ImageElement : IElement
{
    internal ImageElement(string source, string alternative = null, ImageSize? size = null, bool? circle = null)
    {
        Source = source;
        Alternative = alternative;
        Size = size;
        Circle = circle;
    }

    public ElementType Type => ElementType.Image;

    /// <summary>
    ///     图片类型（MimeType）限制说明：目前仅支持 image/jpeg, image/gif, image/png 这 3 种
    /// </summary>
    public string Source { get; internal set; }

    public string Alternative { get; internal set; }

    public ImageSize? Size { get; internal set; }

    public bool? Circle { get; internal set; }

    public static implicit operator ImageElement(string source) => new(source);
    
    private string DebuggerDisplay => $"{Type}: {Source}";
}