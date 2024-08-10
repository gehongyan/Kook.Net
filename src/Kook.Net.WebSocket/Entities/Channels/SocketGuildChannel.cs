using Kook.Rest;
using System.Collections.Immutable;
using System.Diagnostics;
using Model = Kook.API.Channel;

namespace Kook.WebSocket;

/// <summary>
///     表示一个基于网关的服务器频道。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketGuildChannel : SocketChannel, IGuildChannel
{
    private ImmutableArray<RolePermissionOverwrite> _rolePermissionOverwrites;
    private ImmutableArray<UserPermissionOverwrite> _userPermissionOverwrites;

    #region SocketGuildChannel

    /// <inheritdoc cref="P:Kook.IGuildChannel.Guild" />
    public SocketGuild Guild { get; }

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public int? Position { get; private set; }

    /// <inheritdoc />
    public ChannelType Type { get; internal set; }

    /// <inheritdoc />
    public ulong? CreatorId { get; private set; }

    /// <summary>
    ///     获取创建此频道的用户。
    /// </summary>
    /// <remarks>
    ///     此属性会尝试从缓存的用户列表中获取此频道的创建者。如果用户不是此服务器的成员，或该用户不在本地缓存的用户列表中，则此属性将返回
    ///     <c>null</c>。在这种情况下，可以尝试通过
    ///     <see cref="M:Kook.IKookClient.GetUserAsync(System.UInt64,Kook.CacheMode,Kook.RequestOptions)"/>
    ///     获取指定的用户信息。
    /// </remarks>
    public SocketGuildUser? Creator => CreatorId.HasValue ? GetUser(CreatorId.Value) : null;

    /// <inheritdoc />
    public IReadOnlyCollection<RolePermissionOverwrite> RolePermissionOverwrites => _rolePermissionOverwrites;

    /// <inheritdoc />
    public IReadOnlyCollection<UserPermissionOverwrite> UserPermissionOverwrites => _userPermissionOverwrites;

    /// <inheritdoc cref="P:Kook.WebSocket.SocketChannel.Users" />
    public new virtual IReadOnlyCollection<SocketGuildUser> Users => [];

    internal SocketGuildChannel(KookSocketClient kook, ulong id, SocketGuild guild)
        : base(kook, id)
    {
        Name = string.Empty;
        Guild = guild;
        Type = ChannelType.Unspecified;
        _rolePermissionOverwrites = [];
        _userPermissionOverwrites = [];
    }

    internal static SocketGuildChannel Create(SocketGuild guild, ClientState state, Model model) =>
        model.Type switch
        {
            ChannelType.Category => SocketCategoryChannel.Create(guild, state, model),
            ChannelType.Text => SocketTextChannel.Create(guild, state, model),
            ChannelType.Voice => SocketVoiceChannel.Create(guild, state, model),
            _ => new SocketGuildChannel(guild.Kook, model.Id, guild)
        };

    internal override void Update(ClientState state, Model model)
    {
        Name = model.Name;
        Position = model.Position;
        if (model.CreatorId.HasValue)
            CreatorId = model.CreatorId.Value;

        _rolePermissionOverwrites = [..model.RolePermissionOverwrites.Select(x => x.ToEntity())];
        _userPermissionOverwrites = [..model.UserPermissionOverwrites.Select(x => x.ToEntity(Kook, state))];
    }

    internal void RemoveRolePermissionOverwrite(uint roleId)
    {
        _rolePermissionOverwrites = [.._rolePermissionOverwrites.Where(x => x.Target != roleId)];
    }

    internal void RemoveUserPermissionOverwrite(ulong userId)
    {
        _userPermissionOverwrites = [.._userPermissionOverwrites.Where(x => x.Target.Id != userId)];
    }

    /// <inheritdoc />
    public override Task UpdateAsync(RequestOptions? options = null) =>
        SocketChannelHelper.UpdateAsync(this, options);

    /// <inheritdoc />
    public Task ModifyAsync(Action<ModifyGuildChannelProperties> func, RequestOptions? options = null) =>
        ChannelHelper.ModifyAsync(this, Kook, func, options);

    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions? options = null) =>
        ChannelHelper.DeleteGuildChannelAsync(this, Kook, options);

    /// <inheritdoc />
    public virtual OverwritePermissions? GetPermissionOverwrite(IUser user) =>
        _userPermissionOverwrites.FirstOrDefault(x => x.Target.Id == user.Id)?.Permissions;

    /// <inheritdoc />
    public virtual OverwritePermissions? GetPermissionOverwrite(IRole role) =>
        _rolePermissionOverwrites.FirstOrDefault(x => x.Target == role.Id)?.Permissions;

    /// <inheritdoc cref="M:Kook.IGuildChannel.AddPermissionOverwriteAsync(Kook.IGuildUser,Kook.RequestOptions)" />
    public async Task AddPermissionOverwriteAsync(IGuildUser user, RequestOptions? options = null)
    {
        UserPermissionOverwrite perms = await ChannelHelper
            .AddPermissionOverwriteAsync(this, Kook, user, options)
            .ConfigureAwait(false);
        _userPermissionOverwrites = [.._userPermissionOverwrites, perms];
    }

    /// <inheritdoc cref="M:Kook.IGuildChannel.AddPermissionOverwriteAsync(Kook.IRole,Kook.RequestOptions)" />
    public async Task AddPermissionOverwriteAsync(IRole role, RequestOptions? options = null)
    {
        RolePermissionOverwrite perms = await ChannelHelper
            .AddPermissionOverwriteAsync(this, Kook, role, options)
            .ConfigureAwait(false);
        _rolePermissionOverwrites = [.._rolePermissionOverwrites, perms];
    }

    /// <inheritdoc cref="M:Kook.IGuildChannel.RemovePermissionOverwriteAsync(Kook.IGuildUser,Kook.RequestOptions)" />
    public async Task RemovePermissionOverwriteAsync(IGuildUser user, RequestOptions? options = null)
    {
        await ChannelHelper.RemovePermissionOverwriteAsync(this, Kook, user, options).ConfigureAwait(false);
        RemoveUserPermissionOverwrite(user.Id);
    }

    /// <inheritdoc cref="M:Kook.IGuildChannel.RemovePermissionOverwriteAsync(Kook.IRole,Kook.RequestOptions)" />
    public async Task RemovePermissionOverwriteAsync(IRole role, RequestOptions? options = null)
    {
        await ChannelHelper.RemovePermissionOverwriteAsync(this, Kook, role, options).ConfigureAwait(false);
        RemoveRolePermissionOverwrite(role.Id);
    }

    /// <inheritdoc cref="M:Kook.IGuildChannel.ModifyPermissionOverwriteAsync(Kook.IGuildUser,System.Func{Kook.OverwritePermissions,Kook.OverwritePermissions},Kook.RequestOptions)" />
    public async Task ModifyPermissionOverwriteAsync(IGuildUser user,
        Func<OverwritePermissions, OverwritePermissions> func, RequestOptions? options = null)
    {
        UserPermissionOverwrite perms = await ChannelHelper
            .ModifyPermissionOverwriteAsync(this, Kook, user, func, options)
            .ConfigureAwait(false);
        _userPermissionOverwrites = [.._userPermissionOverwrites.Where(x => x.Target.Id != user.Id), perms];
    }

    /// <inheritdoc cref="M:Kook.IGuildChannel.ModifyPermissionOverwriteAsync(Kook.IRole,System.Func{Kook.OverwritePermissions,Kook.OverwritePermissions},Kook.RequestOptions)" />
    public async Task ModifyPermissionOverwriteAsync(IRole role,
        Func<OverwritePermissions, OverwritePermissions> func, RequestOptions? options = null)
    {
        RolePermissionOverwrite perms = await ChannelHelper
            .ModifyPermissionOverwriteAsync(this, Kook, role, func, options)
            .ConfigureAwait(false);
        _rolePermissionOverwrites = [.._rolePermissionOverwrites.Where(x => x.Target != role.Id), perms];
    }

    /// <summary>
    ///     获取此频道中的一个服务器用户。
    /// </summary>
    /// <param name="id"> 要获取的服务器用户的 ID。 </param>
    /// <returns> 如果找到了具有指定 ID 的服务器用户，则返回该用户；否则返回 <c>null</c>。 </returns>
    public new virtual SocketGuildUser? GetUser(ulong id) => null;

    /// <inheritdoc cref="P:Kook.WebSocket.SocketGuildChannel.Name" />
    public override string ToString() => Name;

    private string DebuggerDisplay => $"{Name} ({Id}, Guild)";
    internal new SocketGuildChannel Clone() => (SocketGuildChannel)MemberwiseClone();

    #endregion

    #region SocketChannel

    internal override SocketUser? GetUserInternal(ulong id) => GetUser(id);

    internal override IReadOnlyCollection<SocketUser> GetUsersInternal() => Users;

    #endregion

    #region IGuildChannel

    /// <inheritdoc />
    IGuild IGuildChannel.Guild => Guild;

    /// <inheritdoc />
    ulong IGuildChannel.GuildId => Guild.Id;

    /// <inheritdoc />
    async Task IGuildChannel.AddPermissionOverwriteAsync(IRole role, RequestOptions? options) =>
        await AddPermissionOverwriteAsync(role, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task IGuildChannel.AddPermissionOverwriteAsync(IGuildUser user, RequestOptions? options) =>
        await AddPermissionOverwriteAsync(user, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task IGuildChannel.RemovePermissionOverwriteAsync(IRole role, RequestOptions? options) =>
        await RemovePermissionOverwriteAsync(role, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task IGuildChannel.RemovePermissionOverwriteAsync(IGuildUser user, RequestOptions? options) =>
        await RemovePermissionOverwriteAsync(user, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task IGuildChannel.ModifyPermissionOverwriteAsync(IRole role,
        Func<OverwritePermissions, OverwritePermissions> func, RequestOptions? options) =>
        await ModifyPermissionOverwriteAsync(role, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task IGuildChannel.ModifyPermissionOverwriteAsync(IGuildUser user,
        Func<OverwritePermissions, OverwritePermissions> func, RequestOptions? options) =>
        await ModifyPermissionOverwriteAsync(user, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(
        CacheMode mode, RequestOptions? options) =>
        ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable(); //Overridden in Text/Voice

    /// <inheritdoc />
    Task<IGuildUser?> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IGuildUser?>(GetUser(id)); //Overridden in Text/Voice

    /// <inheritdoc />
    Task<IUser?> IGuildChannel.GetCreatorAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IUser?>(Creator);

    #endregion

    #region IChannel

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions? options) =>
        ImmutableArray.Create<IReadOnlyCollection<IUser>>(Users).ToAsyncEnumerable(); //Overridden in Text/Voice

    /// <inheritdoc />
    Task<IUser?> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IUser?>(GetUser(id)); //Overridden in Text/Voice

    #endregion
}
