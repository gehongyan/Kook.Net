using System.Diagnostics;
using Model = Kook.API.Role;

namespace Kook.Rest;

/// <summary>
///     Represents a REST-based role.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestRole : RestEntity<uint>, IRole
{
    #region RestRole

    /// <summary>
    ///     Gets the guild that owns this role.
    /// </summary>
    /// <returns>
    ///     An <see cref="IGuild"/> representing the parent guild of this role.
    /// </returns>
    internal IGuild Guild { get; }

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
    ///     Returns a value that determines if the role is an @everyone role.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the role is @everyone; otherwise <c>false</c>.
    /// </returns>
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
        IsHoisted = model.Hoist;
        IsMentionable = model.Mentionable;
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
    ///     Gets a collection of users that have this role.
    /// </summary>
    /// <param name="options"> The options to be used when fetching the users. </param>
    /// <returns> An asynchronous enumerable that contains a collection of users that have this role. </returns>
    public IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(RequestOptions? options = null) =>
        GuildHelper.SearchUsersAsync(Guild, Kook, x => x.RoleId = Id, KookConfig.MaxUsersPerBatch, options);

    /// <inheritdoc />
    public int CompareTo(IRole? role) => RoleUtils.Compare(this, role);

    #endregion

    #region IRole

    /// <inheritdoc />
    IGuild IRole.Guild => Guild;

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IRole.GetUsersAsync(CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? GetUsersAsync(options)
            : AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();

    #endregion

    /// <summary>
    ///     Gets the name of the role.
    /// </summary>
    /// <returns>
    ///     A string that is the name of the role.
    /// </returns>
    public override string ToString() => Name;

    private string DebuggerDisplay => $"{Name} ({Id})";
}
