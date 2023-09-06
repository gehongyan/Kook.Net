using System.Collections.Immutable;

namespace Kook;

/// <summary>
///     Representing an image group module builder for create an <see cref="ImageGroupModule"/>.
/// </summary>
public class ImageGroupModuleBuilder : IModuleBuilder, IEquatable<ImageGroupModuleBuilder>
{
    private List<ImageElementBuilder> _elements;

    /// <summary>
    ///     Returns the maximum number of elements allowed by Kook.
    /// </summary>
    public const int MaxElementCount = 9;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageGroupModuleBuilder"/> class.
    /// </summary>
    public ImageGroupModuleBuilder() => Elements = new List<ImageElementBuilder>();

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImageGroupModuleBuilder"/> class.
    /// </summary>
    public ImageGroupModuleBuilder(List<ImageElementBuilder> elements) => Elements = elements;

    /// <inheritdoc />
    public ModuleType Type => ModuleType.ImageGroup;

    /// <summary>
    ///     Gets or sets the elements of the image group.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    /// <returns>
    ///     An <see cref="ImageElementBuilder"/> containing the elements of the image group.
    /// </returns>
    public List<ImageElementBuilder> Elements
    {
        get => _elements;
        set
        {
            if (value is null)
                throw new ArgumentNullException(
                    nameof(Elements),
                    "Element cannot be null.");
            if (value.Count > MaxElementCount)
                throw new ArgumentException(
                    $"Element count must be less than or equal to {MaxElementCount}.",
                    nameof(Elements));

            _elements = value;
        }
    }

    /// <summary>
    ///     Adds an image element to the image group.
    /// </summary>
    /// <param name="field">
    ///     The image element to add.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ImageGroupModuleBuilder AddElement(ImageElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount)
            throw new ArgumentException(
                $"Element count must be less than or equal to {MaxElementCount}.",
                nameof(field));

        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds an image element to the image group.
    /// </summary>
    /// <param name="action">
    ///     The action to add an image element to the image group.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ImageGroupModuleBuilder AddElement(Action<ImageElementBuilder> action)
    {
        ImageElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    }

    /// <summary>
    ///     Builds this builder into an <see cref="ImageGroupModule"/>.
    /// </summary>
    /// <returns>
    ///     An <see cref="ImageGroupModule"/> representing the built image group module object.
    /// </returns>
    public ImageGroupModule Build()
    {
        if (Elements is null or { Count: 0 })
            throw new ArgumentNullException(
                nameof(Elements),
                "Element cannot be null or empty list.");
        return new ImageGroupModule(Elements.Select(e => e.Build()).ToImmutableArray());
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="ImageGroupModuleBuilder"/> is equal to the current <see cref="ImageGroupModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ImageGroupModuleBuilder"/> is equal to the current <see cref="ImageGroupModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(ImageGroupModuleBuilder left, ImageGroupModuleBuilder right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="ImageGroupModuleBuilder"/> is not equal to the current <see cref="ImageGroupModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ImageGroupModuleBuilder"/> is not equal to the current <see cref="ImageGroupModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(ImageGroupModuleBuilder left, ImageGroupModuleBuilder right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="ImageGroupModuleBuilder"/> is equal to the current <see cref="ImageGroupModuleBuilder"/>.</summary>
    /// <remarks>If the object passes is an <see cref="ImageGroupModuleBuilder"/>, <see cref="Equals(ImageGroupModuleBuilder)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="ImageGroupModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ImageGroupModuleBuilder"/> is equal to the current <see cref="ImageGroupModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is ImageGroupModuleBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="ImageGroupModuleBuilder"/> is equal to the current <see cref="ImageGroupModuleBuilder"/>.</summary>
    /// <param name="imageGroupModuleBuilder">The <see cref="ImageGroupModuleBuilder"/> to compare with the current <see cref="ImageGroupModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ImageGroupModuleBuilder"/> is equal to the current <see cref="ImageGroupModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(ImageGroupModuleBuilder imageGroupModuleBuilder)
    {
        if (imageGroupModuleBuilder is null) return false;

        if (Elements.Count != imageGroupModuleBuilder.Elements.Count) return false;

        for (int i = 0; i < Elements.Count; i++)
            if (Elements[i] != imageGroupModuleBuilder.Elements[i])
                return false;

        return Type == imageGroupModuleBuilder.Type;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();
}
