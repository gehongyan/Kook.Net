namespace Kook.WebSocket;

/// <summary>
///     Represents a generic WebSocket-based channel that can send and receive messages.
/// </summary>
public interface ISocketMessageChannel : IMessageChannel
{
    /// <summary>
    ///     Gets all messages in this channel's cache.
    /// </summary>
    /// <returns>
    ///     A read-only collection of WebSocket-based messages.
    /// </returns>
    IReadOnlyCollection<SocketMessage> CachedMessages { get; }

    /// <summary>
    ///     Gets a cached message from this channel.
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         This method requires the use of cache, which is not enabled by default; if caching is not enabled,
    ///         this method will always return <c>null</c>. Please refer to
    ///         <see cref="Kook.WebSocket.KookSocketConfig.MessageCacheSize" /> for more details.
    ///     </note>
    ///     <para>
    ///         This method retrieves the message from the local WebSocket cache and does not send any additional
    ///         request to Kook. This message may be a message that has been deleted.
    ///     </para>
    /// </remarks>
    /// <param name="id">The Guid of the message.</param>
    /// <returns>
    ///     A WebSocket-based message object; <c>null</c> if it does not exist in the cache or if caching is not
    ///     enabled.
    /// </returns>
    SocketMessage? GetCachedMessage(Guid id);

    /// <summary>
    ///     Gets the last N cached messages from this message channel.
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         This method requires the use of cache, which is not enabled by default; if caching is not enabled,
    ///         this method will always return an empty collection. Please refer to
    ///         <see cref="Kook.WebSocket.KookSocketConfig.MessageCacheSize" /> for more details.
    ///     </note>
    ///     <para>
    ///         This method retrieves the message(s) from the local WebSocket cache and does not send any additional
    ///         request to Kook. This read-only collection may include messages that have been deleted. The
    ///         maximum number of messages that can be retrieved from this method depends on the
    ///         <see cref="Kook.WebSocket.KookSocketConfig.MessageCacheSize" /> set.
    ///     </para>
    /// </remarks>
    /// <param name="limit">The number of messages to get.</param>
    /// <returns>
    ///     A read-only collection of WebSocket-based messages.
    /// </returns>
    IReadOnlyCollection<SocketMessage> GetCachedMessages(int limit = KookConfig.MaxMessagesPerBatch);

    /// <summary>
    ///     Gets the last N cached messages starting from a certain message in this message channel.
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         This method requires the use of cache, which is not enabled by default; if caching is not enabled,
    ///         this method will always return an empty collection. Please refer to
    ///         <see cref="Kook.WebSocket.KookSocketConfig.MessageCacheSize" /> for more details.
    ///     </note>
    ///     <para>
    ///         This method retrieves the message(s) from the local WebSocket cache and does not send any additional
    ///         request to Kook. This read-only collection may include messages that have been deleted. The
    ///         maximum number of messages that can be retrieved from this method depends on the
    ///         <see cref="Kook.WebSocket.KookSocketConfig.MessageCacheSize" /> set.
    ///     </para>
    /// </remarks>
    /// <param name="fromMessageId">The message ID to start the fetching from.</param>
    /// <param name="dir">The direction of which the message should be gotten from.</param>
    /// <param name="limit">The number of messages to get.</param>
    /// <returns>
    ///     A read-only collection of WebSocket-based messages.
    /// </returns>
    IReadOnlyCollection<SocketMessage> GetCachedMessages(Guid fromMessageId, Direction dir, int limit = KookConfig.MaxMessagesPerBatch);

    /// <summary>
    ///     Gets the last N cached messages starting from a certain message in this message channel.
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         This method requires the use of cache, which is not enabled by default; if caching is not enabled,
    ///         this method will always return an empty collection. Please refer to
    ///         <see cref="Kook.WebSocket.KookSocketConfig.MessageCacheSize" /> for more details.
    ///     </note>
    ///     <para>
    ///         This method retrieves the message(s) from the local WebSocket cache and does not send any additional
    ///         request to Kook. This read-only collection may include messages that have been deleted. The
    ///         maximum number of messages that can be retrieved from this method depends on the
    ///         <see cref="Kook.WebSocket.KookSocketConfig.MessageCacheSize" /> set.
    ///     </para>
    /// </remarks>
    /// <param name="fromMessage">The message to start the fetching from.</param>
    /// <param name="dir">The direction of which the message should be gotten from.</param>
    /// <param name="limit">The number of messages to get.</param>
    /// <returns>
    ///     A read-only collection of WebSocket-based messages.
    /// </returns>
    IReadOnlyCollection<SocketMessage> GetCachedMessages(IMessage fromMessage, Direction dir, int limit = KookConfig.MaxMessagesPerBatch);
}
