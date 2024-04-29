namespace Kook;

/// <summary>
///     Represents a generic message quote.
/// </summary>
public interface IQuote
{
    /// <summary>
    ///     Gets the identifier of the message this quote refers to.
    /// </summary>
    Guid QuotedMessageId { get; }
}
