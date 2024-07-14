namespace Kook;

/// <summary>
///     Represents a generic card builder for creating a <see cref="ICard"/>.
/// </summary>
public interface ICardBuilder
{
    /// <summary>
    ///     Gets the type of the <see cref="ICard"/> this builder creates.
    /// </summary>
    CardType Type { get; }

    /// <summary>
    ///     构建当前构建器为一个 <see cref="ICard"/>。
    /// </summary>
    /// <returns>
    ///     An <see cref="ICard"/> represents the built card object.
    /// </returns>
    ICard Build();
}
