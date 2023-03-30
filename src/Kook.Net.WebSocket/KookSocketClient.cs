using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kook.API;
using Kook.API.Gateway;
using Kook.API.Rest;
using Kook.Logging;
using Kook.Net;
using Kook.Net.Udp;
using Kook.Net.WebSockets;
using Kook.Rest;
using Reaction = Kook.API.Gateway.Reaction;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based KOOK client.
/// </summary>
public partial class KookSocketClient : BaseSocketClient, IKookClient
{
    #region KookSocketClient

    private readonly JsonSerializerOptions _serializerOptions;

    private readonly ConcurrentQueue<long> _heartbeatTimes;
    private readonly ConnectionManager _connection;
    private readonly Logger _gatewayLogger;
    private readonly SemaphoreSlim _stateLock;

    private Guid? _sessionId;
    private int _lastSeq;
    private int _retryCount = 0;
    private Task _heartbeatTask, _guildDownloadTask;
    private int _unavailableGuildCount;
    private long _lastGuildAvailableTime, _lastMessageTime;

    private bool _isDisposed;

    /// <inheritdoc />
    public override KookSocketRestClient Rest { get; }

    /// <inheritdoc />
    public ConnectionState ConnectionState => _connection.State;

    /// <inheritdoc />
    public override int Latency { get; protected set; }

    #endregion

    // From KookSocketConfig
    internal int MessageCacheSize { get; private set; }
    internal ClientState State { get; private set; }
    internal UdpSocketProvider UdpSocketProvider { get; private set; }
    internal WebSocketProvider WebSocketProvider { get; private set; }
    internal bool AlwaysDownloadUsers { get; private set; }
    internal bool AlwaysDownloadVoiceStates { get; private set; }
    internal bool AlwaysDownloadBoostSubscriptions { get; private set; }
    internal int? HandlerTimeout { get; private set; }
    internal new KookSocketApiClient ApiClient => base.ApiClient;

    /// <inheritdoc />
    public override IReadOnlyCollection<SocketGuild> Guilds => State.Guilds;

    /// <summary>
    ///     Gets a collection of direct message channels opened in this session.
    /// </summary>
    /// <remarks>
    ///     This method returns a collection of currently opened direct message channels.
    ///     <note type="warning">
    ///         This method will not return previously opened DM channels outside of the current session! If you
    ///         have just started the client, this may return an empty collection.
    ///     </note>
    /// </remarks>
    /// <returns>
    ///     A collection of DM channels that have been opened in this session.
    /// </returns>
    public IReadOnlyCollection<SocketDMChannel> DMChannels
        => State.DMChannels.OfType<SocketDMChannel>().ToImmutableArray();

    /// <summary>
    ///     Initializes a new REST/WebSocket-based Kook client.
    /// </summary>
    public KookSocketClient() : this(new KookSocketConfig())
    {
    }

    /// <summary>
    ///     Initializes a new REST/WebSocket-based Kook client with the provided configuration.
    /// </summary>
    /// <param name="config">The configuration to be used with the client.</param>
    public KookSocketClient(KookSocketConfig config) : this(config, CreateApiClient(config))
    {
    }

    private KookSocketClient(KookSocketConfig config, KookSocketApiClient client)
        : base(config, client)
    {
        MessageCacheSize = config.MessageCacheSize;
        UdpSocketProvider = config.UdpSocketProvider;
        WebSocketProvider = config.WebSocketProvider;
        AlwaysDownloadUsers = config.AlwaysDownloadUsers;
        AlwaysDownloadVoiceStates = config.AlwaysDownloadVoiceStates;
        AlwaysDownloadBoostSubscriptions = config.AlwaysDownloadBoostSubscriptions;
        HandlerTimeout = config.HandlerTimeout;
        State = new ClientState(0, 0);
        Rest = new KookSocketRestClient(config, ApiClient);
        _heartbeatTimes = new ConcurrentQueue<long>();

        _stateLock = new SemaphoreSlim(1, 1);
        _gatewayLogger = LogManager.CreateLogger("Gateway");
        _connection = new ConnectionManager(_stateLock, _gatewayLogger, config.ConnectionTimeout,
            OnConnectingAsync, OnDisconnectingAsync, x => ApiClient.Disconnected += x);
        _connection.Connected += () => TimedInvokeAsync(_connectedEvent, nameof(Connected));
        _connection.Disconnected += (ex, recon) => TimedInvokeAsync(_disconnectedEvent, nameof(Disconnected), ex);

        _serializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        ApiClient.SentGatewayMessage += async socketFrameType =>
            await _gatewayLogger.DebugAsync($"Sent {socketFrameType}").ConfigureAwait(false);
        ApiClient.ReceivedGatewayEvent += ProcessMessageAsync;

        LeftGuild += async g => await _gatewayLogger.InfoAsync($"Left {g.Name}").ConfigureAwait(false);
        JoinedGuild += async g => await _gatewayLogger.InfoAsync($"Joined {g.Name}").ConfigureAwait(false);
        GuildAvailable += async g => await _gatewayLogger.VerboseAsync($"Connected to {g.Name}").ConfigureAwait(false);
        GuildUnavailable += async g => await _gatewayLogger.VerboseAsync($"Disconnected from {g.Name}").ConfigureAwait(false);
        LatencyUpdated += async (old, val) => await _gatewayLogger.DebugAsync($"Latency = {val} ms").ConfigureAwait(false);

        GuildAvailable += g =>
        {
            if (_guildDownloadTask?.IsCompleted == true
                && ConnectionState == ConnectionState.Connected)
            {
                if (AlwaysDownloadUsers && g.HasAllMembers is not true) _ = g.DownloadUsersAsync();

                if (AlwaysDownloadVoiceStates) _ = g.DownloadVoiceStatesAsync();

                if (AlwaysDownloadBoostSubscriptions) _ = g.DownloadBoostSubscriptionsAsync();
            }

            return Task.Delay(0);
        };
    }

    private static KookSocketApiClient CreateApiClient(KookSocketConfig config)
        => new(config.RestClientProvider, config.WebSocketProvider, KookConfig.UserAgent,
            config.AcceptLanguage, config.GatewayHost, defaultRatelimitCallback: config.DefaultRatelimitCallback);

