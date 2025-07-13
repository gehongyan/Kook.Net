namespace Kook;

/// <summary>
///     表示一个通用的帖子分区。
/// </summary>
public interface IThreadCategory : IEntity<ulong>
{
    /// <summary>
    ///     获取帖子分区的名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     获取此帖子分区的角色的所有权限重写配置。
    /// </summary>
    IReadOnlyCollection<RolePermissionOverwrite> RolePermissionOverwrites { get; }

    /// <summary>
    ///     获取此帖子分区的用户的所有权限重写配置。
    /// </summary>
    IReadOnlyCollection<UserPermissionOverwrite> UserPermissionOverwrites { get; }

    #region Get Threads


    /// <summary>
    ///     获取此帖子分区中的最新的一些帖子。
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
    ///     此方法将尝试获取此分区最新的 <paramref name="limit"/> 条帖子。此方法会根据 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 300 条帖子，而 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/> 的常量为
    ///     <c>30</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="limit"> 要获取的帖子数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的帖子集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<IThread>> GetThreadsAsync(int limit = KookConfig.MaxThreadsPerBatch,
        RequestOptions? options = null);

    /// <summary>
    ///     获取此帖子分区中的一些帖子。
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
    ///     此方法将尝试获取此分区最新的 <paramref name="limit"/> 条帖子。此方法会根据 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条帖子，而 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referenceTimestamp"> 要开始获取帖子的参考位置的帖子的创建时间或最后活跃时间，由 <paramref name="sortOrder"/> 参数决定，获取的结果不包含其时间为此值的帖子。 </param>
    /// <param name="sortOrder"> 获取帖子列表的排序方式。 </param>
    /// <param name="limit"> 要获取的帖子数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的帖子集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<IThread>> GetThreadsAsync(DateTimeOffset referenceTimestamp,
        ThreadSortOrder sortOrder = ThreadSortOrder.CreationTime, int limit = KookConfig.MaxThreadsPerBatch,
        RequestOptions? options = null);

    /// <summary>
    ///     获取此帖子分区中的一些帖子。
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
    ///     此方法将尝试获取此分区最新的 <paramref name="limit"/> 条帖子。此方法会根据 <see cref="Kook.KookConfig.MaxMessagesPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 500 条帖子，而 <see cref="Kook.KookConfig.MaxMessagesPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referenceThread"> 要开始获取帖子的参考位置的帖子，获取的结果不包含此帖子。 </param>
    /// <param name="sortOrder"> 获取帖子列表的排序方式。 </param>
    /// <param name="limit"> 要获取的帖子数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的帖子集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<IThread>> GetThreadsAsync(IThread referenceThread,
        ThreadSortOrder sortOrder = ThreadSortOrder.CreationTime, int limit = KookConfig.MaxThreadsPerBatch,
        RequestOptions? options = null);

    #endregion

    #region Create Threads

    /// <summary>
    ///     发布一个新的帖子到此帖子分区。
    /// </summary>
    /// <param name="title"> 帖子标题。 </param>
    /// <param name="content"> 帖子文本内容，文本将会被包装在无侧边卡片内发送。 </param>
    /// <param name="cover"> 帖子封面的图片链接。 </param>
    /// <param name="tags"> 帖子的话题标签。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。任务的结果包含新创建的帖子。 </returns>
    Task<IThread> CreateThreadAsync(string title, string content, string? cover = null,
        ThreadTag[]? tags = null, RequestOptions? options = null);

    /// <summary>
    ///     发布一个新的帖子到此帖子分区。
    /// </summary>
    /// <param name="title"> 帖子标题。 </param>
    /// <param name="card"> 帖子的卡片内容。 </param>
    /// <param name="cover"> 帖子封面的图片链接。 </param>
    /// <param name="tags"> 帖子的话题标签。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。任务的结果包含新创建的帖子。 </returns>
    Task<IThread> CreateThreadAsync(string title, ICard card, string? cover = null,
        ThreadTag[]? tags = null, RequestOptions? options = null);

    /// <summary>
    ///     发布一个新的帖子到此帖子分区。
    /// </summary>
    /// <param name="title"> 帖子标题。 </param>
    /// <param name="cards"> 帖子的卡片内容。 </param>
    /// <param name="cover"> 帖子封面的图片链接。 </param>
    /// <param name="tags"> 帖子的话题标签。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步修改操作的任务。任务的结果包含新创建的帖子。 </returns>
    Task<IThread> CreateThreadAsync(string title, IEnumerable<ICard> cards, string? cover = null,
        ThreadTag[]? tags = null, RequestOptions? options = null);

    #endregion
}
