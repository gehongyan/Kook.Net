namespace Kook;

/// <summary>
///     表示一个帖子评论内的通用的帖子回复。
/// </summary>
public interface IThreadReply : IEntity<ulong>, IDeletable
{
    /// <summary>
    ///     获取此帖子回复的回复所在的帖子。
    /// </summary>
    IThread Thread { get; }

    /// <summary>
    ///     获取此帖子回复的回复所在的帖子评论。
    /// </summary>
    IThreadPost Post { get; }

    /// <summary>
    ///     获取此帖子回复的内容。
    /// </summary>
    /// <remarks>
    ///     此属性为卡片内容的原始代码。
    /// </remarks>
    string Content { get; }

    /// <summary>
    ///     获取此帖子回复中提及的所有用户的 ID。
    /// </summary>
    IReadOnlyCollection<ulong> MentionedUserIds { get; }

    /// <summary>
    ///     获取此帖子回复中提及的所有角色的 ID。
    /// </summary>
    IReadOnlyCollection<uint> MentionedRoleIds { get; }

    /// <summary>
    ///     获取此帖子回复中提及的所有频道。
    /// </summary>
    IReadOnlyCollection<ulong> MentionedChannelIds { get; }

    /// <summary>
    ///     获取此帖子回复是否提及了全体成员。
    /// </summary>
    bool MentionedEveryone { get; }

    /// <summary>
    ///     获取此帖子回复是否提及了在线成员。
    /// </summary>
    bool MentionedHere { get; }

    /// <summary>
    ///     获取此帖子回复的作者。
    /// </summary>
    IUser Author { get; }

    /// <summary>
    ///     获取此帖子的发布时间。
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    ///     获取此帖子回复是否已被编辑。
    /// </summary>
    bool IsEdited { get; }

    /// <summary>
    ///     在此帖子回复所属的帖子评论中回复此帖子回复。
    /// </summary>
    /// <param name="content"> 要发布的文本内容。 </param>
    /// <param name="isKMarkdown"> 是否为 KMarkdown 格式。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示新创建帖子评论的回复操作的异步任务。 </returns>
    Task<IThreadReply> ReplyAsync(string content, bool isKMarkdown = false, RequestOptions? options = null);

    /// <summary>
    ///     在此帖子回复所属的帖子评论中回复此帖子回复。
    /// </summary>
    /// <param name="card"> 要发布的卡片内容。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示新创建帖子评论的回复操作的异步任务。 </returns>
    /// <remarks>
    ///     卡片内仅允许包含文本内容。
    /// </remarks>
    Task<IThreadReply> ReplyAsync(ICard card, RequestOptions? options = null);

    /// <summary>
    ///     在此帖子回复所属的帖子评论中回复此帖子回复。
    /// </summary>
    /// <param name="cards"> 要发布的卡片内容。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示新创建帖子评论的回复操作的异步任务。 </returns>
    /// <remarks>
    ///     卡片内仅允许包含文本内容。
    /// </remarks>
    Task<IThreadReply> ReplyAsync(IEnumerable<ICard> cards, RequestOptions? options = null);
}
