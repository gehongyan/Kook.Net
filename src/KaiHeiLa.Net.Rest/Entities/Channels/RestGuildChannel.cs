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

    internal IGuild Guild { get; }
    /// <inheritdoc />
    public ChannelType Type { get; set; }
    /// <inheritdoc />
    public string Name { get; private set; }
    /// <inheritdoc />
    public int Position { get; private set; }
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
    public override async Task UpdateAsync(RequestOptions options = null)
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
    public virtual OverwritePermissions? GetPermissionOverwrite(IUser user)
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
    public virtual OverwritePermissions? GetPermissionOverwrite(IRole role)
    {
        for (int i = 0; i < _rolePermissionOverwrites.Length; i++)
        {
            if (_rolePermissionOverwrites[i].Target == role.Id)
                return _rolePermissionOverwrites[i].Permissions;
        }
        return null;
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
    Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IGuildUser>(null); //Overridden in Text/Voice
    #endregion
}