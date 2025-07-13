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
