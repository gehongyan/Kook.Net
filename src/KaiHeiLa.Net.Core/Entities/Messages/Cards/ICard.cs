namespace KaiHeiLa;

/// <summary>
///     Represents a generic card.
/// </summary>
public interface ICard
{
    /// <summary>
    ///     Gets the type of the card.
    /// </summary>
    /// <returns>
    ///     A <see cref="CardType"/> value that represents the type of the card.
    /// </returns>
    CardType Type { get; }
    
    /// <summary>
    ///     Gets the number of the modules in this card.
    /// </summary>
    /// <returns>
    ///     An <see langword="int"/> value that represents how many modules are in this card.
    /// </returns>
    int ModuleCount { get; }
}