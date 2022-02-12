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
    
    public string Source { get; internal set; }

    public string Alternative { get; internal set; }

    public ImageSize? Size { get; internal set; }

    public bool? Circle { get; internal set; }

    private string DebuggerDisplay => $"{Type}: {Source}";
}