using Kook.API.Gateway;
using Kook.API.Rest;
using Kook.Rest;
using System.Collections.Immutable;
using System.Diagnostics;
using MemberModel = Kook.API.Rest.GuildMember;
using UserModel = Kook.API.User;

namespace Kook.WebSocket;

/// <summary>
///     表示一个基于网关的服务器用户。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketGuildUser : SocketUser, IGuildUser, IUpdateable
{
    #region SocketGuildUser

    private ImmutableArray<uint> _roleIds;

    internal override SocketGlobalUser GlobalUser { get; }

    /// <inheritdoc cref="Kook.IGuildUser.Guild" />
    public SocketGuild Guild { get; }

    /// <inheritdoc />
    public string DisplayName => Nickname ?? Username;

    /// <inheritdoc />
    public string? Nickname { get; private set; }

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
    public new string PlainTextMention => MentionUtils.PlainTextMentionUser(Nickname ?? Username, Id);

    /// <inheritdoc />
    public override bool? IsBot
    {
        get => GlobalUser.IsBot;
        internal set => GlobalUser.IsBot = value;
    }

    /// <inheritdoc />
    public override string Username
    {
        get => GlobalUser.Username;
        internal set => GlobalUser.Username = value;
    }

    /// <inheritdoc />
    public override ushort IdentifyNumberValue
    {
        get => GlobalUser.IdentifyNumberValue;
        internal set => GlobalUser.IdentifyNumberValue = value;
    }

    /// <inheritdoc />
    public override string Avatar
    {
        get => GlobalUser.Avatar;
        internal set => GlobalUser.Avatar = value;
    }

    /// <inheritdoc />
    public override string? BuffAvatar
    {
        get => GlobalUser.BuffAvatar;
        internal set => GlobalUser.BuffAvatar = value;
    }

    /// <inheritdoc />
    public override string? Banner
    {
        get => GlobalUser.Banner;
        internal set => GlobalUser.Banner = value;
    }

    /// <inheritdoc />
    public override bool? IsBanned
    {
        get => GlobalUser.IsBanned;
        internal set => GlobalUser.IsBanned = value;
    }

    /// <inheritdoc />
    public override bool? HasBuff
    {
        get => GlobalUser.HasBuff;
        internal set => GlobalUser.HasBuff = value;
    }

    /// <inheritdoc />
    public override bool? HasAnnualBuff
    {
        get => GlobalUser.HasAnnualBuff;
        internal set => GlobalUser.HasAnnualBuff = value;
    }

    /// <inheritdoc />
    public override bool? IsDenoiseEnabled
    {
        get => GlobalUser.IsDenoiseEnabled;
        internal set => GlobalUser.IsDenoiseEnabled = value;
    }

    /// <inheritdoc />
    public override UserTag? UserTag
    {
        get => GlobalUser.UserTag;
        internal set => GlobalUser.UserTag = value;
    }

    /// <inheritdoc />
    public override IReadOnlyCollection<Nameplate> Nameplates
    {
        get => GlobalUser.Nameplates;
        internal set => GlobalUser.Nameplates = value;
    }

    /// <inheritdoc />
    public GuildPermissions GuildPermissions => new(Permissions.ResolveGuild(Guild, this));

    /// <inheritdoc />
    internal override SocketPresence Presence
    {
        get => GlobalUser.Presence;
        set => GlobalUser.Presence = value;
    }

    /// <inheritdoc />
    public bool? IsDeafened => VoiceState?.IsDeafened;

    /// <inheritdoc />
    public bool? IsMuted => VoiceState?.IsMuted;

    /// <inheritdoc cref="Kook.WebSocket.SocketVoiceState.LiveStreamStatus" />
    public LiveStreamStatus? LiveStreamStatus => VoiceState?.LiveStreamStatus;

    /// <summary>
    ///     获取此用户在该服务器内的所有服务器助力信息。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         当 <see cref="Kook.WebSocket.KookSocketConfig.AlwaysDownloadBoostSubscriptions"/> 为 <c>true</c>
    ///         时，Bot 启动时会自动下载所有服务器的所有助力信息。否则，此属性将为 <c>null</c>。调用
    ///         <see cref="Kook.WebSocket.SocketGuild.DownloadBoostSubscriptionsAsync(Kook.RequestOptions)"/>
    ///         也可以立即下载服务器的所有助力信息，下载完成后，此属性值将被设定。
    ///         <br />
    ///         网关不会发布有关此属性值变更的事件，此属性值可能并不准确。要获取准确的服务器订阅信息，请调用
    ///         <see cref="Kook.WebSocket.SocketGuild.GetBoostSubscriptionsAsync(Kook.RequestOptions)"/>。
    ///     </note>
    /// </remarks>
    public IReadOnlyCollection<BoostSubscriptionMetadata> BoostSubscriptions => Guild.BoostSubscriptions?
            .Where(x => x.Key.Id == Id)
            .SelectMany(x => x.Value)
            .ToImmutableArray()
        ?? [];

    /// <summary>
    ///     获取此用户在该服务器内拥有的所有角色。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         由于网关不会发布有关服务器用户角色变更的事件，此属性值可能并不准确。要获取准确的角色信息，请在使用此属性前调用
    ///         <see cref="Kook.WebSocket.SocketGuildUser.UpdateAsync(Kook.RequestOptions)"/>。
    ///     </note>
    /// </remarks>
    public IReadOnlyCollection<SocketRole> Roles => _roleIds
        .Select(x => Guild.GetRole(x) ?? new SocketRole(Guild, x))
        .Where(x => x != null)
        .ToImmutableArray();

    /// <inheritdoc cref="Kook.WebSocket.SocketVoiceState.VoiceChannel" />
    /// <summary>
    ///     <br />
    ///     <note type="warning">
    ///         默认情况下，此属性不会返回用户在 Bot 启动前所连接到的语音频道。如需让 Bot 在启动后自动获取所有用户的语音状态，请设置
    ///         <see cref="Kook.WebSocket.KookSocketConfig.AlwaysDownloadVoiceStates"/>，也可以在使用此属性前调用
    ///         <see cref="Kook.WebSocket.SocketGuild.DownloadVoiceStatesAsync(Kook.RequestOptions)"/> 或
    ///     </note>
    /// </summary>
    public SocketVoiceChannel? VoiceChannel => VoiceState?.VoiceChannel;

    /// <summary>
    ///     获取此用户的语音状态。
    /// </summary>
    public SocketVoiceState? VoiceState => Guild.GetVoiceState(Id);

    internal SocketGuildUser(SocketGuild guild, SocketGlobalUser globalUser)
        : base(guild.Kook, globalUser.Id)
    {
        _roleIds = [];
        Guild = guild;
        GlobalUser = globalUser;
    }

    internal static SocketGuildUser Create(SocketGuild guild, ClientState state, UserModel model)
    {
        SocketGuildUser entity = new(guild, guild.Kook.GetOrCreateUser(state, model));
        entity.Update(state, model);
        entity.UpdatePresence(model.Online, model.OperatingSystem);
        return entity;
    }

    internal static SocketGuildUser Create(SocketGuild guild, ClientState state, MemberModel model)
    {
        SocketGuildUser entity = new(guild, guild.Kook.GetOrCreateUser(state, model));
        entity.Update(state, model);
        entity.UpdatePresence(model.Online, model.OperatingSystem);
        return entity;
    }

    internal static SocketGuildUser Create(SocketGuild guild, ClientState state, RichGuild model)
    {
        if (guild.Kook.CurrentUser is null)
            throw new InvalidOperationException("The current user is not set well via login.");
        SocketGuildUser entity = new(guild, guild.Kook.CurrentUser.GlobalUser);
        entity.Update(state, model);
        return entity;
    }

    internal void Update(ClientState state, MemberModel model)
    {
        base.Update(state, model);
        // The KOOK API returns the user's nickname as the same as their username
        // if they don't have their nickname set.
        Nickname = model.Nickname == Username ? null : model.Nickname;
        IsMobileVerified = model.MobileVerified;
        JoinedAt = model.JoinedAt;
        ActiveAt = model.ActiveAt;
        Color = model.Color;
        IsOwner = model.IsOwner ?? Guild.OwnerId == Id;
        if (model.Roles is not null)
            _roleIds = [..model.Roles];
    }

    internal void Update(ClientState state, RichGuild guildModel)
    {
        Nickname = guildModel.CurrentUserNickname == Username ? null : guildModel.CurrentUserNickname;
        if (guildModel.CurrentUserRoles is not null)
            _roleIds = [..guildModel.CurrentUserRoles];
    }

    internal void UpdateNickname()
    {
        if (Nickname == Username)
            Nickname = null;
    }

    internal void Update(ClientState state, GuildMemberUpdateEvent model)
    {
        if (model.Nickname is not null)
        {
            Nickname = model.Nickname == Username || string.IsNullOrWhiteSpace(model.Nickname)
                ? null
                : model.Nickname;
        }
    }

    internal bool Update(ClientState state, GuildUpdateSelfEvent model)
    {
        bool hasChanges = false;
        hasChanges |= ValueHelper.SetIfChanged(
            () => Nickname,
            x => Nickname = x,
            model.CurrentUserNickname == Username || string.IsNullOrWhiteSpace(model.CurrentUserNickname)
                ? null
                : model.CurrentUserNickname);
        return hasChanges;
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
    public Task ModifyNicknameAsync(string? name, RequestOptions? options = null) =>
        UserHelper.ModifyNicknameAsync(this, Kook, name, options);

    /// <inheritdoc />
    public Task<IReadOnlyCollection<BoostSubscriptionMetadata>> GetBoostSubscriptionsAsync(
        RequestOptions? options = null) =>
        UserHelper.GetBoostSubscriptionsAsync(this, Kook, options);

    /// <inheritdoc />
    public Task KickAsync(RequestOptions? options = null) =>
        UserHelper.KickAsync(this, Kook, options);

    /// <inheritdoc />
    public Task AddRoleAsync(uint roleId, RequestOptions? options = null) =>
        AddRolesAsync([roleId], options);

    /// <inheritdoc />
    public Task AddRoleAsync(IRole role, RequestOptions? options = null) =>
        AddRoleAsync(role.Id, options);

    /// <inheritdoc />
    public Task AddRolesAsync(IEnumerable<uint> roleIds, RequestOptions? options = null) =>
        SocketUserHelper.AddRolesAsync(this, Kook, roleIds, options);

    /// <inheritdoc />
    public Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions? options = null) =>
        AddRolesAsync(roles.Select(x => x.Id), options);

    /// <inheritdoc />
    public Task RemoveRoleAsync(uint roleId, RequestOptions? options = null) =>
        RemoveRolesAsync([roleId], options);

    /// <inheritdoc />
    public Task RemoveRoleAsync(IRole role, RequestOptions? options = null) =>
        RemoveRoleAsync(role.Id, options);

    /// <inheritdoc />
    public Task RemoveRolesAsync(IEnumerable<uint> roleIds, RequestOptions? options = null) =>
        SocketUserHelper.RemoveRolesAsync(this, Kook, roleIds, options);

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

    /// <inheritdoc cref="Kook.IGuildUser.GetConnectedVoiceChannelsAsync(Kook.RequestOptions)"/>
    public async Task<IReadOnlyCollection<SocketVoiceChannel>> GetConnectedVoiceChannelsAsync(RequestOptions? options = null)
    {
        IReadOnlyCollection<SocketVoiceChannel> channels =
            await SocketUserHelper.GetConnectedChannelsAsync(this, Kook, options).ConfigureAwait(false);
        Guild.AddOrUpdateVoiceState(Id, channels);
        return channels;
    }

    /// <inheritdoc />
    public Task UpdateAsync(RequestOptions? options = null) =>
        SocketUserHelper.UpdateAsync(this, Kook, options);

    /// <inheritdoc />
    public ChannelPermissions GetPermissions(IGuildChannel channel) =>
        new(Permissions.ResolveChannel(Guild, this, channel, GuildPermissions.RawValue));

    #endregion

    #region IGuildUser

    /// <inheritdoc />
    IGuild IGuildUser.Guild => Guild;

    /// <inheritdoc />
    ulong IGuildUser.GuildId => Guild.Id;

    /// <inheritdoc />
    IReadOnlyCollection<uint> IGuildUser.RoleIds => _roleIds;

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IVoiceChannel>> IGuildUser.GetConnectedVoiceChannelsAsync(RequestOptions? options) =>
        await GetConnectedVoiceChannelsAsync(options).ConfigureAwait(false);

    #endregion

    #region IVoiceState

    /// <inheritdoc />
    IVoiceChannel? IVoiceState.VoiceChannel => VoiceChannel;

    /// <inheritdoc />
    IReadOnlyCollection<IVoiceChannel> IVoiceState.VoiceChannels => VoiceState?.VoiceChannels ?? [];

    #endregion

    private string DebuggerDisplay =>
        $"{this.UsernameAndIdentifyNumber(Kook.FormatUsersInBidirectionalUnicode)} ({Id}{
            (IsBot ?? false ? ", Bot" : "")}, Guild)";

    internal new SocketGuildUser Clone() => (SocketGuildUser)MemberwiseClone();
}
