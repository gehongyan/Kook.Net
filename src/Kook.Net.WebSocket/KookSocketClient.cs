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
using Kook.Net.Converters;
using Kook.Net.Queue;
using Kook.Net.Udp;
using Kook.Net.WebSockets;
using Kook.Rest;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based KOOK client.
/// </summary>
public partial class KookSocketClient : BaseSocketClient, IKookClient
{
    #region KookSocketClient

    private const string MessageTypeGroup = "GROUP";
    private const string MessageTypePerson = "PERSON";

    private readonly JsonSerializerOptions _serializerOptions;

    private readonly ConcurrentQueue<long> _heartbeatTimes;
    private readonly Logger _gatewayLogger;
    private readonly SemaphoreSlim _stateLock;

    private Guid? _sessionId;
    private int _lastSeq;
    private int _retryCount;
    internal Task? _heartbeatTask;
    private Task? _guildDownloadTask;
    private long _lastMessageTime;
    private int _nextAudioId;

    private bool _isDisposed;

    /// <inheritdoc />
    public override KookSocketRestClient Rest { get; }

    internal virtual ConnectionManager Connection { get; }

    /// <inheritdoc />
    public override ConnectionState ConnectionState => Connection.State;

    /// <inheritdoc />
    public override int Latency { get; protected set; }

    #endregion

    // From KookSocketConfig
    internal int MessageCacheSize { get; private set; }
    internal ClientState State { get; private set; }
    internal UdpSocketProvider UdpSocketProvider { get; private set; }
    internal WebSocketProvider WebSocketProvider { get; private set; }
    internal BaseMessageQueue MessageQueue { get; private set; }
    internal uint SmallNumberOfGuildsThreshold { get; private set; }
    internal uint LargeNumberOfGuildsThreshold { get; private set; }
    internal StartupCacheFetchMode StartupCacheFetchMode { get; private set; }
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
    public IReadOnlyCollection<SocketDMChannel> DMChannels =>
        State.DMChannels.Where(x => x is not null).ToImmutableArray();

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

    internal KookSocketClient(KookSocketConfig config, KookSocketApiClient client)
        : base(config, client)
    {
        MessageCacheSize = config.MessageCacheSize;
        UdpSocketProvider = config.UdpSocketProvider;
        WebSocketProvider = config.WebSocketProvider;
        MessageQueue = config.MessageQueueProvider(ProcessGatewayEventAsync);
        SmallNumberOfGuildsThreshold = config.SmallNumberOfGuildsThreshold;
        LargeNumberOfGuildsThreshold = config.LargeNumberOfGuildsThreshold;
        // StartupCacheFetchMode will be set to the current config value whenever the socket client starts up
        StartupCacheFetchMode = config.StartupCacheFetchMode;
        AlwaysDownloadUsers = config.AlwaysDownloadUsers;
        AlwaysDownloadVoiceStates = config.AlwaysDownloadVoiceStates;
        AlwaysDownloadBoostSubscriptions = config.AlwaysDownloadBoostSubscriptions;
        HandlerTimeout = config.HandlerTimeout;
        State = new ClientState(0, 0);
        Rest = new KookSocketRestClient(config, ApiClient);
        _heartbeatTimes = new ConcurrentQueue<long>();

        _stateLock = new SemaphoreSlim(1, 1);
        _gatewayLogger = LogManager.CreateLogger("Gateway");
        ConnectionManager connectionManager = new(_stateLock, _gatewayLogger, config.ConnectionTimeout,
            OnConnectingAsync, OnDisconnectingAsync, x => ApiClient.Disconnected += x);
        connectionManager.Connected += () => TimedInvokeAsync(_connectedEvent, nameof(Connected));
        connectionManager.Disconnected += (ex, _) => TimedInvokeAsync(_disconnectedEvent, nameof(Disconnected), ex);
        Connection = connectionManager;

        _serializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters = { CardConverterFactory.Instance }
        };

        ApiClient.SentGatewayMessage += async socketFrameType =>
            await _gatewayLogger.DebugAsync($"Sent {socketFrameType}").ConfigureAwait(false);
        ApiClient.ReceivedGatewayEvent += ProcessMessageAsync;

        LeftGuild += async g => await _gatewayLogger.InfoAsync($"Left {g.Name}").ConfigureAwait(false);
        JoinedGuild += async g => await _gatewayLogger.InfoAsync($"Joined {g.Name}").ConfigureAwait(false);
        GuildAvailable += async g => await _gatewayLogger.VerboseAsync($"Connected to {g.Name}").ConfigureAwait(false);
        GuildUnavailable += async g => await _gatewayLogger.VerboseAsync($"Disconnected from {g.Name}").ConfigureAwait(false);
        LatencyUpdated += async (_, val) => await _gatewayLogger.DebugAsync($"Latency = {val} ms").ConfigureAwait(false);

