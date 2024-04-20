namespace Kook;

/// <summary>
///     Represents an embed in a message that
/// </summary>
public struct CardEmbed : IEmbed
{
    internal CardEmbed(ICard card)
    {
        Card = card;
    }

    /// <summary>
    ///     Gets the cards in this embed.
    /// </summary>
    public ICard Card { get; internal set; }

    /// <inheritdoc />
    public EmbedType Type => EmbedType.Card;
}
