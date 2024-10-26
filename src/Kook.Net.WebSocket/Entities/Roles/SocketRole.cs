using System.Collections.Immutable;
using System.Diagnostics;
using Kook.API.Rest;
using Kook.Rest;
using Model = Kook.API.Role;

namespace Kook.WebSocket;

/// <summary>
///     表示一个基于网关的可授予服务器用户的角色。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketRole : SocketEntity<uint>, IRole
{
    #region SocketRole

    /// <inheritdoc cref="Kook.IRole.Guild" />
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
    ///     获取此角色是否为 <c>@全体成员</c> 角色。
    /// </summary>
    public bool IsEveryone => Id == 0;

    /// <inheritdoc />
    public string KMarkdownMention => IsEveryone ? MentionUtils.KMarkdownMentionRole("all") : MentionUtils.KMarkdownMentionRole(Id);

    /// <inheritdoc />
    public string PlainTextMention => IsEveryone ? "@全体成员" : MentionUtils.PlainTextMentionRole(Id);

    /// <summary>
    ///     获取拥有此角色的所有用户。
    /// </summary>
    /// <remarks>
    ///     此属性将从缓存中获取拥有此角色的所有用户。如果缓存中不存在用户，则此属性将返回一个空集合。
    /// </remarks>
    /// <seealso cref="Kook.WebSocket.SocketRole.GetUsersAsync(Kook.RequestOptions)"/>
    public IEnumerable<SocketGuildUser> Members
        => Guild.Users.Where(x => x.Roles.Any(r => r.Id == Id));

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
    ///     获取拥有此角色的用户的集合。
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         返回的集合是一个异步可枚举对象；调用
    ///         <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///         可以异步枚举所有分页，并将其合并为一个集合。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         请勿一次性获取过多消息，这可能会导致抢占式速率限制，甚至触发实际的速率限制，从而导致 Bot 服务暂停。
    ///     </note>
    ///     <br />
    ///     此方法将尝试获取拥有此角色的所有服务器用户。此方法会根据 <see cref="Kook.KookConfig.MaxUsersPerBatch"/>
    ///     将请求拆分。换句话说，如果存在 500 个用户拥有此角色，而 <see cref="Kook.KookConfig.MaxUsersPerBatch"/> 的常量为
    ///     <c>50</c>，则请求将被拆分为 10 个单独请求，因此异步枚举器会异步枚举返回 10 个响应。
    ///     <see cref="Kook.AsyncEnumerableExtensions.FlattenAsync{T}(System.Collections.Generic.IAsyncEnumerable{System.Collections.Generic.IEnumerable{T}})" />
    ///     方法可以展开这 10 个响应返回的集合，并将其合并为一个集合。
    /// </remarks>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 分页的用户集合的异步可枚举对象。 </returns>
    /// <seealso cref="Kook.WebSocket.SocketRole.Members"/>
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

    /// <inheritdoc cref="Kook.WebSocket.SocketRole.Name" />
    public override string ToString() => Name;

    private string DebuggerDisplay => $"{Name} ({Id}, {Type})";
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
