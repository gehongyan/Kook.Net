namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的通用的消息频道，可以用来发送和接收消息。
/// </summary>
public interface IRestMessageChannel : IMessageChannel
{
    /// <summary>
    ///     从此消息频道获取一条消息。
    /// </summary>
    /// <param name="id"> 消息的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务结果包含检索到的消息；如果未找到具有指定 ID 的消息，则返回 <c>null</c>。 </returns>
    Task<RestMessage> GetMessageAsync(Guid id, RequestOptions? options = null);

    /// <summary>
    ///     获取此消息频道中的最新的一些消息。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多消息，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此频道最新的 <paramref name="limit"/> 条消息。此方法会根据 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条消息，而 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的消息集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null);

    /// <summary>
    ///     获取此消息频道中的一些消息。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多消息，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此频道最新的 <paramref name="limit"/> 条消息。此方法会根据 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条消息，而 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referenceMessageId"> 要开始获取消息的参考位置的消息的 ID。 </param>
    /// <param name="dir"> 要以参考位置为基准，获取消息的方向。 </param>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的消息集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null);

    /// <summary>
    ///     获取此消息频道中的一些消息。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多消息，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此频道最新的 <paramref name="limit"/> 条消息。此方法会根据 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条消息，而 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="M:Kook.AsyncEnumerableExtensions.FlattenAsync``1(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{``0}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referenceMessage"> 要开始获取消息的参考位置的消息。 </param>
    /// <param name="dir"> 要以参考位置为基准，获取消息的方向。 </param>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的消息集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null);
}
