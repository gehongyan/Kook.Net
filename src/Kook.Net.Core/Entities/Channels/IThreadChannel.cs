namespace Kook;

/// <summary>
///     表示服务器中一个通用的帖子频道，可以浏览、发布和回复帖子。
/// </summary>
public interface IThreadChannel : INestedChannel, IMentionable
{
    /// <summary>
    ///     获取此频道的说明。
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     获取此频道设置的发帖速率限制。
    /// </summary>
    /// <remarks>
    ///     拥有 <see cref="Kook.ChannelPermission.ManageMessages"/> 或
    ///     <see cref="Kook.ChannelPermission.ManageChannels"/> 权限的用户不受慢速模式延迟的限制。
    /// </remarks>
    /// <returns> 一个 <c>int</c>，表示用户在可以发布另一条帖子之前需要等待的时间（以秒为单位）；如果未启用，则为 <c>0</c>。 </returns>
    int PostCreationInterval { get; }

    /// <summary>
    ///     获取此频道设置的回帖速率限制。
    /// </summary>
    /// <remarks>
    ///     拥有 <see cref="Kook.ChannelPermission.ManageMessages"/> 或
    ///     <see cref="Kook.ChannelPermission.ManageChannels"/> 权限的用户不受慢速模式延迟的限制。
    /// </remarks>
    /// <returns> 一个 <c>int</c>，表示用户在可以对任意帖子发布另一条回复之前需要等待的时间（以秒为单位）；如果未启用，则为 <c>0</c>。 </returns>
    int? ReplyInterval { get; }

    /// <summary>
    ///     获取此频道设置的帖子默认布局。
    /// </summary>
    ThreadLayout? DefaultLayout { get; }

    /// <summary>
    ///     获取此频道设置的帖子默认排序。
    /// </summary>
    ThreadSortOrder? DefaultSortOrder { get; }

    /// <summary>
    ///     修改此频道的属性。
    /// </summary>
    /// <param name="func"> 一个包含修改帖子频道的属性的委托。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。 </returns>
    /// <seealso cref="Kook.ModifyThreadChannelProperties"/>
    Task ModifyAsync(Action<ModifyThreadChannelProperties> func, RequestOptions? options = null);

    /// <summary>
    ///     获取此帖子频道的所有帖子分区。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。任务的结果包含此帖子频道的所有帖子分区。 </returns>
    Task<IReadOnlyCollection<IThreadCategory>> GetThreadCategoriesAsync(RequestOptions? options = null);

    #region Get Threads

    /// <summary>
    ///     从此帖子频道获取一个帖子。
    /// </summary>
    /// <param name="id"> 帖子的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务结果包含具有指定 ID 的帖子。 </returns>
    Task<IThread> GetThreadAsync(ulong id, RequestOptions? options = null);

