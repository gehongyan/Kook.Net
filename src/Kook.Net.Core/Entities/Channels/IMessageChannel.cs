namespace Kook;

public interface IMessageChannel : IChannel
{
    #region Send Messages

    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends a file as if you are uploading a file directly from your Kook client.
    /// </remarks>
    /// <param name="path">The file path of the file.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="type">The type of the file.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="ephemeralUser">The user only who can see the message. Leave null to let everyone see the message.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendFileAsync(string path, string fileName = null,
        AttachmentType type = AttachmentType.File, IQuote quote = null, IUser ephemeralUser = null,
        RequestOptions options = null);

    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends a file as if you are uploading a file directly from your Kook client.
    /// </remarks>
    /// <param name="stream">The stream of the file.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="type">The type of the file.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="ephemeralUser">The user only who can see the message. Leave null to let everyone see the message.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendFileAsync(Stream stream, string fileName,
        AttachmentType type = AttachmentType.File, IQuote quote = null, IUser ephemeralUser = null,
        RequestOptions options = null);
    /// <summary>
    ///     Sends a file to this message channel.
    /// </summary>
    /// <remarks>
    ///     This method sends a file as if you are uploading a file directly from your Kook client.
    /// </remarks>
    /// <param name="attachment">The attachment containing the file.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="ephemeralUser">The user only who can see the message. Leave null to let everyone see the message.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendFileAsync(FileAttachment attachment,
        IQuote quote = null, IUser ephemeralUser = null, RequestOptions options = null);
    /// <summary>
    ///     Sends a text message to this message channel.
    /// </summary>
    /// <param name="text">The message to be sent.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="ephemeralUser">The user only who can see the message. Leave null to let everyone see the message.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendTextAsync(string text, IQuote quote = null,
        IUser ephemeralUser = null, RequestOptions options = null);
    /// <summary>
    ///     Sends a card message to this message channel.
    /// </summary>
    /// <param name="card">The card to be sent.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="ephemeralUser">The user only who can see the message. Leave null to let everyone see the message.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendCardAsync(ICard card,
        IQuote quote = null, IUser ephemeralUser = null, RequestOptions options = null);
    /// <summary>
    ///     Sends a card message to this message channel.
    /// </summary>
    /// <param name="cards">The cards to be sent.</param>
    /// <param name="quote">The message quote to be included. Used to reply to specific messages.</param>
    /// <param name="ephemeralUser">The user only who can see the message. Leave null to let everyone see the message.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the identifier and timestamp of the sent message.
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(IEnumerable<ICard> cards,
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

    #endregion

    #region Get Messages

    /// <summary>
    ///     Gets the last N messages from this message channel.
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         The returned collection is an asynchronous enumerable object; one must call 
    ///         <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> to access the individual messages as a
    ///         collection.
    ///     </note>
    ///     <note type="warning">
    ///         Do not fetch too many messages at once! This may cause unwanted preemptive rate limit or even actual
    ///         rate limit, causing your bot to freeze!
    ///     </note>
    ///     This method will attempt to fetch the number of messages specified under <paramref name="limit"/>. The
    ///     library will attempt to split up the requests according to your <paramref name="limit"/> and 
    ///     <see cref="KookConfig.MaxMessagesPerBatch"/>. In other words, should the user request 500 messages,
    ///     and the <see cref="Kook.KookConfig.MaxMessagesPerBatch"/> constant is <c>100</c>, the request will
    ///     be split into 5 individual requests; thus returning 5 individual asynchronous responses, hence the need
    ///     of flattening.
    /// </remarks>
    /// <param name="limit">The numbers of message to be gotten from.</param>
    /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from
    /// cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     Paged collection of messages.
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = KookConfig.MaxMessagesPerBatch,
        CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    /// <summary>
    ///     Gets a collection of messages in this channel.
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         The returned collection is an asynchronous enumerable object; one must call 
    ///         <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> to access the individual messages as a
    ///         collection.
    ///     </note>
    ///     <note type="warning">
    ///         Do not fetch too many messages at once! This may cause unwanted preemptive rate limit or even actual
    ///         rate limit, causing your bot to freeze!
    ///     </note>
    ///     This method will attempt to fetch the number of messages specified under <paramref name="limit"/> around
    ///     the message <paramref name="referenceMessageId"/> depending on the <paramref name="dir"/>. The library will
    ///     attempt to split up the requests according to your <paramref name="limit"/> and 
    ///     <see cref="KookConfig.MaxMessagesPerBatch"/>. In other words, should the user request 500 messages,
    ///     and the <see cref="Kook.KookConfig.MaxMessagesPerBatch"/> constant is <c>100</c>, the request will
    ///     be split into 5 individual requests; thus returning 5 individual asynchronous responses, hence the need
    ///     of flattening.
    /// </remarks>
    /// <param name="referenceMessageId">The ID of the starting message to get the messages from.</param>
    /// <param name="dir">The direction of the messages to be gotten from.</param>
    /// <param name="limit">The numbers of message to be gotten from.</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from
    /// cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     Paged collection of messages.
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir, int limit = KookConfig.MaxMessagesPerBatch,
        CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    /// <summary>
    ///     Gets a collection of messages in this channel.
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         The returned collection is an asynchronous enumerable object; one must call 
    ///         <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> to access the individual messages as a
    ///         collection.
    ///     </note>
    ///     <note type="warning">
    ///         Do not fetch too many messages at once! This may cause unwanted preemptive rate limit or even actual
    ///         rate limit, causing your bot to freeze!
    ///     </note>
    ///     This method will attempt to fetch the number of messages specified under <paramref name="limit"/> around
    ///     the message <paramref name="referenceMessage"/> depending on the <paramref name="dir"/>. The library will
    ///     attempt to split up the requests according to your <paramref name="limit"/> and 
    ///     <see cref="KookConfig.MaxMessagesPerBatch"/>. In other words, should the user request 500 messages,
    ///     and the <see cref="Kook.KookConfig.MaxMessagesPerBatch"/> constant is <c>100</c>, the request will
    ///     be split into 5 individual requests; thus returning 5 individual asynchronous responses, hence the need
    ///     of flattening.
    /// </remarks>
    /// <param name="referenceMessage">The starting message to get the messages from.</param>
    /// <param name="dir">The direction of the messages to be gotten from.</param>
    /// <param name="limit">The numbers of message to be gotten from.</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from
    /// cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     Paged collection of messages.
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir, int limit = KookConfig.MaxMessagesPerBatch,
        CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

    #endregion

    #region Delete Messages

    /// <summary>
    ///     Deletes a message.
    /// </summary>
    /// <param name="messageId">The identifier of the message that would be removed.</param>
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

    #endregion

    #region Modify Messages

    /// <summary>
    ///     Modifies a message.
    /// </summary>
    /// <remarks>
    ///     This method modifies this message with the specified properties. To see an example of this
    ///     method and what properties are available, please refer to <see cref="MessageProperties"/>.
    /// </remarks>
    /// <param name="messageId">The identifier of the message that would be changed.</param>
    /// <param name="func">A delegate containing the properties to modify the message with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous modification operation.
    /// </returns>
    Task ModifyMessageAsync(Guid messageId, Action<MessageProperties> func, RequestOptions options = null);

    #endregion
}
