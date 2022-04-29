namespace KaiHeiLa;

/// <summary>
///     Gets a generic tag found in messages.
/// </summary>
/// <seealso cref="IMessage.Tags"/>
public interface ITag
{
    /// <summary>
    ///     Gets position of the tag in the message.
    /// </summary>
    int Index { get; }
    
    /// <summary>
    ///     Gets the length of the tag.
    /// </summary>
    int Length { get; }
    
    /// <summary>
    ///     Gets the type of the tag.
    /// </summary>
    TagType Type { get; }
    
    /// <summary>
    ///     Gets the key of the tag.
    /// </summary>
    dynamic Key { get; }
    
    /// <summary>
    ///     Gets the value of the tag.
    /// </summary>
    object Value { get; }
}