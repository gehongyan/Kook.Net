using Model = KaiHeiLa.API.Role;

using System.Diagnostics;

namespace KaiHeiLa.WebSocket;
/// <summary>
///     Represents a WebSocket-based role to be given to a guild user.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketRole : SocketEntity<uint>, IRole
{
    #region SocketRole

    /// <summary>
    ///     Gets the guild that owns this role.
    /// </summary>
    /// <returns>
    ///     A <see cref="SocketGuild"/> representing the parent guild of this role.
    /// </returns>
    public SocketGuild Guild { get; }
    
    public string Name { get; private set; }
    public Color Color { get; private set; }
    public int Position { get; private set; }
    public bool IsHoisted { get; private set; }
    public bool IsMentionable { get; private set; }
    public GuildPermissions Permissions { get; private set; }

    /// <summary>
    ///     Returns a value that determines if the role is an @everyone role.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the role is @everyone; otherwise <c>false</c>.
    /// </returns>
    public bool IsEveryone => Id == 0;
    /// <inheritdoc />
    public string Mention => IsEveryone ? "@everyone" : MentionUtils.MentionRole(Id);
    internal SocketRole(SocketGuild guild, uint id)
        : base(guild.KaiHeiLa, id)
    {
        Guild = guild;
    }
    internal static SocketRole Create(SocketGuild guild, ClientState state, Model model)
    {
        var entity = new SocketRole(guild, model.Id);
        entity.Update(state, model);
        return entity;
    }
    internal void Update(ClientState state, Model model)
    {
        Name = model.Name;
        Color = new Color(model.Color);
        Position = model.Position;
        IsHoisted = model.Hoist switch
        {
            0 => false, 
            1 => true, 
            _ => throw new ArgumentOutOfRangeException(nameof(model.Hoist))
        };
        IsMentionable = model.Mentionable switch
        {
            0 => false, 
            1 => true, 
            _ => throw new ArgumentOutOfRangeException(nameof(model.Mentionable))
        };
        Permissions = new GuildPermissions(model.Permissions);
    }
    
    #endregion
    
    /// <summary>
    ///     Gets the name of the role.
    /// </summary>
    /// <returns>
    ///     A string that resolves to <see cref="KaiHeiLa.WebSocket.SocketRole.Name" />.
    /// </returns>
    public override string ToString() => Name;
    private string DebuggerDisplay => $"{Name} ({Id})";
    internal SocketRole Clone() => MemberwiseClone() as SocketRole;
    
    #region IRole
    /// <inheritdoc />
    IGuild IRole.Guild => Guild;
    #endregion
}