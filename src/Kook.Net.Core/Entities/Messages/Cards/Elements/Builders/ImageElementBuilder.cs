using Kook.Utils;

namespace Kook;

/// <summary>
///     An element builder to build an <see cref="ImageElement"/>.
/// </summary>
public class ImageElementBuilder : IElementBuilder, IEquatable<ImageElementBuilder>
{
    private string _alternative;

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
    public ImageElementBuilder(string source, string alternative = null, ImageSize? size = null, bool? circle = null)
    {
        WithSource(source);
        WithAlternative(alternative);
        WithSize(size ?? ImageSize.Small);
        WithCircle(circle ?? false);
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
    public string Source { get; set; }

    /// <summary>
    ///     Gets or sets the alternative text of an <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="value"/> is greater than <see cref="MaxAlternativeLength"/>.
    /// </exception>
    /// <returns>
    ///     A string that represents the alternative text of the <see cref="ImageElementBuilder"/>.
    /// </returns>
    public string Alternative
    {
        get => _alternative;
        set
        {
            if (value?.Length > MaxAlternativeLength)
                throw new ArgumentException(
                    $"Image alternative length must be less than or equal to {MaxAlternativeLength}.",
                    nameof(Alternative));

            _alternative = value;
        }
    }

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
    public ImageElementBuilder WithSource(string source)
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
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="alternative"/> is greater than <see cref="MaxAlternativeLength"/>.
    /// </exception>
    public ImageElementBuilder WithAlternative(string alternative)
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
    public ImageElementBuilder WithSize(ImageSize size)
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
    public ImageElementBuilder WithCircle(bool circle)
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
    /// <exception cref="InvalidOperationException">
    ///     The source url does not include a protocol (either HTTP or HTTPS).
    /// </exception>
    public ImageElement Build()
    {
        if (!string.IsNullOrEmpty(Source)) UrlValidation.Validate(Source);

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
    /// <exception cref="ArgumentException" accessor="set">
    ///     The length of <paramref name="source"/> is greater than <see cref="MaxAlternativeLength"/>.
    /// </exception>
    public static implicit operator ImageElementBuilder(string source) => new ImageElementBuilder()
        .WithSource(source);

    /// <inheritdoc />
    IElement IElementBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="ImageElementBuilder"/> is equal to the current <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ImageElementBuilder"/> is equal to the current <see cref="ImageElementBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(ImageElementBuilder left, ImageElementBuilder right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="ImageElementBuilder"/> is not equal to the current <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ImageElementBuilder"/> is not equal to the current <see cref="ImageElementBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(ImageElementBuilder left, ImageElementBuilder right)
        => !(left == right);

    /// <summary>
    ///     Determines whether the specified <see cref="object"/> is equal to the current <see cref="ImageElementBuilder"/>.
    /// </summary>
    /// <param name="obj"> The <see cref="object"/> to compare with the current <see cref="ImageElementBuilder"/>. </param>
    /// <returns> <c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="ImageElementBuilder"/>; otherwise, <c>false</c>. </returns>
    public override bool Equals(object obj)
        => obj is ImageElementBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="ImageElementBuilder"/> is equal to the current <see cref="ImageElementBuilder"/>.</summary>
    /// <param name="imageElementBuilder">The <see cref="ImageElementBuilder"/> to compare with the current <see cref="ImageElementBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ImageElementBuilder"/> is equal to the current <see cref="ImageElementBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(ImageElementBuilder imageElementBuilder)
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
}
