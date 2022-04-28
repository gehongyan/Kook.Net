using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     An image element that can be used in modules.
/// </summary>
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

    /// <summary>
    ///     Gets the type of the element.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> value that represents the theme of the button.
    /// </returns>
    public ElementType Type => ElementType.Image;
    
    /// <summary>
    ///     Gets the source of the image.
    /// </summary>
    /// <returns>
    ///     A string that represents the source of the image.
    /// </returns>
    public string Source { get; internal set; }

    /// <summary>
    ///     Gets the alternative text of the image.
    /// </summary>
    /// <returns>
    ///     A string that represents the alternative text of the image.
    /// </returns>
    public string Alternative { get; internal set; }

    /// <summary>
    ///     Gets the size of the image.
    /// </summary>
    /// <returns>
    ///     An <see cref="ImageSize"/> that represents the size of the image;
    ///     or <c>null</c> if the size is not specified.
    /// </returns>
    public ImageSize? Size { get; internal set; }

    /// <summary>
    ///     Gets a value indicating whether the image should be rendered as a circle.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the image should be rendered as a circle; otherwise, <c>false</c>;
    ///     or <c>null</c> if whether the image should be rendered as a circle is not specified.
    /// </returns>
    public bool? Circle { get; internal set; }

    private string DebuggerDisplay => $"{Type}: {Source}";
}