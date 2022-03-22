using System.Diagnostics;
using Model = KaiHeiLa.API.Role;

namespace KaiHeiLa.Rest;

/// <summary>
///     Represents a REST-based role.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class RestRole : RestEntity<uint>, IRole
{
    #region RestRole

    internal IGuild Guild { get; }
    /// <inheritdoc />
    public Color Color { get; private set; }
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

    internal RestRole(BaseKaiHeiLaClient kaiHeiLa, IGuild guild, uint id)
        : base(kaiHeiLa, id)
    {
        Guild = guild;
    }
    internal static RestRole Create(BaseKaiHeiLaClient kaiHeiLa, IGuild guild, Model model)
    {
        var entity = new RestRole(kaiHeiLa, guild, model.Id);
        entity.Update(model);
        return entity;
    }
    internal void Update(Model model)
    {
        Name = model.Name;
        IsHoisted = model.Hoist == 1;
        IsMentionable = model.Mentionable == 1;
        Position = model.Position;
        Color = new Color(model.Color);
        Permissions = new GuildPermissions(model.Permissions);
    }
    
    /// <inheritdoc />
    public async Task ModifyAsync(Action<RoleProperties> func, RequestOptions options = null)
    {
        var model = await RoleHelper.ModifyAsync(this, KaiHeiLa, func, options).ConfigureAwait(false);
        Update(model);
    }
    
    public IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(RequestOptions options = null)
    {
        void Func(SearchGuildMemberProperties p) => p.RoleId = Id;
        return GuildHelper.SearchUsersAsync(Guild, KaiHeiLa, Func, KaiHeiLaConfig.MaxUsersPerBatch, options);
    }
    
    #endregion
    
    #region IRole
    /// <inheritdoc />
    IGuild IRole.Guild
    {
        get
        {
            if (Guild != null)
                return Guild;
            throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
        }
    }
    
    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IRole.GetUsersAsync(CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return GetUsersAsync(options);
        else
            return AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();
    }
    
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