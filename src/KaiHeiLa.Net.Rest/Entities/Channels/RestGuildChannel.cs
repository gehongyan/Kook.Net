using Model = KaiHeiLa.API.Channel;

using System.Collections.Immutable;

namespace KaiHeiLa.Rest;

public class RestGuildChannel : RestChannel, IGuildChannel
{
    #region RestGuildChannel

    private ImmutableArray<RolePermissionOverwrite> _rolePermissionOverwrites;
    private ImmutableArray<UserPermissionOverwrite> _userPermissionOverwrites;
    
    /// <inheritdoc />
    public virtual IReadOnlyCollection<RolePermissionOverwrite> RolePermissionOverwrites => _rolePermissionOverwrites;
    /// <inheritdoc />
    public virtual IReadOnlyCollection<UserPermissionOverwrite> UserPermissionOverwrites => _userPermissionOverwrites;

    /// <summary>
    ///     Gets the guild associated with this channel.
    /// </summary>
    /// <returns>
    ///     A guild object that this channel belongs to.
    /// </returns>
    internal IGuild Guild { get; }
    /// <inheritdoc />
    public ChannelType Type { get; private set; }
    /// <inheritdoc />
    public string Name { get; private set; }
    /// <inheritdoc />
    public int? Position { get; private set; }
    /// <inheritdoc />
    public ulong GuildId => Guild.Id;
    
