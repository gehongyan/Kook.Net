using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     Represents a container module builder for creating a <see cref="ContainerModule"/>.
/// </summary>
public class ContainerModuleBuilder : IModuleBuilder, IEquatable<ContainerModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <summary>
    ///     Returns the maximum number of elements allowed by Kook.
    /// </summary>
    public const int MaxElementCount = 9;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ContainerModuleBuilder"/> class.
    /// </summary>
    public ContainerModuleBuilder()
    {
        Elements = [];
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ContainerModuleBuilder"/> class.
    /// </summary>
    public ContainerModuleBuilder(IList<ImageElementBuilder> elements)
    {
        Elements = elements;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Container;

    /// <summary>
    ///     Gets or sets the image elements in the container module.
    /// </summary>
    /// <returns>
    ///     An <see cref="IList{ImageElementBuilder}"/> containing the image elements in this image container module.
    /// </returns>
    public IList<ImageElementBuilder> Elements { get; set; }

    /// <summary>
    ///     Adds an image element to the container module.
    /// </summary>
    /// <param name="field">
    ///     The image element to add.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ContainerModuleBuilder AddElement(ImageElementBuilder field)
    {
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
    public ContainerModuleBuilder AddElement(Action<ImageElementBuilder>? action = null)
    {
        ImageElementBuilder field = new();
        action?.Invoke(field);
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="ContainerModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="ContainerModule"/> representing the built container module object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Elements"/> cannot be null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Elements"/> cannot be an empty list.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Elements"/> count must be less than or equal to <see cref="MaxElementCount"/>.
    /// </exception>
    public ContainerModule Build()
    {
        if (Elements is null)
            throw new ArgumentNullException(nameof(Elements), "Element cannot be null.");

        if (Elements.Count == 0)
            throw new ArgumentException("Element cannot be an empty list.", nameof(Elements));

        if (Elements.Count > MaxElementCount)
            throw new ArgumentException(
                $"Element count must be less than or equal to {MaxElementCount}.",
                nameof(Elements));

        return new ContainerModule([..Elements.Select(e => e.Build())]);
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="ContainerModuleBuilder"/> is equal to the current <see cref="ContainerModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ContainerModuleBuilder"/> is equal to the current <see cref="ContainerModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(ContainerModuleBuilder? left, ContainerModuleBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="ContainerModuleBuilder"/> is not equal to the current <see cref="ContainerModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ContainerModuleBuilder"/> is not equal to the current <see cref="ContainerModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(ContainerModuleBuilder? left, ContainerModuleBuilder? right) =>
        !(left == right);

    /// <summary>Determines whether the specified <see cref="ContainerModuleBuilder"/> is equal to the current <see cref="ContainerModuleBuilder"/>.</summary>
    /// <remarks>If the object passes is an <see cref="ContainerModuleBuilder"/>, <see cref="Equals(ContainerModuleBuilder)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="ContainerModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ContainerModuleBuilder"/> is equal to the current <see cref="ContainerModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ContainerModuleBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="ContainerModuleBuilder"/> is equal to the current <see cref="ContainerModuleBuilder"/>.</summary>
    /// <param name="containerModuleBuilder">The <see cref="ContainerModuleBuilder"/> to compare with the current <see cref="ContainerModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ContainerModuleBuilder"/> is equal to the current <see cref="ContainerModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)] ContainerModuleBuilder? containerModuleBuilder)
    {
        if (containerModuleBuilder is null)
            return false;

        if (Elements.Count != containerModuleBuilder.Elements.Count)
            return false;

        if (Elements
            .Zip(containerModuleBuilder.Elements, (x, y) => (x, y))
            .Any(pair => pair.x != pair.y))
            return false;

        return Type == containerModuleBuilder.Type;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IModuleBuilder>.Equals([NotNullWhen(true)] IModuleBuilder? moduleBuilder) =>
        Equals(moduleBuilder as ContainerModuleBuilder);
}
