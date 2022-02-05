using System.Collections.Immutable;
using System.Diagnostics;
using KaiHeiLa.Rest;
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
    public ChannelType Type { get; internal init; }

    public IReadOnlyCollection<RolePermissionOverwrite> RolePermissionOverwrites => _rolePermissionOverwrites;
    public IReadOnlyCollection<UserPermissionOverwrite> UserPermissionOverwrites => _userPermissionOverwrites;
    
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
        Position = model.Level;

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

    IGuild IGuildChannel.Guild => Guild;
    
    ulong IGuildChannel.GuildId => Guild.Id;

    #endregion
}