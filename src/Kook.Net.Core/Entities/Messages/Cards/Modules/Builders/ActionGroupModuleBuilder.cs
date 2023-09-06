using System.Collections.Immutable;

namespace Kook;

/// <summary>
///     Represents a action group module builder for creating an <see cref="ActionGroupModule"/>.
/// </summary>
public class ActionGroupModuleBuilder : IModuleBuilder, IEquatable<ActionGroupModuleBuilder>
{
    private List<ButtonElementBuilder> _elements;

    /// <summary>
    ///     Returns the maximum number of elements allowed by Kook.
    /// </summary>
    public const int MaxElementCount = 4;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActionGroupModuleBuilder"/> class.
    /// </summary>
    public ActionGroupModuleBuilder() => Elements = new List<ButtonElementBuilder>();

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActionGroupModuleBuilder"/> class.
    /// </summary>
    public ActionGroupModuleBuilder(List<ButtonElementBuilder> elements) => Elements = elements;

    /// <inheritdoc />
    public ModuleType Type => ModuleType.ActionGroup;

    /// <summary>
    ///     Gets or sets the button elements of the action group module.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    /// <returns>
    ///     A <see cref="List{ButtonElementBuilder}"/> containing the button elements of the action group module.
    /// </returns>
    public List<ButtonElementBuilder> Elements
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
    ///     Adds a button element to the action group module.
    /// </summary>
    /// <param name="field">
    ///     The button element to add.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ActionGroupModuleBuilder AddElement(ButtonElementBuilder field)
    {
        if (Elements.Count >= MaxElementCount)
            throw new ArgumentException(
                $"Element count must be less than or equal to {MaxElementCount}.",
                nameof(field));

        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     Adds a button element to the action group module.
    /// </summary>
    /// <param name="action">
    ///     The action to add a button element to the action group module.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The addition operation would cause the number of elements to exceed <see cref="MaxElementCount"/>.
    /// </exception>
    public ActionGroupModuleBuilder AddElement(Action<ButtonElementBuilder> action)
    {
        ButtonElementBuilder field = new();
        action(field);
        AddElement(field);
        return this;
    }

    /// <summary>
    ///     Builds this builder into an <see cref="ActionGroupModule"/>.
    /// </summary>
    /// <returns>
    ///     An <see cref="ActionGroupModule"/> representing the built action group module object.
    /// </returns>
    public ActionGroupModule Build()
    {
        if (Elements is null or { Count: 0 })
            throw new ArgumentNullException(
                nameof(Elements),
                "Element cannot be null or empty list.");
        return new ActionGroupModule(Elements.Select(e => e.Build()).ToImmutableArray());
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="ActionGroupModuleBuilder"/> is equal to the current <see cref="ActionGroupModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ActionGroupModuleBuilder"/> is equal to the current <see cref="ActionGroupModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(ActionGroupModuleBuilder left, ActionGroupModuleBuilder right)
        => left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="ActionGroupModuleBuilder"/> is not equal to the current <see cref="ActionGroupModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ActionGroupModuleBuilder"/> is not equal to the current <see cref="ActionGroupModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(ActionGroupModuleBuilder left, ActionGroupModuleBuilder right)
        => !(left == right);

    /// <summary>Determines whether the specified <see cref="ActionGroupModuleBuilder"/> is equal to the current <see cref="ActionGroupModuleBuilder"/>.</summary>
    /// <remarks>If the object passes is an <see cref="ActionGroupModuleBuilder"/>, <see cref="Equals(ActionGroupModuleBuilder)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="ActionGroupModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ActionGroupModuleBuilder"/> is equal to the current <see cref="ActionGroupModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
        => obj is ActionGroupModuleBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="ActionGroupModuleBuilder"/> is equal to the current <see cref="ActionGroupModuleBuilder"/>.</summary>
    /// <param name="actionGroupModuleBuilder">The <see cref="ActionGroupModuleBuilder"/> to compare with the current <see cref="ActionGroupModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ActionGroupModuleBuilder"/> is equal to the current <see cref="ActionGroupModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(ActionGroupModuleBuilder actionGroupModuleBuilder)
    {
        if (actionGroupModuleBuilder is null) return false;

        if (Elements.Count != actionGroupModuleBuilder.Elements.Count) return false;

        for (int i = 0; i < Elements.Count; i++)
            if (Elements[i] != actionGroupModuleBuilder.Elements[i])
                return false;

        return Type == actionGroupModuleBuilder.Type;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();
}
