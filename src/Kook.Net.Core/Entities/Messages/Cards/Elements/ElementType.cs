namespace Kook;

/// <summary>
///     表示一个 <see cref="IElement"/> 的类型。
/// </summary>
public enum ElementType
{
    /// <summary>
    ///     纯文本元素。
    /// </summary>
    PlainText,

    /// <summary>
    ///     KMarkdown 文本元素。
    /// </summary>
    KMarkdown,

    /// <summary>
    ///     图片元素。
    /// </summary>
    Image,

    /// <summary>
    ///     按钮元素。
    /// </summary>
    Button,

    /// <summary>
    ///     区域文本结构。
    /// </summary>
    Paragraph
}