    /// <summary>
    ///     获取此帖子频道中的最新的一些帖子。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多帖子，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此频道最新的 <paramref name="limit"/> 条帖子。此方法会根据 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 300 条帖子，而 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/> 的常量为
    ///     <c>30</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="limit"> 要获取的帖子数量。 </param>
    /// <param name="category"> 要获取的帖子所在的分区，如果为 <c>null</c>，则获取所有分区的帖子。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的帖子集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<IThread>> GetThreadsAsync(int limit = KookConfig.MaxThreadsPerBatch,
        IThreadCategory? category = null, RequestOptions? options = null);

    /// <summary>
    ///     获取此帖子频道中的一些帖子。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多帖子，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此频道最新的 <paramref name="limit"/> 条帖子。此方法会根据 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条帖子，而 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referenceTimestamp"> 要开始获取帖子的参考位置的帖子的创建时间或最后活跃时间，由 <paramref name="sortOrder"/> 参数决定，获取的结果不包含其时间为此值的帖子。 </param>
    /// <param name="sortOrder"> 获取帖子列表的排序方式。 </param>
    /// <param name="limit"> 要获取的帖子数量。 </param>
    /// <param name="category"> 要获取的帖子所在的分区，如果为 <c>null</c>，则获取所有分区的帖子。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的帖子集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<IThread>> GetThreadsAsync(DateTimeOffset referenceTimestamp,
        ThreadSortOrder sortOrder = ThreadSortOrder.CreationTime, int limit = KookConfig.MaxThreadsPerBatch,
        IThreadCategory? category = null, RequestOptions? options = null);

    /// <summary>
    ///     获取此帖子频道中的一些帖子。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多帖子，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此频道最新的 <paramref name="limit"/> 条帖子。此方法会根据 <see cref="Kook.KookConfig.MaxMessagesPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条帖子，而 <see cref="Kook.KookConfig.MaxMessagesPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referenceThread"> 要开始获取帖子的参考位置的帖子，获取的结果不包含此帖子。 </param>
    /// <param name="sortOrder"> 获取帖子列表的排序方式。 </param>
    /// <param name="limit"> 要获取的帖子数量。 </param>
    /// <param name="category"> 要获取的帖子所在的分区，如果为 <c>null</c>，则获取所有分区的帖子。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的帖子集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<IThread>> GetThreadsAsync(IThread referenceThread,
        ThreadSortOrder sortOrder = ThreadSortOrder.CreationTime, int limit = KookConfig.MaxThreadsPerBatch,
        IThreadCategory? category = null, RequestOptions? options = null);

    #endregion

    #region Create Threads

    /// <summary>
    ///     发布一个新的帖子到此帖子频道。
    /// </summary>
    /// <param name="title"> 帖子标题。 </param>
    /// <param name="content"> 帖子文本内容，文本将会被包装在无侧边卡片内发送。 </param>
    /// <param name="cover"> 帖子封面的图片链接。 </param>
    /// <param name="category"> 帖子的分区。 </param>
    /// <param name="tags"> 帖子的话题标签。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。任务的结果包含新创建的帖子。 </returns>
    Task<IThread> CreateThreadAsync(string title, string content, string? cover = null,
        IThreadCategory? category = null, ThreadTag[]? tags = null, RequestOptions? options = null);

    /// <summary>
    ///     发布一个新的帖子到此帖子频道。
    /// </summary>
    /// <param name="title"> 帖子标题。 </param>
    /// <param name="card"> 帖子的卡片内容。 </param>
    /// <param name="cover"> 帖子封面的图片链接。 </param>
    /// <param name="category"> 帖子的分区。 </param>
    /// <param name="tags"> 帖子的话题标签。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。任务的结果包含新创建的帖子。 </returns>
    Task<IThread> CreateThreadAsync(string title, ICard card, string? cover = null,
        IThreadCategory? category = null, ThreadTag[]? tags = null, RequestOptions? options = null);

    /// <summary>
    ///     发布一个新的帖子到此帖子频道。
    /// </summary>
    /// <param name="title"> 帖子标题。 </param>
    /// <param name="cards"> 帖子的卡片内容。 </param>
    /// <param name="cover"> 帖子封面的图片链接。 </param>
    /// <param name="category"> 帖子的分区。 </param>
    /// <param name="tags"> 帖子的话题标签。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。任务的结果包含新创建的帖子。 </returns>
    Task<IThread> CreateThreadAsync(string title, IEnumerable<ICard> cards, string? cover = null,
        IThreadCategory? category = null, ThreadTag[]? tags = null, RequestOptions? options = null);

    #endregion

    #region Delete Threads

    /// <summary>
    ///     删除一个帖子。
    /// </summary>
    /// <param name="threadId"> 要删除的帖子的 ID。</param>
    /// <param name="options"> 发送请求时要使用的选项。</param>
    /// <returns> 一个表示异步删除操作的任务。 </returns>
    Task DeleteThreadAsync(ulong threadId, RequestOptions? options = null);

    /// <summary>
    ///     删除一个帖子。
    /// </summary>
    /// <param name="thread"> 要删除的帖子。</param>
    /// <param name="options"> 发送请求时要使用的选项。</param>
    /// <returns> 一个表示异步删除操作的任务。 </returns>
    Task DeleteThreadAsync(IThread thread, RequestOptions? options = null);

    /// <summary>
    ///     删除一个帖子的主楼内容。
    /// </summary>
    /// <param name="threadId"> 要删除其主楼内容的帖子的 ID。</param>
    /// <param name="options"> 发送请求时要使用的选项。</param>
    /// <returns> 一个表示异步删除操作的任务。 </returns>
    /// <remarks>
    ///     当帖子无任何评论时，删除主楼内容会导致帖子被删除。
    /// </remarks>
    Task DeleteThreadContentAsync(ulong threadId, RequestOptions? options = null);

    /// <summary>
    ///     删除一个帖子的主楼内容。
    /// </summary>
    /// <param name="thread"> 要删除其主楼内容的帖子。</param>
    /// <param name="options"> 发送请求时要使用的选项。</param>
    /// <returns> 一个表示异步删除操作的任务。 </returns>
    /// <remarks>
    ///     当帖子无任何评论时，删除主楼内容会导致帖子被删除。
    /// </remarks>
    Task DeleteThreadContentAsync(IThread thread, RequestOptions? options = null);

    #endregion
}
