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
    ///     <see cref="KaiHeiLaConfig.MaxMessagesPerBatch"/>. In other words, should the user request 500 messages,
    ///     and the <see cref="KaiHeiLa.KaiHeiLaConfig.MaxMessagesPerBatch"/> constant is <c>100</c>, the request will
    ///     be split into 5 individual requests; thus returning 5 individual asynchronous responses, hence the need
    ///     of flattening.
    /// </remarks>
    /// <example>
    ///     <para>The following example downloads 300 messages and gets messages that belong to the user 
    ///     <c>53905483156684800</c>.</para>
    ///     <code language="cs" region="GetMessagesAsync.FromLimit.Standard"
    ///           source="..\..\..\KaiHeiLa.Net.Examples\Core\Entities\Channels\IMessageChannel.Examples.cs" />
    /// </example>
    /// <param name="limit">The numbers of message to be gotten from.</param>
    /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from
    /// cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     Paged collection of messages.
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = KaiHeiLaConfig.MaxMessagesPerBatch, 
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
    ///     <see cref="KaiHeiLaConfig.MaxMessagesPerBatch"/>. In other words, should the user request 500 messages,
    ///     and the <see cref="KaiHeiLa.KaiHeiLaConfig.MaxMessagesPerBatch"/> constant is <c>100</c>, the request will
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
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir, int limit = KaiHeiLaConfig.MaxMessagesPerBatch, 
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
    ///     <see cref="KaiHeiLaConfig.MaxMessagesPerBatch"/>. In other words, should the user request 500 messages,
    ///     and the <see cref="KaiHeiLa.KaiHeiLaConfig.MaxMessagesPerBatch"/> constant is <c>100</c>, the request will
    ///     be split into 5 individual requests; thus returning 5 individual asynchronous responses, hence the need
    ///     of flattening.
    /// </remarks>
    /// <example>
    ///     <para>The following example gets 5 message prior to a specific message, <c>oldMessage</c>.</para>
    ///     <code language="cs" region="GetMessagesAsync.FromMessage"
    ///           source="..\..\..\KaiHeiLa.Net.Examples\Core\Entities\Channels\IMessageChannel.Examples.cs" />
    /// </example>
    /// <param name="referenceMessage">The starting message to get the messages from.</param>
    /// <param name="dir">The direction of the messages to be gotten from.</param>
    /// <param name="limit">The numbers of message to be gotten from.</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from
    /// cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     Paged collection of messages.
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir, int limit = KaiHeiLaConfig.MaxMessagesPerBatch, 
        CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    
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
}