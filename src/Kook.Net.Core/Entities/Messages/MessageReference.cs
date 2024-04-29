using System.Diagnostics;

namespace Kook;

/// <summary>
///     Represents a message reference.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class MessageReference : IQuote
{
    /// <summary>
    ///     Gets an empty quote whose quoted message is null.
    /// </summary>
    /// <remarks>
    ///     Used to delete a quote when modifying a message.
    /// </remarks>
    public static MessageReference Empty => new(Guid.Empty);

    /// <summary>
    ///     Creates a new instance of <see cref="MessageReference"/> with the specified quoted message identifier.
    /// </summary>
    /// <param name="quotedMessageId">
    ///     The identifier of the message that will be quoted.
    ///     If <see langword="null"/>, the quote will be empty.
    /// </param>
    public MessageReference(Guid quotedMessageId)
    {
        QuotedMessageId = quotedMessageId;
    }

    /// <inheritdoc />
    public Guid QuotedMessageId { get; }

    private string DebuggerDisplay => $"Quote: {QuotedMessageId}";
}
