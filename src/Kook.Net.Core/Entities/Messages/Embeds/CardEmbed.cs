namespace Kook;

/// <summary>
///     表示一个消息中解析出的卡片嵌入式内容。
/// </summary>
public struct CardEmbed : IEmbed
{
    internal CardEmbed(ICard card)
    {
        Card = card;
    }

    /// <summary>
    ///     获取卡片。
    /// </summary>
    public ICard Card { get; }

    /// <inheritdoc />
    public EmbedType Type => EmbedType.Card;
}
