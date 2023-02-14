namespace Kook;

/// <summary>
///     Represents a generic message quote.
/// </summary>
public interface IQuote : IEntity<string>
{
    /// <summary>
    ///     Gets the identifier of the message this quote refers to.
    /// </summary>
    Guid QuotedMessageId { get; }

    /// <summary>
    ///     Gets the type of the message this quote refers to.
    /// </summary>
    MessageType Type { get; }

    /// <summary>
    ///     Gets the content of the message this quote refers to.
    /// </summary>
    /// <returns>
    ///     A string that contains the body of the message;
    ///     note that this field may be empty or the original code if the message is not a text based message.
    /// </returns>
    string Content { get; }

    /// <summary>
    ///     Gets the time this message was sent.
    /// </summary>
    /// <returns>
    ///     Time of when the message was sent.
    /// </returns>
    DateTimeOffset CreateAt { get; }

    /// <summary>
    ///     Gets the author of this message.
    /// </summary>
    IUser Author { get; }
}
