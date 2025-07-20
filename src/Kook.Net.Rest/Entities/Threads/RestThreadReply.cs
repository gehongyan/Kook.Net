using System.Collections.Immutable;
using Model = Kook.API.ThreadPost;
using ExtendedModel = Kook.API.ExtendedThreadPost;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的帖子评论。
/// </summary>
public class RestThreadReply : RestEntity<ulong>, IThreadReply
{
    private bool _isMentioningEveryone;
    private bool _isMentioningHere;
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

    /// <inheritdoc cref="Kook.IThreadReply.ReplyAsync(System.String,Kook.RequestOptions)" />
    public async Task<RestThreadReply> ReplyAsync(string content, RequestOptions? options = null)
    {
        return await ThreadHelper.CreateThreadReplyAsync(
            Post, Kook, content, Id, options).ConfigureAwait(false);
    }

    #region IThreadReply

    /// <inheritdoc />
    async Task<IThreadReply> IThreadReply.ReplyAsync(string content, RequestOptions? options) =>
        await ReplyAsync(content, options).ConfigureAwait(false);

    #endregion
}
