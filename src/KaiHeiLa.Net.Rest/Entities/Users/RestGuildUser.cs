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
    public string DisplayName => Nickname ?? Username;
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
    /// <exception cref="InvalidOperationException" accessor="get">Resolving permissions requires the parent guild to be downloaded.</exception>
    public GuildPermissions GuildPermissions
    {
        get
        {
            if (!Guild.Available)
                throw new InvalidOperationException("Resolving permissions requires the parent guild to be downloaded.");
            return new GuildPermissions(Permissions.ResolveGuild(Guild, this));
        }
    }
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
    public override async Task ReloadAsync(RequestOptions options = null)
    {
        var model = await KaiHeiLa.ApiClient.GetGuildMemberAsync(GuildId, Id, options).ConfigureAwait(false);
        Update(model);
    }
    
    /// <inheritdoc />
    public async Task ModifyNicknameAsync(Action<string> func, RequestOptions options = null)
    {
        var nickname = await UserHelper.ModifyNicknameAsync(this, KaiHeiLa, func, options);
        Nickname = nickname;
    }
    /// <inheritdoc />
    public Task KickAsync(RequestOptions options = null)
        => UserHelper.KickAsync(this, KaiHeiLa, options);
    /// <inheritdoc />
    public Task AddRoleAsync(uint roleId, RequestOptions options = null)
        => AddRolesAsync(new[] { roleId }, options);
    /// <inheritdoc />
    public Task AddRoleAsync(IRole role, RequestOptions options = null)
        => AddRoleAsync(role.Id, options);
    /// <inheritdoc />
    public Task AddRolesAsync(IEnumerable<uint> roleIds, RequestOptions options = null)
        => UserHelper.AddRolesAsync(this, KaiHeiLa, roleIds, options);
    /// <inheritdoc />
    public Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null)
        => AddRolesAsync(roles.Select(x => x.Id), options);
    /// <inheritdoc />
    public Task RemoveRoleAsync(uint roleId, RequestOptions options = null)
        => RemoveRolesAsync(new[] { roleId }, options);
    /// <inheritdoc />
    public Task RemoveRoleAsync(IRole role, RequestOptions options = null)
        => RemoveRoleAsync(role.Id, options);
    /// <inheritdoc />
    public Task RemoveRolesAsync(IEnumerable<uint> roleIds, RequestOptions options = null)
        => UserHelper.RemoveRolesAsync(this, KaiHeiLa, roleIds, options);
    /// <inheritdoc />
    public Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null)
        => RemoveRolesAsync(roles.Select(x => x.Id));
    /// <inheritdoc />
    public Task MuteAsync(RequestOptions options = null) 
        => GuildHelper.MuteUserAsync(this, KaiHeiLa, options);
    /// <inheritdoc />
    public Task DeafenAsync(RequestOptions options = null) 
        => GuildHelper.DeafenUserAsync(this, KaiHeiLa, options);
    /// <inheritdoc />
    public Task UnmuteAsync(RequestOptions options = null) 
        => GuildHelper.UnmuteUserAsync(this, KaiHeiLa, options);
    /// <inheritdoc />
    public Task UndeafenAsync(RequestOptions options = null) 
        => GuildHelper.UndeafenUserAsync(this, KaiHeiLa, options);
    /// <inheritdoc />
    public Task<IReadOnlyCollection<IVoiceChannel>> GetConnectedVoiceChannelsAsync(RequestOptions options = null)
        => UserHelper.GetConnectedChannelAsync(this, KaiHeiLa, options);
    
    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Resolving permissions requires the parent guild to be downloaded.</exception>
    public ChannelPermissions GetPermissions(IGuildChannel channel)
    {
        var guildPerms = GuildPermissions;
        return new ChannelPermissions(Permissions.ResolveChannel(Guild, this, channel, guildPerms.RawValue));
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