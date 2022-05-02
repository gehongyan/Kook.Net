using System.Diagnostics;
using KaiHeiLa.Rest;
using Model = KaiHeiLa.API.Quote;

namespace KaiHeiLa;

/// <inheritdoc cref="IQuote"/>
public class Quote : IQuote
{
    /// <inheritdoc />
    public string Id { get; }
    /// <inheritdoc />
    public Guid QuotedMessageId { get; }
    /// <inheritdoc />
    public MessageType Type { get; }
    /// <inheritdoc />
    public string Content { get; }
    /// <inheritdoc />
    public DateTimeOffset CreateAt { get; }
    /// <inheritdoc />
    public IUser Author { get; }

    /// <summary>
    ///     Gets an empty quote whose quoted message is null.
    /// </summary>
    /// <remarks>
    ///     Used to delete a quote when modifying a message.
    /// </remarks>
    public Quote Empty => new Quote(Guid.Empty);
    
    /// <summary>
    ///     Initializes a new instance of the <see cref="Quote"/> class.
    /// </summary>
    /// <param name="quotedMessageId">
    ///     The quoted message identifier.
    /// </param>
    public Quote(Guid quotedMessageId)
    {
        QuotedMessageId = quotedMessageId;
    }
    
    internal Quote(string id, Guid quotedMessageId, MessageType type, string content, DateTimeOffset createAt, IUser author)
    {
        Id = id;
        QuotedMessageId = quotedMessageId;
        Type = type;
        Content = content;
        CreateAt = createAt;
        Author = author;
    }

    internal static Quote Create(Model model, IUser author)
        => new Quote(model.Id, model.QuotedMessageId, model.Type, model.Content, model.CreateAt, author);
}