    internal override void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                StopAsync().GetAwaiter().GetResult();
                ApiClient?.Dispose();
                _stateLock?.Dispose();
            }

            _isDisposed = true;
        }

        base.Dispose(disposing);
    }

    private async Task OnConnectingAsync()
    {
        try
        {
            if (_retryCount >= 2)
            {
                _sessionId = null;
                _lastSeq = 0;
                _retryCount = 0;
                await _gatewayLogger.DebugAsync("Resuming session failed").ConfigureAwait(false);
            }

            await _gatewayLogger.DebugAsync("Connecting ApiClient").ConfigureAwait(false);
            if (_sessionId != null)
            {
                _retryCount++;
                await _gatewayLogger.DebugAsync("Resuming session").ConfigureAwait(false);
            }

            await ApiClient.ConnectAsync(_sessionId, _lastSeq).ConfigureAwait(false);
        }
        catch (HttpException ex)
        {
            if (ex.HttpCode == HttpStatusCode.Unauthorized)
                _connection.CriticalError(ex);
            else
                _connection.Error(ex);
        }
        catch
        {
        }

        await _connection.WaitAsync().ConfigureAwait(false);
    }

    private async Task OnDisconnectingAsync(Exception ex)
    {
        await _gatewayLogger.DebugAsync("Disconnecting ApiClient").ConfigureAwait(false);
        await ApiClient.DisconnectAsync(ex).ConfigureAwait(false);

        //Wait for tasks to complete
        await _gatewayLogger.DebugAsync("Waiting for heartbeater").ConfigureAwait(false);
        Task heartbeatTask = _heartbeatTask;
        if (heartbeatTask != null) await heartbeatTask.ConfigureAwait(false);

        _heartbeatTask = null;

        while (_heartbeatTimes.TryDequeue(out _))
        {
        }

        _lastMessageTime = 0;

        //Raise virtual GUILD_UNAVAILABLEs
        await _gatewayLogger.DebugAsync("Raising virtual GuildUnavailables").ConfigureAwait(false);
        foreach (SocketGuild guild in State.Guilds)
            if (guild.IsAvailable)
                await GuildUnavailableAsync(guild).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override SocketGuild GetGuild(ulong id)
        => State.GetGuild(id);

    /// <inheritdoc />
    public override SocketChannel GetChannel(ulong id)
        => State.GetChannel(id);

    /// <inheritdoc />
    public override SocketDMChannel GetDMChannel(Guid chatCode)
        => State.GetDMChannel(chatCode);

    /// <summary>
    ///     Gets a generic channel from the cache or does a rest request if unavailable.
    /// </summary>
    /// <param name="id">The identifier of the channel.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the channel associated
    ///     with the identifier; <c>null</c> when the channel cannot be found.
    /// </returns>
    public async ValueTask<IChannel> GetChannelAsync(ulong id, RequestOptions options = null)
        => GetChannel(id) ?? (IChannel)await ClientHelper.GetChannelAsync(this, id, options).ConfigureAwait(false);

    /// <summary>
    ///     Gets a direct message channel from the cache or does a rest request if unavailable.
    /// </summary>
    /// <param name="chatCode">The identifier of the channel.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the channel associated
    ///     with the identifier; <c>null</c> when the channel cannot be found.
    /// </returns>
    public async ValueTask<IDMChannel> GetDMChannelAsync(Guid chatCode, RequestOptions options = null)
        => await ClientHelper.GetDMChannelAsync(this, chatCode, options).ConfigureAwait(false);

    /// <summary>
    ///     Gets a collection of direct message channels from the cache or does a rest request if unavailable.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the channel associated
    ///     with the identifier; <c>null</c> when the channel cannot be found.
    /// </returns>
    public async ValueTask<IReadOnlyCollection<IDMChannel>> GetDMChannelsAsync(RequestOptions options = null)
        => (await ClientHelper.GetDMChannelsAsync(this, options).ConfigureAwait(false)).ToImmutableArray();

    /// <summary>
    ///     Gets a user from the cache or does a rest request if unavailable.
    /// </summary>
    /// <param name="id">The identifier of the user (e.g. `168693960628371456`).</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the user associated with
    ///     the identifier; <c>null</c> if the user is not found.
    /// </returns>
    public async ValueTask<IUser> GetUserAsync(ulong id, RequestOptions options = null)
        => await ((IKookClient)this).GetUserAsync(id, CacheMode.AllowDownload, options).ConfigureAwait(false);

    /// <inheritdoc />
    public override SocketUser GetUser(ulong id)
        => State.GetUser(id);

    /// <inheritdoc />
    public override SocketUser GetUser(string username, string identifyNumber)
        => State.Users.FirstOrDefault(x => x.IdentifyNumber == identifyNumber && x.Username == username);

    internal SocketGlobalUser GetOrCreateUser(ClientState state, User model) =>
        state.GetOrAddUser(model.Id, x => SocketGlobalUser.Create(this, state, model));

    internal SocketUser GetOrCreateTemporaryUser(ClientState state, User model) =>
        state.GetUser(model.Id) ?? (SocketUser)SocketUnknownUser.Create(this, state, model.Id);

    internal SocketGlobalUser GetOrCreateSelfUser(ClientState state, User model) =>
        state.GetOrAddUser(model.Id, x =>
        {
            SocketGlobalUser user = SocketGlobalUser.Create(this, state, model);
            user.GlobalUser.AddRef();
            return user;
        });

    internal void RemoveUser(ulong id)
        => State.RemoveUser(id);

    /// <summary>
    ///     Downloads all users for the specified guilds.
    /// </summary>
    /// <param name="guilds">
    ///     The guilds to download the users for. If <c>null</c>, all available guilds will be downloaded.
    /// </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public override async Task DownloadUsersAsync(IEnumerable<IGuild> guilds, RequestOptions options)
    {
        if (ConnectionState == ConnectionState.Connected)
            await ProcessUserDownloadsAsync((guilds ?? Guilds.Where(x => x.IsAvailable)).Select(x => GetGuild(x.Id)).Where(x => x != null), options)
                .ConfigureAwait(false);
    }

    private async Task ProcessUserDownloadsAsync(IEnumerable<SocketGuild> guilds, RequestOptions options)
    {
        foreach (SocketGuild socketGuild in guilds)
        {
            IEnumerable<GuildMember> guildMembers =
                await ApiClient.GetGuildMembersAsync(socketGuild.Id, options: options).FlattenAsync().ConfigureAwait(false);
            socketGuild.Update(State, guildMembers.ToImmutableArray());
        }
    }

    /// <summary>
    ///     Downloads all voice states for the specified guilds.
    /// </summary>
    /// <param name="guilds">
    ///     The guilds to download the voice states for. If <c>null</c>, all available guilds will be downloaded.
    /// </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public async Task DownloadVoiceStatesAsync(IEnumerable<IGuild> guilds = null, RequestOptions options = null)
    {
        if (ConnectionState == ConnectionState.Connected)
            await ProcessVoiceStateDownloadsAsync((guilds ?? Guilds.Where(x => x.IsAvailable)).Select(x => GetGuild(x.Id)).Where(x => x != null),
                options).ConfigureAwait(false);
    }

    private async Task ProcessVoiceStateDownloadsAsync(IEnumerable<SocketGuild> guilds, RequestOptions options)
    {
        foreach (SocketGuild socketGuild in guilds)
        {
            foreach (ulong channelId in socketGuild.VoiceChannels.Select(x => x.Id))
            {
                IReadOnlyCollection<User> users = await ApiClient.GetConnectedUsersAsync(channelId, options).ConfigureAwait(false);
                foreach (User user in users) socketGuild.AddOrUpdateVoiceState(user.Id, channelId);
            }

            GetGuildMuteDeafListResponse model = await ApiClient.GetGuildMutedDeafenedUsersAsync(socketGuild.Id).ConfigureAwait(false);
            foreach (ulong id in model.Muted.UserIds) socketGuild.AddOrUpdateVoiceState(id, true);

            foreach (ulong id in socketGuild.Users.Select(x => x.Id).Except(model.Deafened.UserIds)) socketGuild.AddOrUpdateVoiceState(id, false);

            foreach (ulong id in model.Deafened.UserIds) socketGuild.AddOrUpdateVoiceState(id, isDeafened: true);

            foreach (ulong id in socketGuild.Users.Select(x => x.Id).Except(model.Muted.UserIds))
                socketGuild.AddOrUpdateVoiceState(id, isDeafened: false);
        }
    }

    /// <summary>
    ///     Downloads all boost subscriptions for the specified guilds.
    /// </summary>
    /// <param name="guilds">
    ///     The guilds to download the boost subscriptions for. If <c>null</c>, all available guilds will be downloaded.
    /// </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public async Task DownloadBoostSubscriptionsAsync(IEnumerable<IGuild> guilds = null, RequestOptions options = null)
    {
        if (ConnectionState == ConnectionState.Connected)
            await ProcessBoostSubscriptionsDownloadsAsync(
                (guilds ?? Guilds.Where(x => x.IsAvailable)).Select(x => GetGuild(x.Id)).Where(x => x != null), options).ConfigureAwait(false);
    }

    private async Task ProcessBoostSubscriptionsDownloadsAsync(IEnumerable<SocketGuild> guilds, RequestOptions options)
    {
        foreach (SocketGuild socketGuild in guilds)
        {
            IEnumerable<BoostSubscription> subscriptions = await ApiClient.GetGuildBoostSubscriptionsAsync(socketGuild.Id, options: options)
                .FlattenAsync().ConfigureAwait(false);
            socketGuild.Update(State, subscriptions.ToImmutableArray());
        }
    }

    #region ProcessMessageAsync

    private async Task ProcessMessageAsync(GatewaySocketFrameType gatewaySocketFrameType, int? sequence, object payload)
    {
        if (sequence != null)
        {
            if (sequence.Value != _lastSeq + 1)
                await _gatewayLogger.WarningAsync($"Missed a sequence number. Expected {_lastSeq + 1}, got {sequence.Value}.");
            _lastSeq = sequence.Value;
        }

        _lastMessageTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        try
        {
            switch (gatewaySocketFrameType)
            {
                case GatewaySocketFrameType.Event:
                    GatewayEvent gatewayEvent =
                        ((JsonElement)payload).Deserialize<GatewayEvent>(_serializerOptions);

                    dynamic eventExtraData = gatewayEvent!.Type switch
                    {
                        MessageType.System => ((JsonElement)gatewayEvent.ExtraData)
                            .Deserialize<GatewaySystemEventExtraData>(_serializerOptions),
                        _ when gatewayEvent.ChannelType == "GROUP" => ((JsonElement)gatewayEvent.ExtraData)
                            .Deserialize<GatewayGroupMessageExtraData>(_serializerOptions),
                        _ when gatewayEvent.ChannelType == "PERSON" => ((JsonElement)gatewayEvent.ExtraData)
                            .Deserialize<GatewayPersonMessageExtraData>(_serializerOptions),
                        _ => throw new InvalidOperationException("Unknown event type")
                    };

                    switch (gatewayEvent.Type)
                    {
                        case MessageType.Text:
                        case MessageType.Image:
                        case MessageType.Video:
                        case MessageType.File:
                        case MessageType.Audio:
                        case MessageType.KMarkdown:
                        case MessageType.Poke:
                        case MessageType.Card:
                            {
                                await _gatewayLogger.DebugAsync($"Received Message ({gatewayEvent.Type}, {gatewayEvent.ChannelType})")
                                    .ConfigureAwait(false);
                                if (gatewayEvent.ChannelType == "GROUP")
                                {
                                    GatewayGroupMessageExtraData extraData = eventExtraData as GatewayGroupMessageExtraData;

                                    ISocketMessageChannel channel = GetChannel(gatewayEvent.TargetId) as ISocketMessageChannel;
                                    SocketGuild guild = (channel as SocketGuildChannel)?.Guild;

                                    SocketUser author;
                                    if (guild != null)
                                        author = guild.GetUser(extraData.Author.Id);
                                    else
                                        author = (channel as SocketChannel)?.GetUser(extraData.Author.Id);

                                    if (author == null)
                                    {
                                        if (guild != null)
                                            author = guild.AddOrUpdateUser(extraData.Author);
                                        else
                                        {
                                            await UnknownChannelUserAsync(gatewayEvent.Type.ToString(), extraData.Author.Id, channel.Id)
                                                .ConfigureAwait(false);
                                            return;
                                        }
                                    }

                                    SocketMessage msg = SocketMessage.Create(this, State, author, channel, extraData, gatewayEvent);
                                    SocketChannelHelper.AddMessage(channel, this, msg);
                                    await TimedInvokeAsync(_messageReceivedEvent, nameof(MessageReceived), msg).ConfigureAwait(false);
                                }
                                else if (gatewayEvent.ChannelType == "PERSON")
                                {
                                    GatewayPersonMessageExtraData extraData = eventExtraData as GatewayPersonMessageExtraData;
                                    SocketDMChannel channel = CreateDMChannel(extraData.Code, extraData.Author, State);

                                    SocketUser author = channel.GetUser(extraData.Author.Id);
                                    if (author == null)
                                    {
                                        await UnknownChannelUserAsync(gatewayEvent.Type.ToString(), extraData.Author.Id, extraData.Code)
                                            .ConfigureAwait(false);
                                        return;
                                    }

                                    SocketMessage msg = SocketMessage.Create(this, State, author, channel, extraData, gatewayEvent);
                                    SocketChannelHelper.AddMessage(channel, this, msg);
                                    await TimedInvokeAsync(_directMessageReceivedEvent, nameof(DirectMessageReceived), msg).ConfigureAwait(false);
                                }
                            }
                            break;

                        case MessageType.System:
                            {
                                GatewaySystemEventExtraData extraData = eventExtraData as GatewaySystemEventExtraData;
                                switch (gatewayEvent.ChannelType, extraData!.Type)
                                {
                                    #region Channels

                                    // 频道内用户添加 reaction
                                    case ("GROUP", "added_reaction"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (added_reaction)").ConfigureAwait(false);

                                            Reaction data = ((JsonElement)extraData.Body).Deserialize<Reaction>(_serializerOptions);
                                            ISocketMessageChannel channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                                            SocketUserMessage cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                            bool isMsgCached = cachedMsg is not null;

                                            IUser user = channel is not null
                                                ? await channel.GetUserAsync(data.UserId, CacheMode.CacheOnly).ConfigureAwait(false)
                                                : GetUser(data.UserId);
                                            user ??= SocketUnknownUser.Create(this, State, data.UserId);

                                            Cacheable<IMessageChannel, ulong> cacheableChannel = new(channel,
                                                data.ChannelId, channel != null,
                                                async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);
                                            Cacheable<IUserMessage, Guid> cacheableMsg = new(cachedMsg, data.MessageId,
                                                isMsgCached, async () =>
                                                {
                                                    IMessageChannel channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                                    return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false) as IUserMessage;
                                                });

                                            SocketReaction reaction = SocketReaction.Create(data, channel, cachedMsg, user);

                                            cachedMsg?.AddReaction(reaction);

                                            await TimedInvokeAsync(_reactionAddedEvent, nameof(ReactionAdded), cacheableMsg, cacheableChannel,
                                                reaction).ConfigureAwait(false);
                                        }
                                        break;

                                    // 频道内用户取消 reaction
                                    case ("GROUP", "deleted_reaction"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (deleted_reaction)").ConfigureAwait(false);

                                            Reaction data = ((JsonElement)extraData.Body).Deserialize<Reaction>(_serializerOptions);
                                            ISocketMessageChannel channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                                            SocketUserMessage cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                            bool isMsgCached = cachedMsg is not null;

                                            IUser user = channel is not null
                                                ? await channel.GetUserAsync(data.UserId, CacheMode.CacheOnly).ConfigureAwait(false)
                                                : GetUser(data.UserId);
                                            user ??= SocketUnknownUser.Create(this, State, data.UserId);

                                            Cacheable<IMessageChannel, ulong> cacheableChannel = new(channel,
                                                data.ChannelId, channel != null,
                                                async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);
                                            Cacheable<IUserMessage, Guid> cacheableMsg = new(cachedMsg, data.MessageId,
                                                isMsgCached, async () =>
                                                {
                                                    IMessageChannel channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                                    return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false) as IUserMessage;
                                                });
                                            SocketReaction reaction = SocketReaction.Create(data, channel, cachedMsg, user);

                                            cachedMsg?.RemoveReaction(reaction);

                                            await TimedInvokeAsync(_reactionRemovedEvent, nameof(ReactionRemoved), cacheableMsg, cacheableChannel,
                                                reaction).ConfigureAwait(false);
                                        }
                                        break;

                                    // 频道消息更新
                                    case ("GROUP", "updated_message"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (updated_message)").ConfigureAwait(false);
                                            MessageUpdateEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<MessageUpdateEvent>(_serializerOptions);

                                            if (GetChannel(data.ChannelId) is not ISocketMessageChannel channel)
                                            {
                                                await UnknownChannelAsync(extraData.Type, data.ChannelId).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketGuild guild = (channel as SocketGuildChannel)?.Guild;
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketMessage cachedMsg = channel.GetCachedMessage(data.MessageId);
                                            Cacheable<IMessage, Guid> cacheableBefore;
                                            Cacheable<SocketMessage, Guid> cacheableAfter;
                                            if (cachedMsg != null)
                                            {
                                                SocketMessage before = cachedMsg.Clone();
                                                cachedMsg.Update(State, data);
                                                cacheableBefore = new Cacheable<IMessage, Guid>(before, data.MessageId, true,
                                                    () => Task.FromResult((IMessage)null));
                                                cacheableAfter = new Cacheable<SocketMessage, Guid>(cachedMsg, data.MessageId, true,
                                                    async () => await channel
                                                        .GetMessageAsync(cachedMsg.Id).ConfigureAwait(false) as SocketMessage);
                                            }
                                            else
                                            {
                                                cacheableBefore = new Cacheable<IMessage, Guid>(null, data.MessageId, false,
                                                    () => Task.FromResult((IMessage)null));
                                                cacheableAfter = new Cacheable<SocketMessage, Guid>(null, data.MessageId, false,
                                                    async () =>
                                                    {
                                                        Message msg = await ApiClient.GetMessageAsync(data.MessageId).ConfigureAwait(false);
                                                        SocketUser author = guild.GetUser(msg.Author.Id)
                                                            ?? (SocketUser)new SocketUnknownUser(this, msg.Author.Id);
                                                        SocketMessage after = SocketMessage.Create(this, State, author, channel, msg);
                                                        return after;
                                                    });
                                            }

                                            await TimedInvokeAsync(_messageUpdatedEvent, nameof(MessageUpdated), cacheableBefore, cacheableAfter,
                                                    channel)
                                                .ConfigureAwait(false);
                                        }
                                        break;

                                    // 频道消息被删除
                                    case ("GROUP", "deleted_message"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (deleted_message)").ConfigureAwait(false);
                                            MessageDeleteEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<MessageDeleteEvent>(_serializerOptions);
                                            ISocketMessageChannel channel = GetChannel(data.ChannelId) as ISocketMessageChannel;
                                            SocketGuild guild = (channel as SocketGuildChannel)?.Guild;

                                            SocketMessage msg = null;
                                            if (channel != null) msg = SocketChannelHelper.RemoveMessage(channel, this, data.MessageId);

                                            Cacheable<IMessage, Guid> cacheableMsg = new(msg, data.MessageId, msg != null,
                                                () => Task.FromResult((IMessage)null));
                                            Cacheable<IMessageChannel, ulong> cacheableChannel = new(channel,
                                                data.ChannelId, channel != null,
                                                async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);

                                            await TimedInvokeAsync(_messageDeletedEvent, nameof(MessageDeleted), cacheableMsg, cacheableChannel)
                                                .ConfigureAwait(false);
                                        }
                                        break;

                                    // 新增频道
                                    case ("GROUP", "added_channel"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (added_channel)").ConfigureAwait(false);
                                            Channel data = ((JsonElement)extraData.Body).Deserialize<Channel>(_serializerOptions);
                                            SocketChannel channel = null;
                                            SocketGuild guild = State.GetGuild(data.GuildId);
                                            if (guild != null)
                                                channel = guild.AddChannel(State, data);
                                            else
                                            {
                                                await UnknownGuildAsync(extraData.Type, data.GuildId).ConfigureAwait(false);
                                                return;
                                            }

                                            if (channel != null)
                                                await TimedInvokeAsync(_channelCreatedEvent, nameof(ChannelCreated), channel).ConfigureAwait(false);
                                        }
                                        break;

                                    // 修改频道信息
                                    case ("GROUP", "updated_channel"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (updated_channel)").ConfigureAwait(false);
                                            Channel data = ((JsonElement)extraData.Body).Deserialize<Channel>(_serializerOptions);
                                            SocketChannel channel = State.GetChannel(data.Id);
                                            if (channel != null)
                                            {
                                                SocketChannel before = channel.Clone();
                                                channel.Update(State, data);

                                                SocketGuild guild = (channel as SocketGuildChannel)?.Guild;
                                                await TimedInvokeAsync(_channelUpdatedEvent, nameof(ChannelUpdated), before, channel)
                                                    .ConfigureAwait(false);
                                            }
                                            else
                                            {
                                                await UnknownChannelAsync(extraData.Type, data.Id).ConfigureAwait(false);
                                                return;
                                            }
                                        }
                                        break;

                                    // 删除频道
                                    case ("GROUP", "deleted_channel"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (deleted_channel)").ConfigureAwait(false);
                                            ChannelDeleteEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<ChannelDeleteEvent>(_serializerOptions);
                                            SocketChannel channel = State.GetChannel(data.ChannelId);

                                            SocketGuild guild = (channel as SocketGuildChannel)?.Guild;
                                            if (guild != null)
                                                channel = guild.RemoveChannel(State, data.ChannelId);
                                            else
                                            {
                                                await UnknownGuildAsync(extraData.Type, 0).ConfigureAwait(false);
                                                return;
                                            }

                                            if (channel != null)
                                                await TimedInvokeAsync(_channelDestroyedEvent, nameof(ChannelDestroyed), channel)
                                                    .ConfigureAwait(false);
                                            else
                                            {
                                                await UnknownChannelAsync(extraData.Type, data.ChannelId, guild?.Id ?? 0).ConfigureAwait(false);
                                                return;
                                            }
                                        }
                                        break;

                                    // 新的频道置顶消息
                                    case ("GROUP", "pinned_message"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (pinned_message)").ConfigureAwait(false);
                                            PinnedMessageEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<PinnedMessageEvent>(_serializerOptions);
                                            ISocketMessageChannel channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                                            if (channel == null)
                                            {
                                                await UnknownChannelAsync(extraData.Type, data.ChannelId).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketGuild guild = (channel as SocketGuildChannel)?.Guild;
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketGuildUser operatorUser = guild.GetUser(data.OperatorUserId);
                                            Cacheable<SocketGuildUser, ulong> cacheableOperatorUser =
                                                new(operatorUser, data.OperatorUserId, operatorUser != null,
                                                    async () =>
                                                    {
                                                        GuildMember model = await ApiClient
                                                            .GetGuildMemberAsync(guild.Id, data.OperatorUserId).ConfigureAwait(false);
                                                        return guild.AddOrUpdateUser(model);
                                                    });

                                            SocketUserMessage cachedMsg = channel.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                            Cacheable<IMessage, Guid> cacheableBefore;
                                            Cacheable<SocketMessage, Guid> cacheableAfter;

                                            if (cachedMsg != null)
                                            {
                                                SocketMessage before = cachedMsg.Clone();
                                                cachedMsg.IsPinned = true;
                                                cacheableBefore = new Cacheable<IMessage, Guid>(before, data.MessageId, true,
                                                    () => Task.FromResult((IMessage)null));
                                                cacheableAfter = new Cacheable<SocketMessage, Guid>(cachedMsg, data.MessageId, true,
                                                    async () => await channel
                                                        .GetMessageAsync(cachedMsg.Id).ConfigureAwait(false) as SocketMessage);
                                            }
                                            else
                                            {
                                                cacheableBefore = new Cacheable<IMessage, Guid>(null, data.MessageId, false,
                                                    () => Task.FromResult((IMessage)null));
                                                cacheableAfter = new Cacheable<SocketMessage, Guid>(null, data.MessageId, false,
                                                    async () =>
                                                    {
                                                        Message msg = await ApiClient.GetMessageAsync(data.MessageId).ConfigureAwait(false);
                                                        SocketUser author = guild.GetUser(msg.Author.Id)
                                                            ?? (SocketUser)new SocketUnknownUser(this, msg.Author.Id);
                                                        SocketMessage after = SocketMessage.Create(this, State, author, channel, msg);
                                                        return after;
                                                    });
                                            }

                                            await TimedInvokeAsync(_messagePinnedEvent, nameof(MessagePinned), cacheableBefore, cacheableAfter,
                                                channel, cacheableOperatorUser).ConfigureAwait(false);
                                        }
                                        break;

                                    // 取消频道置顶消息
                                    case ("GROUP", "unpinned_message"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (unpinned_message)").ConfigureAwait(false);
                                            UnpinnedMessageEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<UnpinnedMessageEvent>(_serializerOptions);
                                            ISocketMessageChannel channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                                            if (channel == null)
                                            {
                                                await UnknownChannelAsync(extraData.Type, data.ChannelId).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketGuild guild = (channel as SocketGuildChannel)?.Guild;
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketGuildUser operatorUser = guild.GetUser(data.OperatorUserId);
                                            Cacheable<SocketGuildUser, ulong> cacheableOperatorUser =
                                                new(operatorUser, data.OperatorUserId, operatorUser != null,
                                                    async () =>
                                                    {
                                                        GuildMember model = await ApiClient
                                                            .GetGuildMemberAsync(guild.Id, data.OperatorUserId).ConfigureAwait(false);
                                                        return guild.AddOrUpdateUser(model);
                                                    });

                                            SocketUserMessage cachedMsg = channel.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                            Cacheable<IMessage, Guid> cacheableBefore;
                                            Cacheable<SocketMessage, Guid> cacheableAfter;

                                            if (cachedMsg != null)
                                            {
                                                SocketMessage before = cachedMsg.Clone();
                                                cachedMsg.IsPinned = false;
                                                cacheableBefore = new Cacheable<IMessage, Guid>(before, data.MessageId, true,
                                                    () => Task.FromResult((IMessage)null));
                                                cacheableAfter = new Cacheable<SocketMessage, Guid>(cachedMsg, data.MessageId, true,
                                                    async () => await channel
                                                        .GetMessageAsync(cachedMsg.Id).ConfigureAwait(false) as SocketMessage);
                                            }
                                            else
                                            {
                                                cacheableBefore = new Cacheable<IMessage, Guid>(null, data.MessageId, false,
                                                    () => Task.FromResult((IMessage)null));
                                                cacheableAfter = new Cacheable<SocketMessage, Guid>(null, data.MessageId, false,
                                                    async () =>
                                                    {
                                                        Message msg = await ApiClient.GetMessageAsync(data.MessageId).ConfigureAwait(false);
                                                        SocketUser author = guild.GetUser(msg.Author.Id)
                                                            ?? (SocketUser)new SocketUnknownUser(this, msg.Author.Id);
                                                        SocketMessage after = SocketMessage.Create(this, State, author, channel, msg);
                                                        return after;
                                                    });
                                            }

                                            await TimedInvokeAsync(_messageUnpinnedEvent, nameof(MessageUnpinned), cacheableBefore, cacheableAfter,
                                                channel, cacheableOperatorUser).ConfigureAwait(false);
                                        }
                                        break;

                                    #endregion

                                    #region Direct Messages

                                    // 私聊消息更新
                                    case ("PERSON", "updated_private_message"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (updated_private_message)").ConfigureAwait(false);
                                            DirectMessageUpdateEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<DirectMessageUpdateEvent>(_serializerOptions);
                                            User user = await ApiClient.GetUserAsync(data.AuthorId).ConfigureAwait(false);
                                            SocketDMChannel channel = CreateDMChannel(data.ChatCode, user, State);

                                            SocketMessage cachedMsg = channel?.GetCachedMessage(data.MessageId);
                                            Cacheable<IMessage, Guid> cacheableBefore;
                                            Cacheable<SocketMessage, Guid> cacheableAfter;
                                            if (cachedMsg != null)
                                            {
                                                SocketMessage before = cachedMsg.Clone();
                                                cachedMsg.Update(State, data);
                                                cacheableBefore = new Cacheable<IMessage, Guid>(before, data.MessageId, true,
                                                    () => Task.FromResult((IMessage)null));
                                                cacheableAfter = new Cacheable<SocketMessage, Guid>(cachedMsg, data.MessageId, true,
                                                    async () => await channel
                                                        .GetMessageAsync(cachedMsg.Id).ConfigureAwait(false) as SocketMessage);
                                            }
                                            else
                                            {
                                                cacheableBefore = new Cacheable<IMessage, Guid>(null, data.MessageId, false,
                                                    () => Task.FromResult((IMessage)null));
                                                cacheableAfter = new Cacheable<SocketMessage, Guid>(null, data.MessageId, false,
                                                    async () =>
                                                    {
                                                        DirectMessage msg = await ApiClient.GetDirectMessageAsync(data.MessageId, data.ChatCode)
                                                            .ConfigureAwait(false);
                                                        SocketUser author = State.GetUser(data.AuthorId)
                                                            ?? (SocketUser)new SocketUnknownUser(this, data.AuthorId);
                                                        SocketMessage after = SocketMessage.Create(this, State, author, channel, msg);
                                                        return after;
                                                    });
                                            }

                                            await TimedInvokeAsync(_directMessageUpdatedEvent, nameof(DirectMessageUpdated), cacheableBefore,
                                                cacheableAfter,
                                                channel).ConfigureAwait(false);
                                        }
                                        break;

                                    // 私聊消息被删除
                                    case ("PERSON", "deleted_private_message"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (deleted_private_message)").ConfigureAwait(false);
                                            DirectMessageDeleteEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<DirectMessageDeleteEvent>(_serializerOptions);
                                            User user = await ApiClient.GetUserAsync(data.AuthorId).ConfigureAwait(false);
                                            SocketDMChannel channel = CreateDMChannel(data.ChatCode, user, State);

                                            SocketMessage msg = null;
                                            if (channel != null) msg = SocketChannelHelper.RemoveMessage(channel, this, data.MessageId);

                                            Cacheable<IMessage, Guid> cacheableMsg = new(msg, data.MessageId, msg != null,
                                                () => Task.FromResult((IMessage)null));
                                            Cacheable<IDMChannel, Guid> cacheableChannel = new(channel, data.ChatCode,
                                                channel != null, async () => await GetDMChannelAsync(data.ChatCode).ConfigureAwait(false));

                                            await TimedInvokeAsync(_directMessageDeletedEvent, nameof(DirectMessageDeleted), cacheableMsg,
                                                cacheableChannel).ConfigureAwait(false);
                                        }
                                        break;

                                    // 私聊内用户添加 reaction
                                    case ("PERSON", "private_added_reaction"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (private_added_reaction)").ConfigureAwait(false);

                                            PrivateReaction data = ((JsonElement)extraData.Body).Deserialize<PrivateReaction>(_serializerOptions);
                                            ISocketMessageChannel channel = GetDMChannel(data.ChatCode) as ISocketMessageChannel;

                                            SocketUserMessage cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                            bool isMsgCached = cachedMsg is not null;

                                            IUser user = channel is not null
                                                ? await channel.GetUserAsync(data.UserId, CacheMode.CacheOnly).ConfigureAwait(false)
                                                : GetUser(data.UserId);
                                            user ??= SocketUnknownUser.Create(this, State, data.UserId);

                                            Cacheable<IDMChannel, Guid> cacheableChannel = new((IDMChannel)channel,
                                                data.ChatCode, channel != null,
                                                async () => await GetDMChannelAsync(data.ChatCode).ConfigureAwait(false));
                                            Cacheable<IUserMessage, Guid> cacheableMsg = new(cachedMsg, data.MessageId,
                                                isMsgCached, async () =>
                                                {
                                                    IDMChannel channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                                    return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false) as IUserMessage;
                                                });
                                            SocketReaction reaction = SocketReaction.Create(data, (IDMChannel)channel, cachedMsg, user);

                                            cachedMsg?.AddReaction(reaction);

                                            await TimedInvokeAsync(_directReactionAddedEvent, nameof(DirectReactionAdded), cacheableMsg,
                                                cacheableChannel, reaction).ConfigureAwait(false);
                                        }
                                        break;

                                    // 私聊内用户取消 reaction
                                    case ("PERSON", "private_deleted_reaction"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (private_deleted_reaction)").ConfigureAwait(false);

                                            PrivateReaction data = ((JsonElement)extraData.Body).Deserialize<PrivateReaction>(_serializerOptions);
                                            ISocketMessageChannel channel = GetDMChannel(data.ChatCode) as ISocketMessageChannel;

                                            SocketUserMessage cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                            bool isMsgCached = cachedMsg is not null;

                                            IUser user = channel is not null
                                                ? await channel.GetUserAsync(data.UserId, CacheMode.CacheOnly).ConfigureAwait(false)
                                                : GetUser(data.UserId);
                                            user ??= SocketUnknownUser.Create(this, State, data.UserId);

                                            Cacheable<IDMChannel, Guid> cacheableChannel = new((IDMChannel)channel,
                                                data.ChatCode, channel != null,
                                                async () => await GetDMChannelAsync(data.ChatCode).ConfigureAwait(false));
                                            Cacheable<IUserMessage, Guid> cacheableMsg = new(cachedMsg, data.MessageId,
                                                isMsgCached, async () =>
                                                {
                                                    IDMChannel channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                                    return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false) as IUserMessage;
                                                });
                                            SocketReaction reaction = SocketReaction.Create(data, (IDMChannel)channel, cachedMsg, user);

                                            cachedMsg?.RemoveReaction(reaction);

                                            await TimedInvokeAsync(_directReactionRemovedEvent, nameof(DirectReactionRemoved), cacheableMsg,
                                                cacheableChannel, reaction).ConfigureAwait(false);
                                        }
                                        break;

                                    #endregion

                                    #region Guild Members

                                    // 新成员加入服务器
                                    case ("GROUP", "joined_guild"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (joined_guild)").ConfigureAwait(false);
                                            SocketGuild guild = State.GetGuild(gatewayEvent.TargetId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            GuildMemberAddEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<GuildMemberAddEvent>(_serializerOptions);
                                            GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, data.UserId).ConfigureAwait(false);
                                            SocketGuildUser user = guild.AddOrUpdateUser(model);
                                            guild.MemberCount++;
                                            await TimedInvokeAsync(_userJoinedEvent, nameof(UserJoined), user, data.JoinedAt)
                                                .ConfigureAwait(false);
                                        }
                                        break;

                                    // 服务器成员退出
                                    case ("GROUP", "exited_guild"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (exited_guild)").ConfigureAwait(false);
                                            SocketGuild guild = State.GetGuild(gatewayEvent.TargetId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            GuildMemberRemoveEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<GuildMemberRemoveEvent>(_serializerOptions);
                                            SocketUser user = guild.RemoveUser(data.UserId);
                                            guild.MemberCount--;
                                            user ??= State.GetUser(data.UserId);

                                            Cacheable<SocketUser, ulong> cacheableUser = new(user, data.UserId, user != null,
                                                async () =>
                                                {
                                                    User model = await ApiClient.GetUserAsync(data.UserId).ConfigureAwait(false);
                                                    user = State.GetOrAddUser(data.UserId, _ => SocketGlobalUser.Create(this, State, model));
                                                    user.Update(State, model);
                                                    user.UpdatePresence(model.Online, model.OperatingSystem);
                                                    return user;
                                                });

                                            await TimedInvokeAsync(_userLeftEvent, nameof(UserLeft), guild, cacheableUser, data.ExitedAt)
                                                .ConfigureAwait(false);
                                        }
                                        break;

                                    // 服务器成员信息更新
                                    case ("GROUP", "updated_guild_member"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (updated_guild_member)").ConfigureAwait(false);
                                            SocketGuild guild = State.GetGuild(gatewayEvent.TargetId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            GuildMemberUpdateEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<GuildMemberUpdateEvent>(_serializerOptions);
                                            SocketGuildUser user = guild.GetUser(data.UserId);

                                            if (user != null)
                                            {
                                                SocketGuildUser before = user.Clone();
                                                user.Update(State, data);

                                                Cacheable<SocketGuildUser, ulong> cacheableBefore = new(before, user.Id, true,
                                                    () => Task.FromResult<SocketGuildUser>(null));
                                                Cacheable<SocketGuildUser, ulong> cacheableAfter = new(user, user.Id, true,
                                                    () => Task.FromResult(user));
                                                await TimedInvokeAsync(_guildMemberUpdatedEvent, nameof(GuildMemberUpdated),
                                                    cacheableBefore, cacheableAfter).ConfigureAwait(false);
                                            }
                                            else
                                            {
                                                Cacheable<SocketGuildUser, ulong> cacheableBefore = new(null, data.UserId, false,
                                                    () => Task.FromResult<SocketGuildUser>(null));
                                                Cacheable<SocketGuildUser, ulong> cacheableAfter = new(null, data.UserId, false,
                                                    async () =>
                                                    {
                                                        GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, data.UserId)
                                                            .ConfigureAwait(false);
                                                        return guild.AddOrUpdateUser(model);
                                                    });
                                                await TimedInvokeAsync(_guildMemberUpdatedEvent, nameof(GuildMemberUpdated),
                                                    cacheableBefore, cacheableAfter).ConfigureAwait(false);
                                            }
                                        }
                                        break;

                                    // 服务器成员上线
                                    case ("PERSON", "guild_member_online"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (guild_member_online)").ConfigureAwait(false);
                                            GuildMemberOnlineEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<GuildMemberOnlineEvent>(_serializerOptions);
                                            List<Cacheable<SocketGuildUser, ulong>> users = new();
                                            foreach (ulong guildId in data.CommonGuilds)
                                            {
                                                SocketGuild guild = State.GetGuild(guildId);
                                                if (guild == null)
                                                {
                                                    await UnknownGuildAsync(extraData.Type, guildId).ConfigureAwait(false);
                                                    return;
                                                }

                                                SocketGuildUser user = guild.GetUser(data.UserId);
                                                if (user is not null)
                                                {
                                                    user.Presence.Update(true);
                                                    users.Add(new Cacheable<SocketGuildUser, ulong>(user, data.UserId, true,
                                                        () => Task.FromResult(user)));
                                                }
                                                else
                                                    users.Add(new Cacheable<SocketGuildUser, ulong>(null, data.UserId, false,
                                                        async () =>
                                                        {
                                                            GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, data.UserId)
                                                                .ConfigureAwait(false);
                                                            user = guild.AddOrUpdateUser(model);
                                                            user.Presence.Update(true);
                                                            return user;
                                                        }));
                                            }

                                            await TimedInvokeAsync(_guildMemberOnlineEvent, nameof(GuildMemberOnline), users, data.OnlineAt)
                                                .ConfigureAwait(false);
                                        }
                                        break;

                                    // 服务器成员下线
                                    case ("PERSON", "guild_member_offline"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (guild_member_offline)").ConfigureAwait(false);
                                            GuildMemberOfflineEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<GuildMemberOfflineEvent>(_serializerOptions);
                                            List<Cacheable<SocketGuildUser, ulong>> users = new();
                                            foreach (ulong guildId in data.CommonGuilds)
                                            {
                                                SocketGuild guild = State.GetGuild(guildId);
                                                if (guild == null)
                                                {
                                                    await UnknownGuildAsync(extraData.Type, guildId).ConfigureAwait(false);
                                                    return;
                                                }

                                                SocketGuildUser user = guild.GetUser(data.UserId);
                                                if (user is not null)
                                                {
                                                    user.Presence.Update(false);
                                                    users.Add(new Cacheable<SocketGuildUser, ulong>(user, data.UserId, true,
                                                        () => Task.FromResult(user)));
                                                }
                                                else
                                                    users.Add(new Cacheable<SocketGuildUser, ulong>(null, data.UserId, false,
                                                        async () =>
                                                        {
                                                            GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, data.UserId)
                                                                .ConfigureAwait(false);
                                                            user = guild.AddOrUpdateUser(model);
                                                            user.Presence.Update(false);
                                                            return user;
                                                        }));
                                            }

                                            await TimedInvokeAsync(_guildMemberOfflineEvent, nameof(GuildMemberOffline), users, data.OfflineAt)
                                                .ConfigureAwait(false);
                                        }
                                        break;

                                    #endregion

                                    #region Guild Roles

                                    // 服务器角色增加
                                    case ("GROUP", "added_role"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (added_role)").ConfigureAwait(false);

                                            Role data = ((JsonElement)extraData.Body).Deserialize<Role>(_serializerOptions);
                                            SocketGuild guild = State.GetGuild(gatewayEvent.TargetId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketRole role = guild.AddRole(data);
                                            await TimedInvokeAsync(_roleCreatedEvent, nameof(RoleCreated), role).ConfigureAwait(false);
                                        }
                                        break;

                                    // 服务器角色删除
                                    case ("GROUP", "deleted_role"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (deleted_role)").ConfigureAwait(false);

                                            Role data = ((JsonElement)extraData.Body).Deserialize<Role>(_serializerOptions);
                                            SocketGuild guild = State.GetGuild(gatewayEvent.TargetId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketRole role = guild.RemoveRole(data.Id);
                                            await TimedInvokeAsync(_roleDeletedEvent, nameof(RoleDeleted), role).ConfigureAwait(false);
                                        }
                                        break;

                                    // 服务器角色更新
                                    case ("GROUP", "updated_role"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (updated_role)").ConfigureAwait(false);

                                            Role data = ((JsonElement)extraData.Body).Deserialize<Role>(_serializerOptions);
                                            SocketGuild guild = State.GetGuild(gatewayEvent.TargetId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketRole role = guild.GetRole(data.Id);
                                            if (role == null)
                                            {
                                                await UnknownRoleAsync(extraData.Type, data.Id, guild.Id).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketRole before = role.Clone();
                                            role.Update(State, data);

                                            await TimedInvokeAsync(_roleUpdatedEvent, nameof(RoleUpdated), before, role)
                                                .ConfigureAwait(false);
                                        }
                                        break;

                                    #endregion

                                    #region Guild Emojis

                                    // 服务器表情新增
                                    case ("GROUP", "added_emoji"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (added_emoji)").ConfigureAwait(false);

                                            GuildEmojiEvent data = ((JsonElement)extraData.Body).Deserialize<GuildEmojiEvent>(_serializerOptions);
                                            SocketGuild guild = State.GetGuild(gatewayEvent.TargetId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            GuildEmote emote = guild.AddEmote(data);
                                            await TimedInvokeAsync(_emoteCreatedEvent, nameof(EmoteCreated), emote, guild).ConfigureAwait(false);
                                        }
                                        break;

                                    // 服务器表情更新
                                    case ("GROUP", "updated_emoji"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (updated_emoji)").ConfigureAwait(false);

                                            GuildEmojiEvent data = ((JsonElement)extraData.Body).Deserialize<GuildEmojiEvent>(_serializerOptions);
                                            SocketGuild guild = State.GetGuild(gatewayEvent.TargetId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            GuildEmote emote = guild.GetEmote(data?.Id);
                                            GuildEmote before = emote.Clone();
                                            GuildEmote after = guild.AddOrUpdateEmote(data);
                                            await TimedInvokeAsync(_emoteUpdatedEvent, nameof(EmoteUpdated), before, after, guild)
                                                .ConfigureAwait(false);
                                        }
                                        break;

                                    // 服务器表情删除
                                    case ("GROUP", "deleted_emoji"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (deleted_emoji)").ConfigureAwait(false);

                                            GuildEmojiEvent data = ((JsonElement)extraData.Body).Deserialize<GuildEmojiEvent>(_serializerOptions);
                                            SocketGuild guild = State.GetGuild(gatewayEvent.TargetId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            GuildEmote emote = guild.RemoveEmote(data?.Id);
                                            await TimedInvokeAsync(_emoteDeletedEvent, nameof(EmoteDeleted), emote, guild).ConfigureAwait(false);
                                        }
                                        break;

                                    #endregion

                                    #region Guilds

                                    // 服务器信息更新
                                    case ("GROUP", "updated_guild"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (updated_guild)").ConfigureAwait(false);

                                            GuildEvent data = ((JsonElement)extraData.Body).Deserialize<GuildEvent>(_serializerOptions);
                                            SocketGuild guild = State.GetGuild(data.GuildId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, data.GuildId).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketGuild before = guild.Clone();
                                            guild.Update(State, data);
                                            if (AlwaysDownloadBoostSubscriptions
                                                && (before.BoostSubscriptionCount != guild.BoostSubscriptionCount
                                                    || before.BufferBoostSubscriptionCount != guild.BufferBoostSubscriptionCount))
                                                await guild.DownloadBoostSubscriptionsAsync().ConfigureAwait(false);

                                            await TimedInvokeAsync(_guildUpdatedEvent, nameof(GuildUpdated), before, guild).ConfigureAwait(false);
                                        }
                                        break;

                                    // 服务器删除
                                    case ("GROUP", "deleted_guild"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (deleted_guild)").ConfigureAwait(false);
                                            GuildEvent data = ((JsonElement)extraData.Body).Deserialize<GuildEvent>(_serializerOptions);

                                            SocketGuild guild = RemoveGuild(data.GuildId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            await GuildUnavailableAsync(guild).ConfigureAwait(false);
                                            await TimedInvokeAsync(_leftGuildEvent, nameof(LeftGuild), guild).ConfigureAwait(false);
                                            ((IDisposable)guild).Dispose();
                                        }
                                        break;

                                    // 服务器封禁用户
                                    case ("GROUP", "added_block_list"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (added_block_list)").ConfigureAwait(false);
                                            GuildBanEvent data = ((JsonElement)extraData.Body).Deserialize<GuildBanEvent>(_serializerOptions);
                                            SocketGuild guild = State.GetGuild(gatewayEvent.TargetId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketUser operatorUser = guild.GetUser(data.OperatorUserId)
                                                ?? (SocketUser)SocketUnknownUser.Create(this, State, data.OperatorUserId);
                                            IReadOnlyCollection<SocketUser> bannedUsers = data.UserIds.Select(id => guild.GetUser(id)
                                                    ?? (SocketUser)SocketUnknownUser.Create(this, State, id))
                                                .ToReadOnlyCollection(() => data.UserIds.Length);
                                            await TimedInvokeAsync(_userBannedEvent, nameof(UserBanned), bannedUsers, operatorUser, guild)
                                                .ConfigureAwait(false);
                                        }
                                        break;

                                    // 服务器取消封禁用户
                                    case ("GROUP", "deleted_block_list"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (deleted_block_list)").ConfigureAwait(false);
                                            GuildBanEvent data = ((JsonElement)extraData.Body).Deserialize<GuildBanEvent>(_serializerOptions);
                                            SocketGuild guild = State.GetGuild(gatewayEvent.TargetId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketUser operatorUser = guild.GetUser(data.OperatorUserId)
                                                ?? (SocketUser)SocketUnknownUser.Create(this, State, data.OperatorUserId);
                                            IReadOnlyCollection<SocketUser> unbannedUsers = data.UserIds.Select(id => guild.GetUser(id)
                                                    ?? (SocketUser)SocketUnknownUser.Create(this, State, id))
                                                .ToReadOnlyCollection(() => data.UserIds.Length);
                                            await TimedInvokeAsync(_userUnbannedEvent, nameof(UserUnbanned), unbannedUsers, operatorUser, guild)
                                                .ConfigureAwait(false);
                                        }
                                        break;

                                    #endregion

                                    #region Users

                                    // 用户加入语音频道
                                    case ("GROUP", "joined_channel"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (joined_channel)").ConfigureAwait(false);
                                            UserVoiceEvent data = ((JsonElement)extraData.Body).Deserialize<UserVoiceEvent>(_serializerOptions);
                                            SocketGuild guild = State.GetGuild(gatewayEvent.TargetId);
                                            if (guild != null)
                                            {
                                                SocketVoiceChannel channel = GetChannel(data.ChannelId) as SocketVoiceChannel;

                                                if (channel == null)
                                                {
                                                    await UnknownChannelAsync(extraData.Type, data.ChannelId).ConfigureAwait(false);
                                                    return;
                                                }

                                                SocketUser user = guild.GetUser(data.UserId)
                                                    ?? SocketUnknownUser.Create(this, State, data.UserId) as SocketUser;
                                                guild.AddOrUpdateVoiceState(user.Id, channel.Id);

                                                await TimedInvokeAsync(_userConnectedEvent, nameof(UserConnected), user, channel, guild, data.At)
                                                    .ConfigureAwait(false);
                                            }
                                            else
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }
                                        }
                                        break;

                                    // 用户退出语音频道
                                    case ("GROUP", "exited_channel"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (exited_channel)").ConfigureAwait(false);
                                            UserVoiceEvent data = ((JsonElement)extraData.Body).Deserialize<UserVoiceEvent>(_serializerOptions);
                                            SocketGuild guild = State.GetGuild(gatewayEvent.TargetId);
                                            if (guild != null)
                                            {
                                                SocketVoiceChannel channel = GetChannel(data.ChannelId) as SocketVoiceChannel;

                                                if (channel == null)
                                                {
                                                    await UnknownChannelAsync(extraData.Type, data.ChannelId).ConfigureAwait(false);
                                                    return;
                                                }

                                                SocketUser user = guild.GetUser(data.UserId)
                                                    ?? SocketUnknownUser.Create(this, State, data.UserId) as SocketUser;
                                                guild.AddOrUpdateVoiceState(user.Id, null);

                                                await TimedInvokeAsync(_userDisconnectedEvent, nameof(UserDisconnected), user, channel, guild,
                                                    data.At).ConfigureAwait(false);
                                            }
                                            else
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }
                                        }
                                        break;

                                    // 用户信息更新
                                    case ("PERSON", "user_updated"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (user_updated)").ConfigureAwait(false);
                                            UserUpdateEvent data = ((JsonElement)extraData.Body).Deserialize<UserUpdateEvent>(_serializerOptions);
                                            if (data.UserId == CurrentUser.Id)
                                            {
                                                SocketSelfUser before = CurrentUser.Clone();
                                                CurrentUser.Update(State, data);
                                                await TimedInvokeAsync(_selfUpdatedEvent, nameof(CurrentUserUpdated), before, CurrentUser)
                                                    .ConfigureAwait(false);
                                            }
                                            else
                                            {
                                                SocketUser user = GetUser(data.UserId);
                                                if (user != null)
                                                {
                                                    SocketUser before = user.Clone();
                                                    user.Update(State, data);
                                                    await TimedInvokeAsync(_userUpdatedEvent, nameof(UserUpdated),
                                                            new Cacheable<SocketUser, ulong>(before, data.UserId, true,
                                                                () => Task.FromResult((SocketUser)null)),
                                                            new Cacheable<SocketUser, ulong>(user, data.UserId, true,
                                                                async () =>
                                                                {
                                                                    User model = await ApiClient.GetUserAsync(data.UserId).ConfigureAwait(false);
                                                                    return SocketGlobalUser.Create(this, State, model);
                                                                }))
                                                        .ConfigureAwait(false);
                                                }
                                                else
                                                    await TimedInvokeAsync(_userUpdatedEvent, nameof(UserUpdated),
                                                        new Cacheable<SocketUser, ulong>(null, data.UserId, false,
                                                            () => Task.FromResult((SocketUser)null)),
                                                        new Cacheable<SocketUser, ulong>(null, data.UserId, false,
                                                            async () =>
                                                            {
                                                                User model = await ApiClient.GetUserAsync(data.UserId).ConfigureAwait(false);
                                                                return SocketGlobalUser.Create(this, State, model);
                                                            }));
                                            }
                                        }
                                        break;

                                    // 自己新加入服务器
                                    case ("PERSON", "self_joined_guild"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (self_joined_guild)").ConfigureAwait(false);
                                            SelfGuildEvent data = ((JsonElement)extraData.Body).Deserialize<SelfGuildEvent>(_serializerOptions);

                                            _ = Task.Run(async () =>
                                            {
                                                try
                                                {
                                                    int maxRetryTime = 10;
                                                    while (true)
                                                    {
                                                        try
                                                        {
                                                            return await ApiClient.GetGuildAsync(data.GuildId).ConfigureAwait(false);
                                                        }
                                                        catch (HttpException ex) when (ex.HttpCode == HttpStatusCode.Forbidden)
                                                        {
                                                            if (--maxRetryTime == 0) throw;
                                                        }

                                                        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    await _gatewayLogger
                                                        .ErrorAsync($"Error handling {gatewaySocketFrameType}. Payload: {payload}", e)
                                                        .ConfigureAwait(false);
                                                    return null;
                                                }
                                            }).ContinueWith(async t =>
                                            {
                                                ExtendedGuild model = t.Result;
                                                if (model is null) return;
                                                SocketGuild guild = AddGuild(model, State);
                                                guild.Update(State, model);
                                                await TimedInvokeAsync(_joinedGuildEvent, nameof(JoinedGuild), guild).ConfigureAwait(false);
                                                await GuildAvailableAsync(guild).ConfigureAwait(false);
                                            });
                                        }
                                        break;

                                    // 自己退出服务器
                                    case ("PERSON", "self_exited_guild"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (self_exited_guild)").ConfigureAwait(false);
                                            SelfGuildEvent data = ((JsonElement)extraData.Body).Deserialize<SelfGuildEvent>(_serializerOptions);

                                            SocketGuild guild = RemoveGuild(data.GuildId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                return;
                                            }

                                            await GuildUnavailableAsync(guild).ConfigureAwait(false);
                                            await TimedInvokeAsync(_leftGuildEvent, nameof(LeftGuild), guild).ConfigureAwait(false);
                                            ((IDisposable)guild).Dispose();
                                        }
                                        break;

                                    #endregion

                                    #region Interactions

                                    // Card 消息中的 Button 点击事件
                                    case ("PERSON", "message_btn_click"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (message_btn_click)").ConfigureAwait(false);
                                            MessageButtonClickEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<MessageButtonClickEvent>(_serializerOptions);
                                            if (data.GuildId.HasValue)
                                            {
                                                SocketTextChannel channel = GetChannel(data.ChannelId) as SocketTextChannel;
                                                SocketGuild guild = GetGuild(data.GuildId.Value);
                                                if (guild == null)
                                                {
                                                    await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                    return;
                                                }

                                                if (channel == null)
                                                {
                                                    await UnknownChannelAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                    return;
                                                }

                                                SocketUser user = channel.GetUser(data.UserId)
                                                    ?? SocketUnknownUser.Create(this, State, data.UserId) as SocketUser;
                                                IMessage msg = await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false);
                                                await TimedInvokeAsync(_messageButtonClickedEvent, nameof(MessageButtonClicked), data.Value, user,
                                                    msg, channel, guild).ConfigureAwait(false);
                                            }
                                            else
                                            {
                                                SocketUser user = GetUser(data.UserId)
                                                    ?? SocketUnknownUser.Create(this, State, data.UserId);
                                                SocketDMChannel channel = await user.CreateDMChannelAsync().ConfigureAwait(false);
                                                if (channel == null)
                                                {
                                                    await UnknownChannelAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                                    return;
                                                }

                                                IMessage message = await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false);
                                                await TimedInvokeAsync(_directMessageButtonClickedEvent, nameof(DirectMessageButtonClicked),
                                                    data.Value, user, message, channel).ConfigureAwait(false);
                                            }
                                        }
                                        break;

                                    #endregion

                                    default:
                                        await _gatewayLogger.WarningAsync($"Unknown SystemEventType ({extraData.Type}). Payload: {payload}")
                                            .ConfigureAwait(false);
                                        break;
                                }
                            }
                            break;
                        default:
                            await _gatewayLogger.WarningAsync($"Unknown Event Type ({gatewayEvent.Type}). Payload: {payload}")
                                .ConfigureAwait(false);
                            break;
                    }

                    break;

                case GatewaySocketFrameType.Hello:
                    {
                        // Process Hello
                        await _gatewayLogger.DebugAsync("Received Hello").ConfigureAwait(false);
                        try
                        {
                            GatewayHelloPayload gatewayHelloPayload =
                                ((JsonElement)payload).Deserialize<GatewayHelloPayload>(_serializerOptions);
                            _sessionId = gatewayHelloPayload?.SessionId;
                            _heartbeatTask = RunHeartbeatAsync(_connection.CancelToken);
                        }
                        catch (Exception ex)
                        {
                            _connection.CriticalError(new Exception("Processing Hello failed", ex));
                            return;
                        }

                        // Get current user
                        try
                        {
                            SelfUser selfUser = await ApiClient.GetSelfUserAsync().ConfigureAwait(false);
                            SocketSelfUser currentUser = SocketSelfUser.Create(this, State, selfUser);
                            Rest.CreateRestSelfUser(selfUser);
                            ApiClient.CurrentUserId = currentUser.Id;
                            Rest.CurrentUser = RestSelfUser.Create(this, selfUser);
                            CurrentUser = currentUser;
                        }
                        catch (Exception ex)
                        {
                            _connection.CriticalError(new Exception("Processing SelfUser failed", ex));
                            return;
                        }

                        // Download guild data
                        try
                        {
                            IReadOnlyCollection<RichGuild> guilds = await ApiClient.ListGuildsAsync().ConfigureAwait(false);
                            ClientState state = new(guilds.Count, 0);
                            int unavailableGuilds = 0;
                            foreach (RichGuild guild in guilds)
                            {
                                RichGuild model = guild;
                                SocketGuild socketGuild = AddGuild(model, state);
                                if (!socketGuild.IsAvailable)
                                    unavailableGuilds++;
                                else
                                    await GuildAvailableAsync(socketGuild).ConfigureAwait(false);
                            }

                            _unavailableGuildCount = unavailableGuilds;
                            State = state;
                        }
                        catch (Exception ex)
                        {
                            _connection.CriticalError(new Exception("Processing Guilds failed", ex));
                            return;
                        }

                        // // Download guild data
                        // try
                        // {
                        //     var guilds = (await ApiClient.GetGuildsAsync().FlattenAsync().ConfigureAwait(false)).ToList();
                        //     var state = new ClientState(guilds.Count, 0);
                        //     int unavailableGuilds = 0;
                        //     foreach (Guild guild in guilds)
                        //     {
                        //         var model = guild;
                        //         var socketGuild = AddGuild(model, state);
                        //         if (!socketGuild.IsAvailable)
                        //             unavailableGuilds++;
                        //         else
                        //             await GuildAvailableAsync(socketGuild).ConfigureAwait(false);
                        //     }
                        //     _unavailableGuildCount = unavailableGuilds;
                        //     State = state;
                        // }
                        // catch (Exception ex)
                        // {
                        //     _connection.CriticalError(new Exception("Processing Guilds failed", ex));
                        //     return;
                        // }

                        _lastGuildAvailableTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        _guildDownloadTask = WaitForGuildsAsync(_connection.CancelToken, _gatewayLogger)
                            .ContinueWith(async task =>
                            {
                                if (task.IsFaulted)
                                {
                                    _connection.Error(task.Exception);
                                    return;
                                }
                                else if (_connection.CancelToken.IsCancellationRequested) return;

                                // Download user list if enabled
                                if (_baseConfig.AlwaysDownloadUsers)
                                    _ = DownloadUsersAsync(Guilds.Where(x => x.IsAvailable && x.HasAllMembers is not true), RequestOptions.Default);

                                if (_baseConfig.AlwaysDownloadVoiceStates)
                                    _ = DownloadVoiceStatesAsync(Guilds.Where(x => x.IsAvailable), RequestOptions.Default);

                                if (_baseConfig.AlwaysDownloadBoostSubscriptions)
                                    _ = DownloadBoostSubscriptionsAsync(Guilds.Where(x => x.IsAvailable), RequestOptions.Default);

                                await TimedInvokeAsync(_readyEvent, nameof(Ready)).ConfigureAwait(false);
                                await _gatewayLogger.InfoAsync("Ready").ConfigureAwait(false);
                            });

                        _ = _connection.CompleteAsync();
                    }
                    break;

                case GatewaySocketFrameType.Pong:
                    {
                        await _gatewayLogger.DebugAsync("Received Pong").ConfigureAwait(false);
                        if (_heartbeatTimes.TryDequeue(out long time))
                        {
                            int latency = (int)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - time);
                            int before = Latency;
                            Latency = latency;

                            await TimedInvokeAsync(_latencyUpdatedEvent, nameof(LatencyUpdated), before, latency)
                                .ConfigureAwait(false);
                        }
                    }
                    break;

                case GatewaySocketFrameType.Reconnect:
                    {
                        await _gatewayLogger.DebugAsync("Received Reconnect").ConfigureAwait(false);
                        GatewayReconnectPayload gatewayReconnectPayload =
                            ((JsonElement)payload).Deserialize<GatewayReconnectPayload>(_serializerOptions);
                        if (gatewayReconnectPayload?.Code is KookErrorCode.MissingResumeArgument
                            or KookErrorCode.SessionExpired
                            or KookErrorCode.InvalidSequenceNumber)
                        {
                            _sessionId = null;
                            _lastSeq = 0;
                        }

                        _connection.Error(new GatewayReconnectException($"Server requested a reconnect, resuming session failed"
                            + $"{(string.IsNullOrWhiteSpace(gatewayReconnectPayload?.Message) ? string.Empty : $": {gatewayReconnectPayload.Message}")}"));
                    }
                    break;

                case GatewaySocketFrameType.ResumeAck:
                    {
                        await _gatewayLogger.DebugAsync("Received ResumeAck").ConfigureAwait(false);
                        _ = _connection.CompleteAsync();

                        //Notify the client that these guilds are available again
                        foreach (SocketGuild guild in State.Guilds)
                            if (guild.IsAvailable)
                                await GuildAvailableAsync(guild).ConfigureAwait(false);

                        await _gatewayLogger.InfoAsync("Resumed previous session").ConfigureAwait(false);
                    }
                    break;

                default:
                    await _gatewayLogger.WarningAsync($"Unknown Socket Frame Type ({gatewaySocketFrameType}). Payload: {payload}")
                        .ConfigureAwait(false);
                    break;
            }
        }
        catch (Exception ex)
        {
            await _gatewayLogger.ErrorAsync($"Error handling {gatewaySocketFrameType}. Payload: {payload}", ex).ConfigureAwait(false);
        }
    }

    #endregion

    /// <inheritdoc />
    public override async Task StartAsync() => await _connection.StartAsync().ConfigureAwait(false);

    /// <inheritdoc />
    public override async Task StopAsync() => await _connection.StopAsync().ConfigureAwait(false);

    private async Task RunHeartbeatAsync(CancellationToken cancelToken)
    {
        int intervalMillis = KookSocketConfig.HeartbeatIntervalMilliseconds;
        try
        {
            await _gatewayLogger.DebugAsync("Heartbeat Started").ConfigureAwait(false);
            while (!cancelToken.IsCancellationRequested)
            {
                long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                //Did server respond to our last heartbeat, or are we still receiving messages (long load?)
                if (_heartbeatTimes.IsEmpty && now - _lastMessageTime > intervalMillis + 1000.0 / 64)
                    if (ConnectionState == ConnectionState.Connected && (_guildDownloadTask?.IsCompleted ?? true))
                    {
                        _connection.Error(new GatewayReconnectException("Server missed last heartbeat"));
                        return;
                    }

                _heartbeatTimes.Enqueue(now);
                try
                {
                    await ApiClient.SendHeartbeatAsync(_lastSeq).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await _gatewayLogger.WarningAsync("Heartbeat Errored", ex).ConfigureAwait(false);
                }

                await Task.Delay(intervalMillis, cancelToken).ConfigureAwait(false);
            }

            await _gatewayLogger.DebugAsync("Heartbeat Stopped").ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            await _gatewayLogger.DebugAsync("Heartbeat Stopped").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _gatewayLogger.ErrorAsync("Heartbeat Errored", ex).ConfigureAwait(false);
        }
    }

    private async Task WaitForGuildsAsync(CancellationToken cancelToken, Logger logger)
    {
        //Wait for GUILD_AVAILABLEs
        try
        {
            await logger.DebugAsync("GuildDownloader Started").ConfigureAwait(false);
            while (_unavailableGuildCount != 0
                   && DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _lastGuildAvailableTime < _baseConfig.MaxWaitBetweenGuildAvailablesBeforeReady)
                await Task.Delay(500, cancelToken).ConfigureAwait(false);

            await logger.DebugAsync("GuildDownloader Stopped").ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            await logger.DebugAsync("GuildDownloader Stopped").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await logger.ErrorAsync("GuildDownloader Errored", ex).ConfigureAwait(false);
        }
    }

    internal SocketGuild AddGuild(Guild model, ClientState state)
    {
        SocketGuild guild = SocketGuild.Create(this, state, model);
        state.AddGuild(guild);
        return guild;
    }

    internal SocketGuild AddGuild(RichGuild model, ClientState state)
    {
        SocketGuild guild = SocketGuild.Create(this, state, model);
        state.AddGuild(guild);
        return guild;
    }

    internal SocketGuild RemoveGuild(ulong id)
        => State.RemoveGuild(id);

    /// <exception cref="InvalidOperationException">Unexpected channel type is created.</exception>
    internal ISocketPrivateChannel AddDMChannel(UserChat model, ClientState state)
    {
        SocketDMChannel channel = SocketDMChannel.Create(this, state, model.Code, model.Recipient);
        state.AddDMChannel(channel);
        return channel;
    }

    internal SocketDMChannel CreateDMChannel(Guid chatCode, User model, ClientState state) => SocketDMChannel.Create(this, state, chatCode, model);
    internal SocketDMChannel CreateDMChannel(Guid chatCode, SocketUser user, ClientState state) => new(this, chatCode, user);

    private async Task GuildAvailableAsync(SocketGuild guild)
    {
        if (!guild.IsConnected)
        {
            guild.IsConnected = true;
            await TimedInvokeAsync(_guildAvailableEvent, nameof(GuildAvailable), guild).ConfigureAwait(false);
        }
    }

    private async Task GuildUnavailableAsync(SocketGuild guild)
    {
        if (guild.IsConnected)
        {
            guild.IsConnected = false;
            await TimedInvokeAsync(_guildUnavailableEvent, nameof(GuildUnavailable), guild).ConfigureAwait(false);
        }
    }

    private async Task TimedInvokeAsync(AsyncEvent<Func<Task>> eventHandler, string name)
    {
        if (eventHandler.HasSubscribers)
        {
            if (HandlerTimeout.HasValue)
                await TimeoutWrap(name, eventHandler.InvokeAsync).ConfigureAwait(false);
            else
                await eventHandler.InvokeAsync().ConfigureAwait(false);
        }
    }

    private async Task TimedInvokeAsync<T>(AsyncEvent<Func<T, Task>> eventHandler, string name, T arg)
    {
        if (eventHandler.HasSubscribers)
        {
            if (HandlerTimeout.HasValue)
                await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg)).ConfigureAwait(false);
            else
                await eventHandler.InvokeAsync(arg).ConfigureAwait(false);
        }
    }

    private async Task TimedInvokeAsync<T1, T2>(AsyncEvent<Func<T1, T2, Task>> eventHandler, string name, T1 arg1,
        T2 arg2)
    {
        if (eventHandler.HasSubscribers)
        {
            if (HandlerTimeout.HasValue)
                await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2)).ConfigureAwait(false);
            else
                await eventHandler.InvokeAsync(arg1, arg2).ConfigureAwait(false);
        }
    }

    private async Task TimedInvokeAsync<T1, T2, T3>(AsyncEvent<Func<T1, T2, T3, Task>> eventHandler, string name,
        T1 arg1, T2 arg2, T3 arg3)
    {
        if (eventHandler.HasSubscribers)
        {
            if (HandlerTimeout.HasValue)
                await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2, arg3)).ConfigureAwait(false);
            else
                await eventHandler.InvokeAsync(arg1, arg2, arg3).ConfigureAwait(false);
        }
    }

    private async Task TimedInvokeAsync<T1, T2, T3, T4>(AsyncEvent<Func<T1, T2, T3, T4, Task>> eventHandler,
        string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (eventHandler.HasSubscribers)
        {
            if (HandlerTimeout.HasValue)
                await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2, arg3, arg4)).ConfigureAwait(false);
            else
                await eventHandler.InvokeAsync(arg1, arg2, arg3, arg4).ConfigureAwait(false);
        }
    }

    private async Task TimedInvokeAsync<T1, T2, T3, T4, T5>(AsyncEvent<Func<T1, T2, T3, T4, T5, Task>> eventHandler,
        string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        if (eventHandler.HasSubscribers)
        {
            if (HandlerTimeout.HasValue)
                await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2, arg3, arg4, arg5))
                    .ConfigureAwait(false);
            else
                await eventHandler.InvokeAsync(arg1, arg2, arg3, arg4, arg5).ConfigureAwait(false);
        }
    }

    private async Task TimeoutWrap(string name, Func<Task> action)
    {
        try
        {
            Task timeoutTask = Task.Delay(HandlerTimeout.Value);
            Task handlersTask = action();
            if (await Task.WhenAny(timeoutTask, handlersTask).ConfigureAwait(false) == timeoutTask)
                await _gatewayLogger.WarningAsync($"A {name} handler is blocking the gateway task.")
                    .ConfigureAwait(false);

            await handlersTask.ConfigureAwait(false); //Ensure the handler completes
        }
        catch (Exception ex)
        {
            await _gatewayLogger.WarningAsync($"A {name} handler has thrown an unhandled exception.", ex)
                .ConfigureAwait(false);
        }
    }

    private async Task UnknownChannelUserAsync(string evnt, ulong userId, Guid chatCode)
    {
        string details = $"{evnt} User={userId} ChatCode={chatCode}";
        await _gatewayLogger.WarningAsync($"Unknown User ({details}).").ConfigureAwait(false);
    }

    private async Task UnknownGlobalUserAsync(string evnt, ulong userId)
    {
        string details = $"{evnt} User={userId}";
        await _gatewayLogger.WarningAsync($"Unknown User ({details}).").ConfigureAwait(false);
    }

    private async Task UnknownChannelUserAsync(string evnt, ulong userId, ulong channelId)
    {
        string details = $"{evnt} User={userId} Channel={channelId}";
        await _gatewayLogger.WarningAsync($"Unknown User ({details}).").ConfigureAwait(false);
    }

    private async Task UnknownGuildUserAsync(string evnt, ulong userId, ulong guildId)
    {
        string details = $"{evnt} User={userId} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unknown User ({details}).").ConfigureAwait(false);
    }

    private async Task IncompleteGuildUserAsync(string evnt, ulong userId, ulong guildId)
    {
        string details = $"{evnt} User={userId} Guild={guildId}";
        await _gatewayLogger.DebugAsync($"User has not been downloaded ({details}).").ConfigureAwait(false);
    }

    private async Task UnknownPrivateChannelAsync(string evnt, Guid chatCode)
    {
        string details = $"{evnt} Channel={chatCode}";
        await _gatewayLogger.WarningAsync($"Unknown Private Channel ({details}).").ConfigureAwait(false);
    }

    private async Task UnknownChannelAsync(string evnt, ulong channelId)
    {
        string details = $"{evnt} Channel={channelId}";
        await _gatewayLogger.WarningAsync($"Unknown Channel ({details}).").ConfigureAwait(false);
    }

    private async Task UnknownChannelAsync(string evnt, ulong channelId, ulong guildId)
    {
        if (guildId == 0)
        {
            await UnknownChannelAsync(evnt, channelId).ConfigureAwait(false);
            return;
        }

        string details = $"{evnt} Channel={channelId} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unknown Channel ({details}).").ConfigureAwait(false);
    }

    private async Task UnknownRoleAsync(string evnt, ulong roleId, ulong guildId)
    {
        string details = $"{evnt} Role={roleId} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unknown Role ({details}).").ConfigureAwait(false);
    }

    private async Task UnknownGuildAsync(string evnt, ulong guildId)
    {
        string details = $"{evnt} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unknown Guild ({details}).").ConfigureAwait(false);
    }

    private async Task UnknownGuildEventAsync(string evnt, ulong eventId, ulong guildId)
    {
        string details = $"{evnt} Event={eventId} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unknown Guild Event ({details}).").ConfigureAwait(false);
    }

    private async Task UnsyncedGuildAsync(string evnt, ulong guildId)
    {
        string details = $"{evnt} Guild={guildId}";
        await _gatewayLogger.DebugAsync($"Unsynced Guild ({details}).").ConfigureAwait(false);
    }

    #region IKookClient

    /// <inheritdoc />
    Task<IReadOnlyCollection<IGuild>> IKookClient.GetGuildsAsync(CacheMode mode, RequestOptions options)
        => Task.FromResult<IReadOnlyCollection<IGuild>>(Guilds);

    /// <inheritdoc />
    Task<IGuild> IKookClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions options)
        => Task.FromResult<IGuild>(GetGuild(id));

    /// <inheritdoc />
    async Task<IUser> IKookClient.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
    {
        SocketUser user = GetUser(id);
        if (user is not null || mode == CacheMode.CacheOnly) return user;

        return await Rest.GetUserAsync(id, options).ConfigureAwait(false);
    }

    /// <inheritdoc />
    async Task<IChannel> IKookClient.GetChannelAsync(ulong id, CacheMode mode, RequestOptions options)
        => mode == CacheMode.AllowDownload ? await GetChannelAsync(id, options).ConfigureAwait(false) : GetChannel(id);

    /// <inheritdoc />
    async Task<IDMChannel> IKookClient.GetDMChannelAsync(Guid chatCode, CacheMode mode, RequestOptions options)
        => mode == CacheMode.AllowDownload ? await GetDMChannelAsync(chatCode, options).ConfigureAwait(false) : GetDMChannel(chatCode);

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IDMChannel>> IKookClient.GetDMChannelsAsync(CacheMode mode, RequestOptions options)
        => mode == CacheMode.AllowDownload ? await GetDMChannelsAsync(options).ConfigureAwait(false) : DMChannels;

    #endregion
}
