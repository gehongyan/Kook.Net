using Kook.API;
using System.Collections.Immutable;
using Model = Kook.API.Channel;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的服务器频道。
/// </summary>
public class RestGuildChannel : RestChannel, IGuildChannel
{
    #region RestGuildChannel

    private ImmutableArray<RolePermissionOverwrite> _rolePermissionOverwrites = [];
    private ImmutableArray<UserPermissionOverwrite> _userPermissionOverwrites = [];

    /// <inheritdoc />
    public virtual IReadOnlyCollection<RolePermissionOverwrite> RolePermissionOverwrites => _rolePermissionOverwrites;

    /// <inheritdoc />
    public virtual IReadOnlyCollection<UserPermissionOverwrite> UserPermissionOverwrites => _userPermissionOverwrites;

    /// <inheritdoc cref="Kook.IGuildChannel.Guild" />
    public IGuild Guild { get; }

    /// <inheritdoc />
    public ChannelType Type { get; internal set; }

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public int? Position { get; private set; }

    /// <inheritdoc />
    public ulong GuildId => Guild.Id;

    /// <inheritdoc />
    public ulong? CreatorId { get; private set; }

    internal RestGuildChannel(BaseKookClient kook, IGuild guild, ulong id)
        : base(kook, id)
    {
        Guild = guild;
        Type = ChannelType.Unspecified;
        Name = string.Empty;
    }

    internal static RestGuildChannel Create(BaseKookClient kook, IGuild guild, Model model) =>
        model.Type switch
        {
            ChannelType.Text => RestTextChannel.Create(kook, guild, model),
            ChannelType.Voice => RestVoiceChannel.Create(kook, guild, model),
            ChannelType.Thread => RestThreadChannel.Create(kook, guild, model),
            ChannelType.Category => RestCategoryChannel.Create(kook, guild, model),
            _ => new RestGuildChannel(kook, guild, model.Id)
        };

    internal static RestGuildChannel Create(BaseKookClient kook, IGuild guild, MentionedChannel model)
    {
        RestGuildChannel entity = new(kook, guild, model.Id);
        entity.Update(model);
        return entity;
    }

    internal override void Update(Model model)
    {
        Type = model.Type;
        Name = model.Name;
        Position = model.Position;
        CreatorId = model.CreatorId;

        IEnumerable<UserPermissionOverwrite> userPermissionOverwrites = model
            .UserPermissionOverwrites
            .Select(x => x.ToEntity(RestUser.Create(Kook, x.User)));
        _userPermissionOverwrites = [..userPermissionOverwrites];

        IEnumerable<RolePermissionOverwrite> rolePermissionOverwrites = model
            .RolePermissionOverwrites
            .Select(x => x.ToEntity(Guild.GetRole(x.RoleId)));
        _rolePermissionOverwrites = [..rolePermissionOverwrites];
    }

    internal void Update(MentionedChannel model) => Name = model.Name;

    /// <inheritdoc />
    public async Task ModifyAsync(Action<ModifyGuildChannelProperties> func, RequestOptions? options = null)
    {
        Model model = await ChannelHelper.ModifyAsync(this, Kook, func, options).ConfigureAwait(false);
        Update(model);
    }

