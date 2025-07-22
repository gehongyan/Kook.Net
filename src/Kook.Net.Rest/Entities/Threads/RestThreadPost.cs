using System.Collections.Immutable;
using System.Diagnostics;
using Model = Kook.API.ThreadPost;
using ExtendedModel = Kook.API.ExtendedThreadPost;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的帖子评论。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestThreadPost : RestEntity<ulong>, IThreadPost
{
    private bool _isMentioningEveryone;
    private bool _isMentioningHere;
    private ImmutableArray<IAttachment> _attachments = [];
    private ImmutableArray<ICard> _cards = [];
    private ImmutableArray<RestUser> _userMentions = [];
    private ImmutableArray<ulong> _userMentionIds = [];
    private ImmutableArray<uint> _roleMentionIds = [];
    private ImmutableArray<ulong> _channelMentionIds = [];
    private ImmutableArray<RestThreadReply> _replies = [];

    /// <inheritdoc />
    public IThread Thread { get; }

    /// <inheritdoc />
    public string Content { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<ICard> Cards => _cards;

    /// <inheritdoc />
    public IReadOnlyCollection<IAttachment> Attachments => _attachments;

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

    /// <inheritdoc cref="Kook.IThread.Author" />
    public IUser Author { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset Timestamp { get; private set; }

    /// <inheritdoc />
    public bool IsEdited { get; private set; }

    /// <inheritdoc cref="Kook.IThreadPost.Replies" />
    public IReadOnlyCollection<RestThreadReply> Replies => _replies;

    /// <inheritdoc />
    internal RestThreadPost(BaseKookClient kook, ulong id, IThread thread, IUser author)
        : base(kook, id)
    {
        Thread = thread;
        Author = author;
        Content = string.Empty;
    }

    internal static RestThreadPost Create(BaseKookClient kook, IThread thread, IUser author, Model model)
    {
        RestThreadPost entity = new(kook, model.Id, thread, author);
        entity.Update(model);
        return entity;
    }

    internal static RestThreadPost Create(BaseKookClient kook, IThread thread, IUser author, ExtendedModel model)
    {
        RestThreadPost entity = new(kook, model.Id, thread, author);
        entity.Update(model);
        return entity;
    }

    internal void Update(Model model)
    {
        Content = model.Content;
        _cards = MessageHelper.ParseCards(model.Content);
        _attachments = [..MessageHelper.ParseAttachments(_cards)];
        _userMentionIds = [..model.MentionedUserIds];
        _userMentions = [..model.MentionedUsers.Select(x => RestUser.Create(Kook, x))];
        _isMentioningEveryone = model.MentionAll;
        _isMentioningHere = model.MentionHere;
        _roleMentionIds = [..model.MentionedRoles.Select(x => x.Id)];
        _channelMentionIds = [..model.MentionedChannels.Select(x => x.Id)];
        IsEdited = model.IsUpdated;
    }

    internal void Update(ExtendedModel model)
    {
        Update(model as Model);
        Timestamp = model.CreateTime;
        Author = RestUser.Create(Kook, model.User);
        if (model.Replies is { Length: > 0 } replies)
        {
            ImmutableArray<RestThreadReply>.Builder builder = ImmutableArray.CreateBuilder<RestThreadReply>();
            foreach (ExtendedModel reply in replies)
            {
                RestGuildUser author = RestGuildUser.Create(Kook, Thread.Channel.Guild, reply.User);
                RestThreadReply entity = RestThreadReply.Create(Kook, this, author, reply);
                builder.Add(entity);
            }
            _replies = builder.ToImmutable();
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadPostAsync(Thread.Channel, Kook, this, options).ConfigureAwait(false);

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<IThreadReply>> GetRepliesAsync(
        int limit = KookConfig.MaxThreadsPerBatch, RequestOptions? options = null) =>
        ThreadHelper.GetThreadRepliesAsync(this, Kook, limit, options);

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<IThreadReply>> GetRepliesAsync(DateTimeOffset referenceTimestamp,
        SortMode sortMode = SortMode.Ascending,
        int limit = KookConfig.MaxThreadsPerBatch, RequestOptions? options = null) =>
        ThreadHelper.GetThreadRepliesAsync(this, Kook, referenceTimestamp, sortMode, limit, options);

    /// <inheritdoc />
    public IAsyncEnumerable<IReadOnlyCollection<IThreadReply>> GetRepliesAsync(IThreadReply referenceReply,
        SortMode sortMode = SortMode.Ascending,
        int limit = KookConfig.MaxThreadsPerBatch, RequestOptions? options = null) =>
        ThreadHelper.GetThreadRepliesAsync(this, Kook, referenceReply, sortMode, limit, options);

    /// <inheritdoc cref="Kook.IThreadPost.CreateReplyAsync(System.String,System.Boolean,System.Nullable{System.UInt64},Kook.RequestOptions)" />
    public async Task<RestThreadReply> CreateReplyAsync(string content, bool isKMarkdown = false,
        ulong? referenceReplyId = null, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadReplyAsync(this, Kook, content, isKMarkdown, referenceReplyId, options).ConfigureAwait(false);

    /// <inheritdoc cref="Kook.IThreadPost.CreateReplyAsync(Kook.ICard,System.Nullable{System.UInt64},Kook.RequestOptions)" />
    public async Task<RestThreadReply> CreateReplyAsync(ICard card, ulong? referenceReplyId = null, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadReplyAsync(this, Kook, card, referenceReplyId, options).ConfigureAwait(false);

    /// <inheritdoc cref="Kook.IThreadPost.CreateReplyAsync(System.Collections.Generic.IEnumerable{Kook.ICard},System.Nullable{System.UInt64},Kook.RequestOptions)" />
    public async Task<RestThreadReply> CreateReplyAsync(IEnumerable<ICard> cards, ulong? referenceReplyId = null, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadReplyAsync(this, Kook, cards, referenceReplyId, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DeleteReplyAsync(ulong replyId, RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadReplyAsync(Thread.Channel, Kook, Thread.Id, replyId, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DeleteReplyAsync(IThreadReply reply, RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadReplyAsync(Thread.Channel, Kook, reply, options).ConfigureAwait(false);

    private string DebuggerDisplay => $"{Author}: {Content} ({Id}{
        Attachments.Count switch
        {
            0 => string.Empty,
            1 => ", 1 Attachment",
            _ => $", {Attachments.Count} Attachments"
        }})";

    #region IThreadPost

    /// <inheritdoc />
    IReadOnlyCollection<IThreadReply> IThreadPost.Replies => _replies;

    /// <inheritdoc />
    async Task<IThreadReply> IThreadPost.CreateReplyAsync(string content, bool isKMarkdown, ulong? referenceReplyId,
        RequestOptions? options) =>
        await CreateReplyAsync(content, isKMarkdown, referenceReplyId, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IThreadReply> IThreadPost.CreateReplyAsync(ICard card, ulong? referenceReplyId, RequestOptions? options) =>
        await CreateReplyAsync(card, referenceReplyId, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IThreadReply> IThreadPost.CreateReplyAsync(IEnumerable<ICard> cards, ulong? referenceReplyId, RequestOptions? options) =>
        await CreateReplyAsync(cards, referenceReplyId, options).ConfigureAwait(false);

    #endregion
}
