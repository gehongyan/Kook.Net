using System.Collections.Immutable;

namespace Kook;

/// <summary>
///     Represents a container module builder for creating a <see cref="ContainerModule"/>.
/// </summary>
public class ContainerModuleBuilder : IModuleBuilder, IEquatable<ContainerModuleBuilder>
{
    private List<ImageElementBuilder> _elements;

    /// <summary>
    ///     Returns the maximum number of elements allowed by Kook.
    /// </summary>
    public const int MaxElementCount = 9;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ContainerModuleBuilder"/> class.
    /// </summary>
    public ContainerModuleBuilder() => Elements = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="ContainerModuleBuilder"/> class.
    /// </summary>
    public ContainerModuleBuilder(List<ImageElementBuilder> elements) => Elements = elements;

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Container;

    /// <summary>
    ///     Gets or sets the image elements in the container module.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The number of <paramref name="value"/> is greater than <see cref="MaxElementCount"/>.
    /// </exception>
    /// <returns>
    ///     A <see cref="List{ImageElementBuilder}"/> containing the image elements in this image container module.
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
    ///     Adds an image element to the container module.
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
    public ContainerModuleBuilder AddElement(ImageElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount)
            throw new ArgumentException(
                $"Element count must be less than or equal to {MaxElementCount}.",
                nameof(field));

        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds an image element to the container module.
    /// </summary>
    /// <param name="action">
    ///     The action to add an image element to the container module.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ContainerModuleBuilder AddElement(Action<ImageElementBuilder> action)
    {
        ImageElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="ContainerModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="ContainerModule"/> representing the built container module object.
    /// </returns>
    public ContainerModule Build()
    {
        if (Elements is null or { Count: 0 })
            throw new ArgumentNullException(
                nameof(Elements),
                "Element cannot be null or empty list.");
        return new ContainerModule(Elements.Select(e => e.Build()).ToImmutableArray());
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="ContainerModuleBuilder"/> is equal to the current <see cref="ContainerModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ContainerModuleBuilder"/> is equal to the current <see cref="ContainerModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(ContainerModuleBuilder left, ContainerModuleBuilder right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="ContainerModuleBuilder"/> is not equal to the current <see cref="ContainerModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ContainerModuleBuilder"/> is not equal to the current <see cref="ContainerModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(ContainerModuleBuilder left, ContainerModuleBuilder right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="ContainerModuleBuilder"/> is equal to the current <see cref="ContainerModuleBuilder"/>.</summary>
    /// <remarks>If the object passes is an <see cref="ContainerModuleBuilder"/>, <see cref="Equals(ContainerModuleBuilder)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="ContainerModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ContainerModuleBuilder"/> is equal to the current <see cref="ContainerModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is ContainerModuleBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="ContainerModuleBuilder"/> is equal to the current <see cref="ContainerModuleBuilder"/>.</summary>
    /// <param name="containerModuleBuilder">The <see cref="ContainerModuleBuilder"/> to compare with the current <see cref="ContainerModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ContainerModuleBuilder"/> is equal to the current <see cref="ContainerModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(ContainerModuleBuilder containerModuleBuilder)
    {
        if (containerModuleBuilder is null) return false;

        if (Elements.Count != containerModuleBuilder.Elements.Count) return false;

        for (int i = 0; i < Elements.Count; i++)
            if (Elements[i] != containerModuleBuilder.Elements[i])
                return false;

        return Type == containerModuleBuilder.Type;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();
}
