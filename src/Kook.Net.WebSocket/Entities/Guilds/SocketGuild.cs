using Kook.API.Gateway;
using Kook.Rest;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
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
///     Represents a WebSocket-based guild object.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SocketGuild : SocketEntity<ulong>, IGuild, IDisposable, IUpdateable
{
    #region SocketGuild

    private ConcurrentDictionary<ulong, SocketGuildChannel> _channels;
    private ConcurrentDictionary<ulong, SocketGuildUser> _members;
    private ConcurrentDictionary<uint, SocketRole> _roles;
    private ConcurrentDictionary<ulong, SocketVoiceState> _voiceStates;
    private ConcurrentDictionary<string, GuildEmote> _emotes;
    private Dictionary<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>> _boostSubscriptions;

    /// <inheritdoc />
    public string Name { get; private set; }

    /// <inheritdoc />
    public string Topic { get; private set; }

    /// <inheritdoc />
    public ulong OwnerId { get; private set; }

    /// <summary> Gets the user that owns this guild. </summary>
    public SocketGuildUser Owner => GetUser(OwnerId);

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
    public string AutoDeleteTime { get; private set; }

    /// <inheritdoc cref="IGuild.RecommendInfo"/>
    public RecommendInfo RecommendInfo { get; private set; }

    /// <summary>
    ///     Gets the number of members.
    /// </summary>
    /// <remarks>
    ///     This property retrieves the number of members returned by Kook.
    ///     <note type="tip">
    ///     <para>
    ///         Due to how this property is returned by Kook instead of relying on the WebSocket cache, the
    ///         number here is the most accurate in terms of counting the number of users within this guild.
    ///     </para>
    ///     <para>
    ///         Use this instead of enumerating the count of the
    ///         <see cref="Kook.WebSocket.SocketGuild.Users" /> collection, as you may see discrepancy
    ///         between that and this property.
    ///     </para>
    ///     </note>
    ///     <note type="warning">
    ///         Only when <see cref="KookSocketConfig.AlwaysDownloadUsers"/> is set to <see langword="true"/>
    ///         will this property be populated upon startup. Otherwise, this property will be <see langword="null"/>,
    ///         and will be populated when <see cref="DownloadUsersAsync"/> is called.
    ///     </note>
    /// </remarks>
    public int? MemberCount { get; internal set; }

    /// <summary> Gets the number of members downloaded to the local guild cache. </summary>
    public int DownloadedMemberCount { get; private set; }

    internal bool IsAvailable { get; private set; }

    /// <summary> Indicates whether the client is connected to this guild. </summary>
    public bool IsConnected { get; internal set; }

    /// <summary> Indicates whether the client has all the members downloaded to the local guild cache. </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         If <see cref="MemberCount"/> is <see langword="null"/>, this property will always return <see langword="null"/>,
    ///         which means that the client is unable to determine whether all the members are downloaded or not.
    ///     </note>
    /// </remarks>
    public bool? HasAllMembers => MemberCount is null ? null : MemberCount <= DownloadedMemberCount;

    /// <inheritdoc/>
    public int MaxBitrate => GuildHelper.GetMaxBitrate(this);

    /// <inheritdoc/>
    public ulong MaxUploadLimit => GuildHelper.GetUploadLimit(this);

    /// <summary>
    ///     Gets the current logged-in user.
    /// </summary>
    public SocketGuildUser CurrentUser => _members.TryGetValue(Kook.CurrentUser.Id, out SocketGuildUser member) ? member : null;

    /// <summary>
    ///     Gets the built-in role containing all users in this guild.
    /// </summary>
    /// <returns>
    ///     A role object that represents an <c>@everyone</c> role in this guild.
    /// </returns>
    public SocketRole EveryoneRole => GetRole(0);

    /// <summary>
    ///     Gets a collection of all text channels in this guild.
    /// </summary>
    /// <returns>
    ///     A read-only collection of message channels found within this guild.
    /// </returns>
    public IReadOnlyCollection<SocketTextChannel> TextChannels
        => Channels.OfType<SocketTextChannel>().ToImmutableArray();

    /// <summary>
    ///     Gets a collection of all voice channels in this guild.
    /// </summary>
    /// <returns>
    ///     A read-only collection of voice channels found within this guild.
    /// </returns>
    public IReadOnlyCollection<SocketVoiceChannel> VoiceChannels
        => Channels.OfType<SocketVoiceChannel>().ToImmutableArray();

    /// <summary>
    ///     Gets a collection of all stage channels in this guild.
    /// </summary>
    /// <returns>
    ///     A read-only collection of stage channels found within this guild.
    /// </returns>
    /// <summary>
    ///     Gets a collection of all category channels in this guild.
    /// </summary>
    /// <returns>
    ///     A read-only collection of category channels found within this guild.
    /// </returns>
    public IReadOnlyCollection<SocketCategoryChannel> CategoryChannels
        => Channels.OfType<SocketCategoryChannel>().ToImmutableArray();

    /// <summary>
    ///     Gets a collection of all channels in this guild.
    /// </summary>
    /// <returns>
    ///     A read-only collection of generic channels found within this guild.
    /// </returns>
    public IReadOnlyCollection<SocketGuildChannel> Channels
    {
        get
        {
            ConcurrentDictionary<ulong, SocketGuildChannel> channels = _channels;
            ClientState state = Kook.State;
            return channels.Select(x => x.Value).Where(x => x != null).ToReadOnlyCollection(channels);
        }
    }

    /// <summary>
    ///     Gets the default text channel for this guild.
    /// </summary>
    /// <remarks>
    ///     This property retrieves default text channel for this guild.
    /// </remarks>
    /// <returns>
    ///     A <see cref="SocketTextChannel"/> representing the default text channel for this guild.
    /// </returns>
    public SocketTextChannel DefaultChannel => TextChannels
        .Where(c => CurrentUser?.GetPermissions(c).ViewChannel is true)
        .SingleOrDefault(c => c.Id == DefaultChannelId);

    /// <summary>
    ///     Gets the welcome text channel for this guild.
    /// </summary>
    /// <remarks>
    ///     This property retrieves default text channel for this guild.
    /// </remarks>
    /// <returns>
    ///     A <see cref="SocketTextChannel"/> representing the default text channel for this guild.
    /// </returns>
    public SocketTextChannel WelcomeChannel => TextChannels
        .Where(c => CurrentUser?.GetPermissions(c).ViewChannel is true)
        .SingleOrDefault(c => c.Id == WelcomeChannelId);

    /// <inheritdoc cref="IGuild.Emotes"/>
    public IReadOnlyCollection<GuildEmote> Emotes => _emotes
        .Select(x => x.Value).Where(x => x != null).ToReadOnlyCollection(_emotes);

    /// <summary>
    ///     Gets a dictionary of all boost subscriptions for this guild.
    /// </summary>
    /// <returns>
    ///     A read-only dictionary containing all boost subscription metadata for this guild grouped by users;
    ///     or <see langword="null"/> if the boost subscription data has never been cached.
    /// </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         <para>
    ///             Only when <see cref="KookSocketConfig.AlwaysDownloadBoostSubscriptions"/> is set to <see langword="true"/>
    ///             will this property be populated upon startup. Due to the lack of event support for boost subscriptions,
    ///             this property will never be updated. The changes of <see cref="SocketGuild.BoostSubscriptionCount"/> will trigger the update
    ///             of this property, but KOOK gateway will not publish this event resulting from the changes of total boost subscription
    ///             count. To fetch the latest boost subscription data, use <see cref="DownloadBoostSubscriptionsAsync"/> or
    ///             <see cref="KookSocketClient.DownloadBoostSubscriptionsAsync"/> upon a <see cref="KookSocketClient"/> to
    ///             manually download the latest boost subscription data, or <see cref="GetBoostSubscriptionsAsync"/>.
    ///         </para>
    ///     </note>
    /// </remarks>
    /// <seealso cref="ValidBoostSubscriptions"/>
    /// <seealso cref="DownloadBoostSubscriptionsAsync"/>
    /// <seealso cref="KookSocketClient.DownloadBoostSubscriptionsAsync"/>
    /// <seealso cref="KookSocketClient.AlwaysDownloadBoostSubscriptions"/>
    public ImmutableDictionary<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>> BoostSubscriptions =>
        _boostSubscriptions?.ToImmutableDictionary();

    /// <summary>
    ///     Gets a dictionary of all boost subscriptions which have not expired for this guild.
    /// </summary>
    /// <returns>
    ///     A read-only dictionary containing all boost subscription metadata which have not expired for this guild grouped by users;
    ///     or <see langword="null"/> if the boost subscription data has never been cached.
    /// </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         <para>
    ///             Only when <see cref="KookSocketConfig.AlwaysDownloadBoostSubscriptions"/> is set to <see langword="true"/>
    ///             will this property be populated upon startup. Due to the lack of event support for boost subscriptions,
    ///             this property will never be updated. The changes of <see cref="SocketGuild.BoostSubscriptionCount"/> will trigger the update
    ///             of this property, but KOOK gateway will not publish this event resulting from the changes of total boost subscription
    ///             count. To fetch the latest boost subscription data, use <see cref="DownloadBoostSubscriptionsAsync"/> or
    ///             <see cref="KookSocketClient.DownloadBoostSubscriptionsAsync"/> upon a <see cref="KookSocketClient"/> to
    ///             manually download the latest boost subscription data, or <see cref="GetBoostSubscriptionsAsync"/>.
    ///         </para>
    ///     </note>
    /// </remarks>
    /// <seealso cref="BoostSubscriptions"/>
    /// <seealso cref="DownloadBoostSubscriptionsAsync"/>
    /// <seealso cref="KookSocketClient.DownloadBoostSubscriptionsAsync"/>
    /// <seealso cref="KookSocketClient.AlwaysDownloadBoostSubscriptions"/>
    public ImmutableDictionary<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>> ValidBoostSubscriptions =>
        _boostSubscriptions?.Select(x =>
                new KeyValuePair<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>>(x.Key, x.Value
                    .Where(y => y.IsValid)
                    .ToImmutableArray() as IReadOnlyCollection<BoostSubscriptionMetadata>))
            .Where(x => x.Value.Any())
            .ToImmutableDictionary();

    /// <summary>
    ///     Gets a collection of users in this guild.
    /// </summary>
    /// <remarks>
    ///     This property retrieves all users found within this guild.
    ///     <note type="warning">
    ///         <para>
    ///             This property may not always return all the members for large guilds (i.e. guilds containing
    ///             100+ users). If you are simply looking to get the number of users present in this guild,
    ///             consider using <see cref="MemberCount"/> instead.
    ///         </para>
    ///         <para>
    ///             Otherwise, you may need to enable <see cref="KookSocketConfig.AlwaysDownloadUsers"/> to fetch
    ///             the full user list upon startup, or use <see cref="DownloadUsersAsync"/> to manually download
    ///             the users.
    ///         </para>
    ///     </note>
    /// </remarks>
    /// <returns>
    ///     A collection of guild users found within this guild.
    /// </returns>
    /// <seealso cref="DownloadUsersAsync"/>
    /// <seealso cref="KookSocketClient.AlwaysDownloadUsers"/>
    /// <seealso cref="KookSocketClient.DownloadUsersAsync"/>
    public IReadOnlyCollection<SocketGuildUser> Users => _members.ToReadOnlyCollection();

    /// <summary>
    ///     Gets a collection of all roles in this guild.
    /// </summary>
    /// <returns>
    ///     A read-only collection of roles found within this guild.
    /// </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         Due to the lack of event args which should contains the reordered roles data
    ///         when roles are reordered, this property may not be completely accurate.
    ///         To ensure the most accurate results, it is recommended to
    ///         call <see cref="UpdateAsync"/> before this property is used.
    ///     </note>
    /// </remarks>
    public IReadOnlyCollection<SocketRole> Roles => _roles.ToReadOnlyCollection();

    internal SocketGuild(KookSocketClient kook, ulong id) : base(kook, id)
    {
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
        ConcurrentDictionary<ulong, SocketGuildChannel> channels = new(
            ConcurrentHashSet.DefaultConcurrencyLevel,
            (int)(models.Count * 1.05));
        foreach (ChannelModel model in models)
        {
            SocketGuildChannel channel = SocketGuildChannel.Create(this, state, model);
            state.AddChannel(channel);
            channels.TryAdd(channel.Id, channel);
        }

        _channels = channels;
    }

    internal void Update(ClientState state, IReadOnlyCollection<MemberModel> models)
    {
        ConcurrentDictionary<ulong, SocketGuildUser> members = new(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(models.Count * 1.05));
        foreach (MemberModel model in models)
        {
            SocketGuildUser member = SocketGuildUser.Create(this, state, model);
            if (members.TryAdd(member.Id, member)) member.GlobalUser.AddRef();
        }

        DownloadedMemberCount = members.Count;
        _members = members;
        MemberCount = members.Count;
    }

    internal void Update(ClientState state, IReadOnlyCollection<BoostSubscription> models) =>
        _boostSubscriptions = models.GroupBy(x => x.UserId)
            .ToDictionary(x => RestUser.Create(Kook, x.First().User) as IUser,
                x => x.GroupBy(y => (y.StartTime, y.EndTime))
                    .Select(y => new BoostSubscriptionMetadata(y.Key.StartTime, y.Key.EndTime, y.Count()))
                    .ToImmutableArray() as IReadOnlyCollection<BoostSubscriptionMetadata>);

    internal void Update(ClientState state, RichModel model)
    {
        Update(state, model as ExtendedModel);

        Banner = model.Banner;
        if (model.Emojis != null)
        {
            _emotes.Clear();
            foreach (API.Emoji emoji in model.Emojis) _emotes.TryAdd(emoji.Id, emoji.ToEntity(model.Id));
        }
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

        IsAvailable = true;

        ConcurrentDictionary<uint, SocketRole> roles = new(ConcurrentHashSet.DefaultConcurrencyLevel,
            (int)((model.Roles?.Length ?? 0) * 1.05));
        for (int i = 0; i < (model.Roles?.Length ?? 0); i++)
        {
            SocketRole role = SocketRole.Create(this, state, model.Roles![i]);
            roles.TryAdd(role.Id, role);
        }
        _roles = roles;

        int channelCount = model.Channels?.Length ?? 0;
        channelCount += (model.Channels ?? Array.Empty<ChannelModel>()).Sum(x => x.Channels?.Length ?? 0);
        ConcurrentDictionary<ulong, SocketGuildChannel> channels = new(
            ConcurrentHashSet.DefaultConcurrencyLevel,
            (int)(channelCount * 1.05));
        for (int i = 0; i < (model.Channels?.Length ?? 0); i++)
        {
            SocketGuildChannel channel = SocketGuildChannel.Create(this, state, model.Channels![i]);
            state.AddChannel(channel);
            channels.TryAdd(channel.Id, channel);
            for (int j = 0; j < (model.Channels[i].Channels?.Length ?? 0); j++)
            {
                SocketGuildChannel nestedChannel = SocketGuildChannel.Create(this, state, model.Channels![i].Channels![j]);
                state.AddChannel(nestedChannel);
                channels.TryAdd(nestedChannel.Id, nestedChannel);
            }
        }
        _channels = channels;

        _members ??= new ConcurrentDictionary<ulong, SocketGuildUser>();
        _voiceStates ??= new ConcurrentDictionary<ulong, SocketVoiceState>();
        _emotes ??= new ConcurrentDictionary<string, GuildEmote>();
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
    public Task UpdateAsync(RequestOptions options = null)
        => SocketGuildHelper.UpdateAsync(this, Kook, options);

    #endregion

    /// <summary>
    ///     Gets the name of the guild.
    /// </summary>
    /// <returns>
    ///     A string that resolves to <see cref="Kook.WebSocket.SocketGuild.Name"/>.
    /// </returns>
    public override string ToString() => Name;

    private string DebuggerDisplay => $"{Name} ({Id})";
    internal SocketGuild Clone() => MemberwiseClone() as SocketGuild;

    #region General

    /// <inheritdoc />
    public Task LeaveAsync(RequestOptions options = null)
        => GuildHelper.LeaveAsync(this, Kook, options);

    /// <inheritdoc />
    public Task<ImmutableDictionary<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>>> GetBoostSubscriptionsAsync(RequestOptions options = null)
        => SocketGuildHelper.GetBoostSubscriptionsAsync(this, Kook, options);

    /// <inheritdoc />
    public Task<ImmutableDictionary<IUser, IReadOnlyCollection<BoostSubscriptionMetadata>>> GetActiveBoostSubscriptionsAsync(
        RequestOptions options = null)
        => SocketGuildHelper.GetActiveBoostSubscriptionsAsync(this, Kook, options);

    #endregion

    #region Bans

    /// <inheritdoc cref="IGuild.GetBansAsync(RequestOptions)"/>
    public Task<IReadOnlyCollection<RestBan>> GetBansAsync(RequestOptions options = null)
        => GuildHelper.GetBansAsync(this, Kook, options);

    /// <inheritdoc cref="IGuild.GetBanAsync(IUser,RequestOptions)"/>
    public Task<RestBan> GetBanAsync(IUser user, RequestOptions options = null)
        => GuildHelper.GetBanAsync(this, Kook, user.Id, options);

    /// <inheritdoc cref="IGuild.GetBanAsync(ulong,RequestOptions)"/>
    public Task<RestBan> GetBanAsync(ulong userId, RequestOptions options = null)
        => GuildHelper.GetBanAsync(this, Kook, userId, options);

    /// <inheritdoc />
    public Task AddBanAsync(IUser user, int pruneDays = 0, string reason = null, RequestOptions options = null)
        => GuildHelper.AddBanAsync(this, Kook, user.Id, pruneDays, reason, options);

    /// <inheritdoc />
    public Task AddBanAsync(ulong userId, int pruneDays = 0, string reason = null, RequestOptions options = null)
        => GuildHelper.AddBanAsync(this, Kook, userId, pruneDays, reason, options);

    /// <inheritdoc />
    public Task RemoveBanAsync(IUser user, RequestOptions options = null)
        => GuildHelper.RemoveBanAsync(this, Kook, user.Id, options);

    /// <inheritdoc />
    public Task RemoveBanAsync(ulong userId, RequestOptions options = null)
        => GuildHelper.RemoveBanAsync(this, Kook, userId, options);

    #endregion

    #region Channels

    /// <summary>
    ///     Gets a channel in this guild.
    /// </summary>
    /// <param name="id">The identifier for the channel.</param>
    /// <returns>
    ///     A generic channel associated with the specified <paramref name="id" />; <see langword="null"/> if none is found.
    /// </returns>
    public SocketGuildChannel GetChannel(ulong id)
    {
        SocketGuildChannel channel = Kook.State.GetChannel(id) as SocketGuildChannel;
        if (channel?.Guild.Id == Id) return channel;

        return null;
    }

    /// <summary>
    ///     Gets a text channel in this guild.
    /// </summary>
    /// <param name="id">The identifier for the text channel.</param>
    /// <returns>
    ///     A text channel associated with the specified <paramref name="id" />; <see langword="null"/> if none is found.
    /// </returns>
    public SocketTextChannel GetTextChannel(ulong id)
        => GetChannel(id) as SocketTextChannel;

    /// <summary>
    ///     Gets a voice channel in this guild.
    /// </summary>
    /// <param name="id">The identifier for the voice channel.</param>
    /// <returns>
    ///     A voice channel associated with the specified <paramref name="id" />; <see langword="null"/> if none is found.
    /// </returns>
    public SocketVoiceChannel GetVoiceChannel(ulong id)
        => GetChannel(id) as SocketVoiceChannel;

    /// <summary>
    ///     Creates a new text channel in this guild.
    /// </summary>
    /// <param name="name">The new name for the text channel.</param>
    /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous creation operation. The task result contains the newly created
    ///     text channel.
    /// </returns>
    public Task<RestTextChannel> CreateTextChannelAsync(string name, Action<CreateTextChannelProperties> func = null, RequestOptions options = null)
        => GuildHelper.CreateTextChannelAsync(this, Kook, name, options, func);

    /// <summary>
    ///     Creates a new voice channel in this guild.
    /// </summary>
    /// <param name="name">The new name for the voice channel.</param>
    /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <returns>
    ///     A task that represents the asynchronous creation operation. The task result contains the newly created
    ///     voice channel.
    /// </returns>
    public Task<RestVoiceChannel> CreateVoiceChannelAsync(string name, Action<CreateVoiceChannelProperties> func = null,
        RequestOptions options = null)
        => GuildHelper.CreateVoiceChannelAsync(this, Kook, name, options, func);

    /// <summary>
    ///     Creates a new channel category in this guild.
    /// </summary>
    /// <param name="name">The new name for the category.</param>
    /// <param name="func">The delegate containing the properties to be applied to the channel upon its creation.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <returns>
    ///     A task that represents the asynchronous creation operation. The task result contains the newly created
    ///     category channel.
    /// </returns>
    public Task<RestCategoryChannel> CreateCategoryChannelAsync(string name, Action<CreateCategoryChannelProperties> func = null,
        RequestOptions options = null)
        => GuildHelper.CreateCategoryChannelAsync(this, Kook, name, options, func);

    internal SocketGuildChannel AddChannel(ClientState state, ChannelModel model)
    {
        SocketGuildChannel channel = SocketGuildChannel.Create(this, state, model);
        _channels.TryAdd(model.Id, channel);
        state.AddChannel(channel);
        return channel;
    }

    internal SocketGuildChannel AddOrUpdateChannel(ClientState state, ChannelModel model)
    {
        if (_channels.TryGetValue(model.Id, out SocketGuildChannel channel))
            channel.Update(Kook.State, model);
        else
        {
            channel = SocketGuildChannel.Create(this, Kook.State, model);
            _channels[channel.Id] = channel;
            state.AddChannel(channel);
        }

        return channel;
    }

    internal SocketGuildChannel RemoveChannel(ClientState state, ulong id)
    {
        if (_channels.TryRemove(id, out SocketGuildChannel _)) return state.RemoveChannel(id) as SocketGuildChannel;

        return null;
    }

    internal void PurgeChannelCache(ClientState state)
    {
        foreach (KeyValuePair<ulong, SocketGuildChannel> channelId in _channels) state.RemoveChannel(channelId.Key);

        _channels.Clear();
    }

    #endregion

    // #region Invites
    //
    // /// <summary>
    // ///     Gets a collection of all invites in this guild.
    // /// </summary>
    // /// <param name="options">The options to be used when sending the request.</param>
    // /// <returns>
    // ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of
    // ///     invite metadata, each representing information for an invite found within this guild.
    // /// </returns>
    // public Task<IReadOnlyCollection<RestInvite>> GetInvitesAsync(RequestOptions options = null)
    //     => GuildHelper.GetInvitesAsync(this, Kook, options);
    //
    // #endregion

    #region Roles

    /// <summary>
    ///     Gets a role in this guild.
    /// </summary>
    /// <param name="id">The identifier for the role.</param>
    /// <returns>
    ///     A role that is associated with the specified <paramref name="id"/>; <see langword="null"/> if none is found.
    /// </returns>
    public SocketRole GetRole(uint id)
    {
        if (_roles.TryGetValue(id, out SocketRole value)) return value;

        return null;
    }

    /// <summary>
    ///     Creates a new role with the provided name.
    /// </summary>
    /// <param name="name">The new name for the role.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <returns>
    ///     A task that represents the asynchronous creation operation. The task result contains the newly created
    ///     role.
    /// </returns>
    public Task<RestRole> CreateRoleAsync(string name, RequestOptions options = null)
        => GuildHelper.CreateRoleAsync(this, Kook, name, options);

    internal SocketRole AddRole(RoleModel model)
    {
        SocketRole role = SocketRole.Create(this, Kook.State, model);
        _roles[model.Id] = role;
        return role;
    }

    internal SocketRole RemoveRole(uint id)
    {
        if (_roles.TryRemove(id, out SocketRole role)) return role;

        return null;
    }

    internal SocketRole AddOrUpdateRole(RoleModel model)
    {
        if (_roles.TryGetValue(model.Id, out SocketRole role))
            _roles[model.Id].Update(Kook.State, model);
        else
            role = AddRole(model);

        return role;
    }

    #endregion

    #region Users

    /// <summary>
    ///     Gets a user from this guild.
    /// </summary>
    /// <remarks>
    ///     This method retrieves a user found within this guild.
    ///     <note>
    ///         This may return <see langword="null"/> in the WebSocket implementation due to incomplete user collection in
    ///         large guilds.
    ///     </note>
    /// </remarks>
    /// <param name="id">The identifier of the user.</param>
    /// <returns>
    ///     A guild user associated with the specified <paramref name="id"/>; <see langword="null"/> if none is found.
    /// </returns>
    public SocketGuildUser GetUser(ulong id)
    {
        if (_members.TryGetValue(id, out SocketGuildUser member)) return member;

        return null;
    }

    internal SocketGuildUser AddOrUpdateUser(UserModel model)
    {
        if (_members.TryGetValue(model.Id, out SocketGuildUser member))
        {
            member.GlobalUser?.Update(Kook.State, model);
            member.UpdatePresence(model.Online, model.OperatingSystem);
        }
        else
        {
            member = SocketGuildUser.Create(this, Kook.State, model);
            member.GlobalUser.AddRef();
            _members[member.Id] = member;
            DownloadedMemberCount++;
        }

        return member;
    }

    internal SocketGuildUser AddOrUpdateUser(MemberModel model)
    {
        if (_members.TryGetValue(model.Id, out SocketGuildUser member))
        {
            member.Update(Kook.State, model);
            member.UpdatePresence(model.Online, model.OperatingSystem);
        }
        else
        {
            member = SocketGuildUser.Create(this, Kook.State, model);
            member.GlobalUser.AddRef();
            _members[member.Id] = member;
            DownloadedMemberCount++;
        }

        return member;
    }

    internal SocketGuildUser RemoveUser(ulong id)
    {
        if (_members.TryRemove(id, out SocketGuildUser member))
        {
            DownloadedMemberCount--;
            member.GlobalUser.RemoveRef(Kook);
            return member;
        }

        return null;
    }

    /// <summary>
    ///     Purges this guild's user cache.
    /// </summary>
    public void PurgeUserCache() => PurgeUserCache(_ => true);

    /// <summary>
    ///     Purges this guild's user cache.
    /// </summary>
    /// <param name="predicate">The predicate used to select which users to clear.</param>
    public void PurgeUserCache(Func<SocketGuildUser, bool> predicate)
    {
        IEnumerable<SocketGuildUser> membersToPurge = Users.Where(x => predicate.Invoke(x) && x?.Id != Kook.CurrentUser.Id);
        IEnumerable<SocketGuildUser> membersToKeep = Users.Where(x => !predicate.Invoke(x) || x?.Id == Kook.CurrentUser.Id);

        foreach (SocketGuildUser member in membersToPurge)
            if (_members.TryRemove(member.Id, out _))
                member.GlobalUser.RemoveRef(Kook);

        foreach (SocketGuildUser member in membersToKeep) _members.TryAdd(member.Id, member);

        DownloadedMemberCount = _members.Count;
    }

    /// <summary>
    ///     Gets a collection of all users in this guild.
    /// </summary>
    /// <remarks>
    ///     <para>This method retrieves all users found within this guild through REST.</para>
    ///     <para>Users returned by this method are not cached.</para>
    /// </remarks>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a collection of guild
    ///     users found within this guild.
    /// </returns>
    public IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(RequestOptions options = null)
    {
        if (HasAllMembers is true) return ImmutableArray.Create(Users).ToAsyncEnumerable<IReadOnlyCollection<IGuildUser>>();

        return GuildHelper.GetUsersAsync(this, Kook, KookConfig.MaxUsersPerBatch, 1, options);
    }

    /// <inheritdoc />
    public async Task DownloadUsersAsync(RequestOptions options = null) =>
        await Kook.DownloadUsersAsync(new[] { this }, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DownloadVoiceStatesAsync(RequestOptions options = null) =>
        await Kook.DownloadVoiceStatesAsync(new[] { this }, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task DownloadBoostSubscriptionsAsync(RequestOptions options = null) =>
        await Kook.DownloadBoostSubscriptionsAsync(new[] { this }, options).ConfigureAwait(false);

    /// <summary>
    ///     Gets a collection of users in this guild that the name or nickname contains the
    ///     provided string at <paramref name="func"/>.
    /// </summary>
    /// <remarks>
    ///     The <paramref name="limit"/> can not be higher than <see cref="KookConfig.MaxUsersPerBatch"/>.
    /// </remarks>
    /// <param name="func">A delegate containing the properties to search users with.</param>
    /// <param name="limit">The maximum number of users to be gotten.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a collection of guild
    ///     users that matches the properties with the provided <see cref="Action{SearchGuildMemberProperties}"/>
    ///     at <paramref name="func"/>.
    /// </returns>
    public IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> SearchUsersAsync(Action<SearchGuildMemberProperties> func,
        int limit = KookConfig.MaxUsersPerBatch, RequestOptions options = null)
        => GuildHelper.SearchUsersAsync(this, Kook, func, limit, options);

    #endregion

    #region Voices

    /// <inheritdoc />
    public async Task MoveUsersAsync(IEnumerable<IGuildUser> users, IVoiceChannel targetChannel, RequestOptions options)
        => await ClientHelper.MoveUsersAsync(Kook, users, targetChannel, options).ConfigureAwait(false);

    #endregion

    #region Emotes

    /// <summary>
    ///     Gets a guild emoji in this guild.
    /// </summary>
    /// <param name="id">The identifier for the guild emoji.</param>
    /// <returns>
    ///     A guild emoji associated with the specified <paramref name="id" />; <see langword="null"/> if none is found.
    /// </returns>
    public GuildEmote GetEmote(string id)
    {
        if (_emotes.TryGetValue(id, out GuildEmote emote)) return emote;

        return null;
    }

    internal GuildEmote AddEmote(GuildEmojiEvent model)
    {
        GuildEmote emote = model.ToEntity(Id);
        _emotes.TryAdd(model.Id, emote);
        return emote;
    }

    internal GuildEmote AddOrUpdateEmote(GuildEmojiEvent model)
    {
        GuildEmote emote = model.ToEntity(Id);
        _emotes[model.Id] = emote;
        return emote;
    }

    internal GuildEmote RemoveEmote(string id)
    {
        if (_emotes.TryRemove(id, out GuildEmote emote)) return emote;

        return null;
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<GuildEmote>> GetEmotesAsync(RequestOptions options = null)
        => GuildHelper.GetEmotesAsync(this, Kook, options);

    /// <inheritdoc />
    public Task<GuildEmote> GetEmoteAsync(string id, RequestOptions options = null)
        => GuildHelper.GetEmoteAsync(this, Kook, id, options);

    /// <inheritdoc />
    public Task<GuildEmote> CreateEmoteAsync(string name, Image image, RequestOptions options = null)
        => GuildHelper.CreateEmoteAsync(this, Kook, name, image, options);

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
    public Task ModifyEmoteNameAsync(GuildEmote emote, Action<string> func, RequestOptions options = null)
        => GuildHelper.ModifyEmoteNameAsync(this, Kook, emote, func, options);

    /// <inheritdoc />
    public Task DeleteEmoteAsync(GuildEmote emote, RequestOptions options = null)
        => GuildHelper.DeleteEmoteAsync(this, Kook, emote.Id, options);

    #endregion

    #region Invites

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<IInvite>> GetInvitesAsync(RequestOptions options = null)
        => await GuildHelper.GetInvitesAsync(this, Kook, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(int? maxAge = 604800, int? maxUses = null, RequestOptions options = null)
        => await GuildHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IInvite> CreateInviteAsync(InviteMaxAge maxAge = InviteMaxAge._604800, InviteMaxUses maxUses = InviteMaxUses.Unlimited,
        RequestOptions options = null)
        => await GuildHelper.CreateInviteAsync(this, Kook, maxAge, maxUses, options).ConfigureAwait(false);

    #endregion

    #region Voice States

    internal SocketVoiceState AddOrUpdateVoiceState(ulong userId, ulong? voiceChannelId)
    {
        SocketVoiceChannel voiceChannel = voiceChannelId.HasValue
            ? GetChannel(voiceChannelId.Value) as SocketVoiceChannel
            : null;
        SocketVoiceState socketState = GetVoiceState(userId) ?? SocketVoiceState.Default;
        socketState.Update(voiceChannel);
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

    internal SocketVoiceState? GetVoiceState(ulong id)
    {
        if (_voiceStates.TryGetValue(id, out SocketVoiceState voiceState)) return voiceState;

        return null;
    }

    internal SocketVoiceState? RemoveVoiceState(ulong id)
    {
        if (_voiceStates.TryRemove(id, out SocketVoiceState voiceState)) return voiceState;

        return null;
    }

    #endregion

    #region IGuild

    /// <inheritdoc />
    bool IGuild.Available => true;

    /// <inheritdoc />
    public void Dispose()
    {
    }

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IGuildUser>> IGuild.GetUsersAsync(CacheMode mode, RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload && HasAllMembers is not true)
            return (await GetUsersAsync(options).FlattenAsync().ConfigureAwait(false)).ToImmutableArray();
        else
            return Users;
    }

    /// <inheritdoc />
    Task<IGuildUser> IGuild.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IGuildUser>(GetUser(id));

    /// <inheritdoc />
    Task<IGuildUser> IGuild.GetCurrentUserAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult<IGuildUser>(CurrentUser);

    /// <inheritdoc />
    Task<IGuildUser> IGuild.GetOwnerAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult<IGuildUser>(Owner);

    /// <inheritdoc />
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuild.SearchUsersAsync(Action<SearchGuildMemberProperties> func, int limit, CacheMode mode,
        RequestOptions options)
    {
        if (mode == CacheMode.AllowDownload)
            return SearchUsersAsync(func, limit, options);
        else
            return AsyncEnumerable.Empty<IReadOnlyCollection<IGuildUser>>();
    }

    /// <inheritdoc />
    IRole IGuild.EveryoneRole => EveryoneRole;

    /// <inheritdoc />
    IReadOnlyCollection<IRole> IGuild.Roles => Roles;

    /// <inheritdoc />
    IReadOnlyCollection<GuildEmote> IGuild.Emotes => Emotes;

    /// <inheritdoc />
    IRole IGuild.GetRole(uint id) => GetRole(id);

    /// <inheritdoc />
    IRecommendInfo IGuild.RecommendInfo => RecommendInfo;

    /// <inheritdoc />
    async Task<IRole> IGuild.CreateRoleAsync(string name, RequestOptions options)
        => await CreateRoleAsync(name, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IBan>> IGuild.GetBansAsync(RequestOptions options)
        => await GetBansAsync(options).ConfigureAwait(false);

    /// <inheritdoc/>
    async Task<IBan> IGuild.GetBanAsync(IUser user, RequestOptions options)
        => await GetBanAsync(user, options).ConfigureAwait(false);

    /// <inheritdoc/>
    async Task<IBan> IGuild.GetBanAsync(ulong userId, RequestOptions options)
        => await GetBanAsync(userId, options).ConfigureAwait(false);

    /// <inheritdoc />
    Task<IReadOnlyCollection<IGuildChannel>> IGuild.GetChannelsAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult<IReadOnlyCollection<IGuildChannel>>(Channels);

    /// <inheritdoc />
    Task<IGuildChannel> IGuild.GetChannelAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IGuildChannel>(GetChannel(id));

    /// <inheritdoc />
    Task<ITextChannel> IGuild.GetDefaultChannelAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult<ITextChannel>(DefaultChannel);

    /// <inheritdoc />
    Task<ITextChannel> IGuild.GetWelcomeChannelAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult<ITextChannel>(WelcomeChannel);

    /// <inheritdoc />
    Task<IReadOnlyCollection<ITextChannel>> IGuild.GetTextChannelsAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult<IReadOnlyCollection<ITextChannel>>(TextChannels);

    /// <inheritdoc />
    Task<ITextChannel> IGuild.GetTextChannelAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<ITextChannel>(GetTextChannel(id));

    /// <inheritdoc />
    Task<IReadOnlyCollection<IVoiceChannel>> IGuild.GetVoiceChannelsAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult<IReadOnlyCollection<IVoiceChannel>>(VoiceChannels);

    /// <inheritdoc />
    Task<IVoiceChannel> IGuild.GetVoiceChannelAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IVoiceChannel>(GetVoiceChannel(id));

    /// <inheritdoc />
    Task<IReadOnlyCollection<ICategoryChannel>> IGuild.GetCategoryChannelsAsync(CacheMode mode,
        RequestOptions options)
        => Task.FromResult<IReadOnlyCollection<ICategoryChannel>>(CategoryChannels);

    /// <inheritdoc />
    async Task<ITextChannel> IGuild.CreateTextChannelAsync(string name, Action<CreateTextChannelProperties> func, RequestOptions options)
        => await CreateTextChannelAsync(name, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<IVoiceChannel> IGuild.CreateVoiceChannelAsync(string name, Action<CreateVoiceChannelProperties> func, RequestOptions options)
        => await CreateVoiceChannelAsync(name, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    async Task<ICategoryChannel> IGuild.CreateCategoryChannelAsync(string name, Action<CreateCategoryChannelProperties> func, RequestOptions options)
        => await CreateCategoryChannelAsync(name, func, options).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Stream> GetBadgeAsync(BadgeStyle style = BadgeStyle.GuildName, RequestOptions options = null) =>
        await GuildHelper.GetBadgeAsync(this, Kook, style, options).ConfigureAwait(false);

    #endregion
}
