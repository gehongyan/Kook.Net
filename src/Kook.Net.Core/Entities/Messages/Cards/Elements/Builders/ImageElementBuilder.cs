using System.Diagnostics.CodeAnalysis;
using Kook.Utils;

namespace Kook;

/// <summary>
///     An element builder to build an <see cref="ImageElement"/>.
/// </summary>
public class ImageElementBuilder : IElementBuilder, IEquatable<ImageElementBuilder>, IEquatable<IElementBuilder>
{
    /// <summary>
    ///     Gets the maximum image alternative text length allowed by Kook.
    /// </summary>
    public const int MaxAlternativeLength = 20;

    /// <summary>
    ///     Initialized a new instance of the <see cref="ImageElementBuilder"/> class.
    /// </summary>
    public ImageElementBuilder()
    {
    }

    /// <summary>
    ///     Initialized a new instance of the <see cref="ImageElementBuilder"/> class.
    /// </summary>
    /// <param name="source"> The source of the image. </param>
    /// <param name="alternative"> The alternative text of the image. </param>
    /// <param name="size"> The size of the image. </param>
    /// <param name="circle"> Whether the image should be rendered as a circle. </param>
    public ImageElementBuilder(string source, string? alternative = null, ImageSize size = ImageSize.Small,
        bool circle = false)
    {
        Source = source;
        Alternative = alternative;
        Size = size;
        Circle = circle;
    }

    /// <summary>
    ///     Gets the type of the element that this builder builds.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> that represents the type of element that this builder builds.
    /// </returns>
    public ElementType Type => ElementType.Image;

    /// <summary>
    ///     Gets or sets the source of an <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <returns>
    ///     A string that represents the source of the <see cref="ImageElementBuilder"/>.
    /// </returns>
    public string? Source { get; set; }

    /// <summary>
    ///     Gets or sets the alternative text of an <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <returns>
    ///     A string that represents the alternative text of the <see cref="ImageElementBuilder"/>.
    /// </returns>
    public string? Alternative { get; set; }

    /// <summary>
    ///     Gets or sets the size of the image of an <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <returns>
    ///     An <see cref="ImageSize"/> that represents the size of the image of the <see cref="ImageElementBuilder"/>;
    ///     <c>null</c> if the size is not specified.
    /// </returns>
    public ImageSize? Size { get; set; }

    /// <summary>
    ///     Gets or sets whether the image should be rendered as a circle.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the image should be rendered as a circle; otherwise, <c>false</c>;
    ///     or <c>null</c> if whether the image should be rendered as a circle is not specified.
    /// </returns>
    public bool? Circle { get; set; }

    /// <summary>
    ///     Sets the source of an <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <param name="source">
    ///     The source to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ImageElementBuilder WithSource(string? source)
    {
        Source = source;
        return this;
    }

    /// <summary>
    ///     Sets the alternative text of an <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <param name="alternative">
    ///     The alternative text to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ImageElementBuilder WithAlternative(string? alternative)
    {
        Alternative = alternative;
        return this;
    }

    /// <summary>
    ///     Sets the size of the image of an <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <param name="size">
    ///     The size to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ImageElementBuilder WithSize(ImageSize? size)
    {
        Size = size;
        return this;
    }

    /// <summary>
    ///     Sets whether the image should be rendered as a circle.
    /// </summary>
    /// <param name="circle">
    ///     <c>true</c> if the image should be rendered as a circle; otherwise, <c>false</c>.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ImageElementBuilder WithCircle(bool? circle)
    {
        Circle = circle;
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="ImageElementBuilder"/> into an <see cref="ImageElement"/>.
    /// </summary>
    /// <returns>
    ///     An <see cref="ImageElement"/> represents the built element object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     The <see cref="Source"/> url is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     The <see cref="Source"/> url is empty.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     The <see cref="Source"/> url does not include a protocol (either HTTP or HTTPS).
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     The length of <see cref="Alternative"/> is greater than <see cref="MaxAlternativeLength"/>.
    /// </exception>
    [MemberNotNull(nameof(Source))]
    public ImageElement Build()
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source), "The source url cannot be null or empty.");
        if (string.IsNullOrEmpty(Source))
            throw new ArgumentException("The source url cannot be null or empty.", nameof(Source));

        UrlValidation.Validate(Source);

        if (Alternative?.Length > MaxAlternativeLength)
        {
            throw new ArgumentException(
                $"Image alternative length must be less than or equal to {MaxAlternativeLength}.",
                nameof(Alternative));
        }

        return new ImageElement(Source, Alternative, Size, Circle);
    }

    /// <summary>
    ///     Initialized a new instance of the <see cref="ImageElementBuilder"/> class
    ///     with the specified content.
    /// </summary>
    /// <param name="source">
    ///     The content of the <see cref="ImageElement"/>.
    /// </param>
    /// <returns>
    ///     An <see cref="ImageElementBuilder"/> object that is initialized with the specified image source.
    /// </returns>
    public static implicit operator ImageElementBuilder(string source) => new(source);

    /// <inheritdoc />
    [MemberNotNull(nameof(Source))]
    IElement IElementBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="ImageElementBuilder"/> is equal to the current <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ImageElementBuilder"/> is equal to the current <see cref="ImageElementBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(ImageElementBuilder? left, ImageElementBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="ImageElementBuilder"/> is not equal to the current <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ImageElementBuilder"/> is not equal to the current <see cref="ImageElementBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(ImageElementBuilder? left, ImageElementBuilder? right) =>
        !(left == right);

    /// <summary>
    ///     Determines whether the specified <see cref="object"/> is equal to the current <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <param name="obj"> The <see cref="object"/> to compare with the current <see cref="ImageElementBuilder"/>. </param>
    /// <returns> <c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="ImageElementBuilder"/>; otherwise, <c>false</c>. </returns>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ImageElementBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="ImageElementBuilder"/> is equal to the current <see cref="ImageElementBuilder"/>.</summary>
    /// <param name="imageElementBuilder">The <see cref="ImageElementBuilder"/> to compare with the current <see cref="ImageElementBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ImageElementBuilder"/> is equal to the current <see cref="ImageElementBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)] ImageElementBuilder? imageElementBuilder)
    {
        if (imageElementBuilder is null) return false;

        return Type == imageElementBuilder.Type
            && Source == imageElementBuilder.Source
            && Alternative == imageElementBuilder.Alternative
            && Size == imageElementBuilder.Size
            && Circle == imageElementBuilder.Circle;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IElementBuilder>.Equals([NotNullWhen(true)] IElementBuilder? elementBuilder) =>
        Equals(elementBuilder as ImageElementBuilder);
}
