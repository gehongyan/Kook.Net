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
    ///     Builds the <see cref="ICardBuilder"/> into an <see cref="ICard"/>.
    /// </summary>
    /// <returns>
    ///     An <see cref="ICard"/> represents the built card object.
    /// </returns>
    ICard Build();
}