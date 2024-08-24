namespace Kook;

/// <summary>
///     表示一个通用的卡片构建器，用于构建一个 <see cref="ICard"/>。
/// </summary>
public interface ICardBuilder
{
    /// <summary>
    ///     获取此构建器构建的卡片的类型。
    /// </summary>
    CardType Type { get; }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="ICard"/>。
    /// </summary>
    /// <returns> 由当前构建器表示的属性构建的 <see cref="ICard"/> 对象。 </returns>
    ICard Build();
}
