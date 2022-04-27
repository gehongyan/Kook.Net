namespace KaiHeiLa;

/// <summary>
///     A generic element used in modules.
/// </summary>
public interface IElement
{
    /// <summary>
    ///     Gets the type of this element.
    /// </summary>
    ElementType Type { get; }
}