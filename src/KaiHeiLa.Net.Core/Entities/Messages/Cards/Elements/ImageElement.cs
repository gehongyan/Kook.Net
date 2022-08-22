using System.Diagnostics;

namespace KaiHeiLa;

/// <summary>
///     An image element that can be used in an <see cref="IModule"/>.
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
    public string Source { get; }

    /// <summary>
    ///     Gets the alternative text of the image.
    /// </summary>
    /// <returns>
    ///     A string that represents the alternative text of the image.
    /// </returns>
    public string Alternative { get; }

    /// <summary>
    ///     Gets the size of the image.
    /// </summary>
    /// <returns>
    ///     An <see cref="ImageSize"/> that represents the size of the image;
    ///     or <c>null</c> if the size is not specified.
    /// </returns>
    public ImageSize? Size { get; }

    /// <summary>
    ///     Gets a value indicating whether the image should be rendered as a circle.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the image should be rendered as a circle; otherwise, <c>false</c>;
    ///     or <c>null</c> if whether the image should be rendered as a circle is not specified.
    /// </returns>
    public bool? Circle { get; }

    private string DebuggerDisplay => $"{Type}: {Source}";

    public static bool operator ==(ImageElement left, ImageElement right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(ImageElement left, ImageElement right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="ImageElement"/> is equal to the current <see cref="ImageElement"/>.</summary>
    /// <remarks>If the object passes is an <see cref="ImageElement"/>, <see cref="Equals(ImageElement)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="ImageElement"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ImageElement"/> is equal to the current <see cref="ImageElement"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is ImageElement imageElement && Equals(imageElement);

    /// <summary>Determines whether the specified <see cref="ImageElement"/> is equal to the current <see cref="ImageElement"/>.</summary>
    /// <param name="imageElement">The <see cref="ImageElement"/> to compare with the current <see cref="ImageElement"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ImageElement"/> is equal to the current <see cref="ImageElement"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(ImageElement imageElement)
        => GetHashCode() == imageElement?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => (Type, Source, Alternative, Size, Circle).GetHashCode();
}