using Model = KaiHeiLa.API.Role;

using System.Diagnostics;
using KaiHeiLa.Rest;

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
    public string KMarkdownMention => IsEveryone ? "@everyone" : MentionUtils.KMarkdownMentionRole(Id);

    public string PlainTextMention => IsEveryone ? "@全体成员" : MentionUtils.PlainTextMentionRole(Id);

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
    
    /// <inheritdoc />
    public Task ModifyAsync(Action<RoleProperties> func, RequestOptions options = null)
        => RoleHelper.ModifyAsync(this, KaiHeiLa, func, options);
    
    public IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(RequestOptions options = null)
    {
        void Func(SearchGuildMemberProperties p) => p.RoleId = Id;
        return GuildHelper.SearchUsersAsync(Guild, KaiHeiLa, Func, KaiHeiLaConfig.MaxUsersPerBatch, options);
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
    
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IRole.GetUsersAsync(CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return GetUsersAsync(options);
        else
            return AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();
    }
    
    #endregion
}