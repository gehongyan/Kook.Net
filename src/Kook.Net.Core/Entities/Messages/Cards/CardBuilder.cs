using System.Diagnostics.CodeAnalysis;

namespace Kook;

/// <summary>
///     Represents a builder class for creating a <see cref="Card"/>.
/// </summary>
public class CardBuilder : ICardBuilder, IEquatable<CardBuilder>, IEquatable<ICardBuilder>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CardBuilder"/> class with the specified parameters.
    /// </summary>
    /// <param name="theme"> The theme of the card.</param>
    /// <param name="color"> The color displayed along the left side of the card.</param>
    /// <param name="size"> The size of the card.</param>
    /// <param name="modules"> The modules in the card.</param>
    public CardBuilder(CardTheme theme = CardTheme.Primary, Color? color = null, CardSize size = CardSize.Large, IList<IModuleBuilder>? modules = null)
    {
        Theme = theme;
        Color = color;
        Size = size;
        Modules = modules ?? [];
    }

    /// <summary>
    ///     Gets the type of the card.
    /// </summary>
    /// <returns>
    ///     A <see cref="CardType"/> value that represents the type of the card.
    /// </returns>
    public CardType Type => CardType.Card;

    /// <summary>
    ///     Gets or sets the theme of the card.
    /// </summary>
    /// <returns>
    ///     A <see cref="CardTheme"/> value that represents the theme of the card.
    /// </returns>
    public CardTheme Theme { get; set; }

    /// <summary>
    ///     Gets or sets the color displayed along the left side of the card.
    /// </summary>
    /// <returns>
    ///     A <see cref="Color"/> value that represents the color displayed along the left side of the card.
    /// </returns>
    public Color? Color { get; set; }

    /// <summary>
    ///     Gets or sets the size of the card.
    /// </summary>
    /// <returns>
    ///     A <see cref="CardSize"/> value that represents the size of the card.
    /// </returns>
    public CardSize Size { get; set; } = CardSize.Large;

    /// <summary>
    ///     Gets or sets the modules in the card.
    /// </summary>
    /// <returns>
    ///     An <see cref="IList{IModuleBuilder}"/> containing the modules in the card.
    /// </returns>
    public IList<IModuleBuilder> Modules { get; set; }

    /// <summary>
    ///     Sets the theme of the card.
    /// </summary>
    /// <param name="theme">
    ///     A <see cref="CardTheme"/> value that represents the theme of the card to be set.
    /// </param>
    /// <returns> 当前构建器。 </returns>
    public CardBuilder WithTheme(CardTheme theme)
    {
        Theme = theme;
        return this;
    }

    /// <summary>
    ///     Sets the color displayed along the left side of the card.
    /// </summary>
    /// <param name="color">
    ///     A <see cref="Color"/> value that represents the color displayed along the left side of the card to be set.
    /// </param>
    /// <returns> 当前构建器。 </returns>
    public CardBuilder WithColor(Color? color)
    {
        Color = color;
        return this;
    }

    /// <summary>
    ///     Sets the size of the card.
    /// </summary>
    /// <param name="size">
    ///     A <see cref="CardSize"/> value that represents the size of the card to be set.
    /// </param>
    /// <returns> 当前构建器。 </returns>
    public CardBuilder WithSize(CardSize size)
    {
        Size = size;
        return this;
    }

    /// <summary>
    ///     Adds a module to the card.
    /// </summary>
    /// <param name="module">
    ///     An <see cref="IModuleBuilder"/> that represents the module to be added to the card.
    /// </param>
    /// <returns> 当前构建器。 </returns>
    public CardBuilder AddModule(IModuleBuilder module)
    {
        Modules.Add(module);
        return this;
    }

    /// <summary>
    ///     Adds a module to the card.
    /// </summary>
    /// <param name="action">
    ///     The action to adds a module to the card.
    /// </param>
    /// <returns> 当前构建器。 </returns>
    public CardBuilder AddModule<T>(Action<T>? action = null)
        where T : IModuleBuilder, new()
    {
        T module = new();
        action?.Invoke(module);
        AddModule(module);
        return this;
    }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="Card"/>。
    /// </summary>
    /// <returns>
    ///     A <see cref="KMarkdownElement"/> represents the built element object.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     The theme of the card cannot be invisible, which is reserved for text graphics mixed messages.
    /// </exception>
    public Card Build()
    {
        if (Theme is CardTheme.Invisible)
            throw new InvalidOperationException(
                "The theme of the card cannot be invisible, which is reserved for text graphics mixed messages.");
        return new Card(Theme, Size, Color, [..Modules.Select(m => m.Build())]);
    }

    /// <inheritdoc />
    ICard ICardBuilder.Build() => Build();

    /// <summary>
    ///     判定两个 <see cref="CardBuilder"/> 是否相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="CardBuilder"/> 相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator ==(CardBuilder? left, CardBuilder? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>
    ///     判定两个 <see cref="CardBuilder"/> 是否不相等。
    /// </summary>
    /// <returns> 如果两个 <see cref="CardBuilder"/> 不相等，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    public static bool operator !=(CardBuilder? left, CardBuilder? right) =>
        !(left == right);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) =>
        obj is CardBuilder builder && Equals(builder);

    /// <inheritdoc />
    public bool Equals([NotNullWhen(true)] CardBuilder? cardBuilder)
    {
        if (cardBuilder is null)
            return false;

        if (Modules.Count != cardBuilder.Modules.Count)
            return false;

        if (Modules
            .Zip(cardBuilder.Modules, (x, y) => (x, y))
            .Any(pair => pair.x != pair.y))
            return false;

        return Type == cardBuilder.Type
            && Theme == cardBuilder.Theme
            && Color == cardBuilder.Color
            && Size == cardBuilder.Size;
    }

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();

    bool IEquatable<ICardBuilder>.Equals([NotNullWhen(true)] ICardBuilder? cardBuilder) =>
        Equals(cardBuilder as CardBuilder);
}
