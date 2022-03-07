namespace KaiHeiLa;

public interface IMessageChannel : IChannel
{
    /// <summary>
    ///     Sends a plain text to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendTextMessageAsync(string text, IQuote quote = null,
        IUser ephemeralUser = null, RequestOptions options = null);

    /// <summary>
    ///     Sends an image to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendImageMessageAsync(string path, string fileName = null,
        IQuote quote = null, IUser ephemeralUser = null, RequestOptions options = null);

    /// <summary>
    ///     Sends a video to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendVideoMessageAsync(string path, string fileName = null,
        IQuote quote = null, IUser ephemeralUser = null, RequestOptions options = null);

    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendFileMessageAsync(string path, string fileName = null,
        IQuote quote = null, IUser ephemeralUser = null, RequestOptions options = null);

    // /// <summary>
    // ///     Sends an audio to this message channel.
    // /// </summary>
    // /// <returns>
    // ///     A task that represents an asynchronous send operation for delivering the message. The task result
    // ///     contains the identifier and timestamp of the sent message.
    // /// </returns>
    // Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendAudioMessageAsync(string path, string fileName = null,
    //     IQuote quote = null, IUser ephemeralUser = null, RequestOptions options = null);

    /// <summary>
    ///     Sends a KMarkdown message to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendKMarkdownMessageAsync(string text, IQuote quote = null,
        IUser ephemeralUser = null, RequestOptions options = null);
    
    /// <summary>
    ///     Sends a card message to this message channel.
    /// </summary>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendCardMessageAsync(IEnumerable<ICard> cards,
        IQuote quote = null, IUser ephemeralUser = null, RequestOptions options = null);
    
    /// <summary>
    ///     Gets a message from this message channel.
    /// </summary>
    /// <param name="id">The identifier of the message.</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous get operation for retrieving the message. The task result contains
    ///     the retrieved message; <c>null</c> if no message is found with the specified identifier.
    /// </returns>
    Task<IMessage> GetMessageAsync(Guid id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

    /// <summary>
    ///     Deletes a message.
    /// </summary>
    /// <param name="messageId">The snowflake identifier of the message that would be removed.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous removal operation.
    /// </returns>
    Task DeleteMessageAsync(Guid messageId, RequestOptions options = null);
    /// <summary> Deletes a message based on the provided message in this channel. </summary>
    /// <param name="message">The message that would be removed.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous removal operation.
    /// </returns>
    Task DeleteMessageAsync(IMessage message, RequestOptions options = null);

    /// <summary>
    ///     Modifies a message.
    /// </summary>
    /// <remarks>
    ///     This method modifies this message with the specified properties. To see an example of this
    ///     method and what properties are available, please refer to <see cref="MessageProperties"/>.
    /// </remarks>
    /// <param name="messageId">The snowflake identifier of the message that would be changed.</param>
    /// <param name="func">A delegate containing the properties to modify the message with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous modification operation.
    /// </returns>
    Task ModifyMessageAsync(Guid messageId, Action<MessageProperties> func, RequestOptions options = null);
}