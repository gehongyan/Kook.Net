using System.Threading.Tasks;

namespace KaiHeiLa;

/// <summary>
///     Represents a generic direct-message channel.
/// </summary>
public interface IDMChannel : IMessageChannel, IPrivateChannel, IGuidEntity
{
    #region General

    new Guid Id { get; }

    /// <summary>
    ///     Chat code of the direct-message channel.
    /// </summary>
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
    ///     Sends a plain text to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendTextMessageAsync(string text, IQuote quote = null,
        RequestOptions options = null);
    /// <summary>
    ///     Sends an image to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(string path, string fileName = null,
        IQuote quote = null, RequestOptions options = null);

    /// <summary>
    ///     Sends a video to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(string path, string fileName = null,
        IQuote quote = null, RequestOptions options = null);

    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(string path, string fileName = null,
        IQuote quote = null, RequestOptions options = null);

    // /// <summary>
    // ///     Sends an audio to this message channel.
    // /// </summary>
    // /// <returns>
    // ///     A task that represents an asynchronous send operation for delivering the message. The task result
    // ///     contains the identifier and timestamp of the sent message.
    // /// </returns>
    // Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendAudioMessageAsync(string path, string fileName = null,
    //     IQuote quote = null, RequestOptions options = null);

    /// <summary>
    ///     Sends a KMarkdown message to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendKMarkdownMessageAsync(string text, IQuote quote = null,
        RequestOptions options = null);
        
    /// <summary>
    ///     Sends a card message to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardMessageAsync(IEnumerable<ICard> cards,
        IQuote quote = null, RequestOptions options = null);

    #endregion
        
    #region IMessageChannel

    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendTextMessageAsync(string text,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendTextMessageAsync(text, quote, options);
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendImageMessageAsync(string path, string fileName,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendImageMessageAsync(path, fileName, quote, options);
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendVideoMessageAsync(string path, string fileName,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendVideoMessageAsync(path, fileName, quote, options);
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendFileMessageAsync(string path, string fileName,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendFileMessageAsync(path, fileName, quote, options);
    // Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendAudioMessageAsync(string path, string fileName,
    //     IQuote quote, IUser ephemeralUser, RequestOptions options)
    //     => SendAudioMessageAsync(path, fileName, quote, options);
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendKMarkdownMessageAsync(string text,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendKMarkdownMessageAsync(text, quote, options);
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> IMessageChannel.SendCardMessageAsync(IEnumerable<ICard> cards,
        IQuote quote, IUser ephemeralUser, RequestOptions options)
        => SendCardMessageAsync(cards, quote, options);

    #endregion
}