        GuildAvailable += g =>
        {
            if (_guildDownloadTask?.IsCompleted is true
                && ConnectionState == ConnectionState.Connected)
            {
                if (AlwaysDownloadUsers && g.HasAllMembers is not true)
                    _ = g.DownloadUsersAsync();
                if (AlwaysDownloadVoiceStates)
                    _ = g.DownloadVoiceStatesAsync();
                if (AlwaysDownloadBoostSubscriptions)
                    _ = g.DownloadBoostSubscriptionsAsync();
            }

            return Task.CompletedTask;
        };
    }

    private static KookSocketApiClient CreateApiClient(KookSocketConfig config) =>
        new(config.RestClientProvider, config.WebSocketProvider, KookConfig.UserAgent,
            config.AcceptLanguage, config.GatewayHost, defaultRatelimitCallback: config.DefaultRatelimitCallback);

    internal override void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                try
                {
                    StopAsync().GetAwaiter().GetResult();
                }
                catch (NotSupportedException)
                {
                    // ignored
                }
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
                Connection.CriticalError(ex);
            else
                Connection.Error(ex);
        }
        catch
        {
            // ignored
        }

        await Connection.WaitAsync().ConfigureAwait(false);
    }

    private async Task OnDisconnectingAsync(Exception ex)
    {
        await _gatewayLogger.DebugAsync("Disconnecting ApiClient").ConfigureAwait(false);
        await ApiClient.DisconnectAsync(ex).ConfigureAwait(false);

        //Wait for tasks to complete
        await _gatewayLogger.DebugAsync("Waiting for heartbeater").ConfigureAwait(false);
        Task? heartbeatTask = _heartbeatTask;
        if (heartbeatTask != null)
            await heartbeatTask.ConfigureAwait(false);
        _heartbeatTask = null;
        while (_heartbeatTimes.TryDequeue(out _))
        {
            // flush the queue
        }

        ResetCounter();

        //Raise virtual GUILD_UNAVAILABLEs
        await _gatewayLogger.DebugAsync("Raising virtual GuildUnavailables").ConfigureAwait(false);
        foreach (SocketGuild guild in State.Guilds)
            if (guild.IsAvailable)
                await GuildUnavailableAsync(guild).ConfigureAwait(false);
    }

    private protected void ResetCounter()
    {
        _lastMessageTime = 0;
    }

    /// <inheritdoc />
    public override SocketGuild? GetGuild(ulong id) => State.GetGuild(id);

    /// <inheritdoc />
    public override SocketChannel? GetChannel(ulong id) => State.GetChannel(id);

    /// <inheritdoc />
    public override SocketDMChannel? GetDMChannel(Guid chatCode) => State.GetDMChannel(chatCode);

    /// <inheritdoc />
    public override SocketDMChannel? GetDMChannel(ulong userId) => State.GetDMChannel(userId);

    /// <summary>
    ///     Gets a generic channel from the cache or does a rest request if unavailable.
    /// </summary>
    /// <param name="id">The identifier of the channel.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the channel associated
    ///     with the identifier; <c>null</c> when the channel cannot be found.
    /// </returns>
    public async Task<IChannel> GetChannelAsync(ulong id, RequestOptions? options = null)
    {
        if (GetChannel(id) is { } channel) return channel;
        return await ClientHelper.GetChannelAsync(this, id, options).ConfigureAwait(false);
    }

    /// <summary>
    ///     Gets a direct message channel from the cache or does a rest request if unavailable.
    /// </summary>
    /// <param name="chatCode">The identifier of the channel.</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the channel associated
    ///     with the identifier; <c>null</c> when the channel cannot be found.
    /// </returns>
    public async Task<IDMChannel> GetDMChannelAsync(Guid chatCode, RequestOptions? options = null) =>
        await ClientHelper.GetDMChannelAsync(this, chatCode, options).ConfigureAwait(false);

    /// <summary>
    ///     Gets a collection of direct message channels from the cache or does a rest request if unavailable.
    /// </summary>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the channel associated
    ///     with the identifier; <c>null</c> when the channel cannot be found.
    /// </returns>
    public async Task<IReadOnlyCollection<IDMChannel>> GetDMChannelsAsync(RequestOptions? options = null) =>
        (await ClientHelper.GetDMChannelsAsync(this, options).ConfigureAwait(false)).ToImmutableArray();

    /// <summary>
    ///     Gets a user from the cache or does a rest request if unavailable.
    /// </summary>
    /// <param name="id">The identifier of the user (e.g. `168693960628371456`).</param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the user associated with
    ///     the identifier; <c>null</c> if the user is not found.
    /// </returns>
    public async Task<IUser> GetUserAsync(ulong id, RequestOptions? options = null)
    {
        if (GetUser(id) is { } user) return user;
        return await Rest.GetUserAsync(id, options).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override SocketUser? GetUser(ulong id) => State.GetUser(id);

    /// <inheritdoc />
    public override SocketUser? GetUser(string username, string identifyNumber) =>
        State.Users.FirstOrDefault(x => x.IdentifyNumber == identifyNumber && x.Username == username);

    internal SocketGlobalUser GetOrCreateUser(ClientState state, User model) =>
        state.GetOrAddUser(model.Id, _ => SocketGlobalUser.Create(this, state, model));

    internal SocketUser GetOrCreateTemporaryUser(ClientState state, User model) =>
        state.GetUser(model.Id) ?? (SocketUser)SocketUnknownUser.Create(this, state, model.Id);

    internal SocketGlobalUser GetOrCreateSelfUser(ClientState state, User model) =>
        state.GetOrAddUser(model.Id, _ =>
        {
            SocketGlobalUser user = SocketGlobalUser.Create(this, state, model);
            user.GlobalUser.AddRef();
            return user;
        });

    internal void RemoveUser(ulong id) => State.RemoveUser(id);

    /// <summary>
    ///     Downloads all users for the specified guilds.
    /// </summary>
    /// <param name="guilds">
    ///     The guilds to download the users for. If <c>null</c>, all available guilds will be downloaded.
    /// </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public override async Task DownloadUsersAsync(IEnumerable<IGuild>? guilds = null, RequestOptions? options = null)
    {
        if (ConnectionState != ConnectionState.Connected) return;
        IEnumerable<SocketGuild> socketGuilds = (guilds ?? Guilds.Where(x => x.IsAvailable))
            .Select(x => GetGuild(x.Id))
            .OfType<SocketGuild>();
        await ProcessUserDownloadsAsync(socketGuilds, options).ConfigureAwait(false);
    }

    private async Task ProcessUserDownloadsAsync(IEnumerable<SocketGuild> guilds, RequestOptions? options)
    {
        foreach (SocketGuild socketGuild in guilds)
        {
            if (options?.CancellationToken.IsCancellationRequested is true) return;
            IEnumerable<GuildMember> guildMembers = await ApiClient
                .GetGuildMembersAsync(socketGuild.Id, options: options)
                .FlattenAsync()
                .ConfigureAwait(false);
            socketGuild.Update(State, [..guildMembers]);
        }
    }

    /// <summary>
    ///     Downloads all voice states for the specified guilds.
    /// </summary>
    /// <param name="guilds">
    ///     The guilds to download the voice states for. If <c>null</c>, all available guilds will be downloaded.
    /// </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public override async Task DownloadVoiceStatesAsync(IEnumerable<IGuild>? guilds = null,
        RequestOptions? options = null)
    {
        if (ConnectionState != ConnectionState.Connected) return;
        IEnumerable<SocketGuild> socketGuilds = (guilds ?? Guilds.Where(x => x.IsAvailable))
            .Select(x => GetGuild(x.Id))
            .OfType<SocketGuild>();
        await ProcessVoiceStateDownloadsAsync(socketGuilds, options).ConfigureAwait(false);
    }

    private async Task ProcessVoiceStateDownloadsAsync(IEnumerable<SocketGuild> guilds, RequestOptions? options)
    {
        foreach (SocketGuild socketGuild in guilds)
        {
            foreach (ulong channelId in socketGuild.VoiceChannels.Select(x => x.Id))
            {
                if (options?.CancellationToken.IsCancellationRequested is true) return;
                IReadOnlyCollection<User> users = await ApiClient
                    .GetConnectedUsersAsync(channelId, options)
                    .ConfigureAwait(false);
                foreach (User user in users)
                    socketGuild.AddOrUpdateVoiceState(user.Id, channelId);
            }

            GetGuildMuteDeafListResponse model = await ApiClient
                .GetGuildMutedDeafenedUsersAsync(socketGuild.Id)
                .ConfigureAwait(false);
            foreach (ulong id in model.Muted.UserIds)
                socketGuild.AddOrUpdateVoiceState(id, true);
            foreach (ulong id in socketGuild.Users.Select(x => x.Id).Except(model.Deafened.UserIds))
                socketGuild.AddOrUpdateVoiceState(id, false);
            foreach (ulong id in model.Deafened.UserIds)
                socketGuild.AddOrUpdateVoiceState(id, isDeafened: true);
            foreach (ulong id in socketGuild.Users.Select(x => x.Id).Except(model.Muted.UserIds))
                socketGuild.AddOrUpdateVoiceState(id, isDeafened: false);
        }
    }

    /// <summary>
    ///     Downloads all boost subscriptions for the specified guilds.
    /// </summary>
    /// <param name="guilds">
    ///     The guilds to download the boost subscriptions for. If <c>null</c>, all available guilds will be downloaded.
    ///     To download all boost subscriptions, the current user must has the
    ///     <see cref="GuildPermission.ManageGuild"/> permission.
    /// </param>
    /// <param name="options"> 发送请求时要使用的选项。 </param>
    public override async Task DownloadBoostSubscriptionsAsync(IEnumerable<IGuild>? guilds = null,
        RequestOptions? options = null)
    {
        if (ConnectionState != ConnectionState.Connected) return;
        IEnumerable<SocketGuild> socketGuilds = (guilds ?? Guilds.Where(x => x.IsAvailable))
            .Select(x => GetGuild(x.Id))
            .OfType<SocketGuild>();
        await ProcessBoostSubscriptionsDownloadsAsync(socketGuilds, options).ConfigureAwait(false);
    }

    private async Task ProcessBoostSubscriptionsDownloadsAsync(IEnumerable<SocketGuild> guilds, RequestOptions? options)
    {
        foreach (SocketGuild socketGuild in guilds)
        {
            if (options?.CancellationToken.IsCancellationRequested is true) return;
            IEnumerable<BoostSubscription> subscriptions = await ApiClient
                .GetGuildBoostSubscriptionsAsync(socketGuild.Id, options: options)
                .FlattenAsync()
                .ConfigureAwait(false);
            socketGuild.Update(State, [..subscriptions]);
        }
    }

    #region ProcessMessageAsync

    /// <summary>
    ///     Processes a message received from the gateway.
    /// </summary>
    /// <param name="gatewaySocketFrameType"> The type of the gateway socket frame. </param>
    /// <param name="sequence"> The sequence number of the message. </param>
    /// <param name="payload"> The payload of the message. </param>
    /// <exception cref="InvalidOperationException"> Unknown event type. </exception>
    internal virtual async Task ProcessMessageAsync(GatewaySocketFrameType gatewaySocketFrameType, int? sequence, JsonElement payload)
    {
        if (sequence.HasValue)
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
                    await MessageQueue.EnqueueAsync(payload, sequence ?? _lastSeq).ConfigureAwait(false);
                    break;
                case GatewaySocketFrameType.Hello:
                    await HandleGatewayHelloAsync(payload).ConfigureAwait(false);
                    break;
                case GatewaySocketFrameType.Pong:
                    await HandlePongAsync().ConfigureAwait(false);
                    break;
                case GatewaySocketFrameType.Reconnect:
                    await HandleReconnectAsync(payload).ConfigureAwait(false);
                    break;
                case GatewaySocketFrameType.ResumeAck:
                    await HandleResumeAckAsync().ConfigureAwait(false);
                    break;
                default:
                {
                    await _gatewayLogger
                        .WarningAsync($"Unknown Socket Frame Type ({gatewaySocketFrameType}). Payload: {SerializePayload(payload)}")
                        .ConfigureAwait(false);
                }
                    break;
            }
        }
        catch (Exception ex)
        {
            await _gatewayLogger
                .ErrorAsync($"Error handling {gatewaySocketFrameType}. Payload: {SerializePayload(payload)}", ex)
                .ConfigureAwait(false);
        }
    }

    internal async Task ProcessGatewayEventAsync(JsonElement payload)
    {
        if (!payload.TryGetProperty("type", out JsonElement typeProperty)
            || !typeProperty.TryGetInt32(out int typeValue)
            || !Enum.IsDefined(typeof(MessageType), typeValue))
        {
            await _gatewayLogger
                .WarningAsync($"Unknown Event Type. Payload: {SerializePayload(payload)}")
                .ConfigureAwait(false);
            return;
        }

        if (!payload.TryGetProperty("channel_type", out JsonElement channelTypeProperty)
            || channelTypeProperty.GetString() is not { } channelType)
        {
            await _gatewayLogger
                .WarningAsync($"Unknown Channel Type. Payload: {SerializePayload(payload)}")
                .ConfigureAwait(false);
            return;
        }

        switch ((MessageType)typeValue)
        {
            case MessageType.Text:
            case MessageType.Image:
            case MessageType.Video:
            case MessageType.File:
            case MessageType.Audio:
            case MessageType.KMarkdown:
            case MessageType.Card:
            case MessageType.Poke:
            {
                await _gatewayLogger
                    .DebugAsync($"Received Message ({channelType}, {(MessageType)typeValue})")
                    .ConfigureAwait(false);

                switch (channelType)
                {
                    case MessageTypeGroup:
                    {
                        GatewayEvent<GatewayGroupMessageExtraData>? gatewayEvent =
                            payload.Deserialize<GatewayEvent<GatewayGroupMessageExtraData>>(_serializerOptions);
                        if (gatewayEvent is null)
                        {
                            await _gatewayLogger
                                .WarningAsync($"Unable to deserialize System Group Message. Payload: {SerializePayload(payload)}")
                                .ConfigureAwait(false);
                            break;
                        }
                        await HandleGroupMessage(gatewayEvent).ConfigureAwait(false);
                    }
                        break;
                    case MessageTypePerson:
                    {
                        GatewayEvent<GatewayPersonMessageExtraData>? gatewayEvent =
                            payload.Deserialize<GatewayEvent<GatewayPersonMessageExtraData>>(_serializerOptions);
                        if (gatewayEvent is null)
                        {
                            await _gatewayLogger
                                .WarningAsync($"Unable to deserialize System Person Message. Payload: {SerializePayload(payload)}")
                                .ConfigureAwait(false);
                            break;
                        }
                        await HandlePersonMessage(gatewayEvent).ConfigureAwait(false);
                    }
                        break;
                    default:
                    {
                        await _gatewayLogger
                            .WarningAsync($"Unknown Channel Type ({channelType}). Payload: {SerializePayload(payload)}")
                            .ConfigureAwait(false);
                    }
                        break;
                }
            }
                break;
            case MessageType.System:
            {
                GatewayEvent<GatewaySystemEventExtraData>? gatewayEvent =
                    payload.Deserialize<GatewayEvent<GatewaySystemEventExtraData>>(_serializerOptions);
                if (gatewayEvent is not { ExtraData: { } extraData })
                {
                    await _gatewayLogger
                        .WarningAsync($"Unable to deserialize System Event. Payload: {SerializePayload(payload)}")
                        .ConfigureAwait(false);
                    break;
                }
                await _gatewayLogger
                    .DebugAsync($"Received Event ({channelType}, {extraData.Type})")
                    .ConfigureAwait(false);
                switch (channelType, extraData.Type)
                {
                    #region Channels

                    // 频道内用户添加 reaction
                    case (MessageTypeGroup, "added_reaction"):
                        await HandleAddedReaction(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 频道内用户取消 reaction
                    case (MessageTypeGroup, "deleted_reaction"):
                        await HandleDeletedReaction(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 频道消息更新
                    case (MessageTypeGroup, "updated_message"):
                        await HandleUpdatedMessage(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 频道消息附加 Embeds
                    case (MessageTypeGroup, "embeds_append"):
                        await HandleEmbedsAppend(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 频道消息被删除
                    case (MessageTypeGroup, "deleted_message"):
                        await HandleDeletedMessage(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 新增频道
                    case (MessageTypeGroup, "added_channel"):
                        await HandleAddedChannel(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 修改频道信息 & 修改语音频道服务器区域
                    case (MessageTypeGroup, "updated_channel"):
                    case (MessageTypeGroup, "updated_server_type"):
                        await HandleUpdatedChannel(gatewayEvent).ConfigureAwait(false);
                        break;
                    case (MessageTypeGroup, "sort_channel"):
                        await HandleSortChannel(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 删除频道
                    case (MessageTypeGroup, "deleted_channel"):
                        await HandleDeletedChannel(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 批量创建频道
                    case (MessageTypeGroup, "batch_added_channel"):
                        await HandleBatchAddChannel(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 批量操作频道
                    case (MessageTypeGroup, "batch_updated_channel"):
                        await HandleBatchUpdateChannel(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 批量删除频道
                    case (MessageTypeGroup, "batch_deleted_channel"):
                        await HandleBatchDeleteChannel(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 新的频道置顶消息
                    case (MessageTypeGroup, "pinned_message"):
                        await HandlePinnedMessage(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 取消频道置顶消息
                    case (MessageTypeGroup, "unpinned_message"):
                        await HandleUnpinnedMessage(gatewayEvent).ConfigureAwait(false);
                        break;

                    #endregion

                    #region Direct Messages

                    // 私聊消息更新
                    case (MessageTypePerson, "updated_private_message"):
                        await HandleUpdatedPrivateMessage(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 私聊消息被删除
                    case (MessageTypePerson, "deleted_private_message"):
                        await HandleDeletedPrivateMessage(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 私聊内用户添加 reaction
                    case (MessageTypePerson, "private_added_reaction"):
                        await HandlePrivateAddedReaction(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 私聊内用户取消 reaction
                    case (MessageTypePerson, "private_deleted_reaction"):
                        await HandlePrivateDeletedReaction(gatewayEvent).ConfigureAwait(false);
                        break;

                    #endregion

                    #region Guild Members

                    // 新成员加入服务器
                    case (MessageTypeGroup, "joined_guild"):
                        await HandleJoinedGuild(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 服务器成员退出
                    case (MessageTypeGroup, "exited_guild"):
                        await HandleExitedGuild(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 服务器成员信息更新
                    case (MessageTypeGroup, "updated_guild_member"):
                        await HandleUpdatedGuildMember(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 服务器成员上线
                    case (MessageTypePerson, "guild_member_online"):
                        await HandleGuildMemberOnline(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 服务器成员下线
                    case (MessageTypePerson, "guild_member_offline"):
                        await HandleGuildMemberOffline(gatewayEvent).ConfigureAwait(false);
                        break;

                    #endregion

                    #region Guild Roles

                    // 服务器角色增加
                    case (MessageTypeGroup, "added_role"):
                        await HandleAddedRole(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 服务器角色删除
                    case (MessageTypeGroup, "deleted_role"):
                        await HandleDeletedRole(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 服务器角色更新
                    case (MessageTypeGroup, "updated_role"):
                        await HandleUpdatedRole(gatewayEvent).ConfigureAwait(false);
                        break;

                    #endregion

                    #region Guild Emojis

                    // 服务器表情新增
                    case (MessageTypeGroup, "added_emoji"):
                        await HandleAddedRmoji(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 服务器表情更新
                    case (MessageTypeGroup, "updated_emoji"):
                        await HandleUpdatedEmoji(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 服务器表情删除
                    case (MessageTypeGroup, "deleted_emoji"):
                        await HandleDeletedEmoji(gatewayEvent).ConfigureAwait(false);
                        break;

                    #endregion

                    #region Guilds

                    // 服务器信息更新
                    case (MessageTypeGroup, "updated_guild"):
                        await HandleUpdatedGuild(gatewayEvent).ConfigureAwait(false);
                        break;
                    case (MessageTypePerson, "updated_guild"):
                        await HandleUpdatedGuildSelf(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 服务器删除
                    case (MessageTypeGroup, "deleted_guild"):
                        await HandleDeletedGuild(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 服务器封禁用户
                    case (MessageTypeGroup, "added_block_list"):
                        await HandleAddedBlockList(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 服务器取消封禁用户
                    case (MessageTypeGroup, "deleted_block_list"):
                        await HandleDeletedBlockList(gatewayEvent).ConfigureAwait(false);
                        break;

                    #endregion

                    #region Users

                    // 用户加入语音频道
                    case (MessageTypeGroup, "joined_channel"):
                        await HandleJoinedChannel(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 用户退出语音频道
                    case (MessageTypeGroup, "exited_channel"):
                        await HandleExitedChannel(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 用户直播状态更新
                    case (MessageTypeGroup, "live_status_changed"):
                        await HandleLiveStatusChanged(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 用户被添加服务器闭麦/静音
                    case (MessageTypeGroup, "add_guild_mute"):
                        await HandleAddGuildMute(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 用户被删除服务器闭麦/静音
                    case (MessageTypeGroup, "delete_guild_mute"):
                        await HandleDeleteGuildMute(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 用户信息更新
                    case (MessageTypePerson, "user_updated"):
                        await HandleUserUpdated(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 自己新加入服务器
                    case (MessageTypePerson, "self_joined_guild"):
                        await HandleSelfJoinedGuild(gatewayEvent).ConfigureAwait(false);
                        break;
                    // 自己退出服务器
                    case (MessageTypePerson, "self_exited_guild"):
                        await HandleSelfExitedGuild(gatewayEvent).ConfigureAwait(false);
                        break;

                    #endregion

                    #region Interactions

                    case (MessageTypePerson, "message_btn_click"):
                        await HandleMessageButtonClick(gatewayEvent).ConfigureAwait(false);
                        break;

                    #endregion

                    default:
                    {
                        await _gatewayLogger
                            .WarningAsync($"Unknown SystemEventType ({channelType}, {extraData.Type}). Payload: {SerializePayload(payload)}")
                            .ConfigureAwait(false);
                    }
                        break;
                }
            }
                break;
            default:
            {
                await _gatewayLogger
                    .WarningAsync($"Unknown Event Type ({typeValue}). Payload: {SerializePayload(payload)}")
                    .ConfigureAwait(false);
            }
                break;
        }

    }

    #endregion

    /// <inheritdoc />
    public override async Task StartAsync()
    {
        await MessageQueue.StartAsync();
        await Connection.StartAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task StopAsync()
    {
        await Connection.StopAsync().ConfigureAwait(false);
        await MessageQueue.StopAsync();
    }

    private async Task RunHeartbeatAsync(CancellationToken cancellationToken)
    {
        int intervalMillis = BaseConfig.HeartbeatIntervalMilliseconds;
        try
        {
            await _gatewayLogger.DebugAsync("Heartbeat Started").ConfigureAwait(false);
            while (!cancellationToken.IsCancellationRequested)
            {
                long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                //Did server respond to our last heartbeat, or are we still receiving messages (long load?)
                if (_heartbeatTimes.IsEmpty && now - _lastMessageTime > intervalMillis + 1000.0 / 64
                    && ConnectionState == ConnectionState.Connected && (_guildDownloadTask?.IsCompleted ?? true))
                {
                    Connection.Error(new GatewayReconnectException("Server missed last heartbeat"));
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

                await Task.Delay(intervalMillis, cancellationToken).ConfigureAwait(false);
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

    internal SocketGuild AddGuild(ExtendedGuild model, ClientState state)
    {
        SocketGuild guild = SocketGuild.Create(this, state, model);
        state.AddGuild(guild);
        return guild;
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

    internal SocketGuild? RemoveGuild(ulong id) => State.RemoveGuild(id);

    internal SocketDMChannel AddDMChannel(UserChat model, ClientState state)
    {
        SocketDMChannel channel = SocketDMChannel.Create(this, state, model.Code, model.Recipient);
        state.AddDMChannel(channel);
        return channel;
    }

    internal SocketDMChannel AddDMChannel(Guid chatCode, User model, ClientState state)
    {
        SocketDMChannel channel = SocketDMChannel.Create(this, state, chatCode, model);
        state.AddDMChannel(channel);
        return channel;
    }

    internal SocketDMChannel CreateDMChannel(Guid chatCode, User model, ClientState state) =>
        SocketDMChannel.Create(this, state, chatCode, model);

    internal SocketDMChannel CreateDMChannel(Guid chatCode, SocketUser user, ClientState state) =>
        new(this, chatCode, user);

    private async Task GuildAvailableAsync(SocketGuild guild)
    {
        if (guild.IsConnected) return;
        guild.IsConnected = true;
        await TimedInvokeAsync(_guildAvailableEvent, nameof(GuildAvailable), guild).ConfigureAwait(false);
    }

    internal async Task GuildUnavailableAsync(SocketGuild guild)
    {
        if (!guild.IsConnected) return;
        guild.IsConnected = false;
        await TimedInvokeAsync(_guildUnavailableEvent, nameof(GuildUnavailable), guild).ConfigureAwait(false);
    }

    internal async Task TimedInvokeAsync(AsyncEvent<Func<Task>> eventHandler, string name)
    {
        if (!eventHandler.HasSubscribers) return;
        if (HandlerTimeout.HasValue)
            await TimeoutWrap(name, eventHandler.InvokeAsync).ConfigureAwait(false);
        else
            await eventHandler.InvokeAsync().ConfigureAwait(false);
    }

    internal async Task TimedInvokeAsync<T>(AsyncEvent<Func<T, Task>> eventHandler, string name, T arg)
    {
        if (!eventHandler.HasSubscribers) return;
        if (HandlerTimeout.HasValue)
            await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg)).ConfigureAwait(false);
        else
            await eventHandler.InvokeAsync(arg).ConfigureAwait(false);
    }

    internal async Task TimedInvokeAsync<T1, T2>(AsyncEvent<Func<T1, T2, Task>> eventHandler, string name, T1 arg1,
        T2 arg2)
    {
        if (!eventHandler.HasSubscribers) return;
        if (HandlerTimeout.HasValue)
            await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2)).ConfigureAwait(false);
        else
            await eventHandler.InvokeAsync(arg1, arg2).ConfigureAwait(false);
    }

    internal async Task TimedInvokeAsync<T1, T2, T3>(AsyncEvent<Func<T1, T2, T3, Task>> eventHandler, string name,
        T1 arg1, T2 arg2, T3 arg3)
    {
        if (!eventHandler.HasSubscribers) return;
        if (HandlerTimeout.HasValue)
            await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2, arg3)).ConfigureAwait(false);
        else
            await eventHandler.InvokeAsync(arg1, arg2, arg3).ConfigureAwait(false);
    }

    internal async Task TimedInvokeAsync<T1, T2, T3, T4>(AsyncEvent<Func<T1, T2, T3, T4, Task>> eventHandler,
        string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (!eventHandler.HasSubscribers) return;
        if (HandlerTimeout.HasValue)
            await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2, arg3, arg4)).ConfigureAwait(false);
        else
            await eventHandler.InvokeAsync(arg1, arg2, arg3, arg4).ConfigureAwait(false);
    }

    internal async Task TimedInvokeAsync<T1, T2, T3, T4, T5>(AsyncEvent<Func<T1, T2, T3, T4, T5, Task>> eventHandler,
        string name, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        if (!eventHandler.HasSubscribers) return;
        if (HandlerTimeout.HasValue)
            await TimeoutWrap(name, () => eventHandler.InvokeAsync(arg1, arg2, arg3, arg4, arg5)).ConfigureAwait(false);
        else
            await eventHandler.InvokeAsync(arg1, arg2, arg3, arg4, arg5).ConfigureAwait(false);
    }

    private async Task TimeoutWrap(string name, Func<Task> action)
    {
        try
        {
            if (!HandlerTimeout.HasValue)
            {
                await action().ConfigureAwait(false);
                return;
            }

            Task timeoutTask = Task.Delay(HandlerTimeout.Value);
            Task handlersTask = action();
            if (await Task.WhenAny(timeoutTask, handlersTask).ConfigureAwait(false) == timeoutTask)
            {
                await _gatewayLogger.WarningAsync($"A {name} handler is blocking the gateway task.")
                    .ConfigureAwait(false);
            }

            await handlersTask.ConfigureAwait(false); //Ensure the handler completes
        }
        catch (Exception ex)
        {
            await _gatewayLogger.WarningAsync($"A {name} handler has thrown an unhandled exception.", ex)
                .ConfigureAwait(false);
        }
    }

    private async Task UnknownChannelUserAsync(string evnt, ulong userId, Guid chatCode, object payload)
    {
        string details = $"{evnt} User={userId} ChatCode={chatCode}";
        await _gatewayLogger.WarningAsync($"Unknown User ({details}). Payload: {SerializePayload(payload)}").ConfigureAwait(false);
    }

    private async Task UnknownGlobalUserAsync(string evnt, ulong userId, object payload)
    {
        string details = $"{evnt} User={userId}";
        await _gatewayLogger.WarningAsync($"Unknown User ({details}). Payload: {SerializePayload(payload)}").ConfigureAwait(false);
    }

    private async Task UnknownChannelUserAsync(string evnt, ulong userId, ulong channelId, object payload)
    {
        string details = $"{evnt} User={userId} Channel={channelId}";
        await _gatewayLogger.WarningAsync($"Unknown User ({details}). Payload: {SerializePayload(payload)}").ConfigureAwait(false);
    }

    private async Task UnknownGuildUserAsync(string evnt, ulong userId, ulong guildId, object payload)
    {
        string details = $"{evnt} User={userId} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unknown User ({details}). Payload: {SerializePayload(payload)}").ConfigureAwait(false);
    }

    private async Task IncompleteGuildUserAsync(string evnt, ulong userId, ulong guildId, object payload)
    {
        string details = $"{evnt} User={userId} Guild={guildId}";
        await _gatewayLogger.DebugAsync($"User has not been downloaded ({details}). Payload: {SerializePayload(payload)}").ConfigureAwait(false);
    }

    private async Task UnknownPrivateChannelAsync(string evnt, Guid chatCode, object payload)
    {
        string details = $"{evnt} Channel={chatCode}";
        await _gatewayLogger.WarningAsync($"Unknown Private Channel ({details}). Payload: {SerializePayload(payload)}").ConfigureAwait(false);
    }

    private async Task UnknownChannelAsync(string evnt, ulong channelId, object payload)
    {
        string details = $"{evnt} Channel={channelId}";
        await _gatewayLogger.WarningAsync($"Unknown Channel ({details}). Payload: {SerializePayload(payload)}").ConfigureAwait(false);
    }

    private async Task UnknownChannelAsync(string evnt, ulong channelId, ulong guildId, object payload)
    {
        if (guildId == 0)
        {
            await UnknownChannelAsync(evnt, channelId, payload).ConfigureAwait(false);
            return;
        }

        string details = $"{evnt} Channel={channelId} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unknown Channel ({details}). Payload: {SerializePayload(payload)}").ConfigureAwait(false);
    }

    private async Task UnknownRoleAsync(string evnt, ulong roleId, ulong guildId, object payload)
    {
        string details = $"{evnt} Role={roleId} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unknown Role ({details}). Payload: {SerializePayload(payload)}").ConfigureAwait(false);
    }

    private async Task UnavailableGuildAsync(string evnt, ulong guildId, object payload)
    {
        string details = $"{evnt} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unable to perform action on an unavailable guild ({details}). Payload: {SerializePayload(payload)}").ConfigureAwait(false);
    }

    private async Task UnknownGuildAsync(string evnt, ulong guildId, object payload)
    {
        string details = $"{evnt} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unknown Guild ({details}). Payload: {SerializePayload(payload)}").ConfigureAwait(false);
    }

    private async Task UnknownGuildEventAsync(string evnt, ulong eventId, ulong guildId, object payload)
    {
        string details = $"{evnt} Event={eventId} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unknown Guild Event ({details}). Payload: {SerializePayload(payload)}").ConfigureAwait(false);
    }

    private async Task UnsyncedGuildAsync(string evnt, ulong guildId, object payload)
    {
        string details = $"{evnt} Guild={guildId}";
        await _gatewayLogger.DebugAsync($"Unsynced Guild ({details}). Payload: {SerializePayload(payload)}").ConfigureAwait(false);
    }

    internal int GetAudioId() => _nextAudioId++;

    #region IKookClient

    /// <inheritdoc />
    Task<IReadOnlyCollection<IGuild>> IKookClient.GetGuildsAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IReadOnlyCollection<IGuild>>(Guilds);

    /// <inheritdoc />
    Task<IGuild?> IKookClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IGuild?>(GetGuild(id));

    /// <inheritdoc />
    async Task<IUser?> IKookClient.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options)
    {
        if (GetUser(id) is { } user)
            return user;
        if (mode == CacheMode.CacheOnly)
            return null;
        return await Rest.GetUserAsync(id, options).ConfigureAwait(false);
    }

    /// <inheritdoc />
    async Task<IChannel?> IKookClient.GetChannelAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload ? await GetChannelAsync(id, options).ConfigureAwait(false) : GetChannel(id);

    /// <inheritdoc />
    async Task<IDMChannel?> IKookClient.GetDMChannelAsync(Guid chatCode, CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetDMChannelAsync(chatCode, options).ConfigureAwait(false)
            : GetDMChannel(chatCode);

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IDMChannel>> IKookClient.GetDMChannelsAsync(CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? await GetDMChannelsAsync(options).ConfigureAwait(false)
            : DMChannels;

    #endregion
}
