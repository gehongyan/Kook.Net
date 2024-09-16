using System.Diagnostics;
using Model = Kook.API.Role;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的可授予服务器用户的角色。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestRole : RestEntity<uint>, IRole
{
    #region RestRole

    /// <inheritdoc />
    public IGuild Guild { get; }

    /// <inheritdoc />
    public RoleType Type { get; private set; }

    /// <inheritdoc />
    public Color Color { get; private set; }

    /// <inheritdoc />
    public ColorType ColorType { get; private set; }

    /// <inheritdoc />
    public GradientColor? GradientColor { get; private set; }

    /// <inheritdoc />
    public bool IsHoisted { get; private set; }

    /// <inheritdoc />
    public bool IsMentionable { get; private set; }

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public GuildPermissions Permissions { get; private set; }

    /// <inheritdoc />
    public int Position { get; private set; }

    /// <summary>
    ///     获取此角色是否为 <c>@全体成员</c> 全体成员角色。
    /// </summary>
    public bool IsEveryone => Id == 0;

    /// <inheritdoc />
    public string KMarkdownMention => IsEveryone ? "(met)all(met)" : MentionUtils.KMarkdownMentionRole(Id);

    /// <inheritdoc />
    public string PlainTextMention => IsEveryone ? "@全体成员" : MentionUtils.PlainTextMentionRole(Id);

    internal RestRole(BaseKookClient kook, IGuild guild, uint id)
        : base(kook, id)
    {
        Guild = guild;
        Name = string.Empty;
    }

    internal static RestRole Create(BaseKookClient kook, IGuild guild, Model model)
    {
        RestRole entity = new(kook, guild, model.Id);
        entity.Update(model);
        return entity;
    }

    internal void Update(Model model)
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
    public async Task ModifyAsync(Action<RoleProperties> func, RequestOptions? options = null)
    {
        Model model = await RoleHelper.ModifyAsync(this, Kook, func, options).ConfigureAwait(false);
        Update(model);
    }

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
    public IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(RequestOptions? options = null) =>
        GuildHelper.SearchUsersAsync(Guild, Kook, x => x.RoleId = Id, KookConfig.MaxUsersPerBatch, options);

    /// <inheritdoc />
    public int CompareTo(IRole? role) => RoleUtils.Compare(this, role);

    #endregion

    #region IRole

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IRole.GetUsersAsync(CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? GetUsersAsync(options)
            : AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();

    #endregion

    /// <inheritdoc cref="Kook.Rest.RestRole.Name" />
    /// <returns> 此角色的名称。 </returns>
    public override string ToString() => Name;

    private string DebuggerDisplay => $"{Name} ({Id})";
}
