namespace Kook;

/// <summary>
///     Specifies the type of an <see cref="IElement"/>.
/// </summary>
public enum ElementType
{
    /// <summary>
    ///     The element is a plain text element.
    /// </summary>
    PlainText,
    /// <summary>
    ///     The element is a KMarkdown element.
    /// </summary>
    KMarkdown,
    /// <summary>
    ///     The element is an image element.
    /// </summary>
    Image,
    /// <summary>
    ///     The element is a button element.
    /// </summary>
    Button,
    /// <summary>
    ///     The element is a paragraph struct.
    /// </summary>
    Paragraph
}
