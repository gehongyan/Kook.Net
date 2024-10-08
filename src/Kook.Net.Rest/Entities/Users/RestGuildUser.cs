using System.Collections.Immutable;
using System.Diagnostics;
using MemberModel = Kook.API.Rest.GuildMember;
using UserModel = Kook.API.User;

namespace Kook.Rest;

/// <summary>
///     表示一个基于 REST 的服务器用户。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class RestGuildUser : RestUser, IGuildUser
{
    #region RestGuildUser

    private ImmutableArray<uint> _roleIds;

    /// <inheritdoc />
    public string DisplayName => Nickname ?? Username;

    /// <inheritdoc />
    public string? Nickname { get; private set; }

    /// <inheritdoc />
    public IGuild Guild { get; }

    /// <inheritdoc />
    public ulong GuildId => Guild.Id;

    /// <inheritdoc />
    public bool? IsMobileVerified { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? JoinedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? ActiveAt { get; private set; }

    /// <inheritdoc />
    public Color? Color { get; private set; }

    /// <inheritdoc />
    public bool? IsOwner { get; private set; }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException" accessor="get"> 解析服务器用户的权限需要所属服务器的基础数据已完整下载。 </exception>
    public GuildPermissions GuildPermissions => Guild.IsAvailable
        ? new GuildPermissions(Permissions.ResolveGuild(Guild, this))
        : throw new InvalidOperationException("Resolving permissions requires the parent guild to be downloaded.");

    /// <inheritdoc />
    public IReadOnlyCollection<uint> RoleIds => _roleIds;

    /// <inheritdoc />
    public new string PlainTextMention => MentionUtils.PlainTextMentionUser(Nickname ?? Username, Id);

    internal RestGuildUser(BaseKookClient kook, IGuild guild, ulong id)
        : base(kook, id)
    {
        Guild = guild;
        _roleIds = [0];
    }

    internal static RestGuildUser Create(BaseKookClient kook, IGuild guild, UserModel model)
    {
        RestGuildUser entity = new(kook, guild, model.Id);
        entity.Update(model);
        return entity;
    }

    internal static RestGuildUser Create(BaseKookClient kook, IGuild guild, MemberModel model)
    {
        RestGuildUser entity = new(kook, guild, model.Id);
        entity.Update(model);
        return entity;
    }

    internal static RestGuildUser Create(BaseKookClient kook, IGuild guild, API.MentionedUser model)
    {
        RestGuildUser entity = new(kook, guild, model.Id);
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
        Color = model.Color;
        IsOwner = model.IsOwner ?? Guild.OwnerId == Id;
        if (model.Roles != null)
            _roleIds = [0, ..model.Roles];
    }

    internal override void Update(API.MentionedUser model)
    {
        base.Update(model);
        if (DisplayName != Username)
            Nickname = model.DisplayName;
    }

    internal void AddRole(uint roleId)
    {
        _roleIds = [.._roleIds, roleId];
    }

    internal void RemoveRole(uint roleId)
    {
        _roleIds = _roleIds.Remove(roleId);
    }

    /// <inheritdoc />
    public override async Task UpdateAsync(RequestOptions? options = null)
    {
        MemberModel model = await Kook.ApiClient.GetGuildMemberAsync(GuildId, Id, options).ConfigureAwait(false);
        Update(model);
    }

    /// <inheritdoc />
    public async Task ModifyNicknameAsync(string? name, RequestOptions? options = null)
    {
        string? nickname = await UserHelper.ModifyNicknameAsync(this, Kook, name, options);
        // The KOOK API will clear the nickname if the nickname is set to the same as the username at present.
        Nickname = nickname == Username ? null : nickname;
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<BoostSubscriptionMetadata>> GetBoostSubscriptionsAsync(
        RequestOptions? options = null) =>
        UserHelper.GetBoostSubscriptionsAsync(this, Kook, options);

    /// <inheritdoc />
    public Task KickAsync(RequestOptions? options = null) =>
        UserHelper.KickAsync(this, Kook, options);

    /// <inheritdoc />
    public Task AddRoleAsync(uint roleId, RequestOptions? options = null) =>
        AddRolesAsync(new[] { roleId }, options);

    /// <inheritdoc />
    public Task AddRoleAsync(IRole role, RequestOptions? options = null) =>
        AddRoleAsync(role.Id, options);

    /// <inheritdoc />
    public Task AddRolesAsync(IEnumerable<uint> roleIds, RequestOptions? options = null) =>
        UserHelper.AddRolesAsync(this, Kook, roleIds, options);

    /// <inheritdoc />
    public Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions? options = null) =>
        AddRolesAsync(roles.Select(x => x.Id), options);

    /// <inheritdoc />
    public Task RemoveRoleAsync(uint roleId, RequestOptions? options = null) =>
        RemoveRolesAsync(new[] { roleId }, options);

    /// <inheritdoc />
    public Task RemoveRoleAsync(IRole role, RequestOptions? options = null) =>
        RemoveRoleAsync(role.Id, options);

    /// <inheritdoc />
    public Task RemoveRolesAsync(IEnumerable<uint> roleIds, RequestOptions? options = null) =>
        UserHelper.RemoveRolesAsync(this, Kook, roleIds, options);

    /// <inheritdoc />
    public Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions? options = null) =>
        RemoveRolesAsync(roles.Select(x => x.Id));

    /// <inheritdoc />
    public Task MuteAsync(RequestOptions? options = null) =>
        GuildHelper.MuteUserAsync(this, Kook, options);

    /// <inheritdoc />
    public Task DeafenAsync(RequestOptions? options = null) =>
        GuildHelper.DeafenUserAsync(this, Kook, options);

    /// <inheritdoc />
    public Task UnmuteAsync(RequestOptions? options = null) =>
        GuildHelper.UnmuteUserAsync(this, Kook, options);

    /// <inheritdoc />
    public Task UndeafenAsync(RequestOptions? options = null) =>
        GuildHelper.UndeafenUserAsync(this, Kook, options);

    /// <inheritdoc />
    public Task<IReadOnlyCollection<IVoiceChannel>> GetConnectedVoiceChannelsAsync(RequestOptions? options = null) =>
        UserHelper.GetConnectedChannelAsync(this, Kook, options);

    /// <inheritdoc />
    public ChannelPermissions GetPermissions(IGuildChannel channel)
    {
        GuildPermissions guildPerms = GuildPermissions;
        return new ChannelPermissions(Permissions.ResolveChannel(Guild, this, channel, guildPerms.RawValue));
    }

    /// <inheritdoc />
    public override Task RequestFriendAsync(RequestOptions? options = null) =>
        UserHelper.RequestFriendAsync(this, Kook, options);

    #endregion

    #region IVoiceState

    /// <inheritdoc />
    bool? IVoiceState.IsMuted => null;

    /// <inheritdoc />
    bool? IVoiceState.IsDeafened => null;

    /// <inheritdoc />
    IVoiceChannel? IVoiceState.VoiceChannel => null;

    /// <inheritdoc />
    IReadOnlyCollection<IVoiceChannel> IVoiceState.VoiceChannels => [];

    #endregion
}
