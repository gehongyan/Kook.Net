using System.Collections.Immutable;
using System.Diagnostics;
using UserModel = KaiHeiLa.API.User;
using MemberModel = KaiHeiLa.API.Rest.GuildMember;

namespace KaiHeiLa.Rest;

/// <summary>
///     Represents a REST-based guild user.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class RestGuildUser : RestUser, IGuildUser
{
    #region RestGuildUser

    private ImmutableArray<uint> _roleIds;
    
    /// <inheritdoc />
    public string Nickname { get; private set; }
    internal IGuild Guild { get; private set; }
    /// <inheritdoc />
    public ulong GuildId => Guild.Id;
    /// <inheritdoc />
    public bool IsMobileVerified { get; private set; }
    
    /// <inheritdoc />
    public DateTimeOffset JoinedAt { get; private set; }
    /// <inheritdoc />
    public DateTimeOffset ActiveAt { get; private set; }
    /// <inheritdoc />
    public Color Color { get; private set; }
    /// <inheritdoc />
    public bool? IsOwner { get; set; }
    
    /// <inheritdoc />
    public IReadOnlyCollection<uint> RoleIds => _roleIds;
    
    /// <inheritdoc />
    public new string PlainTextMention => MentionUtils.PlainTextMentionUser(Nickname ?? Username, Id);
    
    internal RestGuildUser(BaseKaiHeiLaClient kaiHeiLa, IGuild guild, ulong id)
        : base(kaiHeiLa, id)
    {
        Guild = guild;
    }
    internal static RestGuildUser Create(BaseKaiHeiLaClient kaiHeiLa, IGuild guild, UserModel model)
    {
        var entity = new RestGuildUser(kaiHeiLa, guild, model.Id);
        entity.Update(model);
        entity.UpdateRoles(Array.Empty<uint>());
        return entity;
    }
    internal void Update(MemberModel model)
    {
        base.Update(model);
        Nickname = model.Nickname;
        IsMobileVerified = model.MobileVerified;
        JoinedAt = model.JoinedAt;
        ActiveAt = model.ActiveAt;
        Color = new Color(model.Color);
        IsOwner = model.IsOwner;
        UpdateRoles(model.Roles);
    }
    private void UpdateRoles(uint[] roleIds)
    {
        var roles = ImmutableArray.CreateBuilder<uint>(roleIds.Length + 1);
        for (int i = 0; i < roleIds.Length; i++)
            roles.Add(roleIds[i]);
        _roleIds = roles.ToImmutable();
    }
    /// <inheritdoc />
    public override async Task UpdateAsync(RequestOptions options = null)
    {
        var model = await KaiHeiLa.ApiClient.GetGuildMemberAsync(GuildId, Id, options).ConfigureAwait(false);
        Update(model);
    }
    
    #endregion
    
    #region IGuildUser
    /// <inheritdoc />
    IGuild IGuildUser.Guild
    {
        get
        {
            if (Guild != null)
                return Guild;
            throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
        }
    }
    #endregion
}