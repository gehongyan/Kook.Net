using System.Diagnostics;

namespace Kook;

/// <summary>
///     Represents a quoted message.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Quote : IQuote
{
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

    /// <summary>
    ///     Gets an empty quote whose quoted message is null.
    /// </summary>
    /// <remarks>
    ///     Used to delete a quote when modifying a message.
    /// </remarks>
    [Obsolete("Use MessageReference.Empty instead.")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static MessageReference Empty => new(Guid.Empty);

    /// <summary>
    ///     Initializes a new instance of the <see cref="Quote"/> class.
    /// </summary>
    /// <param name="quotedMessageId">
    ///     The quoted message identifier.
    /// </param>
    [Obsolete("Use MessageReference instead.")]
    public Quote(Guid quotedMessageId)
    {
        QuotedMessageId = quotedMessageId;
        Content = string.Empty;
        Author = null!;
    }

    internal Quote(Guid quotedMessageId, MessageType type, string content, DateTimeOffset createAt, IUser author)
    {
        QuotedMessageId = quotedMessageId;
        Type = type;
        Content = content;
        CreateAt = createAt;
        Author = author;
    }

    internal static Quote Create(Guid quotedMessageId, MessageType type,
        string content, DateTimeOffset createAt, IUser author) =>
        new(quotedMessageId, type, content, createAt, author);

    private string DebuggerDisplay => $"{Author}: {Content} {QuotedMessageId:P}";
}
