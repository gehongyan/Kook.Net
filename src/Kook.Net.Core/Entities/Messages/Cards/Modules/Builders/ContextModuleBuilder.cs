using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     Represents a context module builder for creating a <see cref="ContextModule"/>.
/// </summary>
public class ContextModuleBuilder : IModuleBuilder, IEquatable<ContextModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <summary>
    ///     Returns the maximum number of elements allowed by Kook.
    /// </summary>
    public const int MaxElementCount = 10;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ContextModuleBuilder"/> class.
    /// </summary>
    public ContextModuleBuilder()
    {
        Elements = [];
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ContextModuleBuilder"/> class.
    /// </summary>
    public ContextModuleBuilder(IList<IElementBuilder> elements)
    {
        Elements = elements;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Context;

    /// <summary>
    ///     Gets or sets the elements of the context module.
    /// </summary>
    public IList<IElementBuilder> Elements { get; set; }

    /// <summary>
    ///     Adds a PlainText element to the context module.
    /// </summary>
    /// <param name="field">
    ///     The PlainText element to add.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ContextModuleBuilder AddElement(PlainTextElementBuilder field)
    {
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds a KMarkdown element to the context module.
    /// </summary>
    /// <param name="field">
    ///     The KMarkdown element to add.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ContextModuleBuilder AddElement(KMarkdownElementBuilder field)
    {
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds an image element to the context module.
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
    public ContextModuleBuilder AddElement(ImageElementBuilder field)
    {
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds an element to the context module.
    /// </summary>
    /// <param name="action">
    ///     The action to add an element to the context module.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ContextModuleBuilder AddElement<T>(Action<T>? action = null)
        where T : IElementBuilder, new()
    {
        T field = new();
        action?.Invoke(field);
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="ContextModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="ContextModule"/> representing the built context module object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     The <see cref="Elements"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     The <see cref="Elements"/> count is greater than <see cref="MaxElementCount"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     The <see cref="Elements"/> contain an element that is not a <see cref="PlainTextElementBuilder"/>,
    ///     <see cref="KMarkdownElementBuilder"/>, or <see cref="ImageElementBuilder"/>.
    /// </exception>
    public ContextModule Build()
    {
        if (Elements is null)
            throw new ArgumentNullException(
                nameof(Elements),
                "Element cannot be null.");

        if (Elements.Count > MaxElementCount)
            throw new ArgumentException(
                $"Element count must be less than or equal to {MaxElementCount}.",
                nameof(Elements));

        if (Elements.Any(e => e is not (PlainTextElementBuilder or KMarkdownElementBuilder or ImageElementBuilder)))
            throw new ArgumentException(
                "Elements must be of type PlainText, KMarkdown or Image.",
                nameof(Elements));

        return new ContextModule([..Elements.Select(e => e.Build())]);
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="ContextModuleBuilder"/> is equal to the current <see cref="ContextModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ContextModuleBuilder"/> is equal to the current <see cref="ContextModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(ContextModuleBuilder? left, ContextModuleBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="ContextModuleBuilder"/> is not equal to the current <see cref="ContextModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ContextModuleBuilder"/> is not equal to the current <see cref="ContextModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(ContextModuleBuilder? left, ContextModuleBuilder? right) =>
        !(left == right);

    /// <summary>Determines whether the specified <see cref="ContextModuleBuilder"/> is equal to the current <see cref="ContextModuleBuilder"/>.</summary>
    /// <remarks>If the object passes is an <see cref="ContextModuleBuilder"/>, <see cref="Equals(ContextModuleBuilder)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="ContextModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ContextModuleBuilder"/> is equal to the current <see cref="ContextModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ContextModuleBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="ContextModuleBuilder"/> is equal to the current <see cref="ContextModuleBuilder"/>.</summary>
    /// <param name="contextModuleBuilder">The <see cref="ContextModuleBuilder"/> to compare with the current <see cref="ContextModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ContextModuleBuilder"/> is equal to the current <see cref="ContextModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)] ContextModuleBuilder? contextModuleBuilder)
    {
        if (contextModuleBuilder is null)
            return false;

        if (Elements.Count != contextModuleBuilder.Elements.Count)
            return false;

        if (Elements
            .Zip(contextModuleBuilder.Elements, (x, y) => (x, y))
            .Any(pair => pair.x != pair.y))
            return false;

        return Type == contextModuleBuilder.Type;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IModuleBuilder>.Equals([NotNullWhen(true)] IModuleBuilder? moduleBuilder) =>
        Equals(moduleBuilder as ContextModuleBuilder);
}
