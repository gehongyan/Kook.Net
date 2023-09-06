using System.Collections.Immutable;

namespace Kook;

/// <summary>
///     Represents a context module builder for creating a <see cref="ContextModule"/>.
/// </summary>
public class ContextModuleBuilder : IModuleBuilder, IEquatable<ContextModuleBuilder>
{
    private List<IElementBuilder> _elements;

    /// <summary>
    ///     Returns the maximum number of elements allowed by Kook.
    /// </summary>
    public const int MaxElementCount = 10;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ContextModuleBuilder"/> class.
    /// </summary>
    public ContextModuleBuilder() => Elements = new List<IElementBuilder>();

    /// <summary>
    ///     Initializes a new instance of the <see cref="ContextModuleBuilder"/> class.
    /// </summary>
    public ContextModuleBuilder(List<IElementBuilder> elements) => Elements = elements;

    /// <inheritdoc />
    public ModuleType Type => ModuleType.Context;

    /// <summary>
    ///     Gets or sets the elements of the context module.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The <paramref name="value"/> contains disallowed type of element builder. Allowed element builders are
    ///     <see cref="PlainTextElementBuilder"/>, <see cref="KMarkdownElementBuilder"/>, and <see cref="ImageElementBuilder"/>.
    /// </exception>
    public List<IElementBuilder> Elements
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

            if (value.Exists(e => e.Type != ElementType.PlainText
                    && e.Type != ElementType.KMarkdown
                    && e.Type != ElementType.Image))
                throw new ArgumentException(
                    "Elements must be of type PlainText, KMarkdown or Image.",
                    nameof(Elements));

            _elements = value;
        }
    }

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
        if (Elements.Count >= MaxElementCount)
            throw new ArgumentException(
                $"Element count must be less than or equal to {MaxElementCount}.",
                nameof(field));

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
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ContextModuleBuilder AddElement(KMarkdownElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount)
            throw new ArgumentException(
                $"Element count must be less than or equal to {MaxElementCount}.",
                nameof(field));

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
        if (Elements.Count >= MaxElementCount)
            throw new ArgumentException(
                $"Element count must be less than or equal to {MaxElementCount}.",
                nameof(field));

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
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ContextModuleBuilder AddElement<T>(Action<T> action = null)
        where T : IElementBuilder, new()
    {
        T field = new();
        action?.Invoke(field);
        switch (field)
        {
            case PlainTextElementBuilder plainText:
                AddElement(plainText);
                break;
            case KMarkdownElementBuilder kMarkdown:
                AddElement(kMarkdown);
                break;
            case ImageElementBuilder image:
                AddElement(image);
                break;
            default:
                throw new ArgumentException(
                    "Elements of contexts must be of type PlainText, KMarkdown or Image.",
                    nameof(action));
        }

        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="ContextModule"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="ContextModule"/> representing the built context module object.
    /// </returns>
    public ContextModule Build() =>
        new(Elements.Select(e => e.Build()).ToImmutableArray());

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="ContextModuleBuilder"/> is equal to the current <see cref="ContextModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ContextModuleBuilder"/> is equal to the current <see cref="ContextModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(ContextModuleBuilder left, ContextModuleBuilder right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="ContextModuleBuilder"/> is not equal to the current <see cref="ContextModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ContextModuleBuilder"/> is not equal to the current <see cref="ContextModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(ContextModuleBuilder left, ContextModuleBuilder right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="ContextModuleBuilder"/> is equal to the current <see cref="ContextModuleBuilder"/>.</summary>
    /// <remarks>If the object passes is an <see cref="ContextModuleBuilder"/>, <see cref="Equals(ContextModuleBuilder)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="ContextModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ContextModuleBuilder"/> is equal to the current <see cref="ContextModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is ContextModuleBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="ContextModuleBuilder"/> is equal to the current <see cref="ContextModuleBuilder"/>.</summary>
    /// <param name="contextModuleBuilder">The <see cref="ContextModuleBuilder"/> to compare with the current <see cref="ContextModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ContextModuleBuilder"/> is equal to the current <see cref="ContextModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(ContextModuleBuilder contextModuleBuilder)
    {
        if (contextModuleBuilder is null) return false;

        if (Elements.Count != contextModuleBuilder.Elements.Count) return false;

        for (int i = 0; i < Elements.Count; i++)
            if (Elements[i] != contextModuleBuilder.Elements[i])
                return false;

        return Type == contextModuleBuilder.Type;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();
}
