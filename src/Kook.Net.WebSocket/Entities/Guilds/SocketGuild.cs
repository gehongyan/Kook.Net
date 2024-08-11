using Kook.API.Gateway;
using Kook.Rest;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using Kook.Audio;
using BoostSubscription = Kook.API.Rest.BoostSubscription;
using ChannelModel = Kook.API.Channel;
using ExtendedModel = Kook.API.Rest.ExtendedGuild;
using MemberModel = Kook.API.Rest.GuildMember;
using Model = Kook.API.Guild;
using RecommendInfo = Kook.Rest.RecommendInfo;
using RichModel = Kook.API.Rest.RichGuild;
using RoleModel = Kook.API.Role;
using UserModel = Kook.API.User;

namespace Kook.WebSocket;

/// <summary>
///     表示一个基于网关的服务器。
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketGuild : SocketEntity<ulong>, IGuild, IDisposable, IUpdateable
{
    #region SocketGuild

    private readonly ConcurrentDictionary<ulong, SocketGuildChannel> _channels;
    private readonly ConcurrentDictionary<ulong, SocketGuildUser> _members;
    private readonly ConcurrentDictionary<uint, SocketRole> _roles;
    private readonly ConcurrentDictionary<ulong, SocketVoiceState> _voiceStates;
    private readonly ConcurrentDictionary<string, GuildEmote> _emotes;
    private readonly Dictionary<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>> _boostSubscriptions;

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public string Topic { get; private set; }

    /// <inheritdoc />
    public ulong OwnerId { get; private set; }

    /// <summary>
    ///     获取此服务器的所有者。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         此属性尝试在缓存的用户列表中获取具有其用户 ID 为 <see cref="P:Kook.WebSocket.SocketGuild.OwnerId"/>
    ///         的用户。如果该用户不在缓存中，则此属性将返回 <c>null</c>。
    ///     </note>
    /// </remarks>
    public SocketGuildUser? Owner => GetUser(OwnerId);

    /// <inheritdoc />
    public string Icon { get; private set; }

    /// <inheritdoc />
    public string Banner { get; private set; }

    /// <inheritdoc />
    public NotifyType NotifyType { get; private set; }

    /// <inheritdoc />
    public string Region { get; private set; }

    /// <inheritdoc />
    public bool IsOpenEnabled { get; private set; }

    /// <inheritdoc />
    public uint? OpenId { get; private set; }

    /// <inheritdoc />
    public ulong? DefaultChannelId { get; private set; }

    /// <inheritdoc />
    public ulong? WelcomeChannelId { get; private set; }

    /// <inheritdoc />
    public GuildFeatures Features { get; private set; }

    /// <inheritdoc />
    public int BoostSubscriptionCount { get; private set; }

    /// <inheritdoc />
    public int BufferBoostSubscriptionCount { get; private set; }

    /// <inheritdoc />
    public BoostLevel BoostLevel { get; private set; }

    /// <summary>
    ///     TODO: To be documented.
    /// </summary>
    public int Status { get; private set; }

    /// <summary>
    ///     TODO: To be documented.
    /// </summary>
    public string? AutoDeleteTime { get; private set; }

    /// <inheritdoc cref="IGuild.RecommendInfo"/>
    public RecommendInfo? RecommendInfo { get; private set; }

    /// <summary>
    ///     获取此服务器的成员数。
    /// </summary>
    /// <remarks>
    ///     <note type="tip">
    ///         在 <see cref="P:Kook.WebSocket.SocketGuild.Users"/> 属性上计数的结果为所缓存用户的数量，
    ///         如果缓存不完整，统计结果可能会与此属性值不一致。
    ///     </note>
    ///     <br />
    ///     <note type="warning">
    ///         当 <see cref="P:Kook.WebSocket.KookSocketConfig.AlwaysDownloadUsers"/> 为 <c>true</c> 时。
    ///         Bot 启动后会自动下载服务器的所有用户，并设定此属性值。否则，此属性将为 <c>null</c>。调用
    ///         <see cref="M:Kook.WebSocket.SocketGuild.DownloadUsersAsync(Kook.RequestOptions)"/>
    ///         也可以立即下载服务器的所有用户，下载完成后，此属性值将被设定。
    ///     </note>
    /// </remarks>
    public int? MemberCount { get; internal set; }

    /// <summary>
    ///     获取此服务器内已缓存的成员数量。
    /// </summary>
    public int DownloadedMemberCount => _members.Count;

    /// <inheritdoc />
    public bool IsAvailable { get; private set; }

    /// <summary>
    ///     获取此服务器是否已连接至网关。
    /// </summary>
    public bool IsConnected { get; internal set; }

    /// <summary>
    ///     获取是否已下载此服务器的所有成员至缓存。
    /// </summary>
    /// <remarks>
    ///     当如法确定是否已下载此服务器的所有成员，或者服务器的成员数量未知时，此属性将返回 <c>null</c>。
    /// </remarks>
    public bool? HasAllMembers => MemberCount is null ? null : MemberCount <= DownloadedMemberCount;

    /// <inheritdoc/>
    public int MaxBitrate => GuildHelper.GetMaxBitrate(this);

    /// <inheritdoc />
    [Obsolete("Use AudioClients instead.")]
    public IAudioClient? AudioClient => VoiceChannels
        .FirstOrDefault(x => x.AudioClient is not null)?
        .AudioClient;

    /// <inheritdoc />
    public IReadOnlyDictionary<ulong, IAudioClient> AudioClients => VoiceChannels
        .Where(x => x.AudioClient is not null)
        .ToDictionary(x => x.Id, x => x.AudioClient!);

    internal IReadOnlyDictionary<ulong, SocketVoiceState> VoiceStates => _voiceStates;

    /// <inheritdoc/>
    public ulong MaxUploadLimit => GuildHelper.GetUploadLimit(this);

    /// <summary>
    ///     Gets the current logged-in user.
    /// </summary>
    public SocketGuildUser? CurrentUser =>
        Kook.CurrentUser is not null
        && _members.TryGetValue(Kook.CurrentUser.Id, out SocketGuildUser? member)
            ? member
            : null;

    /// <inheritdoc cref="P:Kook.IGuild.EveryoneRole" />
    public SocketRole EveryoneRole => GetRole(0) ?? new SocketRole(this, 0);

    /// <summary>
    ///     获取此服务器中所有具有文字聊天能力的频道。
    /// </summary>
    /// <remarks>
    ///     语音频道也是一种文字频道，此属性本意用于获取所有具有文字聊天能力的频道，通过此方法获取到的文字频道列表中也包含了语音频道。
    ///     如需获取频道的实际类型，请参考 <see cref="M:Kook.ChannelExtensions.GetChannelType(Kook.IChannel)"/>。
    /// </remarks>
    public IReadOnlyCollection<SocketTextChannel> TextChannels => [..Channels.OfType<SocketTextChannel>()];

    /// <summary>
    ///     获取此服务器中所有具有语音聊天能力的频道。
    /// </summary>
    public IReadOnlyCollection<SocketVoiceChannel> VoiceChannels => [..Channels.OfType<SocketVoiceChannel>()];

    /// <summary>
    ///     获取此服务器中的所有分组频道。
    /// </summary>
    public IReadOnlyCollection<SocketCategoryChannel> CategoryChannels => [..Channels.OfType<SocketCategoryChannel>()];

    /// <summary>
    ///     获取此服务器的所有频道。
    /// </summary>
    public IReadOnlyCollection<SocketGuildChannel> Channels => [.._channels.Values];

    /// <summary>
    ///     获取此服务器的默认文字频道。
    /// </summary>
    public SocketTextChannel? DefaultChannel => TextChannels
        .Where(x => CurrentUser?.GetPermissions(x).ViewChannel is true)
        .FirstOrDefault(c => c.Id == DefaultChannelId);

    /// <summary>
    ///     获取此服务器的欢迎通知频道。
    /// </summary>
    public SocketTextChannel? WelcomeChannel => TextChannels
        .Where(c => CurrentUser?.GetPermissions(c).ViewChannel is true)
        .FirstOrDefault(c => c.Id == WelcomeChannelId);

    /// <inheritdoc cref="P:Kook.IGuild.Emotes" />
    public IReadOnlyCollection<GuildEmote> Emotes => [.._emotes.Values];

    /// <summary>
    ///     获取此服务器内的所有服务器助力信息。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         当 <see cref="P:Kook.WebSocket.KookSocketConfig.AlwaysDownloadBoostSubscriptions"/> 为 <c>true</c>
    ///         时，Bot 启动时会自动下载所有服务器的所有助力信息。否则，此属性将为 <c>null</c>。调用
    ///         <see cref="M:Kook.WebSocket.SocketGuild.DownloadBoostSubscriptionsAsync(Kook.RequestOptions)"/>
    ///         也可以立即下载服务器的所有助力信息，下载完成后，此属性值将被设定。
    ///         <br />
    ///         网关不会发布有关此属性值变更的事件，此属性值可能并不准确。要获取准确的服务器订阅信息，请调用
    ///         <see cref="M:Kook.WebSocket.SocketGuild.GetBoostSubscriptionsAsync(Kook.RequestOptions)"/>。
    ///     </note>
    /// </remarks>
    public ImmutableDictionary<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>> BoostSubscriptions =>
        _boostSubscriptions.ToImmutableDictionary();

    /// <summary>
    ///     获取此服务器内的所有生效中的服务器助力信息。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         当 <see cref="P:Kook.WebSocket.KookSocketConfig.AlwaysDownloadBoostSubscriptions"/> 为 <c>true</c>
    ///         时，Bot 启动时会自动下载所有服务器的所有助力信息。否则，此属性将为 <c>null</c>。调用
    ///         <see cref="M:Kook.WebSocket.SocketGuild.DownloadBoostSubscriptionsAsync(Kook.RequestOptions)"/>
    ///         也可以立即下载服务器的所有助力信息，下载完成后，此属性值将被设定。
    ///         <br />
    ///         网关不会发布有关此属性值变更的事件，此属性值可能并不准确。要获取准确的服务器订阅信息，请调用
    ///         <see cref="M:Kook.WebSocket.SocketGuild.GetActiveBoostSubscriptionsAsync(Kook.RequestOptions)"/>。
    ///     </note>
    /// </remarks>
    public ImmutableDictionary<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>> ActiveBoostSubscriptions =>
        _boostSubscriptions.Select(x =>
                new KeyValuePair<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>>(
                    x.Key, [..x.Value.Where(y => y.IsValid)]))
            .Where(x => x.Value.Count > 0)
            .ToImmutableDictionary();

    /// <summary>
    ///     获取此服务器内缓存的所有用户。
    /// </summary>
    /// <remarks>
    ///     要获取服务器的总成员数量，请访问 <see cref="P:Kook.WebSocket.SocketGuild.MemberCount"/>。
    /// </remarks>
    public IReadOnlyCollection<SocketGuildUser> Users => _members.ToReadOnlyCollection();

    /// <inheritdoc cref="P:Kook.IGuild.Roles" />
    /// <remarks>
    ///     <note type="warning">
    ///         由于 KOOK 不会通过网关发布有关服务器角色重新排序的事件，此属性值可能并不准确。
    ///         要确保获取准确的服务器角色排序信息，请在使用此属性之前调用
    ///         <see cref="M:Kook.WebSocket.SocketGuild.UpdateAsync(Kook.RequestOptions)"/>。
    ///     </note>
    /// </remarks>
    public IReadOnlyCollection<SocketRole> Roles => _roles.ToReadOnlyCollection();

    internal SocketGuild(KookSocketClient kook, ulong id) : base(kook, id)
    {
        _channels = [];
        _members = [];
        _roles = [];
        _voiceStates = [];
        _emotes = [];
        _boostSubscriptions = [];
        Name = string.Empty;
        Topic = string.Empty;
        Icon = string.Empty;
        Banner = string.Empty;
        Region = string.Empty;
    }

    internal static SocketGuild Create(KookSocketClient client, ClientState state, ExtendedModel model)
    {
        SocketGuild entity = new(client, model.Id);
        entity.Update(state, model);
        return entity;
    }

    internal static SocketGuild Create(KookSocketClient client, ClientState state, Model model)
    {
        SocketGuild entity = new(client, model.Id);
        entity.Update(state, model);
        return entity;
    }

    internal static SocketGuild Create(KookSocketClient client, ClientState state, RichModel model)
    {
        SocketGuild entity = new(client, model.Id);
        entity.Update(state, model);
        return entity;
    }

    internal void Update(ClientState state, IReadOnlyCollection<ChannelModel> models)
    {
        foreach (ChannelModel model in models)
        {
            SocketGuildChannel channel = SocketGuildChannel.Create(this, state, model);
            state.AddChannel(channel);
            _channels.TryAdd(channel.Id, channel);
        }
    }

    internal void Update(ClientState state, IReadOnlyCollection<MemberModel> models)
    {
        foreach (MemberModel model in models)
        {
            SocketGuildUser member = SocketGuildUser.Create(this, state, model);
            if (_members.TryAdd(member.Id, member))
                member.GlobalUser.AddRef();
        }
        MemberCount = _members.Count;
    }

    internal void Update(ClientState state, IReadOnlyCollection<BoostSubscription> models)
    {
        foreach (IGrouping<ulong, BoostSubscription> group in models.GroupBy(x => x.UserId))
        {
            SocketGlobalUser user = state.GetOrAddUser(
                group.Key, _ => SocketGlobalUser.Create(Kook, state, group.First().User));
            IReadOnlyCollection<BoostSubscriptionMetadata> subscriptions =
            [
                ..group.GroupBy(x => (x.StartTime, x.EndTime))
                    .Select(x => new BoostSubscriptionMetadata(x.Key.StartTime, x.Key.EndTime, x.Count()))
            ];
            _boostSubscriptions[user] = subscriptions;
        }
    }

    internal void Update(ClientState state, RichModel model)
    {
        Update(state, model as ExtendedModel);
        Banner = model.Banner;
        _emotes.Clear();
        foreach (API.Emoji emoji in model.Emojis)
            _emotes.TryAdd(emoji.Id, emoji.ToEntity(model.Id));
        AddOrUpdateCurrentUser(model);
    }

    internal void Update(ClientState state, ExtendedModel model)
    {
        Update(state, model as Model);
        Features = model.Features;
        BoostSubscriptionCount = model.BoostSubscriptionCount;
        BufferBoostSubscriptionCount = model.BufferBoostSubscriptionCount;
        BoostLevel = model.BoostLevel;
        Status = model.Status;
        AutoDeleteTime = model.AutoDeleteTime;
        RecommendInfo = model.RecommendInfo?.ToEntity();
    }

    internal void Update(ClientState state, Model model)
    {
        Name = model.Name;
        Topic = model.Topic;
        OwnerId = model.OwnerId;
        Icon = model.Icon;
        NotifyType = model.NotifyType;
        Region = model.Region;
        IsOpenEnabled = model.EnableOpen;
        OpenId = model.OpenId != 0 ? model.OpenId : null;
        if (model.DefaultChannelIdSetting != 0)
            DefaultChannelId = model.DefaultChannelIdSetting;
        else if (model.DefaultChannelId != 0)
            DefaultChannelId = model.DefaultChannelId;
        else
            DefaultChannelId = null;
        WelcomeChannelId = model.WelcomeChannelId != 0 ? model.WelcomeChannelId : null;

        // Only when both roles and channels are not null will the guild be considered available.
        IsAvailable = model.Roles is not null && model.Channels is not null;

        if (model.Roles is { Length: > 0})
        {
            foreach (RoleModel roleModel in model.Roles)
            {
                SocketRole role = SocketRole.Create(this, state, roleModel);
                _roles.TryAdd(role.Id, role);
            }
        }

        if (model.Channels is not null)
        {
            foreach (ChannelModel channelModel in model.Channels)
            {
                SocketGuildChannel channel = SocketGuildChannel.Create(this, state, channelModel);
                state.AddChannel(channel);
                _channels.TryAdd(channel.Id, channel);
                if (channelModel.Channels is not { Length: > 0 }) continue;
                foreach (ChannelModel nestedChannelModel in channelModel.Channels)
                {
                    SocketGuildChannel nestedChannel = SocketGuildChannel.Create(this, state, nestedChannelModel);
                    state.AddChannel(nestedChannel);
                    _channels.TryAdd(nestedChannel.Id, nestedChannel);
                }
            }
        }
    }

    internal void Update(ClientState state, GuildEvent model)
    {
        Name = model.Name;
        Icon = model.Icon;
        NotifyType = model.NotifyType;
        Region = model.Region;
        IsOpenEnabled = model.EnableOpen;
        OpenId = model.OpenId != 0 ? model.OpenId : null;
        DefaultChannelId = model.DefaultChannelId != 0 ? model.DefaultChannelId : null;
        WelcomeChannelId = model.WelcomeChannelId != 0 ? model.WelcomeChannelId : null;
        BoostSubscriptionCount = model.BoostSubscriptionCount;
        BufferBoostSubscriptionCount = model.BufferBoostSubscriptionCount;
        BoostLevel = model.BoostLevel;
        Status = model.Status;

        IsAvailable = true;
    }

    /// <inheritdoc />
    public Task UpdateAsync(RequestOptions? options = null) =>
        SocketGuildHelper.UpdateAsync(this, Kook, options);

    #endregion

    /// <inheritdoc cref="P:Kook.WebSocket.SocketGuild.Name" />
    public override string ToString() => Name;

    private string DebuggerDisplay => $"{Name} ({Id})";
    internal SocketGuild Clone() => (SocketGuild)MemberwiseClone();

    #region General

    /// <inheritdoc />
    public Task LeaveAsync(RequestOptions? options = null) =>
        GuildHelper.LeaveAsync(this, Kook, options);

    /// <inheritdoc />
    public Task<ImmutableDictionary<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>>> GetBoostSubscriptionsAsync(
        RequestOptions? options = null) =>
        SocketGuildHelper.GetBoostSubscriptionsAsync(this, Kook, options);

    /// <inheritdoc />
    public Task<ImmutableDictionary<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>>>
        GetActiveBoostSubscriptionsAsync(RequestOptions? options = null) =>
        SocketGuildHelper.GetActiveBoostSubscriptionsAsync(this, Kook, options);

    #endregion

    #region Bans

    /// <inheritdoc cref="M:Kook.IGuild.GetBansAsync(Kook.RequestOptions)"/>
    public Task<IReadOnlyCollection<RestBan>> GetBansAsync(RequestOptions? options = null) =>
        GuildHelper.GetBansAsync(this, Kook, options);

    /// <inheritdoc cref="M:Kook.IGuild.GetBanAsync(Kook.IUser,Kook.RequestOptions)"/>
    public Task<RestBan?> GetBanAsync(IUser user, RequestOptions? options = null) =>
        GuildHelper.GetBanAsync(this, Kook, user.Id, options);

    /// <inheritdoc cref="M:Kook.IGuild.GetBanAsync(System.UInt64,Kook.RequestOptions)"/>
    public Task<RestBan?> GetBanAsync(ulong userId, RequestOptions? options = null) =>
        GuildHelper.GetBanAsync(this, Kook, userId, options);

    /// <inheritdoc />
    public Task AddBanAsync(IUser user, int pruneDays = 0, string? reason = null, RequestOptions? options = null) =>
        GuildHelper.AddBanAsync(this, Kook, user.Id, pruneDays, reason, options);

    /// <inheritdoc />
    public Task AddBanAsync(ulong userId, int pruneDays = 0, string? reason = null, RequestOptions? options = null) =>
        GuildHelper.AddBanAsync(this, Kook, userId, pruneDays, reason, options);

    /// <inheritdoc />
    public Task RemoveBanAsync(IUser user, RequestOptions? options = null) =>
        GuildHelper.RemoveBanAsync(this, Kook, user.Id, options);

    /// <inheritdoc />
    public Task RemoveBanAsync(ulong userId, RequestOptions? options = null) =>
        GuildHelper.RemoveBanAsync(this, Kook, userId, options);

    #endregion

    #region Channels

    /// <summary>
    ///     获取此服务器内的频道。
    /// </summary>
    /// <param name="id"> 要获取的频道的 ID。 </param>
    /// <returns> 与指定的 <paramref name="id"/> 关联的频道；如果未找到，则返回 <c>null</c>。 </returns>
    public SocketGuildChannel? GetChannel(ulong id) => Kook.State.GetChannel(id) as SocketGuildChannel;

    /// <summary>
    ///     获取此服务器中所有具有文字聊天能力的频道。
    /// </summary>
    /// <param name="id"> 要获取的频道的 ID。 </param>
    /// <returns> 与指定的 <paramref name="id"/> 关联的频道；如果未找到，则返回 <c>null</c>。 </returns>
    /// <remarks>
    ///     语音频道也是一种文字频道，此方法本意用于获取具有文字聊天能力的频道。如果通过此方法传入的 ID 对应的频道是语音频道，那么也会返回对应的语音频道实体。
    ///     如需获取频道的实际类型，请参考 <see cref="M:Kook.ChannelExtensions.GetChannelType(Kook.IChannel)"/>。
    /// </remarks>
    public SocketTextChannel? GetTextChannel(ulong id) => GetChannel(id) as SocketTextChannel;

    /// <summary>
    ///     获取此服务器内指定具有语音聊天能力的频道。
    /// </summary>
    /// <param name="id"> 要获取的频道的 ID。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与指定的 <paramref name="id"/> 关联的频道；如果未找到，则返回 <c>null</c>。 </returns>
    public SocketVoiceChannel? GetVoiceChannel(ulong id) => GetChannel(id) as SocketVoiceChannel;

    /// <summary>
    ///     获取此服务器内指定的分组频道
    /// </summary>
    /// <param name="id"> 要获取的频道的 ID。 </param>
    /// <returns> 与指定的 <paramref name="id"/> 关联的频道；如果未找到，则返回 <c>null</c>。 </returns>
    public SocketCategoryChannel? GetCategoryChannel(ulong id) => GetChannel(id) as SocketCategoryChannel;

    /// <inheritdoc cref="M:Kook.IGuild.CreateTextChannelAsync(System.String,System.Action{Kook.CreateTextChannelProperties},Kook.RequestOptions)" />
    public Task<RestTextChannel> CreateTextChannelAsync(string name,
        Action<CreateTextChannelProperties>? func = null, RequestOptions? options = null) =>
        GuildHelper.CreateTextChannelAsync(this, Kook, name, func, options);

    /// <inheritdoc cref="M:Kook.IGuild.CreateVoiceChannelAsync(System.String,System.Action{Kook.CreateVoiceChannelProperties},Kook.RequestOptions)" />
    public Task<RestVoiceChannel> CreateVoiceChannelAsync(string name,
        Action<CreateVoiceChannelProperties>? func = null, RequestOptions? options = null) =>
        GuildHelper.CreateVoiceChannelAsync(this, Kook, name, func, options);

    /// <inheritdoc cref="M:Kook.IGuild.CreateCategoryChannelAsync(System.String,System.Action{Kook.CreateCategoryChannelProperties},Kook.RequestOptions)" />
    public Task<RestCategoryChannel> CreateCategoryChannelAsync(string name,
        Action<CreateCategoryChannelProperties>? func = null, RequestOptions? options = null) =>
        GuildHelper.CreateCategoryChannelAsync(this, Kook, name, func, options);

    internal SocketGuildChannel AddChannel(ClientState state, ChannelModel model)
    {
        SocketGuildChannel channel = SocketGuildChannel.Create(this, state, model);
        _channels.TryAdd(model.Id, channel);
        state.AddChannel(channel);
        return channel;
    }

    internal SocketGuildChannel AddOrUpdateChannel(ClientState state, ChannelModel model)
    {
        if (_channels.TryGetValue(model.Id, out SocketGuildChannel? cachedChannel))
        {
            cachedChannel.Update(Kook.State, model);
            return cachedChannel;
        }

        SocketGuildChannel channel = SocketGuildChannel.Create(this, Kook.State, model);
        _channels[channel.Id] = channel;
        state.AddChannel(channel);
        return channel;
    }

    internal SocketGuildChannel? RemoveChannel(ClientState state, ulong id) =>
        _channels.TryRemove(id, out SocketGuildChannel? _)
            ? state.RemoveChannel(id) as SocketGuildChannel
            : null;

    internal void PurgeChannelCache(ClientState state)
    {
        foreach (KeyValuePair<ulong, SocketGuildChannel> channelId in _channels)
            state.RemoveChannel(channelId.Key);
        _channels.Clear();
    }

    #endregion

    #region Roles

    /// <inheritdoc cref="M:Kook.IGuild.GetRole(System.UInt32)" />
    public SocketRole? GetRole(uint id) =>
        _roles.TryGetValue(id, out SocketRole? value) ? value : null;

    /// <inheritdoc cref="M:Kook.IGuild.CreateRoleAsync(System.String,Kook.RequestOptions)" />
    public Task<RestRole> CreateRoleAsync(string name, RequestOptions? options = null) =>
        GuildHelper.CreateRoleAsync(this, Kook, name, options);

    internal SocketRole AddRole(RoleModel model)
    {
        SocketRole role = SocketRole.Create(this, Kook.State, model);
        _roles[model.Id] = role;
        return role;
    }

    internal SocketRole? RemoveRole(uint id) =>
        _roles.TryRemove(id, out SocketRole? role) ? role : null;

    internal SocketRole AddOrUpdateRole(RoleModel model)
    {
        if (!_roles.TryGetValue(model.Id, out SocketRole? role))
            return AddRole(model);
        _roles[model.Id].Update(Kook.State, model);
        return role;
    }

    #endregion

    #region Users

    /// <summary>
    ///     获取此服务器内的用户。
    /// </summary>
    /// <remarks>
    ///     用户列表的缓存可能不完整，此方法在可能返回 <c>null</c>，因为在大型服务器中。
    /// </remarks>
    /// <param name="id"> 要获取的用户的 ID。 </param>
    /// <returns> 与指定的 <paramref name="id"/> 关联的用户；如果未找到，则返回 <c>null</c>。 </returns>
    public SocketGuildUser? GetUser(ulong id) =>
        _members.TryGetValue(id, out SocketGuildUser? member) ? member : null;

    internal SocketGuildUser AddOrUpdateUser(UserModel model)
    {
        if (_members.TryGetValue(model.Id, out SocketGuildUser? cachedMember))
        {
            cachedMember.GlobalUser?.Update(Kook.State, model);
            cachedMember.UpdatePresence(model.Online, model.OperatingSystem);
            return cachedMember;
        }

        SocketGuildUser member = SocketGuildUser.Create(this, Kook.State, model);
        member.GlobalUser.AddRef();
        _members[member.Id] = member;
        return member;
    }

    internal SocketGuildUser AddOrUpdateUser(MemberModel model)
    {
        if (_members.TryGetValue(model.Id, out SocketGuildUser? cachedMember))
        {
            cachedMember.Update(Kook.State, model);
            cachedMember.UpdatePresence(model.Online, model.OperatingSystem);
            return cachedMember;
        }

        SocketGuildUser member = SocketGuildUser.Create(this, Kook.State, model);
        member.GlobalUser.AddRef();
        _members[member.Id] = member;
        return member;
    }

    internal SocketGuildUser AddOrUpdateCurrentUser(RichModel model)
    {
        if (Kook.CurrentUser is not null && _members.TryGetValue(Kook.CurrentUser.Id, out SocketGuildUser? member))
        {
            member.Update(Kook.State, model);
        }
        else
        {
            member = SocketGuildUser.Create(this, Kook.State, model);
            member.GlobalUser.AddRef();
            _members[member.Id] = member;
        }

        return member;
    }

    internal SocketGuildUser? RemoveUser(ulong id)
    {
        if (!_members.TryRemove(id, out SocketGuildUser? member))
            return null;
        member.GlobalUser.RemoveRef(Kook);
        return member;
    }

    /// <summary>
    ///     清除此服务器的用户缓存。
    /// </summary>
    public void PurgeUserCache() => PurgeUserCache(_ => true);

    /// <summary>
    ///     清除此服务器的用户缓存。
    /// </summary>
    /// <param name="predicate"> 要清除的用户的筛选条件。 </param>
    public void PurgeUserCache(Predicate<SocketGuildUser> predicate)
    {
        IEnumerable<SocketGuildUser> membersToPurge = Users
            .Where(x => predicate.Invoke(x) && x.Id != Kook.CurrentUser?.Id);
        IEnumerable<SocketGuildUser> membersToKeep = Users
            .Where(x => !predicate.Invoke(x) || x.Id == Kook.CurrentUser?.Id);
        foreach (SocketGuildUser member in membersToPurge)
        {
            if (_members.TryRemove(member.Id, out _))
                member.GlobalUser.RemoveRef(Kook);
        }
        foreach (SocketGuildUser member in membersToKeep)
            _members.TryAdd(member.Id, member);
    }

    /// <summary>
    ///     获取此服务器内的所有用户。
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含此服务器内的所有用户。 </returns>
    public IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(RequestOptions? options = null)
    {
        if (HasAllMembers is true)
            return ImmutableArray.Create(Users).ToAsyncEnumerable<IReadOnlyCollection<IGuildUser>>();
        return GuildHelper.GetUsersAsync(this, Kook, KookConfig.MaxUsersPerBatch, 1, options);
    }

    /// <inheritdoc />
    public async Task DownloadUsersAsync(RequestOptions? options = null) =>
        await Kook.DownloadUsersAsync(new[] { this }, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DownloadVoiceStatesAsync(RequestOptions? options = null) =>
        await Kook.DownloadVoiceStatesAsync(new[] { this }, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DownloadBoostSubscriptionsAsync(RequestOptions? options = null) =>
        await Kook.DownloadBoostSubscriptionsAsync(new[] { this }, options).ConfigureAwait(false);

    /// <summary>
    ///     搜索加入到此服务器内匹配指定搜索条件的所有用户。
    /// </summary>
    /// <remarks>
    ///     此方法使用指定的属性搜索服务器用户。要查看可用的属性，请参考 <see cref="T:Kook.SearchGuildMemberProperties"/>。
    /// </remarks>
    /// <param name="func"> 一个包含要搜索的用户属性及排序条件的委托。 </param>
    /// <param name="limit"> 搜索结果的最大数量。 </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns> 一个表示异步获取操作的任务。任务的结果包含与提供的 <paramref name="func"/> 中指定的属性匹配的服务器用户集合。 </returns>
    public IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> SearchUsersAsync(
        Action<SearchGuildMemberProperties> func, int limit = KookConfig.MaxUsersPerBatch,
        RequestOptions? options = null) =>
        GuildHelper.SearchUsersAsync(this, Kook, func, limit, options);

    #endregion

    #region Voices

    /// <inheritdoc />
    public Task MoveUsersAsync(IEnumerable<IGuildUser> users, IVoiceChannel targetChannel,
        RequestOptions? options = null) =>
        ClientHelper.MoveUsersAsync(Kook, users, targetChannel, options);

    #endregion

    #region Emotes

    /// <summary>
    ///     获取此服务器的指定自定义表情。
    /// </summary>
    /// <param name="id"> 要获取的自定义表情的 ID。 </param>
    /// <returns> 与指定的 <paramref name="id"/> 关联的自定义表情；如果未找到，则返回 <c>null</c>。 </returns>
    public GuildEmote? GetEmote(string id) =>
        _emotes.TryGetValue(id, out GuildEmote? emote) ? emote : null;

    internal GuildEmote AddOrUpdateEmote(GuildEmojiEvent model)
    {
        GuildEmote emote = model.ToEntity(Id);
        _emotes[model.Id] = emote;
        return emote;
    }

    internal GuildEmote? RemoveEmote(string id) =>
        _emotes.TryRemove(id, out GuildEmote? emote) ? emote : null;

    /// <inheritdoc />
    public Task<IReadOnlyCollection<GuildEmote>> GetEmotesAsync(RequestOptions? options = null) =>
        GuildHelper.GetEmotesAsync(this, Kook, options);

    /// <inheritdoc />
    public Task<GuildEmote?> GetEmoteAsync(string id, RequestOptions? options = null) =>
        GuildHelper.GetEmoteAsync(this, Kook, id, options);

    /// <inheritdoc />
    public Task<GuildEmote> CreateEmoteAsync(string name, Image image, RequestOptions? options = null) =>
        GuildHelper.CreateEmoteAsync(this, Kook, name, image, options);

    /// <inheritdoc />
    public Task ModifyEmoteNameAsync(GuildEmote emote, string name, RequestOptions? options = null) =>
        GuildHelper.ModifyEmoteNameAsync(this, Kook, emote, name, options);

    /// <inheritdoc />
    public Task DeleteEmoteAsync(GuildEmote emote, RequestOptions? options = null) =>
        GuildHelper.DeleteEmoteAsync(this, Kook, emote.Id, options);

    #endregion

    #region Invites

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<IInvite>> GetInvitesAsync(RequestOptions? options = null) =>
        await GuildHelper.GetInvitesAsync(this, Kook, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(int? maxAge = 604800,
        int? maxUses = null, RequestOptions? options = null) =>
        await GuildHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800,
        InviteMaxUses maxUses = InviteMaxUses.Unlimited, RequestOptions? options = null) =>
        await GuildHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    #endregion

    #region Voice States

    internal SocketVoiceState AddOrUpdateVoiceState(ulong userId, IEnumerable<SocketVoiceChannel> channels)
    {
        SocketVoiceState socketState = GetVoiceState(userId) ?? SocketVoiceState.Default;
        socketState.Update(channels);
        _voiceStates[userId] = socketState;
        return socketState;
    }

    internal SocketVoiceState AddOrUpdateVoiceState(ulong userId, bool? isMuted = null, bool? isDeafened = null)
    {
        SocketVoiceState socketState = GetVoiceState(userId) ?? SocketVoiceState.Default;
        socketState.Update(isMuted, isDeafened);
        _voiceStates[userId] = socketState;
        return socketState;
    }

    internal SocketVoiceState AddOrUpdateVoiceStateForJoining(ulong userId, SocketVoiceChannel voiceChannel)
    {
        SocketVoiceState socketState = GetVoiceState(userId) ?? SocketVoiceState.Default;
        socketState.Join(voiceChannel);
        _voiceStates[userId] = socketState;
        return socketState;
    }

    internal SocketVoiceState AddOrUpdateVoiceStateForLeaving(ulong userId, SocketVoiceChannel voiceChannel)
    {
        SocketVoiceState socketState = GetVoiceState(userId) ?? SocketVoiceState.Default;
        socketState.Leave(voiceChannel.Id);
        _voiceStates[userId] = socketState;
        return socketState;
    }

    internal SocketVoiceState AddOrUpdateVoiceState(ulong userId, SocketVoiceChannel voiceChannel, LiveInfo liveInfo)
    {
        SocketVoiceState socketState = GetVoiceState(userId) ?? SocketVoiceState.Default;
        socketState.Update(liveInfo.InLive ? voiceChannel : null, liveInfo);
        socketState.Join(voiceChannel);
        _voiceStates[userId] = socketState;
        return socketState;
    }

    internal void ResetAllVoiceStateChannels()
    {
        foreach (SocketVoiceState voiceState in _voiceStates.Values)
            voiceState.ResetChannels();
    }

    internal SocketVoiceState? GetVoiceState(ulong id) =>
        _voiceStates.TryGetValue(id, out SocketVoiceState voiceState) ? voiceState : null;

    internal SocketVoiceState? RemoveVoiceState(ulong id) =>
        _voiceStates.TryRemove(id, out SocketVoiceState voiceState) ? voiceState : null;

    #endregion

    #region IGuild

    /// <inheritdoc />
    void IDisposable.Dispose()
    {
        foreach (SocketVoiceChannel voiceChannel in VoiceChannels)
            ((IDisposable)voiceChannel).Dispose();
    }

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IGuildUser>> IGuild.GetUsersAsync(CacheMode mode, RequestOptions? options)
    {
        if (mode == CacheMode.AllowDownload && HasAllMembers is not true)
            return (await GetUsersAsync(options).FlattenAsync().ConfigureAwait(false)).ToImmutableArray();
        else
            return Users;
    }

    /// <inheritdoc />
    Task<IGuildUser?> IGuild.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IGuildUser?>(GetUser(id));

    /// <inheritdoc />
    Task<IGuildUser?> IGuild.GetCurrentUserAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IGuildUser?>(CurrentUser);

    /// <inheritdoc />
    Task<IGuildUser?> IGuild.GetOwnerAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IGuildUser?>(Owner);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuild.SearchUsersAsync(
        Action<SearchGuildMemberProperties> func, int limit, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? SearchUsersAsync(func, limit, options)
            : AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();

    /// <inheritdoc />
    IRole IGuild.EveryoneRole => EveryoneRole;

    /// <inheritdoc />
    IReadOnlyCollection<IRole> IGuild.Roles => Roles;

    /// <inheritdoc />
    IReadOnlyCollection<GuildEmote> IGuild.Emotes => Emotes;

    /// <inheritdoc />
    IRole? IGuild.GetRole(uint id) => GetRole(id);

    /// <inheritdoc />
    IRecommendInfo? IGuild.RecommendInfo => RecommendInfo;

    /// <inheritdoc />
    async Task<IRole> IGuild.CreateRoleAsync(string name, RequestOptions? options) =>
        await CreateRoleAsync(name, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IBan>> IGuild.GetBansAsync(RequestOptions? options) =>
        await GetBansAsync(options).ConfigureAwait(false);

    /// <inheritdoc/>
    async Task<IBan?> IGuild.GetBanAsync(IUser user, RequestOptions? options) =>
        await GetBanAsync(user, options).ConfigureAwait(false);

    /// <inheritdoc/>
    async Task<IBan?> IGuild.GetBanAsync(ulong userId, RequestOptions? options) =>
        await GetBanAsync(userId, options).ConfigureAwait(false);

    /// <inheritdoc />
    Task<IReadOnlyCollection<IGuildChannel>> IGuild.GetChannelsAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IReadOnlyCollection<IGuildChannel>>(Channels);

    /// <inheritdoc />
    Task<IGuildChannel?> IGuild.GetChannelAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IGuildChannel?>(GetChannel(id));

    /// <inheritdoc />
    Task<ITextChannel?> IGuild.GetDefaultChannelAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<ITextChannel?>(DefaultChannel);

    /// <inheritdoc />
    Task<ITextChannel?> IGuild.GetWelcomeChannelAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<ITextChannel?>(WelcomeChannel);

    /// <inheritdoc />
    Task<IReadOnlyCollection<ITextChannel>> IGuild.GetTextChannelsAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IReadOnlyCollection<ITextChannel>>(TextChannels);

    /// <inheritdoc />
    Task<ITextChannel?> IGuild.GetTextChannelAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<ITextChannel?>(GetTextChannel(id));

    /// <inheritdoc />
    Task<IReadOnlyCollection<IVoiceChannel>> IGuild.GetVoiceChannelsAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IReadOnlyCollection<IVoiceChannel>>(VoiceChannels);

    /// <inheritdoc />
    Task<IVoiceChannel?> IGuild.GetVoiceChannelAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IVoiceChannel?>(GetVoiceChannel(id));

    /// <inheritdoc />
    Task<IReadOnlyCollection<ICategoryChannel>> IGuild.GetCategoryChannelsAsync(
        CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IReadOnlyCollection<ICategoryChannel>>(CategoryChannels);

    /// <inheritdoc />
    async Task<ITextChannel> IGuild.CreateTextChannelAsync(string name,
        Action<CreateTextChannelProperties>? func, RequestOptions? options) =>
        await CreateTextChannelAsync(name, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IVoiceChannel> IGuild.CreateVoiceChannelAsync(string name,
        Action<CreateVoiceChannelProperties>? func, RequestOptions? options) =>
        await CreateVoiceChannelAsync(name, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<ICategoryChannel> IGuild.CreateCategoryChannelAsync(string name,
        Action<CreateCategoryChannelProperties>? func, RequestOptions? options) =>
        await CreateCategoryChannelAsync(name, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Stream> GetBadgeAsync(BadgeStyle style = BadgeStyle.GuildName, RequestOptions? options = null) =>
        await GuildHelper.GetBadgeAsync(this, Kook, style, options).ConfigureAwait(false);

    #endregion
}
