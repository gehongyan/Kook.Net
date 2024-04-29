using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     Represents a action group module builder for creating an <see cref="ActionGroupModule"/>.
/// </summary>
public class ActionGroupModuleBuilder : IModuleBuilder, IEquatable<ActionGroupModuleBuilder>, IEquatable<IModuleBuilder>
{
    /// <summary>
    ///     Returns the maximum number of elements allowed by Kook.
    /// </summary>
    public const int MaxElementCount = 4;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActionGroupModuleBuilder"/> class.
    /// </summary>
    public ActionGroupModuleBuilder()
    {
        Elements = [];
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActionGroupModuleBuilder"/> class.
    /// </summary>
    public ActionGroupModuleBuilder(IList<ButtonElementBuilder> elements)
    {
        Elements = elements;
    }

    /// <inheritdoc />
    public ModuleType Type => ModuleType.ActionGroup;

    /// <summary>
    ///     Gets or sets the button elements of the action group module.
    /// </summary>
    /// <returns>
    ///     An <see cref="IList{ButtonElementBuilder}"/> containing the button elements of the action group module.
    /// </returns>
    public IList<ButtonElementBuilder> Elements { get; set; }

    /// <summary>
    ///     Adds a button element to the action group module.
    /// </summary>
    /// <param name="field">
    ///     The button element to add.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ActionGroupModuleBuilder AddElement(ButtonElementBuilder field)
    {
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
    public ActionGroupModuleBuilder AddElement(Action<ButtonElementBuilder>? action = null)
    {
        ButtonElementBuilder field = new();
        action?.Invoke(field);
        Elements.Add(field);
        return this;
    }

    /// <summary>
    ///     Builds this builder into an <see cref="ActionGroupModule"/>.
    /// </summary>
    /// <returns>
    ///     An <see cref="ActionGroupModule"/> representing the built action group module object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <see cref="Elements"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Elements"/> is an empty list.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     The number of elements of <see cref="Elements"/> is greater than <see cref="MaxElementCount"/>.
    /// </exception>
    public ActionGroupModule Build()
    {
        if (Elements == null)
            throw new ArgumentNullException(
                nameof(Elements), "Element cannot be null or empty list.");
        if (Elements.Count == 0)
            throw new ArgumentException(
                "Element cannot be null or empty list.", nameof(Elements));
        if (Elements.Count > MaxElementCount)
            throw new ArgumentException(
                $"Element count must be less than or equal to {MaxElementCount}.", nameof(Elements));
        return new ActionGroupModule([..Elements.Select(e => e.Build())]);
    }

    /// <inheritdoc />
    IModule IModuleBuilder.Build() => Build();

    /// <summary>
    ///     Determines whether the specified <see cref="ActionGroupModuleBuilder"/> is equal to the current <see cref="ActionGroupModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ActionGroupModuleBuilder"/> is equal to the current <see cref="ActionGroupModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator ==(ActionGroupModuleBuilder? left, ActionGroupModuleBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     Determines whether the specified <see cref="ActionGroupModuleBuilder"/> is not equal to the current <see cref="ActionGroupModuleBuilder"/>.
    /// </summary>
    /// <returns> <c>true</c> if the specified <see cref="ActionGroupModuleBuilder"/> is not equal to the current <see cref="ActionGroupModuleBuilder"/>; otherwise, <c>false</c>. </returns>
    public static bool operator !=(ActionGroupModuleBuilder? left, ActionGroupModuleBuilder? right) =>
        !(left == right);

    /// <summary>Determines whether the specified <see cref="ActionGroupModuleBuilder"/> is equal to the current <see cref="ActionGroupModuleBuilder"/>.</summary>
    /// <remarks>If the object passes is an <see cref="ActionGroupModuleBuilder"/>, <see cref="Equals(ActionGroupModuleBuilder)"/> will be called to compare the 2 instances.</remarks>
    /// <param name="obj">The object to compare with the current <see cref="ActionGroupModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ActionGroupModuleBuilder"/> is equal to the current <see cref="ActionGroupModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is ActionGroupModuleBuilder builder && Equals(builder);

    /// <summary>Determines whether the specified <see cref="ActionGroupModuleBuilder"/> is equal to the current <see cref="ActionGroupModuleBuilder"/>.</summary>
    /// <param name="actionGroupModuleBuilder">The <see cref="ActionGroupModuleBuilder"/> to compare with the current <see cref="ActionGroupModuleBuilder"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="ActionGroupModuleBuilder"/> is equal to the current <see cref="ActionGroupModuleBuilder"/>; otherwise, <c>false</c>.</returns>
    public bool Equals([NotNullWhen(true)] ActionGroupModuleBuilder? actionGroupModuleBuilder)
    {
        if (actionGroupModuleBuilder is null)
            return false;

        if (Elements.Count != actionGroupModuleBuilder.Elements.Count)
            return false;

        if (Elements
            .Zip(actionGroupModuleBuilder.Elements, (x, y) => (x, y))
            .Any(pair => pair.x != pair.y))
            return false;

        return Type == actionGroupModuleBuilder.Type;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<IModuleBuilder>.Equals([NotNullWhen(true)] IModuleBuilder? moduleBuilder) =>
        Equals(moduleBuilder as ActionGroupModuleBuilder);
}
