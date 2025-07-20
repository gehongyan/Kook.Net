namespace Kook;

/// <summary>
///     表示一个通用的帖子。
/// </summary>
public interface IThread : IEntity<ulong>, IDeletable
{
    /// <summary>
    ///     获取此帖子所在的服务器。
    /// </summary>
    IGuild Guild { get; }

    /// <summary>
    ///     获取此帖子所在的频道。
    /// </summary>
    IThreadChannel Channel { get; }

    /// <summary>
    ///     获取此帖子的审核状态。
    /// </summary>
    ThreadAuditStatus AuditStatus { get; }

    /// <summary>
    ///     获取此帖子的标题。
    /// </summary>
    string Title { get; }

    /// <summary>
    ///     获取此帖子的封面图片链接。
    /// </summary>
    string? Cover { get; }

    /// <summary>
    ///     获取此帖子主楼的评论 ID。
    /// </summary>
    /// <remarks>
    ///     帖子的发布者在发布帖子时所发布的内容为主楼。
    /// </remarks>
    ulong PostId { get; }

    /// <summary>
    ///     获取此帖子所包含的所有附件。
    /// </summary>
    IReadOnlyCollection<IAttachment> Attachments { get; }

    /// <summary>
    ///     获取此帖子的作者。
    /// </summary>
    IUser Author { get; }

    /// <summary>
    ///     获取此帖子的所属分区。
    /// </summary>
    IThreadCategory? Category { get; }

    /// <summary>
    ///     获取此帖子所包含的所有话题标签。
    /// </summary>
    IReadOnlyCollection<ThreadTag> ThreadTags { get; }

    /// <summary>
    ///     获取此帖子的内容。
    /// </summary>
    /// <remarks>
    ///     此属性为卡片内容的原始代码。
    /// </remarks>
    string Content { get; }

    /// <summary>
    ///     获取此帖子内容的预览文本。
    /// </summary>
    string PreviewContent { get; }

    /// <summary>
    ///     获取此帖子中提及的所有用户的 ID。
    /// </summary>
    IReadOnlyCollection<ulong> MentionedUserIds { get; }

    /// <summary>
    ///     获取此帖子中提及的所有角色的 ID。
    /// </summary>
    IReadOnlyCollection<uint> MentionedRoleIds { get; }

    /// <summary>
    ///     获取此帖子中提及的所有频道。
    /// </summary>
    IReadOnlyCollection<ulong> MentionedChannelIds { get; }

    /// <summary>
    ///     获取此帖子是否提及了全体成员。
    /// </summary>
    bool MentionedEveryone { get; }

    /// <summary>
    ///     获取此帖子是否提及了在线成员。
    /// </summary>
    bool MentionedHere { get; }

    /// <summary>
    ///     获取此帖子中包含的所有卡片。
    /// </summary>
    IReadOnlyCollection<ICard> Cards { get; }

    /// <summary>
    ///     获取此帖子的发布时间。
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    ///     获取此帖子最后一次活跃的时间。
    /// </summary>
    DateTimeOffset LatestActiveTimestamp { get; }

    /// <summary>
    ///     获取此帖子是否已被编辑。
    /// </summary>
    bool IsEdited { get; }

    /// <summary>
    ///     获取此帖子的内容是否已被删除。
    /// </summary>
    bool IsContentDeleted { get; }

    /// <summary>
    ///     获取此帖子内容被删除的原因。
    /// </summary>
    ThreadContentDeletedBy ContentDeletedBy { get; }

    /// <summary>
    ///     获取收藏此帖子的人数。
    /// </summary>
    int FavoriteCount { get; }

    /// <summary>
    ///     获取此帖子的评论数量。
    /// </summary>
    int PostCount { get; }