    internal RestGuildChannel(BaseKaiHeiLaClient kaiHeiLa, IGuild guild, ulong id, ChannelType type)
        : base(kaiHeiLa, id)
    {
        Type = type;
        Guild = guild;
    }
    internal static RestGuildChannel Create(BaseKaiHeiLaClient kaiHeiLa, IGuild guild, Model model)
    {
        return model.Type switch
        {
            ChannelType.Text => RestTextChannel.Create(kaiHeiLa, guild, model),
            ChannelType.Voice => RestVoiceChannel.Create(kaiHeiLa, guild, model),
            ChannelType.Category => RestCategoryChannel.Create(kaiHeiLa, guild, model),
            _ => new RestGuildChannel(kaiHeiLa, guild, model.Id, model.Type),
        };
    }
    internal override void Update(Model model)
    {
        Type = model.Type;
        Name = model.Name;
        Position = model.Position;
        
        if (model.UserPermissionOverwrites is not null)
        {
            var overwrites = model.UserPermissionOverwrites;
            var newOverwrites = ImmutableArray.CreateBuilder<UserPermissionOverwrite>(overwrites.Length);
            for (int i = 0; i < overwrites.Length; i++)
                newOverwrites.Add(overwrites[i].ToEntity());
            _userPermissionOverwrites = newOverwrites.ToImmutable();
        }
        if (model.RolePermissionOverwrites is not null)
        {
            var overwrites = model.RolePermissionOverwrites;
            var newOverwrites = ImmutableArray.CreateBuilder<RolePermissionOverwrite>(overwrites.Length);
            for (int i = 0; i < overwrites.Length; i++)
                newOverwrites.Add(overwrites[i].ToEntity());
            _rolePermissionOverwrites = newOverwrites.ToImmutable();
        }
    }
    
    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions options = null)
        => ChannelHelper.DeleteGuildChannelAsync(this, KaiHeiLa, options);
    
    /// <inheritdoc />
    public override async Task ReloadAsync(RequestOptions options = null)
    {
        var model = await KaiHeiLa.ApiClient.GetGuildChannelAsync(Id, options).ConfigureAwait(false);
        Update(model);
    }

    /// <summary>
    ///     Gets the permission overwrite for a specific user.
    /// </summary>
    /// <param name="user">The user to get the overwrite from.</param>
    /// <returns>
    ///     An overwrite object for the targeted user; <c>null</c> if none is set.
    /// </returns>
    public OverwritePermissions? GetPermissionOverwrite(IUser user)
    {
        for (int i = 0; i < _userPermissionOverwrites.Length; i++)
        {
            if (_userPermissionOverwrites[i].Target.Id == user.Id)
                return _userPermissionOverwrites[i].Permissions;
        }
        return null;
    }
    /// <summary>
    ///     Gets the permission overwrite for a specific role.
    /// </summary>
    /// <param name="role">The role to get the overwrite from.</param>
    /// <returns>
    ///     An overwrite object for the targeted role; <c>null</c> if none is set.
    /// </returns>
    public OverwritePermissions? GetPermissionOverwrite(IRole role)
    {
        for (int i = 0; i < _rolePermissionOverwrites.Length; i++)
        {
            if (_rolePermissionOverwrites[i].Target == role.Id)
                return _rolePermissionOverwrites[i].Permissions;
        }
        return null;
    }
    
    /// <summary>
    ///     Adds the permission overwrite for the given user.
    /// </summary>
    /// <param name="user">The user to add the overwrite to.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task representing the asynchronous permission operation for adding the specified permissions to the channel.
    /// </returns>
    public async Task AddPermissionOverwriteAsync(IGuildUser user, RequestOptions options = null)
    {
        var perms =  await ChannelHelper.AddPermissionOverwriteAsync(this, KaiHeiLa, user, options).ConfigureAwait(false);
        _userPermissionOverwrites = _userPermissionOverwrites.Add(perms);
    }
    /// <summary>
    ///     Adds the permission overwrite for the given role.
    /// </summary>
    /// <param name="role">The role to add the overwrite to.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task representing the asynchronous permission operation for adding the specified permissions to the channel.
    /// </returns>
    public async Task AddPermissionOverwriteAsync(IRole role, RequestOptions options = null)
    {
        var perms = await ChannelHelper.AddPermissionOverwriteAsync(this, KaiHeiLa, role, options).ConfigureAwait(false);
        _rolePermissionOverwrites = _rolePermissionOverwrites.Add(perms);
    }

    /// <summary>
    ///     Removes the permission overwrite for the given user, if one exists.
    /// </summary>
    /// <param name="user">The user to remove the overwrite from.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task representing the asynchronous operation for removing the specified permissions from the channel.
    /// </returns>
    public async Task RemovePermissionOverwriteAsync(IGuildUser user, RequestOptions options = null)
    {
        await ChannelHelper.RemovePermissionOverwriteAsync(this, KaiHeiLa, user, options).ConfigureAwait(false);

        for (int i = 0; i < _userPermissionOverwrites.Length; i++)
        {
            if (_userPermissionOverwrites[i].Target.Id == user.Id)
            {
                _userPermissionOverwrites = _userPermissionOverwrites.RemoveAt(i);
                return;
            }
        }
    }
    /// <summary>
    ///     Removes the permission overwrite for the given role, if one exists.
    /// </summary>
    /// <param name="role">The role to remove the overwrite from.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task representing the asynchronous operation for removing the specified permissions from the channel.
    /// </returns>
    public async Task RemovePermissionOverwriteAsync(IRole role, RequestOptions options = null)
    {
        await ChannelHelper.RemovePermissionOverwriteAsync(this, KaiHeiLa, role, options).ConfigureAwait(false);

        for (int i = 0; i < _rolePermissionOverwrites.Length; i++)
        {
            if (_rolePermissionOverwrites[i].Target == role.Id)
            {
                _rolePermissionOverwrites = _rolePermissionOverwrites.RemoveAt(i);
                return;
            }
        }
    }

    /// <summary>
    ///     Updates the permission overwrite for the given user, if one exists.
    /// </summary>
    /// <param name="user">The user to modify the overwrite for.</param>
    /// <param name="func">A delegate containing the values to modify the permission overwrite with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task representing the asynchronous operation for removing the specified permissions from the channel.
    /// </returns>
    public async Task ModifyPermissionOverwriteAsync(IGuildUser user, Action<OverwritePermissions> func, RequestOptions options = null)
    {
        var perms = await ChannelHelper.ModifyPermissionOverwriteAsync(this, KaiHeiLa, user, func, options).ConfigureAwait(false);
        
        for (int i = 0; i < _userPermissionOverwrites.Length; i++)
        {
            if (_userPermissionOverwrites[i].Target.Id == user.Id)
            {
                _userPermissionOverwrites = _userPermissionOverwrites.RemoveAt(i);
                _userPermissionOverwrites = _userPermissionOverwrites.Add(perms);
                return;
            }
        }
    }

    /// <summary>
    ///     Updates the permission overwrite for the given role, if one exists.
    /// </summary>
    /// <param name="role">The role to remove the overwrite for.</param>
    /// <param name="func">A delegate containing the values to modify the permission overwrite with.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task representing the asynchronous operation for removing the specified permissions from the channel.
    /// </returns>
    public async Task ModifyPermissionOverwriteAsync(IRole role, Action<OverwritePermissions> func, RequestOptions options = null)
    {
        var perms = await ChannelHelper.ModifyPermissionOverwriteAsync(this, KaiHeiLa, role, func, options).ConfigureAwait(false);
        
        for (int i = 0; i < _rolePermissionOverwrites.Length; i++)
        {
            if (_rolePermissionOverwrites[i].Target == role.Id)
            {
                _rolePermissionOverwrites = _rolePermissionOverwrites.RemoveAt(i);
                _rolePermissionOverwrites = _rolePermissionOverwrites.Add(perms);
                return;
            }
        }
    }
    
    /// <summary>
    ///     Gets the name of this channel.
    /// </summary>
    /// <returns>
    ///     A string that is the name of this channel.
    /// </returns>
    public override string ToString() => Name;
    
    #endregion

    #region IGuildChannel
    
    /// <inheritdoc />
    IGuild IGuildChannel.Guild
    {
        get
        {
            if (Guild != null)
                return Guild;
            throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
        }
    }
    
    /// <inheritdoc />
    OverwritePermissions? IGuildChannel.GetPermissionOverwrite(IRole role)
        => GetPermissionOverwrite(role);
    /// <inheritdoc />
    OverwritePermissions? IGuildChannel.GetPermissionOverwrite(IUser user)
        => GetPermissionOverwrite(user);
    
    /// <inheritdoc />
    async Task IGuildChannel.AddPermissionOverwriteAsync(IRole role, RequestOptions options)
        => await AddPermissionOverwriteAsync(role, options).ConfigureAwait(false);
    /// <inheritdoc />
    async Task IGuildChannel.AddPermissionOverwriteAsync(IGuildUser user, RequestOptions options)
        => await AddPermissionOverwriteAsync(user, options).ConfigureAwait(false);
    /// <inheritdoc />
    async Task IGuildChannel.RemovePermissionOverwriteAsync(IRole role, RequestOptions options)
        => await RemovePermissionOverwriteAsync(role, options).ConfigureAwait(false);
    /// <inheritdoc />
    async Task IGuildChannel.RemovePermissionOverwriteAsync(IGuildUser user, RequestOptions options)
        => await RemovePermissionOverwriteAsync(user, options).ConfigureAwait(false);
    /// <inheritdoc />
    async Task IGuildChannel.ModifyPermissionOverwriteAsync(IRole role, Action<OverwritePermissions> func, RequestOptions options)
        => await ModifyPermissionOverwriteAsync(role, func, options).ConfigureAwait(false);
    /// <inheritdoc />
    async Task IGuildChannel.ModifyPermissionOverwriteAsync(IGuildUser user, Action<OverwritePermissions> func, RequestOptions options)
        => await ModifyPermissionOverwriteAsync(user, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        => AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>(); //Overridden in Text/Voice
    /// <inheritdoc />
    Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IGuildUser>(null); //Overridden in Text/Voice
    #endregion
    
    #region IChannel
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        => AsyncEnumerable.Empty<IReadOnlyCollection<IUser>>(); //Overridden in Text/Voice
    /// <inheritdoc />
    Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IUser>(null); //Overridden in Text/Voice
    #endregion
}