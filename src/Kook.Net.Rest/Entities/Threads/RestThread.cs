using System.Collections.Immutable;
using System.Diagnostics;
using Model = Kook.API.Thread;
using ExtendedModel = Kook.API.ExtendedThread;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的帖子。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestThread : RestEntity<ulong>, IThread, IUpdateable
{
    private bool _isMentioningEveryone;
    private bool _isMentioningHere;
    private ImmutableArray<IAttachment> _attachments = [];
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
    public IReadOnlyCollection<IAttachment> Attachments => _attachments;

    /// <inheritdoc cref="Kook.IThread.Author" />
    public IUser Author { get; }

    /// <inheritdoc cref="Kook.IThread.Category" />
    private RestThreadCategory? Category { get; set; }

    /// <inheritdoc />
    public IReadOnlyCollection<ThreadTag> ThreadTags => _threadTags;

    /// <inheritdoc />
    /// <remarks>
    ///     <note type="warning">
    ///         受限于 KOOK API，由 <see cref="Kook.IThreadChannel.GetThreadsAsync(System.Int32,Kook.IThreadCategory,Kook.RequestOptions)"/>
    ///         及其重载方法返回的帖子实体的此属性为 <c>null</c>。要想获取此属性的值，请先在当前实体上调用 <see cref="IUpdateable.UpdateAsync"/> 方法。
    ///     </note>
    /// </remarks>
    public string? Content { get; private set; }

    /// <inheritdoc />
    public string PreviewContent { get; private set; }

    /// <summary>
    ///     获取此帖子中提及的所有用户。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         受限于 KOOK API，由 <see cref="Kook.IThreadChannel.GetThreadsAsync(System.Int32,Kook.IThreadCategory,Kook.RequestOptions)"/>
    ///         及其重载方法返回的帖子实体的此属性为 <c>null</c>。要想获取此属性的值，请先在当前实体上调用 <see cref="IUpdateable.UpdateAsync"/> 方法。
    ///     </note>
    /// </remarks>
    public IReadOnlyCollection<RestUser> MentionedUsers => _userMentions;

    /// <inheritdoc />
    /// <remarks>
    ///     <note type="warning">
    ///         受限于 KOOK API，由 <see cref="Kook.IThreadChannel.GetThreadsAsync(System.Int32,Kook.IThreadCategory,Kook.RequestOptions)"/>
    ///         及其重载方法返回的帖子实体的此属性为 <c>null</c>。要想获取此属性的值，请先在当前实体上调用 <see cref="IUpdateable.UpdateAsync"/> 方法。
    ///     </note>
    /// </remarks>
    public IReadOnlyCollection<ulong> MentionedUserIds => _userMentionIds;

    /// <inheritdoc />
    /// <remarks>
    ///     <note type="warning">
    ///         受限于 KOOK API，由 <see cref="Kook.IThreadChannel.GetThreadsAsync(System.Int32,Kook.IThreadCategory,Kook.RequestOptions)"/>
    ///         及其重载方法返回的帖子实体的此属性为 <c>null</c>。要想获取此属性的值，请先在当前实体上调用 <see cref="IUpdateable.UpdateAsync"/> 方法。
    ///     </note>
    /// </remarks>
    public IReadOnlyCollection<uint> MentionedRoleIds => _roleMentionIds;

    /// <inheritdoc />
    /// <remarks>
    ///     <note type="warning">
    ///         受限于 KOOK API，由 <see cref="Kook.IThreadChannel.GetThreadsAsync(System.Int32,Kook.IThreadCategory,Kook.RequestOptions)"/>
    ///         及其重载方法返回的帖子实体的此属性为 <c>null</c>。要想获取此属性的值，请先在当前实体上调用 <see cref="IUpdateable.UpdateAsync"/> 方法。
    ///     </note>
    /// </remarks>
    public IReadOnlyCollection<ulong> MentionedChannelIds => _channelMentionIds;

    /// <inheritdoc />
    public bool MentionedEveryone => _isMentioningEveryone;

    /// <inheritdoc />
    public bool MentionedHere => _isMentioningHere;

    /// <inheritdoc />
    /// <remarks>
    ///     <note type="warning">
    ///         受限于 KOOK API，由 <see cref="Kook.IThreadChannel.GetThreadsAsync(System.Int32,Kook.IThreadCategory,Kook.RequestOptions)"/>
    ///         及其重载方法返回的帖子实体的此属性为 <c>null</c>。要想获取此属性的值，请先在当前实体上调用 <see cref="IUpdateable.UpdateAsync"/> 方法。
    ///     </note>
    /// </remarks>
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
        RestThread entity = new(kook, model.Id, channel, author);
        entity.Update(model);
        return entity;
    }

    internal static RestThread Create(BaseKookClient kook, IThreadChannel channel, IUser author, ExtendedModel model)
    {
        RestThread entity = new(kook, model.Id, channel, author);
        entity.Update(model);
        return entity;
    }

    internal void Update(Model model)
    {
        AuditStatus = model.Status;
        Title = model.Title;
        Cover = model.Cover;
        PostId = model.PostId;
        if (model.Content is not null && !string.IsNullOrWhiteSpace(model.Content))
            _cards = MessageHelper.ParseCards(model.Content);
        _attachments = [..model.Medias.Select(Attachment.Create)];
        PreviewContent = model.PreviewContent;
        Category = model.Category is not null
            ? RestThreadCategory.Create(Kook, Channel, model.Category)
            : null;
        _threadTags = [..model.Tags.Select(x => new ThreadTag(x.Id, x.Name, x.Icon))];
        Content = model.Content;
        if (model.MentionedUserIds is not null)
            _userMentionIds = [..model.MentionedUserIds];
        if (model.MentionedUsers is not null)
            _userMentions = [..model.MentionedUsers.Select(x => RestUser.Create(Kook, x))];
        _isMentioningEveryone = model.MentionAll;
        _isMentioningHere = model.MentionHere;
        if (model.MentionedRoles is not null)
            _roleMentionIds = [..model.MentionedRoles.Select(x => x.Id)];
        if (model.MentionedChannels is not null)
            _channelMentionIds = [..model.MentionedChannels.Select(x => x.Id)];
    }

    internal void Update(ExtendedModel model)
    {
        Update(model as Model);
        LatestActiveTimestamp = model.LatestActiveTime;
        Timestamp = model.CreateTime;
        IsEdited = model.IsUpdated;
        IsContentDeleted = model.ContentDeleted;
        ContentDeletedBy = model.ContentDeletedType;
        FavoriteCount = model.CollectNum;
        PostCount = model.PostCount;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(RequestOptions? options = null)
    {
        ExtendedModel model = await Kook.ApiClient.GetThreadAsync(Channel.Id, Id, options).ConfigureAwait(false);
        Update(model);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<IThreadPost>> GetPostsAsync(int limit = KookConfig.MaxThreadsPerBatch,
        RequestOptions? options = null) =>
        ThreadHelper.GetThreadPostsAsync(this, Kook, limit, options);

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<IThreadPost>> GetPostsAsync(DateTimeOffset referenceTimestamp,
        SortMode sortMode = SortMode.Ascending, int limit = KookConfig.MaxThreadsPerBatch,
        RequestOptions? options = null) =>
        ThreadHelper.GetThreadPostsAsync(this, Kook, referenceTimestamp, sortMode, limit, options);

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<IThreadPost>> GetPostsAsync(IThreadPost referencePost,
        SortMode sortMode = SortMode.Ascending, int limit = KookConfig.MaxThreadsPerBatch,
        RequestOptions? options = null) =>
        ThreadHelper.GetThreadPostsAsync(this, Kook, referencePost, sortMode, limit, options);

    /// <inheritdoc cref="Kook.IThread.CreatePostAsync(System.String,System.Boolean,Kook.RequestOptions)" />
    public async Task<IThreadPost> CreatePostAsync(string content, bool isKMarkdown = false, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadPostAsync(this, Kook, content, isKMarkdown, options).ConfigureAwait(false);

    /// <inheritdoc cref="Kook.IThread.CreatePostAsync(Kook.ICard,Kook.RequestOptions)" />
    public async Task<IThreadPost> CreatePostAsync(ICard card, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadPostAsync(this, Kook, card, options).ConfigureAwait(false);

    /// <inheritdoc cref="Kook.IThread.CreatePostAsync(System.Collections.Generic.IEnumerable{Kook.ICard},Kook.RequestOptions)" />
    public async Task<IThreadPost> CreatePostAsync(IEnumerable<ICard> cards, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadPostAsync(this, Kook, cards, options).ConfigureAwait(false);

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

    private string DebuggerDisplay => $"{Author}: [{Title}] {PreviewContent} ({Id}{
        Attachments.Count switch
        {
            0 => string.Empty,
            1 => ", 1 Attachment",
            _ => $", {Attachments.Count} Attachments"
        }}{
        PostCount switch
        {
            0 => string.Empty,
            1 => ", 1 Post",
            _ => $", {PostCount} Posts"
        }})";

    #region IThread

    /// <inheritdoc />
    IUser IThread.Author => Author;

    /// <inheritdoc />
    IThreadCategory? IThread.Category => Category;

    /// <inheritdoc />
    async Task<IThreadPost> IThread.CreatePostAsync(string content, bool isKMarkdown, RequestOptions? options) =>
        await CreatePostAsync(content, isKMarkdown, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IThreadPost> IThread.CreatePostAsync(ICard card, RequestOptions? options) =>
        await CreatePostAsync(card, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IThreadPost> IThread.CreatePostAsync(IEnumerable<ICard> cards, RequestOptions? options) =>
        await CreatePostAsync(cards, options).ConfigureAwait(false);

    #endregion
}