    /// <summary>
    ///     获取此帖子中的最新的一些评论。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多评论，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此最新的 <paramref name="limit"/> 条评论。此方法会根据 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 300 条评论，而 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/> 的常量为
    ///     <c>30</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="limit"> 要获取的评论数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的评论集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<IThreadPost>> GetPostsAsync(
        int limit = KookConfig.MaxThreadsPerBatch, RequestOptions? options = null);

    /// <summary>
    ///     获取此帖子中的一些评论。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多评论，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此最新的 <paramref name="limit"/> 条评论。此方法会根据 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 300 条评论，而 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/> 的常量为
    ///     <c>30</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referenceTimestamp"> </param>
    /// <param name="sortMode"> 要以参考位置为基准，获取评论的排序方式。 </param>
    /// <param name="limit"> 要获取的评论数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的评论集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<IThreadPost>> GetPostsAsync(DateTimeOffset referenceTimestamp,
        SortMode sortMode = SortMode.Ascending, int limit = KookConfig.MaxThreadsPerBatch,
        RequestOptions? options = null);

    /// <summary>
    ///     获取此帖子中的一些评论。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多评论，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取此最新的 <paramref name="limit"/> 条评论。此方法会根据 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/>
    ///     将请求拆分。换句话说，如果要获取 300 条评论，而 <see cref="Kook.KookConfig.MaxThreadsPerBatch"/> 的常量为
    ///     <c>30</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="referencePost"> 要开始获取评论的参考位置的评论，获取的结果不包含此评论。 </param>
    /// <param name="sortMode"> 要以参考位置为基准，获取评论的排序方式。 </param>
    /// <param name="limit"> 要获取的评论数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的评论集合的异步可枚举对象。 </returns>
    IAsyncEnumerable<IReadOnlyCollection<IThreadPost>> GetPostsAsync(IThreadPost referencePost,
        SortMode sortMode = SortMode.Ascending, int limit = KookConfig.MaxThreadsPerBatch,
        RequestOptions? options = null);

    /// <summary>
    ///     在此帖子中创建一条帖子评论。
    /// </summary>
    /// <param name="content"> 要发布的文本内容。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示新创建帖子评论操作的异步任务。 </returns>
    Task<IThreadPost> CreatePostAsync(string content, RequestOptions? options = null);

    /// <summary>
    ///     在此帖子中创建一条帖子评论。
    /// </summary>
    /// <param name="card"> 要发布的卡片内容。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示新创建帖子评论操作的异步任务。 </returns>
    Task<IThreadPost> CreatePostAsync(ICard card, RequestOptions? options = null);

    /// <summary>
    ///     在此帖子中创建一条帖子评论。
    /// </summary>
    /// <param name="cards"> 要发布的卡片内容。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示新创建帖子评论操作的异步任务。 </returns>
    Task<IThreadPost> CreatePostAsync(IEnumerable<ICard> cards, RequestOptions? options = null);

    /// <summary>
    ///     删除此帖子的主楼内容。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示帖子主楼内容删除操作的异步任务。 </returns>
    /// <remarks>
    ///     当帖子无任何评论时，删除主楼内容会导致帖子被删除。
    /// </remarks>
    Task DeleteContentAsync(RequestOptions? options = null);

    /// <summary>
    ///     删除此帖子内的一条帖子评论。
    /// </summary>
    /// <param name="postId"> 要删除的帖子评论的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示帖子评论删除操作的异步任务。 </returns>
    /// <remarks>
    ///     如果帖子的主楼内容已被删除，且此帖子评论是所属帖子的唯一一条评论，则删除此帖子评论会导致该帖子也被删除。
    /// </remarks>
    Task DeletePostAsync(ulong postId, RequestOptions? options = null);

    /// <summary>
    ///     删除此帖子内的一条帖子评论。
    /// </summary>
    /// <param name="post"> 要删除的帖子评论。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示帖子评论删除操作的异步任务。 </returns>
    /// <remarks>
    ///     如果帖子的主楼内容已被删除，且此帖子评论是所属帖子的唯一一条评论，则删除此帖子评论会导致该帖子也被删除。
    /// </remarks>
    Task DeletePostAsync(IThreadPost post, RequestOptions? options = null);

    /// <summary>
    ///     删除此帖子内的帖子评论内的一条帖子回复。
    /// </summary>
    /// <param name="replyId"> 要删除的帖子评论的回复的 ID。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示帖子评论的回复删除操作的异步任务。 </returns>
    Task DeleteReplyAsync(ulong replyId, RequestOptions? options = null);

    /// <summary>
    ///     删除此帖子内的帖子评论内的一条帖子回复。
    /// </summary>
    /// <param name="reply"> 要删除的帖子评论的回复。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示帖子评论的回复删除操作的异步任务。 </returns>
    Task DeleteReplyAsync(IThreadReply reply, RequestOptions? options = null);
}
