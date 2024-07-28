using System.Collections.Immutable;
using System.Diagnostics;
using Kook.API.Rest;
using Kook.Rest;
using Model = Kook.API.Role;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based role to be given to a guild user.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketRole : SocketEntity<uint>, IRole
{
    #region SocketRole

    /// <summary>
    ///     Gets the guild that owns this role.
    /// </summary>
    /// <returns> A <see cref="SocketGuild"/> representing the parent guild of this role. </returns>
    public SocketGuild Guild { get; }

    /// <inheritdoc />
    public RoleType Type { get; private set; }

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public Color Color { get; private set; }

    /// <inheritdoc />
    public ColorType ColorType { get; private set; }

    /// <inheritdoc />
    public GradientColor? GradientColor { get; private set; }

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
    /// <returns> <c>true</c> if the role is @everyone; otherwise <c>false</c>. </returns>
    public bool IsEveryone => Id == 0;

    /// <inheritdoc />
    public string KMarkdownMention => IsEveryone ? MentionUtils.KMarkdownMentionRole("all") : MentionUtils.KMarkdownMentionRole(Id);

    /// <inheritdoc />
    public string PlainTextMention => IsEveryone ? "@全体成员" : MentionUtils.PlainTextMentionRole(Id);

    internal SocketRole(SocketGuild guild, uint id)
        : base(guild.Kook, id)
    {
        Name = string.Empty;
        Guild = guild;
    }

    internal static SocketRole Create(SocketGuild guild, ClientState state, Model model)
    {
        SocketRole entity = new(guild, model.Id);
        entity.Update(state, model);
        return entity;
    }

    internal void Update(ClientState state, Model model)
    {
        Name = model.Name;
        Type = model.Type;
        Color = model.Color;
        ColorType = model.ColorType;
        GradientColor = model.GradientColor;
        Position = model.Position;
        IsHoisted = model.IsHoisted;
        IsMentionable = model.IsMentionable;
        Permissions = new GuildPermissions(model.Permissions);
    }

    /// <inheritdoc />
    public Task ModifyAsync(Action<RoleProperties> func, RequestOptions? options = null) =>
        RoleHelper.ModifyAsync(this, Kook, func, options);

    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions? options = null) =>
        RoleHelper.DeleteAsync(this, Kook, options);

    /// <summary>
    ///     Gets a collection of users with this role.
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> Paged collection of users with this role. </returns>
    /// <remarks>
    ///     If the guild this role belongs to does not has all members cached locally
    ///     by checking <see cref="SocketGuild.HasAllMembers"/>, this method will request
    ///     the data via REST and update the guild users cache, otherwise it will
    ///     return the cached data.
    /// </remarks>
    public async IAsyncEnumerable<IReadOnlyCollection<SocketGuildUser>> GetUsersAsync(RequestOptions? options = null)
    {
        // From SocketGuild.Users
        if (Guild.HasAllMembers is true)
        {
            IEnumerable<IReadOnlyCollection<SocketGuildUser>> userCollections = Guild.Users
                .Where(u => u.Roles.Contains(this))
                .Chunk(KookConfig.MaxUsersPerBatch)
                .Select(c => c.ToImmutableList());
            foreach (IReadOnlyCollection<SocketGuildUser> users in userCollections)
                yield return users;
            yield break;
        }

        // Update SocketGuild.Users by fetching from REST API
        IAsyncEnumerable<IReadOnlyCollection<GuildMember>> restUserCollections = Kook
            .ApiClient
            .GetGuildMembersAsync(Guild.Id, x => x.RoleId = Id, KookConfig.MaxUsersPerBatch, 1, options);
        await foreach (IReadOnlyCollection<GuildMember> restUsers in restUserCollections)
            yield return [..restUsers.Select(restUser => Guild.AddOrUpdateUser(restUser))];
    }

    /// <inheritdoc />
    public int CompareTo(IRole? role) => RoleUtils.Compare(this, role);

    #endregion

    /// <summary>
    ///     Gets the name of the role.
    /// </summary>
    /// <returns> A string that resolves to <see cref="Kook.WebSocket.SocketRole.Name" />. </returns>
    public override string ToString() => Name;

    private string DebuggerDisplay => $"{Name} ({Id})";
    internal SocketRole Clone() => (SocketRole)MemberwiseClone();

    #region IRole

    /// <inheritdoc />
    IGuild IRole.Guild => Guild;

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IRole.GetUsersAsync(CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? GetUsersAsync(options)
            : AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();

    #endregion
}
