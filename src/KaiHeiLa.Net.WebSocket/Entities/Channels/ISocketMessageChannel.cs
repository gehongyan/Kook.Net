using KaiHeiLa.Rest;

namespace KaiHeiLa.WebSocket
{
    /// <summary>
    ///     Represents a generic WebSocket-based channel that can send and receive messages.
    /// </summary>
    public interface ISocketMessageChannel : IMessageChannel
    {
        /// <summary>
        ///     Gets a cached message from this channel.
        /// </summary>
        /// <remarks>
        ///     <note type="warning">
        ///         This method requires the use of cache, which is not enabled by default; if caching is not enabled,
        ///         this method will always return <c>null</c>. Please refer to
        ///         <see cref="KaiHeiLa.WebSocket.KaiHeiLaSocketConfig.MessageCacheSize" /> for more details.
        ///     </note>
        ///     <para>
        ///         This method retrieves the message from the local WebSocket cache and does not send any additional
        ///         request to KaiHeiLa. This message may be a message that has been deleted.
        ///     </para>
        /// </remarks>
        /// <param name="id">The Guid of the message.</param>
        /// <returns>
        ///     A WebSocket-based message object; <c>null</c> if it does not exist in the cache or if caching is not
        ///     enabled.
        /// </returns>
        SocketMessage GetCachedMessage(Guid id);
    }
}
