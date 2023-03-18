namespace Kook;

/// <summary>
///     Represents a generic direct-message channel.
/// </summary>
public interface IDMChannel : IMessageChannel, IPrivateChannel, IEntity<Guid>
{
    #region General

    /// <summary>
    ///     Gets the unique identifier of this direct-message channel.
    /// </summary>
    /// <returns>
    ///     A <see cref="Guid"/> that represents this direct-message channel's unique identifier.
    /// </returns>
    new Guid Id { get; }

    /// <summary>
    ///     Gets the chat code of the direct-message channel.
    /// </summary>
    /// <returns>
    ///     A <see cref="Guid"/> that represents the chat code of the direct-message channel.
    /// </returns>
    Guid ChatCode { get; }

    /// <summary>
    ///     Gets the recipient of all messages in this channel.
    /// </summary>
    /// <returns>
    ///     A user object that represents the other user in this channel.
    /// </returns>
    IUser Recipient { get; }

    /// <summary>
    ///     Closes this private channel, removing it from your channel list.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous close operation.
    /// </returns>
    Task CloseAsync(RequestOptions options = null);

    #endregion

    #region Send Messages

    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendFileAsync(string path, string fileName = null,
        AttachmentType type = AttachmentType.File, IQuote quote = null, RequestOptions options = null);

    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendFileAsync(Stream stream, string fileName = null,
        AttachmentType type = AttachmentType.File, IQuote quote = null, RequestOptions options = null);

    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendFileAsync(FileAttachment attachment,
        IQuote quote = null, RequestOptions options = null);

    /// <summary>
    ///     Sends a text message to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendTextAsync(string text, IQuote quote = null,
        RequestOptions options = null);

    /// <summary>
    ///     Sends a card message to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendCardAsync(ICard card,
        IQuote quote = null, RequestOptions options = null);

    /// <summary>
    ///     Sends a card message to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(IEnumerable<ICard> cards,
        IQuote quote = null, RequestOptions options = null);

    #endregion
}
