namespace KaiHeiLa;

/// <summary>
///     Represents a builder class for creating an <see cref="IElement"/>.
/// </summary>
public interface IElementBuilder
{
    /// <summary>
    ///     Specifies the type of the element to be created.
    /// </summary>
    /// <returns>
    ///     An <see cref="ElementType"/> that specifies the type of the element to be created.
    /// </returns>
    ElementType Type { get; }
    
    /// <summary>
    ///     Builds the <see cref="IElementBuilder"/> into an <see cref="IElement"/>.
    /// </summary>
    /// <returns>
    ///     An <see cref="IElement"/> represents the built element object.
    /// </returns>
    IElement Build();
}