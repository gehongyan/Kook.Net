namespace Kook;

/// <summary>
///     表示一个通用的消息频道，可以用来发送和接收消息。
/// </summary>
public interface IMessageChannel : IChannel
{
    #region Send Messages

    /// <summary>
    ///     发送文件到此消息频道。
    /// </summary>
    /// <param name="path"> 文件的路径。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="ephemeralUser"> 临时消息的接收者。如果设置为指定的用户，则仅该用户可以看到此消息，否则所有人都可以看到此消息。。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendFileAsync(string path, string? filename = null,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, IUser? ephemeralUser = null,
        RequestOptions? options = null);

    /// <summary>
    ///     发送文件到此消息频道。
    /// </summary>
    /// <param name="stream"> 文件的流。 </param>
    /// <param name="filename"> 文件名。 </param>
    /// <param name="type"> 文件的媒体类型。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="ephemeralUser"> 临时消息的接收者。如果设置为指定的用户，则仅该用户可以看到此消息，否则所有人都可以看到此消息。。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendFileAsync(Stream stream, string filename,
        AttachmentType type = AttachmentType.File, IQuote? quote = null, IUser? ephemeralUser = null,
        RequestOptions? options = null);

    /// <summary>
    ///     发送文件到此消息频道。
    /// </summary>
    /// <param name="attachment"> 文件的附件信息。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="ephemeralUser"> 临时消息的接收者。如果设置为指定的用户，则仅该用户可以看到此消息，否则所有人都可以看到此消息。。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendFileAsync(FileAttachment attachment,
        IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null);

    /// <summary>
    ///     发送文本消息到此消息频道。
    /// </summary>
    /// <param name="text"> 要发送的文本。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="ephemeralUser"> 临时消息的接收者。如果设置为指定的用户，则仅该用户可以看到此消息，否则所有人都可以看到此消息。。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendTextAsync(string text, IQuote? quote = null,
        IUser? ephemeralUser = null, RequestOptions? options = null);

    /// <summary>
    ///     发送卡片消息到此消息频道。
    /// </summary>
    /// <param name="card"> 要发送的卡片。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="ephemeralUser"> 临时消息的接收者。如果设置为指定的用户，则仅该用户可以看到此消息，否则所有人都可以看到此消息。。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendCardAsync(ICard card,
        IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null);

    /// <summary>
    ///     发送卡片消息到此消息频道。
    /// </summary>
    /// <param name="cards"> 要发送的卡片。 </param>
    /// <param name="quote"> 消息引用，用于回复消息。 </param>
    /// <param name="ephemeralUser"> 临时消息的接收者。如果设置为指定的用户，则仅该用户可以看到此消息，否则所有人都可以看到此消息。。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步发送操作的任务。任务的结果包含所发送消息的可延迟加载的消息对象。
    /// </returns>
    Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(IEnumerable<ICard> cards,
        IQuote? quote = null, IUser? ephemeralUser = null, RequestOptions? options = null);

    #endregion

    #region Get Messages

    /// <summary>
    ///     从此消息频道获取一条消息。
    /// </summary>
    /// <param name="id"> 消息的 ID。 </param>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步获取操作的额任务。任务结果包含检索到的消息；如果未找到具有指定 ID 的消息，则返回 <c>null</c>。
    /// </returns>
    Task<IMessage?> GetMessageAsync(Guid id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此消息频道中的最新的一些消息。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用 <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/>
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多消息，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此频道最新的 <paramref name="limit"/> 条消息。此方法会根据 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条消息，而 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。<see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/>
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     分页的消息集合的异步可枚举对象。
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = KookConfig.MaxMessagesPerBatch,
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此消息频道中的一些消息。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用 <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/>
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多消息，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此频道最新的 <paramref name="limit"/> 条消息。此方法会根据 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条消息，而 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。<see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/>
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referenceMessageId"> 要开始获取消息的参考位置的消息的 ID。 </param>
    /// <param name="dir"> 要以参考位置为基准，获取消息的方向。 </param>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     分页的消息集合的异步可枚举对象。
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch,
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    /// <summary>
    ///     获取此消息频道中的一些消息。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用 <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/>
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多消息，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此频道最新的 <paramref name="limit"/> 条消息。此方法会根据 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条消息，而 <see cref="F:Kook.KookConfig.MaxMessagesPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。<see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/>
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referenceMessage"> 要开始获取消息的参考位置的消息。 </param>
    /// <param name="dir"> 要以参考位置为基准，获取消息的方向。 </param>
    /// <param name="limit"> 要获取的消息数量。 </param>
    /// <param name="mode"> 指示当前方法是否应该仅从缓存中获取结果，还是可以通过 API 请求获取数据。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     分页的消息集合的异步可枚举对象。
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch,
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null);

    #endregion

    #region Delete Messages

    /// <summary>
    ///     删除一条消息。
    /// </summary>
    /// <param name="messageId"> 要删除的消息的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步删除操作的任务。
    /// </returns>
    Task DeleteMessageAsync(Guid messageId, RequestOptions? options = null);

    /// <summary> 删除一条消息. </summary>
    /// <param name="message"> 要删除的消息。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步删除操作的任务。
    /// </returns>
    Task DeleteMessageAsync(IMessage message, RequestOptions? options = null);

    #endregion

    #region Modify Messages

    /// <summary>
    ///     修改一条消息。
    /// </summary>
    /// <param name="messageId"> 要修改的消息的 ID。 </param>
    /// <param name="func"> 一个包含修改消息属性的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     一个表示异步修改操作的任务。
    /// </returns>
    /// <seealso cref="T:Kook.MessageProperties"/>
    Task ModifyMessageAsync(Guid messageId, Action<MessageProperties> func, RequestOptions? options = null);

    #endregion
}
