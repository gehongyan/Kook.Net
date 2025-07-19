using System.Collections.Immutable;
using Model = Kook.API.Thread;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的帖子。
/// </summary>
public class RestThread : RestEntity<ulong>, IThread
{
    private bool _isMentioningEveryone;
    private bool _isMentioningHere;
    private ImmutableArray<ThreadAttachment> _attachments = [];
    private ImmutableArray<ICard> _cards = [];
    private ImmutableArray<RestUser> _userMentions = [];
    private ImmutableArray<ulong> _userMentionIds = [];
    private ImmutableArray<uint> _roleMentionIds = [];
    private ImmutableArray<ulong> _channelMentionIds = [];
    private ImmutableArray<ThreadTag> _threadTags = [];


    /// <inheritdoc />
    public IGuild Guild { get; }

    /// <inheritdoc />
    public IThreadChannel Channel { get; }

    /// <inheritdoc />
    public ThreadAuditStatus AuditStatus { get; private set; }

    /// <inheritdoc />
    public string Title { get; private set; }

    /// <inheritdoc />
    public string? Cover { get; private set; }

    /// <inheritdoc />
    public ulong PostId { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<IThreadAttachment> Attachments => _attachments;

    /// <inheritdoc cref="Kook.IThread.Author" />
    public IUser Author { get; }

    /// <inheritdoc cref="Kook.IThread.Category" />
    private RestThreadCategory? Category { get; set; }

    /// <inheritdoc />
    public IReadOnlyCollection<ThreadTag> ThreadTags => _threadTags;

    /// <inheritdoc />
    public string Content { get; private set; }

    /// <inheritdoc />
    public string PreviewContent { get; private set; }

    /// <summary>
    ///     获取此帖子中提及的所有用户。
    /// </summary>
    public IReadOnlyCollection<RestUser> MentionedUsers => _userMentions;

    /// <inheritdoc />
    public IReadOnlyCollection<ulong> MentionedUserIds => _userMentionIds;

    /// <inheritdoc />
    public IReadOnlyCollection<uint> MentionedRoleIds => _roleMentionIds;

    /// <inheritdoc />
    public IReadOnlyCollection<ulong> MentionedChannelIds => _channelMentionIds;

    /// <inheritdoc />
    public bool MentionedEveryone => _isMentioningEveryone;

    /// <inheritdoc />
    public bool MentionedHere => _isMentioningHere;

    /// <inheritdoc />
    public IReadOnlyCollection<ICard> Cards => _cards;

    /// <inheritdoc />
    public DateTimeOffset Timestamp { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset LatestActiveTimestamp { get; private set; }

    /// <inheritdoc />
    public bool IsEdited { get; private set; }

    /// <inheritdoc />
    public bool IsContentDeleted { get; private set; }

    /// <inheritdoc />
    public ThreadContentDeletedBy ContentDeletedBy { get; private set; }

    /// <inheritdoc />
    public int FavoriteCount { get; private set; }

    /// <inheritdoc />
    public int PostCount { get; private set; }

    /// <inheritdoc />
    internal RestThread(BaseKookClient kook, ulong id, IThreadChannel channel, IUser author)
        : base(kook, id)
    {
        Guild = channel.Guild;
        Channel = channel;
        Author = author;
        Title = string.Empty;
        Content = string.Empty;
        PreviewContent = string.Empty;
    }

    internal static RestThread Create(BaseKookClient kook, IThreadChannel channel, IUser author, Model model)
    {
        RestThread entity = new(kook, model.Id, channel, RestUser.Create(kook, model.User));
        entity.Update(model);
        return entity;
    }

    internal void Update(Model model)
    {
        AuditStatus = model.Status;
        Title = model.Title;
        Cover = model.Cover;
        PostId = model.PostId;
        _attachments = [..model.Medias.Select(ThreadAttachment.Create)];
        PreviewContent = model.PreviewContent;
        Category = model.Category is not null
            ? RestThreadCategory.Create(Kook, Channel, model.Category)
            : null;
        _threadTags = [..model.Tags.Select(x => new ThreadTag(x.Id, x.Name, x.Icon))];
        Content = model.Content;
        _userMentions = [..model.MentionedUsers.Select(x => RestUser.Create(Kook, x))];
        _isMentioningEveryone = model.MentionAll;
        _isMentioningHere = model.MentionHere;
        _roleMentionIds = [..model.MentionedRoles.Select(x => x.Id)];
        _channelMentionIds = [..model.MentionedChannels.Select(x => x.Id)];
    }

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<IThreadPost>> GetPostsAsync(int limit = KookConfig.MaxThreadsPerBatch, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<IThreadPost>> GetPostsAsync(ulong referencePostId, SortMode sortMode = SortMode.Ascending,
        int limit = KookConfig.MaxThreadsPerBatch, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<IThreadPost>> GetPostsAsync(IThreadPost referencePost, SortMode sortMode = SortMode.Ascending,
        int limit = KookConfig.MaxThreadsPerBatch, RequestOptions? options = null) =>
        throw new NotImplementedException();

    /// <inheritdoc />
    public Task<IThreadPost> CreatePostAsync(string content, RequestOptions? options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<IThreadPost> CreatePostAsync(ICard card, RequestOptions? options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public Task<IThreadPost> CreatePostAsync(IEnumerable<ICard> cards, RequestOptions? options = null) => throw new NotImplementedException();

    /// <inheritdoc />
    public async Task DeleteAsync(RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadAsync(Channel, Kook, this, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DeleteContentAsync(RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadContentAsync(Channel, Kook, this, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DeletePostAsync(ulong postId, RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadPostAsync(Channel, Kook, Id, postId, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DeletePostAsync(IThreadPost post, RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadPostAsync(Channel, Kook, post, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DeleteReplyAsync(ulong replyId, RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadReplyAsync(Channel, Kook, Id, replyId, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DeleteReplyAsync(IThreadReply reply, RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadReplyAsync(Channel, Kook, reply, options).ConfigureAwait(false);

    #region IThread

    /// <inheritdoc />
    IUser IThread.Author => Author;

    /// <inheritdoc />
    IThreadCategory? IThread.Category => Category;

    #endregion
}
