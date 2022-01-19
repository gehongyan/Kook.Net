namespace KaiHeiLa;

/// <summary>
///     卡片
/// </summary>
public class Card : ICard
{
    internal Card(CardTheme theme, Color color, CardSize size)
    {
        Theme = theme;
        Color = color;
        Size = size;
        Modules = new List<IModule>();
    }

    public CardType Type => CardType.Card;

    public CardTheme Theme { get; internal set; }

    public Color Color { get; internal set; }

    public CardSize Size { get; internal set; }

    public ICollection<IModule> Modules { get; internal set; }
}