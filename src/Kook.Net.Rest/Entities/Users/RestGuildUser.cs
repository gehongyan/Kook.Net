using System.Collections.Immutable;
using System.Diagnostics;
using UserModel = Kook.API.User;
using MemberModel = Kook.API.Rest.GuildMember;

namespace Kook.Rest;

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
    
    internal RestGuildUser(BaseKookClient kook, IGuild guild, ulong id)
        : base(kook, id)
    {
        Guild = guild;
    }
    internal static RestGuildUser Create(BaseKookClient kook, IGuild guild, UserModel model)
    {
        var entity = new RestGuildUser(kook, guild, model.Id);
        entity.Update(model);
        entity.UpdateRoles(Array.Empty<uint>());
        return entity;
    }
    internal static RestGuildUser Create(BaseKookClient kook, IGuild guild, MemberModel model)
    {
        var entity = new RestGuildUser(kook, guild, model.Id);
        entity.Update(model);
        return entity;
    }
    internal void Update(MemberModel model)
    {
        base.Update(model);
        // The KOOK API returns the user's nickname as the same as their username
        // if they don't have their nickname set.
        Nickname = model.Nickname == Username ? null : model.Nickname;
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
        roles.Add(0);
        for (int i = 0; i < roleIds.Length; i++)
            roles.Add(roleIds[i]);
        _roleIds = roles.ToImmutable();
    }
    /// <inheritdoc />
    public override async Task UpdateAsync(RequestOptions options = null)
    {
        var model = await Kook.ApiClient.GetGuildMemberAsync(GuildId, Id, options).ConfigureAwait(false);
        Update(model);
    }
    
    /// <inheritdoc />
    public async Task ModifyNicknameAsync(string name, RequestOptions options = null)
    {
        var nickname = await UserHelper.ModifyNicknameAsync(this, Kook, name, options);
        // The KOOK API will clear the nickname if the nickname is set to the same as the username at present.
        if(nickname == Username)
            nickname = null;
        Nickname = nickname;
    }
    /// <inheritdoc />
    public Task KickAsync(RequestOptions options = null)
        => UserHelper.KickAsync(this, Kook, options);
    /// <inheritdoc />
    /// <note type="warning">
    ///     This method will update the cached roles of this user.
    ///     To update the cached roles of this user, please use <see cref="UpdateAsync"/>.
    /// </note>
    public Task AddRoleAsync(uint roleId, RequestOptions options = null)
        => AddRolesAsync(new[] { roleId }, options);
    /// <inheritdoc />
    /// <note type="warning">
    ///     This method will update the cached roles of this user.
    ///     To update the cached roles of this user, please use <see cref="UpdateAsync"/>.
    /// </note>
    public Task AddRoleAsync(IRole role, RequestOptions options = null)
        => AddRoleAsync(role.Id, options);
    /// <inheritdoc />
    /// <note type="warning">
    ///     This method will update the cached roles of this user.
    ///     To update the cached roles of this user, please use <see cref="UpdateAsync"/>.
    /// </note>
    public Task AddRolesAsync(IEnumerable<uint> roleIds, RequestOptions options = null)
        => UserHelper.AddRolesAsync(this, Kook, roleIds, options);
    /// <inheritdoc />
    /// <note type="warning">
    ///     This method will update the cached roles of this user.
    ///     To update the cached roles of this user, please use <see cref="UpdateAsync"/>.
    /// </note>
    public Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null)
        => AddRolesAsync(roles.Select(x => x.Id), options);
    /// <inheritdoc />
    /// <note type="warning">
    ///     This method will update the cached roles of this user.
    ///     To update the cached roles of this user, please use <see cref="UpdateAsync"/>.
    /// </note>
    public Task RemoveRoleAsync(uint roleId, RequestOptions options = null)
        => RemoveRolesAsync(new[] { roleId }, options);
    /// <inheritdoc />
    /// <note type="warning">
    ///     This method will update the cached roles of this user.
    ///     To update the cached roles of this user, please use <see cref="UpdateAsync"/>.
    /// </note>
    public Task RemoveRoleAsync(IRole role, RequestOptions options = null)
        => RemoveRoleAsync(role.Id, options);
    /// <inheritdoc />
    /// <note type="warning">
    ///     This method will update the cached roles of this user.
    ///     To update the cached roles of this user, please use <see cref="UpdateAsync"/>.
    /// </note>
    public Task RemoveRolesAsync(IEnumerable<uint> roleIds, RequestOptions options = null)
        => UserHelper.RemoveRolesAsync(this, Kook, roleIds, options);
    /// <inheritdoc />
    /// <note type="warning">
    ///     This method will update the cached roles of this user.
    ///     To update the cached roles of this user, please use <see cref="UpdateAsync"/>.
    /// </note>
    public Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null)
        => RemoveRolesAsync(roles.Select(x => x.Id));
    /// <inheritdoc />
    public Task MuteAsync(RequestOptions options = null) 
        => GuildHelper.MuteUserAsync(this, Kook, options);
    /// <inheritdoc />
    public Task DeafenAsync(RequestOptions options = null) 
        => GuildHelper.DeafenUserAsync(this, Kook, options);
    /// <inheritdoc />
    public Task UnmuteAsync(RequestOptions options = null) 
        => GuildHelper.UnmuteUserAsync(this, Kook, options);
    /// <inheritdoc />
    public Task UndeafenAsync(RequestOptions options = null) 
        => GuildHelper.UndeafenUserAsync(this, Kook, options);
    /// <inheritdoc />
    public Task<IReadOnlyCollection<IVoiceChannel>> GetConnectedVoiceChannelsAsync(RequestOptions options = null)
        => UserHelper.GetConnectedChannelAsync(this, Kook, options);
    
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
    
    #region IVoiceState
    
    /// <inheritdoc />
    bool? IVoiceState.IsMuted => null;
    /// <inheritdoc />
    bool? IVoiceState.IsDeafened => null;
    /// <inheritdoc />
    IVoiceChannel IVoiceState.VoiceChannel => null;
    
    #endregion
}