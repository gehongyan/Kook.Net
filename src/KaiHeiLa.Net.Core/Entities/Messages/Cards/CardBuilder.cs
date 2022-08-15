using System.Collections.Immutable;

namespace KaiHeiLa;

/// <summary>
///     Represents a builder class for creating a <see cref="Card"/>.
/// </summary>
public class CardBuilder : ICardBuilder
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CardBuilder"/> class.
    /// </summary>
    public CardBuilder()
    {
        Modules = new List<IModuleBuilder>();
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
    public CardSize Size { get; set; }
    
    /// <summary>
    ///     Gets or sets the modules in the card.
    /// </summary>
    /// <returns>
    ///     A <see cref="List{IModuleBuilder}"/> containing the modules in the card.
    /// </returns>
    public List<IModuleBuilder> Modules { get; set; }

    /// <summary>
    ///     Sets the theme of the card.
    /// </summary>
    /// <param name="theme">
    ///     A <see cref="CardTheme"/> value that represents the theme of the card to be set.
    /// </param>
    /// <returns>
    ///     The current builder.
    /// </returns>
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
    /// <returns>
    ///     The current builder.
    /// </returns>
    public CardBuilder WithColor(Color color)
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
    /// <returns>
    ///     The current builder.
    /// </returns>
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
    /// <returns>
    ///     The current builder.
    /// </returns>
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
    /// <returns>
    ///     The current builder.
    /// </returns>
    public CardBuilder AddModule<T>(Action<T> action)
        where T : IModuleBuilder, new()
    {
        T module = new();
        action(module);
        AddModule(module);
        return this;
    }

    /// <summary>
    ///     Builds the <see cref="CardBuilder"/> into a <see cref="Card"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="KMarkdownElement"/> represents the built element object.
    /// </returns>
    public Card Build() =>
        new Card(Theme, Size, Color, Modules.Select(m => m.Build()).ToImmutableArray());

    /// <inheritdoc />
    ICard ICardBuilder.Build() => Build();
}