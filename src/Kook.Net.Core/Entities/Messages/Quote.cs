using System.Diagnostics;

namespace Kook;

/// <summary>
///     Represents a quoted message.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Quote : IQuote, IEntity<string>
{
    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public Guid QuotedMessageId { get; }

    /// <summary>
    ///     Gets the type of the message this quote refers to.
    /// </summary>
    public MessageType Type { get; }

    /// <summary>
    ///     Gets the content of the message this quote refers to.
    /// </summary>
    /// <returns>
    ///     A string that contains the body of the message;
    ///     note that this field may be empty or the original code if the message is not a text based message.
    /// </returns>
    public string Content { get; }

    /// <summary>
    ///     Gets the time this message was sent.
    /// </summary>
    /// <returns>
    ///     Time of when the message was sent.
    /// </returns>
    public DateTimeOffset CreateAt { get; }

    /// <summary>
    ///     Gets the author of this message.
    /// </summary>
    public IUser Author { get; }

    internal Quote(string id, Guid quotedMessageId, MessageType type, string content, DateTimeOffset createAt, IUser author)
    {
        Id = id;
        QuotedMessageId = quotedMessageId;
        Type = type;
        Content = content;
        CreateAt = createAt;
        Author = author;
    }

    internal static Quote Create(string id, Guid quotedMessageId, MessageType type, string content, DateTimeOffset createAt, IUser author) =>
        new(id, quotedMessageId, type, content, createAt, author);

    private string DebuggerDisplay => $"{Author}: {Content} ({Id})";
}
