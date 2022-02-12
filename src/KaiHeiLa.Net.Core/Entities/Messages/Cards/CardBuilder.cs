using System.Collections.Immutable;

namespace KaiHeiLa;

/// <summary>
///     Represents a builder class for creating a <see cref="Card"/>.
/// </summary>
public class CardBuilder : ICardBuilder
{
    public CardBuilder()
    {
        Modules = new List<IModuleBuilder>();
    }

    public CardType Type => CardType.Card;
    
    public CardTheme Theme { get; set; }

    public Color? Color { get; set; }

    public CardSize Size { get; set; }
    
    public List<IModuleBuilder> Modules { get; set; }

    public CardBuilder WithTheme(CardTheme theme)
    {
        Theme = theme;
        return this;
    }
    public CardBuilder WithColor(Color color)
    {
        Color = color;
        return this;
    }
    public CardBuilder WithSize(CardSize size)
    {
        Size = size;
        return this;
    }
    
    public CardBuilder AddModule(IModuleBuilder module)
    {
        Modules.Add(module);
        return this;
    }

    public Card Build() =>
        new Card(Theme, Size, Color, Modules.Select(m => m.Build()).ToImmutableArray());

    ICard ICardBuilder.Build() => Build();
}