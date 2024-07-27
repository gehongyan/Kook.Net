using Kook.API;
using System.Collections.Immutable;
using Model = Kook.API.Channel;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based guild channel in a guild.
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

    /// <summary>
    ///     Gets the guild associated with this channel.
    /// </summary>
    /// <returns> A guild object that this channel belongs to. </returns>
    internal IGuild Guild { get; }

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
            .Select(x => x.ToEntity(Kook));
        _userPermissionOverwrites = [..userPermissionOverwrites];

        IEnumerable<RolePermissionOverwrite> rolePermissionOverwrites = model
            .RolePermissionOverwrites
            .Select(x => x.ToEntity());
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
    ///     Gets the creator of this channel.
    /// </summary>
    /// <remarks>
    ///     This method will try to get the user as a global user. To get the creator as a guild member,
    ///     you will need to get the user through
    ///     <see cref="IGuild.GetUserAsync(ulong,CacheMode,RequestOptions)"/>."/>
    /// </remarks>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> A task that represents the asynchronous get operation. The task result contains the creator of this channel. </returns>
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

    /// <summary>
    ///     Gets the permission overwrite for a specific user.
    /// </summary>
    /// <param name="user">The user to get the overwrite from.</param>
    /// <returns> An overwrite object for the targeted user; <c>null</c> if none is set. </returns>
    public OverwritePermissions? GetPermissionOverwrite(IUser user) =>
        _userPermissionOverwrites.FirstOrDefault(x => x.Target.Id == user.Id)?.Permissions;

    /// <summary>
    ///     Gets the permission overwrite for a specific role.
    /// </summary>
    /// <param name="role">The role to get the overwrite from.</param>
    /// <returns> An overwrite object for the targeted role; <c>null</c> if none is set. </returns>
    public OverwritePermissions? GetPermissionOverwrite(IRole role) =>
        _rolePermissionOverwrites.FirstOrDefault(x => x.Target == role.Id)?.Permissions;

    /// <summary>
    ///     Adds the permission overwrite for the given user.
    /// </summary>
    /// <param name="user">The user to add the overwrite to.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> A task representing the asynchronous permission operation for adding the specified permissions to the channel. </returns>
    public async Task AddPermissionOverwriteAsync(IGuildUser user, RequestOptions? options = null)
    {
        UserPermissionOverwrite permissionOverwrite = await ChannelHelper
            .AddPermissionOverwriteAsync(this, Kook, user, options)
            .ConfigureAwait(false);
        _userPermissionOverwrites = [.._userPermissionOverwrites, permissionOverwrite];
    }

    /// <summary>
    ///     Adds the permission overwrite for the given role.
    /// </summary>
    /// <param name="role">The role to add the overwrite to.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> A task representing the asynchronous permission operation for adding the specified permissions to the channel. </returns>
    public async Task AddPermissionOverwriteAsync(IRole role, RequestOptions? options = null)
    {
        RolePermissionOverwrite permissionOverwrite = await ChannelHelper
            .AddPermissionOverwriteAsync(this, Kook, role, options)
            .ConfigureAwait(false);
        _rolePermissionOverwrites = [.._rolePermissionOverwrites, permissionOverwrite];
    }

    /// <summary>
    ///     Removes the permission overwrite for the given user, if one exists.
    /// </summary>
    /// <param name="user">The user to remove the overwrite from.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> A task representing the asynchronous operation for removing the specified permissions from the channel. </returns>
    public async Task RemovePermissionOverwriteAsync(IGuildUser user, RequestOptions? options = null)
    {
        await ChannelHelper.RemovePermissionOverwriteAsync(this, Kook, user, options).ConfigureAwait(false);
        _userPermissionOverwrites = [.._userPermissionOverwrites.Where(x => x.Target.Id != user.Id)];
    }

    /// <summary>
    ///     Removes the permission overwrite for the given role, if one exists.
    /// </summary>
    /// <param name="role">The role to remove the overwrite from.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> A task representing the asynchronous operation for removing the specified permissions from the channel. </returns>
    public async Task RemovePermissionOverwriteAsync(IRole role, RequestOptions? options = null)
    {
        await ChannelHelper.RemovePermissionOverwriteAsync(this, Kook, role, options).ConfigureAwait(false);
        _rolePermissionOverwrites = [.._rolePermissionOverwrites.Where(x => x.Target != role.Id)];
    }

    /// <summary>
    ///     Updates the permission overwrite for the given user, if one exists.
    /// </summary>
    /// <param name="user">The user to modify the overwrite for.</param>
    /// <param name="func">A delegate containing the values to modify the permission overwrite with.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> A task representing the asynchronous operation for removing the specified permissions from the channel. </returns>
    public async Task ModifyPermissionOverwriteAsync(IGuildUser user,
        Func<OverwritePermissions, OverwritePermissions> func, RequestOptions? options = null)
    {
        UserPermissionOverwrite permission = await ChannelHelper
            .ModifyPermissionOverwriteAsync(this, Kook, user, func, options)
            .ConfigureAwait(false);
        _userPermissionOverwrites = [.._userPermissionOverwrites.Where(x => x.Target.Id != user.Id), permission];
    }

    /// <summary>
    ///     Updates the permission overwrite for the given role, if one exists.
    /// </summary>
    /// <param name="role">The role to remove the overwrite for.</param>
    /// <param name="func">A delegate containing the values to modify the permission overwrite with.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> A task representing the asynchronous operation for removing the specified permissions from the channel. </returns>
    public async Task ModifyPermissionOverwriteAsync(IRole role,
        Func<OverwritePermissions, OverwritePermissions> func, RequestOptions? options = null)
    {
        RolePermissionOverwrite permission = await ChannelHelper
            .ModifyPermissionOverwriteAsync(this, Kook, role, func, options)
            .ConfigureAwait(false);
        _rolePermissionOverwrites = [.._rolePermissionOverwrites.Where(x => x.Target != role.Id), permission];
    }

    /// <summary>
    ///     Gets the name of this channel.
    /// </summary>
    /// <returns> A string that is the name of this channel. </returns>
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
