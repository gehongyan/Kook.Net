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