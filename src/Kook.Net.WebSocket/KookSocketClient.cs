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
    private int _retryCount;
    private Task _heartbeatTask, _guildDownloadTask;
    private int _unavailableGuildCount;
    private long _lastGuildAvailableTime, _lastMessageTime;
    private int _nextAudioId;

    private bool _isDisposed;

    /// <inheritdoc />
    public override KookSocketRestClient Rest { get; }

    /// <inheritdoc />
    public override ConnectionState ConnectionState => _connection.State;

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
        => State.DMChannels.Where(x => x is not null).ToImmutableArray();

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
        _connection.Disconnected += (ex, _) => TimedInvokeAsync(_disconnectedEvent, nameof(Disconnected), ex);

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
        LatencyUpdated += async (_, val) => await _gatewayLogger.DebugAsync($"Latency = {val} ms").ConfigureAwait(false);

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
            // ignored
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

    /// <inheritdoc />
    public override SocketDMChannel GetDMChannel(ulong userId)
        => State.GetDMChannel(userId);

    /// <summary>
    ///     Gets a generic channel from the cache or does a rest request if unavailable.
    /// </summary>
    /// <param name="id">The identifier of the channel.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the channel associated
    ///     with the identifier; <c>null</c> when the channel cannot be found.
    /// </returns>
    public async Task<IChannel> GetChannelAsync(ulong id, RequestOptions options = null)
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
    public async Task<IDMChannel> GetDMChannelAsync(Guid chatCode, RequestOptions options = null)
        => await ClientHelper.GetDMChannelAsync(this, chatCode, options).ConfigureAwait(false);

    /// <summary>
    ///     Gets a collection of direct message channels from the cache or does a rest request if unavailable.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the channel associated
    ///     with the identifier; <c>null</c> when the channel cannot be found.
    /// </returns>
    public async Task<IReadOnlyCollection<IDMChannel>> GetDMChannelsAsync(RequestOptions options = null)
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
    public async Task<IUser> GetUserAsync(ulong id, RequestOptions options = null)
        => await ((IKookClient)this).GetUserAsync(id, CacheMode.AllowDownload, options).ConfigureAwait(false);

    /// <inheritdoc />
    public override SocketUser GetUser(ulong id)
        => State.GetUser(id);

    /// <inheritdoc />
    public override SocketUser GetUser(string username, string identifyNumber)
        => State.Users.FirstOrDefault(x => x.IdentifyNumber == identifyNumber && x.Username == username);

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

    internal void RemoveUser(ulong id)
        => State.RemoveUser(id);

    /// <summary>
    ///     Downloads all users for the specified guilds.
    /// </summary>
    /// <param name="guilds">
    ///     The guilds to download the users for. If <c>null</c>, all available guilds will be downloaded.
    /// </param>
    /// <param name="options">The options to be used when sending the request.</param>
    public override async Task DownloadUsersAsync(IEnumerable<IGuild> guilds = null, RequestOptions options = null)
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
    public override async Task DownloadVoiceStatesAsync(IEnumerable<IGuild> guilds = null, RequestOptions options = null)
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
    /// <param name="options">The options to be used when sending the request.</param>
    public override async Task DownloadBoostSubscriptionsAsync(IEnumerable<IGuild> guilds = null, RequestOptions options = null)
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
                        case MessageType.Card:
                            {
                                await _gatewayLogger.DebugAsync($"Received Message ({gatewayEvent.Type}, {gatewayEvent.ChannelType})")
                                    .ConfigureAwait(false);
                                switch (gatewayEvent.ChannelType)
                                {
                                    case "GROUP" when eventExtraData is GatewayGroupMessageExtraData groupMessageExtraData:
                                        {
                                            SocketGuild guild = GetGuild(groupMessageExtraData.GuildId);

                                            if (guild is null)
                                            {
                                                await UnknownGuildAsync(gatewayEvent.ChannelType, groupMessageExtraData.GuildId, payload)
                                                    .ConfigureAwait(false);
                                                return;
                                            }

                                            SocketTextChannel channel = guild.GetTextChannel(gatewayEvent.TargetId);
                                            SocketGuildUser author = guild.GetUser(groupMessageExtraData.Author.Id) ?? guild.AddOrUpdateUser(groupMessageExtraData.Author);
                                            SocketMessage msg = SocketMessage.Create(this, State, author, channel, groupMessageExtraData, gatewayEvent);
                                            SocketChannelHelper.AddMessage(channel, this, msg);
                                            await TimedInvokeAsync(_messageReceivedEvent, nameof(MessageReceived), msg, author, channel).ConfigureAwait(false);
                                            break;
                                        }
                                    case "PERSON" when eventExtraData is GatewayPersonMessageExtraData personMessageExtraData:
                                        {
                                            SocketUser author = State.GetOrAddUser(personMessageExtraData.Author.Id,
                                                _ => SocketGlobalUser.Create(this, State, personMessageExtraData.Author));
                                            SocketDMChannel channel = GetDMChannel(personMessageExtraData.Code)
                                                ?? AddDMChannel(personMessageExtraData.Code, personMessageExtraData.Author, State);
                                            if (author == null)
                                            {
                                                await UnknownChannelUserAsync(gatewayEvent.Type.ToString(), personMessageExtraData.Author.Id, personMessageExtraData.Code, payload)
                                                    .ConfigureAwait(false);
                                                return;
                                            }

                                            SocketMessage msg = SocketMessage.Create(this, State, author, channel, personMessageExtraData, gatewayEvent);
                                            SocketChannelHelper.AddMessage(channel, this, msg);
                                            await TimedInvokeAsync(_directMessageReceivedEvent, nameof(DirectMessageReceived), msg, author, channel).ConfigureAwait(false);
                                            break;
                                        }
                                    default:
                                        await _gatewayLogger.WarningAsync($"Unknown Event Type ({gatewayEvent.Type}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}")
                                            .ConfigureAwait(false);
                                        break;
                                }
                            }
                            break;

                        case MessageType.Poke:
                            {
                                await _gatewayLogger.DebugAsync($"Received Message ({gatewayEvent.Type}, {gatewayEvent.ChannelType})")
                                    .ConfigureAwait(false);
                                switch (gatewayEvent.ChannelType)
                                {
                                    case "GROUP" when eventExtraData is GatewayGroupMessageExtraData groupMessageExtraData:
                                        {
                                            SocketChannel channel = GetChannel(gatewayEvent.TargetId);

                                            if (channel is not SocketTextChannel textChannel)
                                            {
                                                await UnknownChannelAsync(gatewayEvent.ChannelType, gatewayEvent.TargetId, payload)
                                                    .ConfigureAwait(false);
                                                return;
                                            }

                                            SocketGuild guild = textChannel.Guild;
                                            SocketGuildUser author = guild.GetUser(groupMessageExtraData.Author.Id) ?? guild.AddOrUpdateUser(groupMessageExtraData.Author);
                                            SocketMessage msg = SocketMessage.Create(this, State, author, textChannel, groupMessageExtraData, gatewayEvent);
                                            SocketChannelHelper.AddMessage(textChannel, this, msg);
                                            await TimedInvokeAsync(_messageReceivedEvent, nameof(MessageReceived), msg, author, textChannel).ConfigureAwait(false);
                                            break;
                                        }
                                    case "PERSON" when eventExtraData is GatewayPersonMessageExtraData personMessageExtraData:
                                        {
                                            SocketUser author = State.GetOrAddUser(personMessageExtraData.Author.Id,
                                                _ => SocketGlobalUser.Create(this, State, personMessageExtraData.Author));
                                            SocketDMChannel channel = GetDMChannel(personMessageExtraData.Code)
                                                ?? AddDMChannel(personMessageExtraData.Code, personMessageExtraData.Author, State);
                                            if (author == null)
                                            {
                                                await UnknownChannelUserAsync(gatewayEvent.Type.ToString(), personMessageExtraData.Author.Id, personMessageExtraData.Code, payload)
                                                    .ConfigureAwait(false);
                                                return;
                                            }

                                            SocketMessage msg = SocketMessage.Create(this, State, author, channel, personMessageExtraData, gatewayEvent);
                                            SocketChannelHelper.AddMessage(channel, this, msg);
                                            await TimedInvokeAsync(_directMessageReceivedEvent, nameof(DirectMessageReceived), msg, author, channel).ConfigureAwait(false);
                                            break;
                                        }
                                    default:
                                        await _gatewayLogger.WarningAsync($"Unknown Event Type ({gatewayEvent.Type}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}")
                                            .ConfigureAwait(false);
                                        break;
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

                                            if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
                                            {
                                                await UnknownChannelAsync(extraData.Type, data.ChannelId, payload).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketUserMessage cachedMsg = channel.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                            SocketUser user = GetUser(data.UserId) ?? SocketUnknownUser.Create(this, State, data.UserId);
                                            SocketGuildUser socketGuildUser = channel.GetUser(data.UserId);
                                            Cacheable<SocketGuildUser, ulong> cacheableUser =
                                                new(socketGuildUser, data.UserId, socketGuildUser != null,
                                                    async () =>
                                                    {
                                                        GuildMember model = await ApiClient
                                                            .GetGuildMemberAsync(channel.Guild.Id, data.UserId).ConfigureAwait(false);
                                                        return channel.Guild.AddOrUpdateUser(model);
                                                    });
                                            Cacheable<IMessage, Guid> cacheableMsg = new(cachedMsg, data.MessageId, cachedMsg is not null,
                                                async () => await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false));
                                            SocketReaction reaction = SocketReaction.Create(data, channel, cachedMsg, user);

                                            cachedMsg?.AddReaction(reaction);

                                            await TimedInvokeAsync(_reactionAddedEvent, nameof(ReactionAdded), cacheableMsg, channel,
                                                cacheableUser, reaction).ConfigureAwait(false);
                                        }
                                        break;

                                    // 频道内用户取消 reaction
                                    case ("GROUP", "deleted_reaction"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (deleted_reaction)").ConfigureAwait(false);

                                            Reaction data = ((JsonElement)extraData.Body).Deserialize<Reaction>(_serializerOptions);

                                            if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
                                            {
                                                await UnknownChannelAsync(extraData.Type, data.ChannelId, payload).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketUserMessage cachedMsg = channel.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                            SocketUser user = GetUser(data.UserId) ?? SocketUnknownUser.Create(this, State, data.UserId);
                                            SocketGuildUser socketGuildUser = channel.GetUser(data.UserId);
                                            Cacheable<SocketGuildUser, ulong> cacheableUser =
                                                new(socketGuildUser, data.UserId, socketGuildUser != null,
                                                    async () =>
                                                    {
                                                        GuildMember model = await ApiClient
                                                            .GetGuildMemberAsync(channel.Guild.Id, data.UserId).ConfigureAwait(false);
                                                        return channel.Guild.AddOrUpdateUser(model);
                                                    });
                                            Cacheable<IMessage, Guid> cacheableMsg = new(cachedMsg, data.MessageId, cachedMsg is not null,
                                                async () => await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false));
                                            SocketReaction reaction = SocketReaction.Create(data, channel, cachedMsg, user);

                                            cachedMsg?.RemoveReaction(reaction);

                                            await TimedInvokeAsync(_reactionRemovedEvent, nameof(ReactionRemoved), cacheableMsg, channel,
                                                cacheableUser, reaction).ConfigureAwait(false);
                                        }
                                        break;

                                    // 频道消息更新
                                    case ("GROUP", "updated_message"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (updated_message)").ConfigureAwait(false);
                                            MessageUpdateEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<MessageUpdateEvent>(_serializerOptions);

                                            if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
                                            {
                                                await UnknownChannelAsync(extraData.Type, data.ChannelId, payload).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketGuild guild = channel.Guild;
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketMessage cachedMsg = channel.GetCachedMessage(data.MessageId);
                                            SocketMessage before = cachedMsg?.Clone();
                                            cachedMsg?.Update(State, data);
                                            Cacheable<SocketMessage, Guid> cacheableBefore = new(before, data.MessageId, cachedMsg != null,
                                                () => Task.FromResult((SocketMessage)null));
                                            Cacheable<SocketMessage, Guid> cacheableAfter = new(cachedMsg, data.MessageId, cachedMsg != null,
                                                async () => await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false) as SocketMessage);

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

                                            if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
                                            {
                                                await UnknownChannelAsync(extraData.Type, data.ChannelId, payload).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketMessage msg = SocketChannelHelper.RemoveMessage(channel, this, data.MessageId);
                                            Cacheable<IMessage, Guid> cacheableMsg = new(msg, data.MessageId, msg != null,
                                                () => Task.FromResult((IMessage)null));
                                            await TimedInvokeAsync(_messageDeletedEvent, nameof(MessageDeleted), cacheableMsg, channel)
                                                .ConfigureAwait(false);
                                        }
                                        break;

                                    // 新增频道
                                    case ("GROUP", "added_channel"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (added_channel)").ConfigureAwait(false);
                                            Channel data = ((JsonElement)extraData.Body).Deserialize<Channel>(_serializerOptions);
                                            SocketGuild guild = State.GetGuild(data.GuildId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, data.GuildId, payload)
                                                    .ConfigureAwait(false);
                                                return;
                                            }

                                            SocketChannel channel = guild.AddChannel(State, data);
                                            await TimedInvokeAsync(_channelCreatedEvent, nameof(ChannelCreated), channel).ConfigureAwait(false);
                                        }
                                        break;

                                    // 修改频道信息
                                    case ("GROUP", "updated_channel"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (updated_channel)").ConfigureAwait(false);
                                            Channel data = ((JsonElement)extraData.Body).Deserialize<Channel>(_serializerOptions);
                                            SocketChannel channel = State.GetChannel(data.Id);
                                            if (channel == null)
                                            {
                                                await UnknownChannelAsync(extraData.Type, data.Id, payload)
                                                    .ConfigureAwait(false);
                                                return;
                                            }

                                            SocketChannel before = channel.Clone();
                                            channel.Update(State, data);

                                            await TimedInvokeAsync(_channelUpdatedEvent, nameof(ChannelUpdated), before, channel).ConfigureAwait(false);
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
                                                await UnknownGuildAsync(extraData.Type, 0, payload).ConfigureAwait(false);
                                                return;
                                            }

                                            if (channel == null)
                                            {
                                                await UnknownChannelAsync(extraData.Type, data.ChannelId, guild.Id, payload).ConfigureAwait(false);
                                                return;
                                            }

                                            await TimedInvokeAsync(_channelDestroyedEvent, nameof(ChannelDestroyed), channel).ConfigureAwait(false);
                                        }
                                        break;

                                    // 新的频道置顶消息
                                    case ("GROUP", "pinned_message"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (pinned_message)").ConfigureAwait(false);
                                            PinnedMessageEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<PinnedMessageEvent>(_serializerOptions);

                                            if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
                                            {
                                                await UnknownChannelAsync(extraData.Type, data.ChannelId, payload).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketGuild guild = channel.Guild;
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
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
                                            SocketMessage before = cachedMsg?.Clone();
                                            if (cachedMsg != null)
                                                cachedMsg.IsPinned = true;

                                            Cacheable<SocketMessage, Guid> cacheableBefore = new(before, data.MessageId, before is not null,
                                                () => Task.FromResult((SocketMessage)null));
                                            Cacheable<SocketMessage, Guid> cacheableAfter = new(cachedMsg, data.MessageId, cachedMsg is not null,
                                                async () => await channel
                                                    .GetMessageAsync(data.MessageId).ConfigureAwait(false) as SocketMessage);

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

                                            if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
                                            {
                                                await UnknownChannelAsync(extraData.Type, data.ChannelId, payload).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketGuild guild = channel.Guild;
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
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
                                            SocketMessage before = cachedMsg?.Clone();
                                            if (cachedMsg != null)
                                                cachedMsg.IsPinned = false;

                                            Cacheable<SocketMessage, Guid> cacheableBefore = new(before, data.MessageId, before is not null,
                                                () => Task.FromResult((SocketMessage)null));
                                            Cacheable<SocketMessage, Guid> cacheableAfter = new(cachedMsg, data.MessageId, cachedMsg is not null,
                                                async () => await channel
                                                    .GetMessageAsync(data.MessageId).ConfigureAwait(false) as SocketMessage);

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
                                            SocketDMChannel channel = GetDMChannel(data.ChatCode);
                                            Cacheable<SocketDMChannel, Guid> cacheableChannel = new(channel, data.ChatCode, channel != null,
                                                async () => {
                                                    User user = await ApiClient.GetUserAsync(data.AuthorId).ConfigureAwait(false);
                                                    return CreateDMChannel(data.ChatCode, user, State);
                                                });

                                            SocketMessage cachedMsg = channel?.GetCachedMessage(data.MessageId);
                                            SocketMessage before = cachedMsg?.Clone();
                                            cachedMsg?.Update(State, data);
                                            Cacheable<IMessage, Guid> cacheableBefore = new(before, data.MessageId, before is not null,
                                                () => Task.FromResult((IMessage)null));
                                            SocketUser user = State.GetUser(data.AuthorId);
                                            Cacheable<SocketMessage, Guid> cacheableAfter = new(cachedMsg, data.MessageId, cachedMsg is not null,
                                                async () =>
                                                {
                                                    DirectMessage msg = await ApiClient.GetDirectMessageAsync(data.MessageId, data.ChatCode)
                                                        .ConfigureAwait(false);
                                                    SocketUser author = user ?? new SocketUnknownUser(this, data.AuthorId);
                                                    SocketMessage after = SocketMessage.Create(this, State, author, channel, msg);
                                                    return after;
                                                });
                                            Cacheable<SocketUser, ulong> cacheableUser = new(user, data.AuthorId, user != null,
                                                async () =>
                                                {
                                                    User model = await ApiClient.GetUserAsync(data.AuthorId).ConfigureAwait(false);
                                                    SocketGlobalUser globalUser = State.GetOrAddUser(data.AuthorId, _ => SocketGlobalUser.Create(this, State, model));
                                                    globalUser.Update(State, model);
                                                    globalUser.UpdatePresence(model.Online, model.OperatingSystem);
                                                    return globalUser;
                                                });

                                            await TimedInvokeAsync(_directMessageUpdatedEvent, nameof(DirectMessageUpdated), cacheableBefore,
                                                cacheableAfter, cacheableUser, cacheableChannel).ConfigureAwait(false);
                                        }
                                        break;

                                    // 私聊消息被删除
                                    case ("PERSON", "deleted_private_message"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (deleted_private_message)").ConfigureAwait(false);
                                            DirectMessageDeleteEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<DirectMessageDeleteEvent>(_serializerOptions);
                                            SocketDMChannel channel = GetDMChannel(data.ChatCode);
                                            Cacheable<SocketDMChannel, Guid> cacheableChannel = new(channel, data.ChatCode, channel != null,
                                                async () => {
                                                    User user = await ApiClient.GetUserAsync(data.AuthorId).ConfigureAwait(false);
                                                    return CreateDMChannel(data.ChatCode, user, State);
                                                });

                                            SocketMessage msg = null;
                                            if (channel != null) msg = SocketChannelHelper.RemoveMessage(channel, this, data.MessageId);

                                            Cacheable<IMessage, Guid> cacheableMsg = new(msg, data.MessageId, msg != null,
                                                () => Task.FromResult((IMessage)null));
                                            SocketUser user = State.GetUser(data.AuthorId);
                                            Cacheable<SocketUser, ulong> cacheableUser = new(user, data.AuthorId, user != null,
                                                async () =>
                                                {
                                                    User model = await ApiClient.GetUserAsync(data.AuthorId).ConfigureAwait(false);
                                                    SocketGlobalUser globalUser = State.GetOrAddUser(data.AuthorId, _ => SocketGlobalUser.Create(this, State, model));
                                                    globalUser.Update(State, model);
                                                    globalUser.UpdatePresence(model.Online, model.OperatingSystem);
                                                    return globalUser;
                                                });

                                            await TimedInvokeAsync(_directMessageDeletedEvent, nameof(DirectMessageDeleted), cacheableMsg,
                                                cacheableUser, cacheableChannel).ConfigureAwait(false);
                                        }
                                        break;

                                    // 私聊内用户添加 reaction
                                    case ("PERSON", "private_added_reaction"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (private_added_reaction)").ConfigureAwait(false);

                                            PrivateReaction data = ((JsonElement)extraData.Body).Deserialize<PrivateReaction>(_serializerOptions);
                                            SocketDMChannel channel = GetDMChannel(data.ChatCode);
                                            SocketUserMessage cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                            SocketUser user = GetUser(data.UserId);
                                            SocketUser operatorUser = user ?? SocketUnknownUser.Create(this, State, data.UserId);
                                            Cacheable<SocketUser, ulong> cacheableUser = new(user, data.UserId, user != null,
                                                async () =>
                                                {
                                                    User model = await ApiClient.GetUserAsync(data.UserId).ConfigureAwait(false);
                                                    SocketGlobalUser globalUser = State.GetOrAddUser(data.UserId, _ => SocketGlobalUser.Create(this, State, model));
                                                    globalUser.Update(State, model);
                                                    globalUser.UpdatePresence(model.Online, model.OperatingSystem);
                                                    return globalUser;
                                                });

                                            Cacheable<IDMChannel, Guid> cacheableChannel = new(channel, data.ChatCode, channel is not null,
                                                async () => await GetDMChannelAsync(data.ChatCode).ConfigureAwait(false));
                                            Cacheable<IMessage, Guid> cacheableMsg = new(cachedMsg, data.MessageId, cachedMsg is not null, async () =>
                                            {
                                                IDMChannel channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                                return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false);
                                            });
                                            SocketReaction reaction = SocketReaction.Create(data, channel, cachedMsg, operatorUser);

                                            cachedMsg?.AddReaction(reaction);

                                            await TimedInvokeAsync(_directReactionAddedEvent, nameof(DirectReactionAdded), cacheableMsg,
                                                cacheableChannel, cacheableUser, reaction).ConfigureAwait(false);
                                        }
                                        break;

                                    // 私聊内用户取消 reaction
                                    case ("PERSON", "private_deleted_reaction"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (private_deleted_reaction)").ConfigureAwait(false);

                                            PrivateReaction data = ((JsonElement)extraData.Body).Deserialize<PrivateReaction>(_serializerOptions);
                                            SocketDMChannel channel = GetDMChannel(data.ChatCode);
                                            SocketUserMessage cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                            SocketUser user = GetUser(data.UserId);
                                            SocketUser operatorUser = user ?? SocketUnknownUser.Create(this, State, data.UserId);
                                            Cacheable<SocketUser, ulong> cacheableUser = new(user, data.UserId, user != null,
                                                async () =>
                                                {
                                                    User model = await ApiClient.GetUserAsync(data.UserId).ConfigureAwait(false);
                                                    SocketGlobalUser globalUser = State.GetOrAddUser(data.UserId, _ => SocketGlobalUser.Create(this, State, model));
                                                    globalUser.Update(State, model);
                                                    globalUser.UpdatePresence(model.Online, model.OperatingSystem);
                                                    return globalUser;
                                                });

                                            Cacheable<IDMChannel, Guid> cacheableChannel = new(channel, data.ChatCode, channel is not null,
                                                async () => await GetDMChannelAsync(data.ChatCode).ConfigureAwait(false));
                                            Cacheable<IMessage, Guid> cacheableMsg = new(cachedMsg, data.MessageId, cachedMsg is not null, async () =>
                                            {
                                                IDMChannel channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                                return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false);
                                            });
                                            SocketReaction reaction = SocketReaction.Create(data, channel, cachedMsg, operatorUser);

                                            cachedMsg?.RemoveReaction(reaction);

                                            await TimedInvokeAsync(_directReactionRemovedEvent, nameof(DirectReactionRemoved), cacheableMsg,
                                                cacheableChannel, cacheableUser, reaction).ConfigureAwait(false);
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
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
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
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
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
                                                    SocketGlobalUser globalUser = State.GetOrAddUser(data.UserId, _ => SocketGlobalUser.Create(this, State, model));
                                                    globalUser.Update(State, model);
                                                    globalUser.UpdatePresence(model.Online, model.OperatingSystem);
                                                    return globalUser;
                                                });

                                            foreach (SocketGuildChannel channel in guild.Channels)
                                                channel.RemoveUserPermissionOverwrite(data.UserId);

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
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
                                                return;
                                            }

                                            GuildMemberUpdateEvent data =
                                                ((JsonElement)extraData.Body).Deserialize<GuildMemberUpdateEvent>(_serializerOptions);
                                            SocketGuildUser user = guild.GetUser(data.UserId);

                                            SocketGuildUser before = user?.Clone();
                                            user?.Update(State, data);
                                            Cacheable<SocketGuildUser, ulong> cacheableBefore = new(before, data.UserId, before is not null,
                                                () => Task.FromResult<SocketGuildUser>(null));
                                            Cacheable<SocketGuildUser, ulong> cacheableAfter = new(user, data.UserId, user is not null,
                                                async () =>
                                                {
                                                    GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, data.UserId)
                                                        .ConfigureAwait(false);
                                                    return guild.AddOrUpdateUser(model);
                                                });
                                            await TimedInvokeAsync(_guildMemberUpdatedEvent, nameof(GuildMemberUpdated),
                                                cacheableBefore, cacheableAfter).ConfigureAwait(false);
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
                                                    await UnknownGuildAsync(extraData.Type, guildId, payload).ConfigureAwait(false);
                                                    return;
                                                }

                                                SocketGuildUser user = guild.GetUser(data.UserId);
                                                user?.Presence.Update(true);
                                                users.Add(new Cacheable<SocketGuildUser, ulong>(user, data.UserId, user is not null,
                                                    async () =>
                                                    {
                                                        GuildMember model = await ApiClient
                                                            .GetGuildMemberAsync(guild.Id, data.UserId)
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
                                                    await UnknownGuildAsync(extraData.Type, guildId, payload).ConfigureAwait(false);
                                                    return;
                                                }

                                                SocketGuildUser user = guild.GetUser(data.UserId);
                                                user?.Presence.Update(false);
                                                users.Add(new Cacheable<SocketGuildUser, ulong>(user, data.UserId, user is not null,
                                                    async () =>
                                                    {
                                                        GuildMember model = await ApiClient
                                                            .GetGuildMemberAsync(guild.Id, data.UserId)
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
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
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
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
                                                return;
                                            }

                                            foreach (SocketGuildChannel channel in guild.Channels)
                                                channel.RemoveRolePermissionOverwrite(data.Id);

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
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketRole role = guild.GetRole(data.Id);
                                            if (role == null)
                                            {
                                                await UnknownRoleAsync(extraData.Type, data.Id, guild.Id, payload).ConfigureAwait(false);
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
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
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
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
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
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
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
                                                await UnknownGuildAsync(extraData.Type, data.GuildId, payload).ConfigureAwait(false);
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
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
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
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketUser operatorUser = guild.GetUser(data.OperatorUserId);
                                            Cacheable<SocketUser, ulong> cacheableOperatorUser = new(operatorUser, data.OperatorUserId, operatorUser != null,
                                                async () =>
                                                {
                                                    User model = await ApiClient.GetUserAsync(data.OperatorUserId).ConfigureAwait(false);
                                                    var operatorGlobalUser = State.GetOrAddUser(data.OperatorUserId, _ => SocketGlobalUser.Create(this, State, model));
                                                    operatorGlobalUser.Update(State, model);
                                                    operatorGlobalUser.UpdatePresence(model.Online, model.OperatingSystem);
                                                    return operatorGlobalUser;
                                                });
                                            IReadOnlyCollection<Cacheable<SocketUser, ulong>> bannedUsers = data.UserIds.Select(id =>
                                            {
                                                SocketUser bannedUser = guild.GetUser(id);
                                                return new Cacheable<SocketUser, ulong>(bannedUser, id, bannedUser != null,
                                                    async () =>
                                                    {
                                                        User model = await ApiClient.GetUserAsync(id).ConfigureAwait(false);
                                                        SocketGlobalUser bannedGlobalUser = State.GetOrAddUser(id,
                                                            _ => SocketGlobalUser.Create(this, State, model));
                                                        bannedGlobalUser.Update(State, model);
                                                        bannedGlobalUser.UpdatePresence(model.Online, model.OperatingSystem);
                                                        return bannedGlobalUser;
                                                    });
                                            }).ToReadOnlyCollection(() => data.UserIds.Length);
                                            await TimedInvokeAsync(_userBannedEvent, nameof(UserBanned), bannedUsers, cacheableOperatorUser, guild, data.Reason)
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
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
                                                return;
                                            }

                                            SocketUser operatorUser = guild.GetUser(data.OperatorUserId);
                                            Cacheable<SocketUser, ulong> cacheableOperatorUser = new(operatorUser, data.OperatorUserId, operatorUser != null,
                                                async () =>
                                                {
                                                    User model = await ApiClient.GetUserAsync(data.OperatorUserId).ConfigureAwait(false);
                                                    SocketGlobalUser operatorGlobalUser = State.GetOrAddUser(data.OperatorUserId, _ => SocketGlobalUser.Create(this, State, model));
                                                    operatorGlobalUser.Update(State, model);
                                                    operatorGlobalUser.UpdatePresence(model.Online, model.OperatingSystem);
                                                    return operatorGlobalUser;
                                                });
                                            IReadOnlyCollection<Cacheable<SocketUser, ulong>> unbannedUsers = data.UserIds.Select(id =>
                                            {
                                                SocketUser bannedUser = guild.GetUser(id);
                                                return new Cacheable<SocketUser, ulong>(bannedUser, id, bannedUser != null,
                                                    async () =>
                                                    {
                                                        User model = await ApiClient.GetUserAsync(id).ConfigureAwait(false);
                                                        SocketGlobalUser unbannedGlobalUser = State.GetOrAddUser(id,
                                                            _ => SocketGlobalUser.Create(this, State, model));
                                                        unbannedGlobalUser.Update(State, model);
                                                        unbannedGlobalUser.UpdatePresence(model.Online, model.OperatingSystem);
                                                        return unbannedGlobalUser;
                                                    });
                                            }).ToReadOnlyCollection(() => data.UserIds.Length);
                                            await TimedInvokeAsync(_userUnbannedEvent, nameof(UserUnbanned), unbannedUsers, cacheableOperatorUser, guild)
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
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload)
                                                    .ConfigureAwait(false);
                                                return;
                                            }

                                            SocketVoiceChannel channel = GetChannel(data.ChannelId) as SocketVoiceChannel;

                                            if (channel == null)
                                            {
                                                await UnknownChannelAsync(extraData.Type, data.ChannelId, payload)
                                                    .ConfigureAwait(false);
                                                return;
                                            }

                                            SocketGuildUser user = guild.GetUser(data.UserId);
                                            Cacheable<SocketGuildUser, ulong> cacheableUser = new(user, data.UserId, user != null,
                                                async () =>
                                                {
                                                    GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, data.UserId).ConfigureAwait(false);
                                                    return guild.AddOrUpdateUser(model);
                                                });
                                            guild.AddOrUpdateVoiceState(data.UserId, channel.Id);

                                            await TimedInvokeAsync(_userConnectedEvent, nameof(UserConnected), cacheableUser, channel, data.At)
                                                .ConfigureAwait(false);
                                        }
                                        break;

                                    // 用户退出语音频道
                                    case ("GROUP", "exited_channel"):
                                        {
                                            await _gatewayLogger.DebugAsync("Received Event (exited_channel)").ConfigureAwait(false);
                                            UserVoiceEvent data = ((JsonElement)extraData.Body).Deserialize<UserVoiceEvent>(_serializerOptions);
                                            SocketGuild guild = State.GetGuild(gatewayEvent.TargetId);
                                            if (guild == null)
                                            {
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload)
                                                    .ConfigureAwait(false);
                                                return;
                                            }

                                            SocketVoiceChannel channel = GetChannel(data.ChannelId) as SocketVoiceChannel;

                                            if (channel == null)
                                            {
                                                await UnknownChannelAsync(extraData.Type, data.ChannelId, payload)
                                                    .ConfigureAwait(false);
                                                return;
                                            }

                                            SocketGuildUser user = guild.GetUser(data.UserId);
                                            Cacheable<SocketGuildUser, ulong> cacheableUser = new(user, data.UserId, user != null,
                                                async () =>
                                                {
                                                    GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, data.UserId).ConfigureAwait(false);
                                                    return guild.AddOrUpdateUser(model);
                                                });
                                            guild.AddOrUpdateVoiceState(data.UserId, null);

                                            await TimedInvokeAsync(_userDisconnectedEvent, nameof(UserDisconnected), cacheableUser, channel, data.At)
                                                .ConfigureAwait(false);
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
                                                await TimedInvokeAsync(_currentUserUpdatedEvent, nameof(CurrentUserUpdated), before, CurrentUser)
                                                    .ConfigureAwait(false);
                                            }
                                            else
                                            {
                                                SocketUser user = GetUser(data.UserId);
                                                SocketUser before = user?.Clone();
                                                user?.Update(State, data);
                                                await TimedInvokeAsync(_userUpdatedEvent, nameof(UserUpdated),
                                                        new Cacheable<SocketUser, ulong>(before, data.UserId, before is not null,
                                                            () => Task.FromResult((SocketUser)null)),
                                                        new Cacheable<SocketUser, ulong>(user, data.UserId, user is not null,
                                                            async () =>
                                                            {
                                                                User model = await ApiClient.GetUserAsync(data.UserId).ConfigureAwait(false);
                                                                SocketGlobalUser globalUser = State.GetOrAddUser(data.UserId, _ => SocketGlobalUser.Create(this, State, model));
                                                                globalUser.Update(State, model);
                                                                globalUser.UpdatePresence(model.Online, model.OperatingSystem);
                                                                return globalUser;
                                                            }))
                                                    .ConfigureAwait(false);
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
                                                    int remainingRetryTimes = _baseConfig.MaxJoinedGuildDataFetchingRetryTimes;
                                                    while (true)
                                                    {
                                                        try
                                                        {
                                                            return await ApiClient.GetGuildAsync(data.GuildId)
                                                                .ConfigureAwait(false);
                                                        }
                                                        catch (HttpException ex) when (ex is
                                                            {
                                                                HttpCode: HttpStatusCode.OK,
                                                                KookCode: KookErrorCode.GeneralError
                                                            })
                                                        {
                                                            if (remainingRetryTimes < 0) throw;
                                                        }

                                                        await _gatewayLogger
                                                            .WarningAsync($"Failed to get guild {data.GuildId} after joining. Retrying in {_baseConfig.JoinedGuildDataFetchingRetryDelay:F3} second for {remainingRetryTimes} more times.")
                                                            .ConfigureAwait(false);
                                                        remainingRetryTimes--;
                                                        await Task.Delay(TimeSpan.FromMilliseconds(_baseConfig.JoinedGuildDataFetchingRetryDelay)).ConfigureAwait(false);
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    await _gatewayLogger
                                                        .ErrorAsync($"Error handling {gatewaySocketFrameType}. Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}", e)
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
                                                await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
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
                                                    await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
                                                    return;
                                                }

                                                if (channel == null)
                                                {
                                                    await UnknownChannelAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
                                                    return;
                                                }

                                                SocketGuildUser user = channel.GetUser(data.UserId);
                                                Cacheable<SocketGuildUser, ulong> cacheableUser = new(user, data.UserId, user != null,
                                                    async () =>
                                                    {
                                                        GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, data.UserId).ConfigureAwait(false);
                                                        return guild.AddOrUpdateUser(model);
                                                    });

                                                SocketMessage cachedMsg = channel.GetCachedMessage(data.MessageId);
                                                Cacheable<IMessage, Guid> cacheableMsg = new(cachedMsg, data.MessageId, cachedMsg is not null,
                                                    async () => await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false));
                                                await TimedInvokeAsync(_messageButtonClickedEvent, nameof(MessageButtonClicked),
                                                    data.Value, cacheableUser, cacheableMsg, channel).ConfigureAwait(false);
                                            }
                                            else
                                            {
                                                SocketUser user = GetUser(data.UserId);
                                                Cacheable<SocketUser, ulong> cacheableUser = new(user, data.UserId, user != null,
                                                    async () =>
                                                    {
                                                        User model = await ApiClient.GetUserAsync(data.UserId).ConfigureAwait(false);
                                                        SocketGlobalUser globalUser = State.GetOrAddUser(data.UserId, _ => SocketGlobalUser.Create(this, State, model));
                                                        globalUser.Update(State, model);
                                                        globalUser.UpdatePresence(model.Online, model.OperatingSystem);
                                                        return globalUser;
                                                    });


                                                SocketDMChannel channel = GetDMChannel(data.UserId);
                                                if (channel == null)
                                                {
                                                    UserChat model = await ApiClient
                                                        .CreateUserChatAsync(new CreateUserChatParams() { UserId = data.UserId }).ConfigureAwait(false);
                                                    channel = CreateDMChannel(model.Code, model.Recipient, State);
                                                }

                                                if (channel == null)
                                                {
                                                    await UnknownChannelAsync(extraData.Type, gatewayEvent.TargetId, payload).ConfigureAwait(false);
                                                    return;
                                                }

                                                Cacheable<IMessage, Guid> cacheableMsg = new(null, data.MessageId, false,
                                                        async () => await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false));
                                                await TimedInvokeAsync(_directMessageButtonClickedEvent, nameof(DirectMessageButtonClicked),
                                                    data.Value, cacheableUser, cacheableMsg, channel).ConfigureAwait(false);
                                            }
                                        }
                                        break;

                                    #endregion

                                    default:
                                        await _gatewayLogger.WarningAsync($"Unknown SystemEventType ({extraData.Type}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}")
                                            .ConfigureAwait(false);
                                        break;
                                }
                            }
                            break;
                        default:
                            await _gatewayLogger.WarningAsync($"Unknown Event Type ({gatewayEvent.Type}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}")
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
                            _heartbeatTask = RunHeartbeatAsync(_connection.CancellationToken);
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
                        _guildDownloadTask = WaitForGuildsAsync(_connection.CancellationToken, _gatewayLogger)
                            .ContinueWith(async task =>
                            {
                                if (task.IsFaulted)
                                {
                                    _connection.Error(task.Exception);
                                    return;
                                }
                                else if (_connection.CancellationToken.IsCancellationRequested) return;

                                // Download user list if enabled
                                if (_baseConfig.AlwaysDownloadUsers)
                                {
                                    _ = Task.Run(async () =>
                                    {
                                        try
                                        {
                                            await DownloadUsersAsync(Guilds.Where(x => x.IsAvailable && x.HasAllMembers is not true));
                                        }
                                        catch (Exception ex)
                                        {
                                            await _gatewayLogger.WarningAsync("Downloading users failed", ex).ConfigureAwait(false);
                                        }
                                    });
                                }

                                if (_baseConfig.AlwaysDownloadVoiceStates)
                                {
                                    _ = Task.Run(async () =>
                                    {
                                        try
                                        {
                                            await DownloadVoiceStatesAsync(Guilds.Where(x => x.IsAvailable));
                                        }
                                        catch (Exception ex)
                                        {
                                            await _gatewayLogger.WarningAsync("Downloading voice states failed", ex).ConfigureAwait(false);
                                        }
                                    });
                                }

                                if (_baseConfig.AlwaysDownloadBoostSubscriptions)
                                {
                                    _ = Task.Run(async () =>
                                    {
                                        try
                                        {
                                            await DownloadBoostSubscriptionsAsync(Guilds.Where(x => x.IsAvailable));
                                        }
                                        catch (Exception ex)
                                        {
                                            await _gatewayLogger.WarningAsync("Downloading boost subscriptions failed", ex).ConfigureAwait(false);
                                        }
                                    });
                                }

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
                    await _gatewayLogger.WarningAsync($"Unknown Socket Frame Type ({gatewaySocketFrameType}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}")
                        .ConfigureAwait(false);
                    break;
            }
        }
        catch (Exception ex)
        {
            await _gatewayLogger.ErrorAsync($"Error handling {gatewaySocketFrameType}. Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}", ex).ConfigureAwait(false);
        }
    }

    #endregion

    /// <inheritdoc />
    public override async Task StartAsync() => await _connection.StartAsync().ConfigureAwait(false);

    /// <inheritdoc />
    public override async Task StopAsync() => await _connection.StopAsync().ConfigureAwait(false);

    private async Task RunHeartbeatAsync(CancellationToken cancellationToken)
    {
        int intervalMillis = KookSocketConfig.HeartbeatIntervalMilliseconds;
        try
        {
            await _gatewayLogger.DebugAsync("Heartbeat Started").ConfigureAwait(false);
            while (!cancellationToken.IsCancellationRequested)
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

    private async Task WaitForGuildsAsync(CancellationToken cancellationToken, Logger logger)
    {
        //Wait for GUILD_AVAILABLEs
        try
        {
            await logger.DebugAsync("GuildDownloader Started").ConfigureAwait(false);
            while (_unavailableGuildCount != 0
                   && DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _lastGuildAvailableTime < _baseConfig.MaxWaitBetweenGuildAvailablesBeforeReady)
                await Task.Delay(500, cancellationToken).ConfigureAwait(false);

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
            // ReSharper disable once PossibleInvalidOperationException
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

    private async Task UnknownChannelUserAsync(string evnt, ulong userId, Guid chatCode, object payload)
    {
        string details = $"{evnt} User={userId} ChatCode={chatCode}";
        await _gatewayLogger.WarningAsync($"Unknown User ({details}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}").ConfigureAwait(false);
    }

    private async Task UnknownGlobalUserAsync(string evnt, ulong userId, object payload)
    {
        string details = $"{evnt} User={userId}";
        await _gatewayLogger.WarningAsync($"Unknown User ({details}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}").ConfigureAwait(false);
    }

    private async Task UnknownChannelUserAsync(string evnt, ulong userId, ulong channelId, object payload)
    {
        string details = $"{evnt} User={userId} Channel={channelId}";
        await _gatewayLogger.WarningAsync($"Unknown User ({details}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}").ConfigureAwait(false);
    }

    private async Task UnknownGuildUserAsync(string evnt, ulong userId, ulong guildId, object payload)
    {
        string details = $"{evnt} User={userId} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unknown User ({details}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}").ConfigureAwait(false);
    }

    private async Task IncompleteGuildUserAsync(string evnt, ulong userId, ulong guildId, object payload)
    {
        string details = $"{evnt} User={userId} Guild={guildId}";
        await _gatewayLogger.DebugAsync($"User has not been downloaded ({details}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}").ConfigureAwait(false);
    }

    private async Task UnknownPrivateChannelAsync(string evnt, Guid chatCode, object payload)
    {
        string details = $"{evnt} Channel={chatCode}";
        await _gatewayLogger.WarningAsync($"Unknown Private Channel ({details}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}").ConfigureAwait(false);
    }

    private async Task UnknownChannelAsync(string evnt, ulong channelId, object payload)
    {
        string details = $"{evnt} Channel={channelId}";
        await _gatewayLogger.WarningAsync($"Unknown Channel ({details}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}").ConfigureAwait(false);
    }

    private async Task UnknownChannelAsync(string evnt, ulong channelId, ulong guildId, object payload)
    {
        if (guildId == 0)
        {
            await UnknownChannelAsync(evnt, channelId, payload).ConfigureAwait(false);
            return;
        }

        string details = $"{evnt} Channel={channelId} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unknown Channel ({details}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}").ConfigureAwait(false);
    }

    private async Task UnknownRoleAsync(string evnt, ulong roleId, ulong guildId, object payload)
    {
        string details = $"{evnt} Role={roleId} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unknown Role ({details}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}").ConfigureAwait(false);
    }

    private async Task UnknownGuildAsync(string evnt, ulong guildId, object payload)
    {
        string details = $"{evnt} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unknown Guild ({details}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}").ConfigureAwait(false);
    }

    private async Task UnknownGuildEventAsync(string evnt, ulong eventId, ulong guildId, object payload)
    {
        string details = $"{evnt} Event={eventId} Guild={guildId}";
        await _gatewayLogger.WarningAsync($"Unknown Guild Event ({details}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}").ConfigureAwait(false);
    }

    private async Task UnsyncedGuildAsync(string evnt, ulong guildId, object payload)
    {
        string details = $"{evnt} Guild={guildId}";
        await _gatewayLogger.DebugAsync($"Unsynced Guild ({details}). Payload: {JsonSerializer.Serialize(payload, _serializerOptions)}").ConfigureAwait(false);
    }

    internal int GetAudioId() => _nextAudioId++;

    #region IKookClient

    /// <inheritdoc />
    Task<IReadOnlyCollection<IGuild>> IKookClient.GetGuildsAsync(CacheMode mode, RequestOptions? options = null)
        => Task.FromResult<IReadOnlyCollection<IGuild>>(Guilds);

    /// <inheritdoc />
    Task<IGuild?> IKookClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions? options = null)
        => Task.FromResult<IGuild>(GetGuild(id));

    /// <inheritdoc />
    async Task<IUser?> IKookClient.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options = null)
    {
        SocketUser user = GetUser(id);
        if (user is not null || mode == CacheMode.CacheOnly) return user;

        return await Rest.GetUserAsync(id, options).ConfigureAwait(false);
    }

    /// <inheritdoc />
    async Task<IChannel?> IKookClient.GetChannelAsync(ulong id, CacheMode mode, RequestOptions? options = null)
        => mode == CacheMode.AllowDownload ? await GetChannelAsync(id, options).ConfigureAwait(false) : GetChannel(id);

    /// <inheritdoc />
    async Task<IDMChannel?> IKookClient.GetDMChannelAsync(Guid chatCode, CacheMode mode, RequestOptions? options = null)
        => mode == CacheMode.AllowDownload ? await GetDMChannelAsync(chatCode, options).ConfigureAwait(false) : GetDMChannel(chatCode);

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IDMChannel>> IKookClient.GetDMChannelsAsync(CacheMode mode,
        RequestOptions? options = null)
        => mode == CacheMode.AllowDownload ? await GetDMChannelsAsync(options).ConfigureAwait(false) : DMChannels;

    #endregion
}
