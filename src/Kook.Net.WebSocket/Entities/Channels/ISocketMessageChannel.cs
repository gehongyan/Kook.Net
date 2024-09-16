namespace Kook.WebSocket;

/// <summary>
///    表示一个基于网关的消息频道，可以用来发送和接收消息。
/// </summary>
public interface ISocketMessageChannel : IMessageChannel
{
    /// <summary>
    ///     获取此频道缓存的所有消息。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         要想通过此属性获取缓存的消息，需要启用缓存功能，否则此属性将始终返回空集合。缓存功能是默认禁用的，要想启用缓存，请参考
    ///         <see cref="Kook.WebSocket.KookSocketConfig.MessageCacheSize"/>。
    ///     </note>
    ///     <br />
    ///     此属性从本地的内存缓存中获取消息实体，不会向 KOOK 发送额外的 API 请求。所获取的消息也可能是已经被删除的消息。
    /// </remarks>
    IReadOnlyCollection<SocketMessage> CachedMessages { get; }

    /// <summary>
    ///     获取此频道缓存的消息。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         要想通过此方法获取缓存的消息，需要启用缓存功能，否则此方法将始终返回 <c>null</c>。缓存功能是默认禁用的，要想启用缓存，请参考
    ///         <see cref="Kook.WebSocket.KookSocketConfig.MessageCacheSize"/>。
    ///     </note>
    ///     <br />
    ///     此方法从本地的内存缓存中获取消息实体，不会向 KOOK 发送额外的 API 请求。所获取的消息也可能是已经被删除的消息。
    /// </remarks>
    /// <param name="id"> 消息的 ID。 </param>
    /// <returns>
    ///     如果获取到了缓存的消息，则返回该消息实体；否则返回 <c>null</c>。
    /// </returns>
    SocketMessage? GetCachedMessage(Guid id);

    /// <summary>
    ///     获取此频道缓存的多条消息。
    /// </summary>
    /// <param name="limit"> 要获取的缓存消息的数量。 </param>
    /// <remarks>
    ///     此重载将会从缓存中获取最新的指定数量的缓存消息实体。
    ///     <br />
    ///     <note type="warning">
    ///         要想通过此方法获取缓存的消息，需要启用缓存功能，否则此方法将始终返回空集合。缓存功能是默认禁用的，要想启用缓存，请参考
    ///         <see cref="Kook.WebSocket.KookSocketConfig.MessageCacheSize"/>。
    ///     </note>
    ///     <br />
    ///     此方法从本地的内存缓存中获取消息实体，不会向 KOOK 发送额外的 API 请求。所获取的消息也可能是已经被删除的消息。
    /// </remarks>
    /// <returns> 此频道缓存的所有消息。 </returns>
    IReadOnlyCollection<SocketMessage> GetCachedMessages(int limit = KookConfig.MaxMessagesPerBatch);

    /// <summary>
    ///     获取此频道缓存的多条消息。
    /// </summary>
    /// <param name="referenceMessageId"> 要开始获取消息的参考位置的消息的 ID。 </param>
    /// <param name="dir"> 要以参考位置为基准，获取消息的方向。 </param>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <remarks>
    ///     <note type="warning">
    ///         要想通过此方法获取缓存的消息，需要启用缓存功能，否则此方法将始终返回空集合。缓存功能是默认禁用的，要想启用缓存，请参考
    ///         <see cref="Kook.WebSocket.KookSocketConfig.MessageCacheSize"/>。
    ///     </note>
    ///     <br />
    ///     此方法从本地的内存缓存中获取消息实体，不会向 KOOK 发送额外的 API 请求。所获取的消息也可能是已经被删除的消息。
    /// </remarks>
    /// <returns> 获取到的多条缓存消息。 </returns>
    IReadOnlyCollection<SocketMessage> GetCachedMessages(Guid referenceMessageId, Direction dir, int limit = KookConfig.MaxMessagesPerBatch);

    /// <summary>
    ///     获取此频道缓存的多条消息。
    /// </summary>
    /// <param name="referenceMessage"> 要开始获取消息的参考位置的消息。 </param>
    /// <param name="dir"> 要以参考位置为基准，获取消息的方向。 </param>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <remarks>
    ///     <note type="warning">
    ///         要想通过此方法获取缓存的消息，需要启用缓存功能，否则此方法将始终返回空集合。缓存功能是默认禁用的，要想启用缓存，请参考
    ///         <see cref="Kook.WebSocket.KookSocketConfig.MessageCacheSize"/>。
    ///     </note>
    ///     <br />
    ///     此方法从本地的内存缓存中获取消息实体，不会向 KOOK 发送额外的 API 请求。所获取的消息也可能是已经被删除的消息。
    /// </remarks>
    /// <returns> 获取到的多条缓存消息。 </returns>
    IReadOnlyCollection<SocketMessage> GetCachedMessages(IMessage referenceMessage, Direction dir, int limit = KookConfig.MaxMessagesPerBatch);
}
