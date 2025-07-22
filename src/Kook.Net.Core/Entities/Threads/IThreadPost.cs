namespace Kook;

/// <summary>
///     表示一个通用的帖子评论。
/// </summary>
public interface IThreadPost : IEntity<ulong>, IDeletable
{
    /// <summary>
    ///     获取此帖子评论所在的帖子。
    /// </summary>
    IThread Thread { get; }

    /// <summary>
    ///     获取此帖子评论的内容。
    /// </summary>
    /// <remarks>
    ///     此属性为卡片内容的原始代码。
    /// </remarks>
    string Content { get; }

    /// <summary>
    ///     获取此帖子评论中包含的所有卡片。
    /// </summary>
    IReadOnlyCollection<ICard> Cards { get; }

    /// <summary>
    ///     获取此帖子评论所包含的所有附件。
    /// </summary>
    IReadOnlyCollection<IAttachment> Attachments { get; }

    /// <summary>
    ///     获取此帖子评论中提及的所有用户的 ID。
    /// </summary>
    IReadOnlyCollection<ulong> MentionedUserIds { get; }

    /// <summary>
    ///     获取此帖子评论中提及的所有角色的 ID。
    /// </summary>
    IReadOnlyCollection<uint> MentionedRoleIds { get; }

    /// <summary>
    ///     获取此帖子评论中提及的所有频道。
    /// </summary>
    IReadOnlyCollection<ulong> MentionedChannelIds { get; }

    /// <summary>
    ///     获取此帖子评论是否提及了全体成员。
    /// </summary>
    bool MentionedEveryone { get; }

    /// <summary>
    ///     获取此帖子评论是否提及了在线成员。
    /// </summary>
    bool MentionedHere { get; }

    /// <summary>
    ///     获取此帖子评论的作者。
    /// </summary>
    IUser Author { get; }

    /// <summary>
    ///     获取此帖子的发布时间。
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    ///     获取此帖子评论是否已被编辑。
    /// </summary>
    bool IsEdited { get; }

    /// <summary>
    ///     获取此帖子评论内的帖子回复。
    /// </summary>
    /// <remarks>
    ///     此属性仅包含当前帖子评论内最多 2 条帖子回复，要获取所有帖子回复，请访问
    ///     <see cref="Kook.IThreadPost.GetRepliesAsync(System.Int32,Kook.RequestOptions)"/>
    ///     及其重载方法。
    /// </remarks>
    IReadOnlyCollection<IThreadReply> Replies { get; }

    /// <summary>
    ///     获取此帖子评论中的最新的一些回复。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多回复，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此最新的 <paramref name="limit"/> 条回复。此方法会根据 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 300 条回复，而 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/> 的常量为
    ///     <c>30</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="limit"> 要获取的回复数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的回复集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<IThreadReply>> GetRepliesAsync(
        int limit = KookConfig.MaxThreadsPerBatch, RequestOptions? options = null);

    /// <summary>
    ///     获取此帖子评论中的一些回复。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多回复，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此最新的 <paramref name="limit"/> 条回复。此方法会根据 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 300 条回复，而 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/> 的常量为
    ///     <c>30</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referenceTimestamp"> </param>
    /// <param name="sortMode"> 要以参考位置为基准，获取回复的排序方式。 </param>
    /// <param name="limit"> 要获取的回复数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的回复集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<IThreadReply>> GetRepliesAsync(DateTimeOffset referenceTimestamp,
        SortMode sortMode = SortMode.Ascending, int limit = KookConfig.MaxThreadsPerBatch,
        RequestOptions? options = null);

    /// <summary>
    ///     获取此帖子评论中的一些回复。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多回复，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此最新的 <paramref name="limit"/> 条回复。此方法会根据 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 300 条回复，而 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/> 的常量为
    ///     <c>30</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referenceReply"> 要开始获取回复的参考位置的回复，获取的结果不包含此回复。 </param>
    /// <param name="sortMode"> 要以参考位置为基准，获取回复的排序方式。 </param>
    /// <param name="limit"> 要获取的回复数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的回复集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<IThreadReply>> GetRepliesAsync(IThreadReply referenceReply,
        SortMode sortMode = SortMode.Ascending, int limit = KookConfig.MaxThreadsPerBatch,
        RequestOptions? options = null);

    /// <summary>
    ///     在此帖子评论中创建一条帖子回复。
    /// </summary>
    /// <param name="content"> 要发布的文本内容。 </param>
    /// <param name="isKMarkdown"> 是否为 KMarkdown 格式。 </param>
    /// <param name="referenceReplyId"> 在发布回复时要回复的帖子回复的 ID。如果未指定，则不会回复任何帖子回复。</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示新创建帖子评论的回复操作的异步任务。 </returns>
    Task<IThreadReply> CreateReplyAsync(string content, bool isKMarkdown = false,
        ulong? referenceReplyId = null, RequestOptions? options = null);

    /// <summary>
    ///     在此帖子评论中创建一条帖子回复。
    /// </summary>
    /// <param name="card"> 要发布的卡片内容。 </param>
    /// <param name="referenceReplyId"> 在发布回复时要回复的帖子回复的 ID。如果未指定，则不会回复任何帖子回复。</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示新创建帖子评论的回复操作的异步任务。 </returns>
    /// <remarks>
    ///     卡片内仅允许包含文本内容。
    /// </remarks>
    Task<IThreadReply> CreateReplyAsync(ICard card, ulong? referenceReplyId = null, RequestOptions? options = null);

    /// <summary>
    ///     在此帖子评论中创建一条帖子回复。
    /// </summary>
    /// <param name="cards"> 要发布的卡片内容。 </param>
    /// <param name="referenceReplyId"> 在发布回复时要回复的帖子回复的 ID。如果未指定，则不会回复任何帖子回复。</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示新创建帖子评论的回复操作的异步任务。 </returns>
    /// <remarks>
    ///     卡片内仅允许包含文本内容。
    /// </remarks>
    Task<IThreadReply> CreateReplyAsync(IEnumerable<ICard> cards,
        ulong? referenceReplyId = null, RequestOptions? options = null);

    /// <summary>
    ///     在此帖子评论中删除一条帖子回复。
    /// </summary>
    /// <param name="replyId"> 要删除的帖子评论的回复的 ID。</param>
    /// <param name="options"> 发送请求时要使用的选项。</param>
    /// <returns> 一个表示帖子评论的回复删除操作的异步任务。</returns>
    Task DeleteReplyAsync(ulong replyId, RequestOptions? options = null);

    /// <summary>
    ///     在此帖子评论中删除一条帖子回复。
    /// </summary>
    /// <param name="reply"> 要删除的帖子评论的回复。</param>
    /// <param name="options"> 发送请求时要使用的选项。</param>
    /// <returns> 一个表示帖子评论的回复删除操作的异步任务。</returns>
    Task DeleteReplyAsync(IThreadReply reply, RequestOptions? options = null);
}
