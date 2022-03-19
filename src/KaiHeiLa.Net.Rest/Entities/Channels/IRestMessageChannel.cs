namespace KaiHeiLa.Rest;

/// <summary>
///     Represents a REST-based channel that can send and receive messages.
/// </summary>
public interface IRestMessageChannel : IMessageChannel
{
    /// <summary>
    ///     Gets a message from this message channel.
    /// </summary>
    /// <remarks>
    ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessageAsync(Guid, CacheMode, RequestOptions)"/>.
    ///     Please visit its documentation for more details on this method.
    /// </remarks>
    /// <param name="id">The identifier of the message.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents an asynchronous get operation for retrieving the message. The task result contains
    ///     the retrieved message; <c>null</c> if no message is found with the specified identifier.
    /// </returns>
    Task<RestMessage> GetMessageAsync(Guid id, RequestOptions options = null);
    /// <summary>
    ///     Gets the last N messages from this message channel.
    /// </summary>
    /// <remarks>
    ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessagesAsync(int, CacheMode, RequestOptions)"/>.
    ///     Please visit its documentation for more details on this method.
    /// </remarks>
    /// <param name="limit">The numbers of message to be gotten from.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     Paged collection of messages.
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(int limit = KaiHeiLaConfig.MaxMessagesPerBatch, RequestOptions options = null);
    /// <summary>
    ///     Gets a collection of messages in this channel.
    /// </summary>
    /// <remarks>
    ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessagesAsync(Guid, Direction, int, CacheMode, RequestOptions)"/>.
    ///     Please visit its documentation for more details on this method.
    /// </remarks>
    /// <param name="referenceMessageId">The ID of the starting message to get the messages from.</param>
    /// <param name="dir">The direction of the messages to be gotten from.</param>
    /// <param name="limit">The numbers of message to be gotten from.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     Paged collection of messages.
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir, int limit = KaiHeiLaConfig.MaxMessagesPerBatch, RequestOptions options = null);
    /// <summary>
    ///     Gets a collection of messages in this channel.
    /// </summary>
    /// <remarks>
    ///     This method follows the same behavior as described in <see cref="IMessageChannel.GetMessagesAsync(IMessage, Direction, int, CacheMode, RequestOptions)"/>.
    ///     Please visit its documentation for more details on this method.
    /// </remarks>
    /// <param name="referenceMessage">The starting message to get the messages from.</param>
    /// <param name="dir">The direction of the messages to be gotten from.</param>
    /// <param name="limit">The numbers of message to be gotten from.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     Paged collection of messages.
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir, int limit = KaiHeiLaConfig.MaxMessagesPerBatch, RequestOptions options = null);
    
}