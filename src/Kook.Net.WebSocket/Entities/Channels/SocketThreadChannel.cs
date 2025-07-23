using Kook.Rest;
using System.Collections.Immutable;
using System.Diagnostics;
using Model = Kook.API.Channel;

namespace Kook.WebSocket;

/// <summary>
///     表示服务器中一个基于网关的帖子频道，可以浏览、发布和回复帖子。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketThreadChannel : SocketGuildChannel, IThreadChannel
{
    #region SocketThreadChannel

    /// <inheritdoc />
    public string Topic { get; private set; }

    /// <inheritdoc />
    public int PostCreationInterval { get; private set; }

    /// <inheritdoc />
    /// <remarks>
    ///     <note type="warning">
    ///         此属性值仅在调用
    ///         <see cref="Kook.WebSocket.SocketThreadChannel.ModifyAsync(System.Action{Kook.ModifyThreadChannelProperties},Kook.RequestOptions)"/>
    ///         后或网关下发变更后才会被设置。
    ///     </note>
    /// </remarks>
    public int? ReplyInterval { get; private set; }

    /// <inheritdoc />
    /// <remarks>
    ///     <note type="warning">
    ///         此属性值仅在调用
    ///         <see cref="Kook.WebSocket.SocketThreadChannel.ModifyAsync(System.Action{Kook.ModifyThreadChannelProperties},Kook.RequestOptions)"/>
    ///         后或网关下发变更后才会被设置。
    ///     </note>
    /// </remarks>
    public ThreadLayout? DefaultLayout { get; private set; }

    /// <inheritdoc />
    /// <remarks>
    ///     <note type="warning">
    ///         此属性值仅在调用
    ///         <see cref="Kook.WebSocket.SocketThreadChannel.ModifyAsync(System.Action{Kook.ModifyThreadChannelProperties},Kook.RequestOptions)"/>
    ///         后或网关下发变更后才会被设置。
    ///     </note>
    /// </remarks>
    public ThreadSortOrder? DefaultSortOrder { get; private set; }

    /// <inheritdoc />
    public ulong? CategoryId { get; private set; }

    /// <summary>
    ///     获取此嵌套频道在服务器频道列表中所属的分组频道的。
    /// </summary>
    public ICategoryChannel? Category => CategoryId.HasValue
        ? Guild.GetChannel(CategoryId.Value) as ICategoryChannel
        : null;

    /// <inheritdoc />
    public bool? IsPermissionSynced { get; private set; }

    /// <inheritdoc />
    public string KMarkdownMention => MentionUtils.KMarkdownMentionChannel(Id);

    /// <inheritdoc />
    public string PlainTextMention => MentionUtils.PlainTextMentionChannel(Id);

    /// <inheritdoc />
    public override IReadOnlyCollection<SocketGuildUser> Users => Guild.Users
        .Where(x => Permissions.GetValue(
            Permissions.ResolveChannel(Guild, x, this, Permissions.ResolveGuild(Guild, x)),
            ChannelPermission.ViewChannel))
        .ToImmutableArray();

    internal SocketThreadChannel(KookSocketClient kook, ulong id, SocketGuild guild)
        : base(kook, id, guild)
    {
        Type = ChannelType.Thread;
        Topic = string.Empty;
    }

    internal static new SocketThreadChannel Create(SocketGuild guild, ClientState state, Model model)
    {
        SocketThreadChannel entity = new(guild.Kook, model.Id, guild);
        entity.Update(state, model);
        return entity;
    }

    internal override void Update(ClientState state, Model model)
    {
        base.Update(state, model);
        CategoryId = model.CategoryId != 0 ? model.CategoryId : null;
        Topic = model.Topic ?? string.Empty;
        PostCreationInterval = model.SlowMode / 1000;
        if (model.SlowModeReply.HasValue)
            ReplyInterval = model.SlowModeReply.Value / 1000;
        if (model.DefaultLayout.HasValue)
            DefaultLayout = model.DefaultLayout.Value;
        if (model.SortOrder.HasValue)
            DefaultSortOrder = model.SortOrder.Value;
        IsPermissionSynced = model.PermissionSync;
    }

    /// <inheritdoc />
    public virtual Task ModifyAsync(Action<ModifyThreadChannelProperties> func, RequestOptions? options = null) =>
        ChannelHelper.ModifyAsync(this, Kook, func, options);

    /// <inheritdoc />
    public virtual Task SyncPermissionsAsync(RequestOptions? options = null) =>
        ChannelHelper.SyncPermissionsAsync(this, Kook, options);

    private string DebuggerDisplay => $"{Name} ({Id}, Thread)";
    internal new SocketThreadChannel Clone() => (SocketThreadChannel)MemberwiseClone();

    #endregion

    #region Invites

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<IInvite>> GetInvitesAsync(RequestOptions? options = null) =>
        await ChannelHelper.GetInvitesAsync(this, Kook, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(int? maxAge = 604800,
        int? maxUses = null, RequestOptions? options = null) =>
        await ChannelHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800,
        InviteMaxUses maxUses = InviteMaxUses.Unlimited, RequestOptions? options = null) =>
        await ChannelHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    #endregion

    #region Threads

    /// <inheritdoc cref="Kook.IThreadChannel.GetThreadCategoriesAsync(Kook.RequestOptions)" />
    public async Task<IReadOnlyCollection<RestThreadCategory>> GetThreadCategoriesAsync(RequestOptions? options = null) =>
        await ThreadHelper.GetThreadCategoriesAsync(this, Kook, options);

    /// <inheritdoc cref="Kook.IThreadChannel.GetThreadAsync(System.UInt64,Kook.RequestOptions)" />
    public async Task<RestThread> GetThreadAsync(ulong id, RequestOptions? options = null) =>
        await ThreadHelper.GetThreadAsync(this, Kook, id, options).ConfigureAwait(false);

    /// <inheritdoc cref="Kook.IThreadChannel.GetThreadsAsync(System.Int32,Kook.IThreadCategory,Kook.RequestOptions)" />
    public IAsyncEnumerable<IReadOnlyCollection<RestThread>> GetThreadsAsync(
        int limit = KookConfig.MaxThreadsPerBatch, IThreadCategory? category = null, RequestOptions? options = null) =>
        ThreadHelper.GetThreadsAsync(this, Kook, null, ThreadSortOrder.Inherited, limit, category, options);

    /// <inheritdoc cref="Kook.IThreadChannel.GetThreadsAsync(System.DateTimeOffset,Kook.ThreadSortOrder,System.Int32,Kook.IThreadCategory,Kook.RequestOptions)" />
    public IAsyncEnumerable<IReadOnlyCollection<RestThread>> GetThreadsAsync(DateTimeOffset referenceTimestamp,
        ThreadSortOrder sortOrder = ThreadSortOrder.CreationTime, int limit = KookConfig.MaxThreadsPerBatch,
        IThreadCategory? category = null, RequestOptions? options = null) =>
        ThreadHelper.GetThreadsAsync(this, Kook, referenceTimestamp, sortOrder, limit, category, options);

    /// <inheritdoc cref="Kook.IThreadChannel.GetThreadsAsync(Kook.IThread,Kook.ThreadSortOrder,System.Int32,Kook.IThreadCategory,Kook.RequestOptions)" />
    public IAsyncEnumerable<IReadOnlyCollection<RestThread>> GetThreadsAsync(IThread referenceThread,
        ThreadSortOrder sortOrder = ThreadSortOrder.CreationTime, int limit = KookConfig.MaxThreadsPerBatch,
        IThreadCategory? category = null, RequestOptions? options = null) =>
        ThreadHelper.GetThreadsAsync(this, Kook, sortOrder switch
        {
            ThreadSortOrder.LatestActivity => referenceThread.LatestActiveTimestamp,
            ThreadSortOrder.CreationTime => referenceThread.Timestamp,
            ThreadSortOrder.Inherited when DefaultSortOrder is ThreadSortOrder.LatestActivity => referenceThread.LatestActiveTimestamp,
            ThreadSortOrder.Inherited when DefaultSortOrder is ThreadSortOrder.CreationTime => referenceThread.Timestamp,
            _ => null
        }, sortOrder, limit, category, options);

    /// <inheritdoc cref="Kook.IThreadChannel.CreateThreadAsync(System.String,System.String,System.Boolean,System.String,Kook.IThreadCategory,System.Collections.Generic.IEnumerable{Kook.ThreadTag},Kook.RequestOptions)" />
    public async Task<RestThread> CreateThreadAsync(string title, string content, bool isKMarkdown = false,
        string? cover = null, IThreadCategory? category = null, IEnumerable<ThreadTag>? tags = null, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadAsync(this, Kook, title, content, isKMarkdown, cover, category, tags, options);

    /// <inheritdoc cref="Kook.IThreadChannel.CreateThreadAsync(System.String,Kook.ICard,System.String,Kook.IThreadCategory,System.Collections.Generic.IEnumerable{Kook.ThreadTag},Kook.RequestOptions)" />
    public async Task<RestThread> CreateThreadAsync(string title, ICard card, string? cover = null,
        IThreadCategory? category = null, IEnumerable<ThreadTag>? tags = null, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadAsync(this, Kook, title, card, cover, category, tags, options);

    /// <inheritdoc cref="Kook.IThreadChannel.CreateThreadAsync(System.String,System.Collections.Generic.IEnumerable{Kook.ICard},System.String,Kook.IThreadCategory,System.Collections.Generic.IEnumerable{Kook.ThreadTag},Kook.RequestOptions)" />
    public async Task<RestThread> CreateThreadAsync(string title, IEnumerable<ICard> cards, string? cover = null,
        IThreadCategory? category = null, IEnumerable<ThreadTag>? tags = null, RequestOptions? options = null) =>
        await ThreadHelper.CreateThreadAsync(this, Kook, title, cards, cover, category, tags, options);

    /// <inheritdoc />
    public async Task DeleteThreadAsync(ulong threadId, RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadAsync(this, Kook, threadId, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DeleteThreadAsync(IThread thread, RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadAsync(this, Kook, thread, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DeleteThreadContentAsync(ulong threadId, RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadContentAsync(this, Kook, threadId, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DeleteThreadContentAsync(IThread thread, RequestOptions? options = null) =>
        await ThreadHelper.DeleteThreadContentAsync(this, Kook, thread, options).ConfigureAwait(false);

    #endregion

    #region Users

    /// <inheritdoc />
    public override SocketGuildUser? GetUser(ulong id)
    {
        if (Guild.GetUser(id) is not { } user) return null;
        ulong guildPerms = Permissions.ResolveGuild(Guild, user);
        ulong channelPerms = Permissions.ResolveChannel(Guild, user, this, guildPerms);
        return Permissions.GetValue(channelPerms, ChannelPermission.ViewChannel) ? user : null;
    }

    #endregion

    #region IThreadChannel

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IThreadCategory>> IThreadChannel.GetThreadCategoriesAsync(RequestOptions? options) =>
        await GetThreadCategoriesAsync(options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IThread> IThreadChannel.GetThreadAsync(ulong id, RequestOptions? options) =>
        await GetThreadAsync(id, options).ConfigureAwait(false);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IThread>> IThreadChannel.GetThreadsAsync(int limit,
        IThreadCategory? category, RequestOptions? options) =>
        GetThreadsAsync(limit, category, options);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IThread>> IThreadChannel.GetThreadsAsync(DateTimeOffset referenceTimestamp,
        ThreadSortOrder sortOrder, int limit, IThreadCategory? category, RequestOptions? options) =>
        GetThreadsAsync(referenceTimestamp, sortOrder, limit, category, options);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IThread>> IThreadChannel.GetThreadsAsync(IThread referenceThread,
        ThreadSortOrder sortOrder, int limit, IThreadCategory? category, RequestOptions? options) =>
        GetThreadsAsync(referenceThread, sortOrder, limit, category, options);

    /// <inheritdoc />
    async Task<IThread> IThreadChannel.CreateThreadAsync(string title, string content, bool isKMarkdown,
        string? cover, IThreadCategory? category, IEnumerable<ThreadTag>? tags, RequestOptions? options) =>
        await CreateThreadAsync(title, content, isKMarkdown, cover, category, tags, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IThread> IThreadChannel.CreateThreadAsync(string title, ICard card, string? cover, IThreadCategory? category,
        IEnumerable<ThreadTag>? tags, RequestOptions? options) =>
        await CreateThreadAsync(title, card, cover, category, tags, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IThread> IThreadChannel.CreateThreadAsync(string title, IEnumerable<ICard> cards, string? cover, IThreadCategory? category,
        IEnumerable<ThreadTag>? tags, RequestOptions? options) =>
        await CreateThreadAsync(title, cards, cover, category, tags, options).ConfigureAwait(false);

    #endregion

    #region IGuildChannel

    /// <inheritdoc />
    async Task<IGuildUser?> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options)
    {
        if (GetUser(id) is { } user)
            return user;
        if (mode == CacheMode.CacheOnly)
            return null;
        return await ChannelHelper.GetUserAsync(this, Guild, Kook, id, options).ConfigureAwait(false);
    }

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(
        CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? ChannelHelper.GetUsersAsync(this, Guild, Kook, KookConfig.MaxUsersPerBatch, 1, options)
            : ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();

    #endregion

    #region INestedChannel

    /// <inheritdoc />
    Task<ICategoryChannel?> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult(Category);

    #endregion
}
