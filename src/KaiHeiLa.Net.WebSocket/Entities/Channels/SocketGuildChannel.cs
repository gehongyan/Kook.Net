using System.Collections.Immutable;
using System.Diagnostics;
using Model = KaiHeiLa.API.Channel;

namespace KaiHeiLa.WebSocket;

[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketGuildChannel : SocketChannel, IGuildChannel
{
    private ImmutableArray<RolePermissionOverwrite> _rolePermissionOverwrites;
    private ImmutableArray<UserPermissionOverwrite> _userPermissionOverwrites;

    #region SocketGuildChannel

    /// <summary>
    ///     Gets the guild associated with this channel.
    /// </summary>
    /// <returns>
    ///     A guild object that this channel belongs to.
    /// </returns>
    public SocketGuild Guild { get; }
    /// <inheritdoc />
    public string Name { get; private set; }
    /// <inheritdoc />
    public int Position { get; private set; }
    /// <inheritdoc />
    public ChannelType Type { get; internal set; }

    public IReadOnlyCollection<RolePermissionOverwrite> RolePermissionOverwrites => _rolePermissionOverwrites;
    public IReadOnlyCollection<UserPermissionOverwrite> UserPermissionOverwrites => _userPermissionOverwrites;
    /// <summary>
    ///     Gets a collection of users that are able to view the channel.
    /// </summary>
    /// <returns>
    ///     A read-only collection of users that can access the channel (i.e. the users seen in the user list).
    /// </returns>
    public new virtual IReadOnlyCollection<SocketGuildUser> Users => ImmutableArray.Create<SocketGuildUser>();

    internal SocketGuildChannel(KaiHeiLaSocketClient kaiHeiLa, ulong id, SocketGuild guild)
        : base(kaiHeiLa, id)
    {
        Guild = guild;
    }
    internal static SocketGuildChannel Create(SocketGuild guild, ClientState state, Model model)
    {
        return model.Type switch
        {
            ChannelType.Category => SocketCategoryChannel.Create(guild, state, model),
            ChannelType.Text => SocketTextChannel.Create(guild, state, model),
            ChannelType.Voice => SocketVoiceChannel.Create(guild, state, model),
            _ => new SocketGuildChannel(guild.KaiHeiLa, model.Id, guild),
        };
    }
    
    /// <inheritdoc />
    internal override void Update(ClientState state, Model model)
    {
        Name = model.Name;
        Position = model.Position;

        var rolePermissionOverwrites = model.RolePermissionOverwrites;
        var newRoleOverwrites = ImmutableArray.CreateBuilder<RolePermissionOverwrite>(rolePermissionOverwrites.Length);
        for (int i = 0; i < rolePermissionOverwrites.Length; i++)
            newRoleOverwrites.Add(rolePermissionOverwrites[i].ToEntity());
        _rolePermissionOverwrites = newRoleOverwrites.ToImmutable();
        
        var userPermissionOverwrites = model.UserPermissionOverwrites;
        var newUserOverwrites = ImmutableArray.CreateBuilder<UserPermissionOverwrite>(userPermissionOverwrites.Length);
        for (int i = 0; i < userPermissionOverwrites.Length; i++)
            newUserOverwrites.Add(userPermissionOverwrites[i].ToEntity(KaiHeiLa, state));
        _userPermissionOverwrites = newUserOverwrites.ToImmutable();
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
    
    public new virtual SocketGuildUser GetUser(ulong id) => null;
    
    /// <summary>
    ///     Gets the name of the channel.
    /// </summary>
    /// <returns>
    ///     A string that resolves to <see cref="SocketGuildChannel.Name"/>.
    /// </returns>
    public override string ToString() => Name;
    private string DebuggerDisplay => $"{Name} ({Id}, Guild)";
    internal new SocketGuildChannel Clone() => MemberwiseClone() as SocketGuildChannel;
    
    #endregion


    #region SocketChannel
    
    /// <inheritdoc />
    internal override SocketUser GetUserInternal(ulong id) => GetUser(id);
    /// <inheritdoc />
    internal override IReadOnlyCollection<SocketUser> GetUsersInternal() => Users;
    
    #endregion
    
    #region IGuildChannel
    IGuild IGuildChannel.Guild => Guild;
    ulong IGuildChannel.GuildId => Guild.Id;
    
    /// <inheritdoc />
    Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IGuildUser>(GetUser(id));
    
    #endregion

    #region IChannel
    
    /// <inheritdoc />
    Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IUser>(GetUser(id)); //Overridden in Text/Voice

    #endregion
}