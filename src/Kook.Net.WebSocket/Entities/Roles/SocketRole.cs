using System.Collections.Immutable;
using Model = Kook.API.Role;

using System.Diagnostics;
using Kook.API.Rest;
using Kook.Rest;

namespace Kook.WebSocket;
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
    
    /// <inheritdoc />
    public RoleType? Type { get; set; }
    /// <inheritdoc />
    public string Name { get; private set; }
    /// <inheritdoc />
    public Color Color { get; private set; }
    /// <inheritdoc />
    public int Position { get; private set; }
    /// <inheritdoc />
    public bool IsHoisted { get; private set; }
    /// <inheritdoc />
    public bool IsMentionable { get; private set; }
    /// <inheritdoc />
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
    /// <inheritdoc />
    public string PlainTextMention => IsEveryone ? "@全体成员" : MentionUtils.PlainTextMentionRole(Id);

    internal SocketRole(SocketGuild guild, uint id)
        : base(guild.Kook, id)
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
        Type = model.Type;
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
        => RoleHelper.ModifyAsync(this, Kook, func, options);
    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions options = null)
        => RoleHelper.DeleteAsync(this, Kook, options);
    
    /// <summary>
    ///     Gets a collection of users with this role.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     Paged collection of users with this role.
    /// </returns>
    /// <remarks>
    ///     If the guild this role belongs to does not has all members cached locally
    ///     by checking <see cref="SocketGuild.HasAllMembers"/>, this method will request
    ///     the data via REST and update the guild users cache, otherwise it will
    ///     return the cached data.
    /// </remarks>
    public async IAsyncEnumerable<IReadOnlyCollection<SocketGuildUser>> GetUsersAsync(RequestOptions options = null)
    {
        // From SocketGuild.Users
        if (Guild.HasAllMembers)
        {
            IEnumerable<IReadOnlyCollection<SocketGuildUser>> userCollections = Guild.Users
                .Where(u => u.Roles.Contains(this))
#if NET6_0_OR_GREATER
                .Chunk(KookConfig.MaxUsersPerBatch)
#else
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / KookConfig.MaxUsersPerBatch)
                .Select(x => x.Select(v => v.Value))
#endif
                .Select(c => c.ToImmutableArray() as IReadOnlyCollection<SocketGuildUser>);
            foreach (IReadOnlyCollection<SocketGuildUser> users in userCollections)
                yield return users;
            yield break;
        }

        // Update SocketGuild.Users by fetching from REST API
        void Func(SearchGuildMemberProperties p) => p.RoleId = Id;
        var restUserCollections = Kook.ApiClient
            .GetGuildMembersAsync(Guild.Id, Func, KookConfig.MaxUsersPerBatch, 1, options);
        await foreach (IReadOnlyCollection<GuildMember> restUsers in restUserCollections)
            yield return restUsers.Select(restUser => Guild.AddOrUpdateUser(restUser)).ToList();
    }
    
    /// <inheritdoc />
    public int CompareTo(IRole role) => RoleUtils.Compare(this, role);
    
    #endregion
    
    /// <summary>
    ///     Gets the name of the role.
    /// </summary>
    /// <returns>
    ///     A string that resolves to <see cref="Kook.WebSocket.SocketRole.Name" />.
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