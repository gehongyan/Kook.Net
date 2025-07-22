using System.Collections.Immutable;
using System.Diagnostics;
using Model = Kook.API.ThreadPost;
using ExtendedModel = Kook.API.ExtendedThreadPost;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的帖子评论。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestThreadReply : RestEntity<ulong>, IThreadReply
{
    private bool _isMentioningEveryone;
    private bool _isMentioningHere;
    private ImmutableArray<ICard> _cards = [];
    private ImmutableArray<RestUser> _userMentions = [];
    private ImmutableArray<ulong> _userMentionIds = [];
    private ImmutableArray<uint> _roleMentionIds = [];
    private ImmutableArray<ulong> _channelMentionIds = [];

    /// <inheritdoc />
    public IThread Thread { get; }

    /// <inheritdoc />
    public IThreadPost Post { get; }

    /// <inheritdoc />
    public string Content { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<ICard> Cards => _cards;

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

    /// <inheritdoc />
    public async Task DeleteAsync(RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadReplyAsync(Thread.Channel, Kook, this, options).ConfigureAwait(false);

    /// <inheritdoc />
    internal RestThreadReply(BaseKookClient kook, ulong id, IThreadPost post, IUser author)
        : base(kook, id)
    {
        Thread = post.Thread;
        Post = post;
        Author = author;
        Content = string.Empty;
    }

    internal static RestThreadReply Create(BaseKookClient kook, IThreadPost post, IUser author, Model model)
    {
        RestThreadReply entity = new(kook, model.Id, post, author);
        entity.Update(model);
        return entity;
    }

    internal static RestThreadReply Create(BaseKookClient kook, IThreadPost post, IUser author, ExtendedModel model)
    {
        RestThreadReply entity = new(kook, model.Id, post, author);
        entity.Update(model);
        return entity;
    }

    internal void Update(Model model)
    {
        Content = model.Content;
        _cards = MessageHelper.ParseCards(model.Content);
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
    }

    /// <inheritdoc cref="Kook.IThreadReply.ReplyAsync(System.String,System.Boolean,Kook.RequestOptions)" />
    public async Task<RestThreadReply> ReplyAsync(string content, bool isKMarkdown = false, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadReplyAsync(Post, Kook, content, isKMarkdown, Id, options).ConfigureAwait(false);

    /// <inheritdoc cref="Kook.IThreadReply.ReplyAsync(Kook.ICard,Kook.RequestOptions)" />
    public async Task<RestThreadReply> ReplyAsync(ICard card, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadReplyAsync(Post, Kook, card, Id, options).ConfigureAwait(false);

    /// <inheritdoc cref="Kook.IThreadReply.ReplyAsync(System.Collections.Generic.IEnumerable{Kook.ICard},Kook.RequestOptions)" />
    public async Task<RestThreadReply> ReplyAsync(IEnumerable<ICard> cards, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadReplyAsync(Post, Kook, cards, Id, options).ConfigureAwait(false);

    private string DebuggerDisplay => $"{Author}: {Content} ({Id})";

    #region IThreadReply

    /// <inheritdoc />
    async Task<IThreadReply> IThreadReply.ReplyAsync(string content, bool isKMarkdown, RequestOptions? options) =>
        await ReplyAsync(content, isKMarkdown, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IThreadReply> IThreadReply.ReplyAsync(ICard card, RequestOptions? options) =>
        await ReplyAsync(card, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IThreadReply> IThreadReply.ReplyAsync(IEnumerable<ICard> cards, RequestOptions? options) =>
        await ReplyAsync(cards, options).ConfigureAwait(false);

    #endregion
}