    /// <summary>
    ///     获取此频道的创建者。
    /// </summary>
    /// <remarks>
    ///     此方法会尝试获取服务器创建者的非服务器特定的用户实体。要获取该用户作为此服务器成员的服务器用户实体，请在
    ///     <see cref="Guild"/> 上调用
    ///     <see cref="Kook.IGuild.GetUserAsync(System.UInt64,Kook.CacheMode,Kook.RequestOptions)"/>。
    /// </remarks>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果为此频道的创建者；如果没有找到则为 <c>null</c>。 </returns>
    public async Task<RestUser?> GetCreatorAsync(RequestOptions? options = null)
    {
        if (CreatorId.HasValue)
            return await ClientHelper.GetUserAsync(Kook, CreatorId.Value, options).ConfigureAwait(false);
        return null;
    }

    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions? options = null) =>
        ChannelHelper.DeleteGuildChannelAsync(this, Kook, options);

    /// <inheritdoc />
    public override async Task UpdateAsync(RequestOptions? options = null)
    {
        Model model = await Kook.ApiClient.GetGuildChannelAsync(Id, options).ConfigureAwait(false);
        Update(model);
    }

    /// <inheritdoc cref="Kook.IGuildChannel.GetPermissionOverwrite(Kook.IUser)" />
    public OverwritePermissions? GetPermissionOverwrite(IUser user) =>
        _userPermissionOverwrites.FirstOrDefault(x => x.TargetId == user.Id)?.Permissions;

    /// <inheritdoc cref="Kook.IGuildChannel.GetPermissionOverwrite(Kook.IRole)" />
    public OverwritePermissions? GetPermissionOverwrite(IRole role) =>
        _rolePermissionOverwrites.FirstOrDefault(x => x.TargetId == role.Id)?.Permissions;

    /// <inheritdoc cref="Kook.IGuildChannel.AddPermissionOverwriteAsync(Kook.IGuildUser,Kook.RequestOptions)" />
    public async Task AddPermissionOverwriteAsync(IGuildUser user, RequestOptions? options = null)
    {
        UserPermissionOverwrite perms = await ChannelHelper
            .AddPermissionOverwriteAsync(this, Kook, user, options)
            .ConfigureAwait(false);
        _userPermissionOverwrites = [.._userPermissionOverwrites.Where(x => x.TargetId != perms.TargetId), perms];
    }

    /// <inheritdoc cref="Kook.IGuildChannel.AddPermissionOverwriteAsync(Kook.IRole,Kook.RequestOptions)" />
    public async Task AddPermissionOverwriteAsync(IRole role, RequestOptions? options = null)
    {
        RolePermissionOverwrite perms = await ChannelHelper
            .AddPermissionOverwriteAsync(this, Kook, role, options)
            .ConfigureAwait(false);
        _rolePermissionOverwrites = [.._rolePermissionOverwrites.Where(x => x.TargetId != perms.TargetId), perms];
    }

    /// <inheritdoc cref="Kook.IGuildChannel.RemovePermissionOverwriteAsync(Kook.IGuildUser,Kook.RequestOptions)" />
    public async Task RemovePermissionOverwriteAsync(IGuildUser user, RequestOptions? options = null)
    {
        await ChannelHelper.RemovePermissionOverwriteAsync(this, Kook, user, options).ConfigureAwait(false);
        _userPermissionOverwrites = [.._userPermissionOverwrites.Where(x => x.TargetId != user.Id)];
    }

    /// <inheritdoc cref="Kook.IGuildChannel.RemovePermissionOverwriteAsync(Kook.IRole,Kook.RequestOptions)" />
    public async Task RemovePermissionOverwriteAsync(IRole role, RequestOptions? options = null)
    {
        await ChannelHelper.RemovePermissionOverwriteAsync(this, Kook, role, options).ConfigureAwait(false);
        _rolePermissionOverwrites = [.._rolePermissionOverwrites.Where(x => x.TargetId != role.Id)];
    }

    /// <inheritdoc cref="Kook.IGuildChannel.ModifyPermissionOverwriteAsync(Kook.IGuildUser,System.Func{Kook.OverwritePermissions,Kook.OverwritePermissions},Kook.RequestOptions)" />
    public async Task ModifyPermissionOverwriteAsync(IGuildUser user,
        Func<OverwritePermissions, OverwritePermissions> func, RequestOptions? options = null)
    {
        UserPermissionOverwrite permission = await ChannelHelper
            .ModifyPermissionOverwriteAsync(this, Kook, user, func, options)
            .ConfigureAwait(false);
        _userPermissionOverwrites = [.._userPermissionOverwrites.Where(x => x.TargetId != user.Id), permission];
    }

    /// <inheritdoc cref="Kook.IGuildChannel.ModifyPermissionOverwriteAsync(Kook.IRole,System.Func{Kook.OverwritePermissions,Kook.OverwritePermissions},Kook.RequestOptions)" />
    public async Task ModifyPermissionOverwriteAsync(IRole role,
        Func<OverwritePermissions, OverwritePermissions> func, RequestOptions? options = null)
    {
        RolePermissionOverwrite permission = await ChannelHelper
            .ModifyPermissionOverwriteAsync(this, Kook, role, func, options)
            .ConfigureAwait(false);
        _rolePermissionOverwrites = [.._rolePermissionOverwrites.Where(x => x.TargetId != role.Id), permission];
    }

    /// <inheritdoc cref="Kook.Rest.RestGuildChannel.Name" />
    /// <returns> 此频道的名称。 </returns>
    public override string ToString() => Name;

    #endregion

    #region IGuildChannel

    /// <inheritdoc />
    IGuild IGuildChannel.Guild => Guild;

    /// <inheritdoc />
    OverwritePermissions? IGuildChannel.GetPermissionOverwrite(IRole role) => GetPermissionOverwrite(role);

    /// <inheritdoc />
    OverwritePermissions? IGuildChannel.GetPermissionOverwrite(IUser user) => GetPermissionOverwrite(user);

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
        AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>(); //Overridden in Text/Voice

    /// <inheritdoc />
    Task<IGuildUser?> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IGuildUser?>(null); //Overridden in Text/Voice

    /// <inheritdoc />
    async Task<IUser?> IGuildChannel.GetCreatorAsync(CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetCreatorAsync(options).ConfigureAwait(false)
            : null;

    #endregion

    #region IChannel

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions? options) =>
        AsyncEnumerable.Empty<IReadOnlyCollection<IUser>>(); //Overridden in Text/Voice

    /// <inheritdoc />
    Task<IUser?> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IUser?>(null); //Overridden in Text/Voice

    #endregion
}
