namespace Kook;

/// <summary>
///     表示一个帖子评论内的通用的帖子回复。
/// </summary>
public interface IThreadReply : IEntity<ulong>, IDeletable
{
    /// <summary>
    ///     获取此帖子评论的回复所在的帖子。
    /// </summary>
    IThread Thread { get; }

    /// <summary>
    ///     获取此帖子评论的回复所在的帖子评论。
    /// </summary>
    IThreadPost Post { get; }

    /// <summary>
    ///     获取此帖子评论的内容。
    /// </summary>
    /// <remarks>
    ///     此属性为卡片内容的原始代码。
    /// </remarks>
    string Content { get; }

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
}
