using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using KaiHeiLa.API;
using KaiHeiLa.API.Gateway;
using KaiHeiLa.API.Rest;
using KaiHeiLa.Logging;
using KaiHeiLa.Net.WebSockets;
using KaiHeiLa.Rest;
using Reaction = KaiHeiLa.API.Gateway.Reaction;

namespace KaiHeiLa.WebSocket;

public partial class KaiHeiLaSocketClient : BaseSocketClient, IKaiHeiLaClient
{
    #region KaiHeiLaSocketClient

    private readonly JsonSerializerOptions _serializerOptions;

    private readonly ConcurrentQueue<long> _heartbeatTimes;
    private readonly ConnectionManager _connection;
    private readonly Logger _gatewayLogger;
    private readonly SemaphoreSlim _stateLock;

    private Guid _sessionId;
    private int _lastSeq = 0;
    private Task _heartbeatTask, _guildDownloadTask;
    private int _unavailableGuildCount;
    private long _lastGuildAvailableTime, _lastMessageTime;

    private bool _isDisposed;

    public override KaiHeiLaSocketRestClient Rest { get; }
    public ConnectionState ConnectionState => _connection.State;
    public override int Latency { get; protected set; }

    #endregion

    // From KaiHeiLaSocketConfig
    internal int MessageCacheSize { get; private set; }
    internal ClientState State { get; private set; }
    internal WebSocketProvider WebSocketProvider { get; private set; }
    internal bool AlwaysDownloadUsers { get; private set; }
    internal int? HandlerTimeout { get; private set; }
    internal new KaiHeiLaSocketApiClient ApiClient => base.ApiClient;
    /// <inheritdoc />
    public override IReadOnlyCollection<SocketGuild> Guilds => State.Guilds;

    public KaiHeiLaSocketClient(KaiHeiLaSocketConfig config) : this(config, CreateApiClient(config)) { }

    private KaiHeiLaSocketClient(KaiHeiLaSocketConfig config, KaiHeiLaSocketApiClient client)
        : base(config, client)
    {
        MessageCacheSize = config.MessageCacheSize;
        WebSocketProvider = config.WebSocketProvider;
        AlwaysDownloadUsers = config.AlwaysDownloadUsers;
        HandlerTimeout = config.HandlerTimeout;
        State = new ClientState(0, 0);
        Rest = new KaiHeiLaSocketRestClient(config, ApiClient);
        _heartbeatTimes = new ConcurrentQueue<long>();

        _stateLock = new SemaphoreSlim(1, 1);
        _gatewayLogger = LogManager.CreateLogger("Gateway");
        _connection = new ConnectionManager(_stateLock, _gatewayLogger, config.ConnectionTimeout,
            OnConnectingAsync, OnDisconnectingAsync, x => ApiClient.Disconnected += x);
        _connection.Connected += () => TimedInvokeAsync(_connectedEvent, nameof(Connected));
        _connection.Disconnected += (ex, recon) => TimedInvokeAsync(_disconnectedEvent, nameof(Disconnected), ex);

        _serializerOptions = new JsonSerializerOptions {Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping};

        ApiClient.SentGatewayMessage += async socketFrameType =>
            await _gatewayLogger.DebugAsync($"Sent {socketFrameType}").ConfigureAwait(false);
        ApiClient.ReceivedGatewayEvent += ProcessMessageAsync;
        
        // LeftGuild += async g => await _gatewayLogger.InfoAsync($"Left {g.Name}").ConfigureAwait(false);
        // JoinedGuild += async g => await _gatewayLogger.InfoAsync($"Joined {g.Name}").ConfigureAwait(false);
        GuildAvailable += async g => await _gatewayLogger.VerboseAsync($"Connected to {g.Name}").ConfigureAwait(false);
        GuildUnavailable += async g => await _gatewayLogger.VerboseAsync($"Disconnected from {g.Name}").ConfigureAwait(false);
        LatencyUpdated += async (old, val) => await _gatewayLogger.DebugAsync($"Latency = {val} ms").ConfigureAwait(false);

        GuildAvailable += g =>
        {
            if (_guildDownloadTask?.IsCompleted == true && ConnectionState == ConnectionState.Connected && AlwaysDownloadUsers && !g.HasAllMembers)
            {
                var _ = g.DownloadUsersAsync();
            }
            return Task.Delay(0);
        };
    }

    private static KaiHeiLaSocketApiClient CreateApiClient(KaiHeiLaSocketConfig config)
        => new KaiHeiLaSocketApiClient(config.RestClientProvider, config.WebSocketProvider, KaiHeiLaRestConfig.UserAgent, 
            config.GatewayHost, defaultRatelimitCallback: config.DefaultRatelimitCallback);

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
            await _gatewayLogger.DebugAsync("Connecting ApiClient").ConfigureAwait(false);
            await ApiClient.ConnectAsync().ConfigureAwait(false);
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
        var heartbeatTask = _heartbeatTask;
        if (heartbeatTask != null)
            await heartbeatTask.ConfigureAwait(false);
        _heartbeatTask = null;

        while (_heartbeatTimes.TryDequeue(out _)) { }
        _lastMessageTime = 0;
        
        //Raise virtual GUILD_UNAVAILABLEs
        await _gatewayLogger.DebugAsync("Raising virtual GuildUnavailables").ConfigureAwait(false);
        foreach (var guild in State.Guilds)
        {
            if (guild.IsAvailable)
                await GuildUnavailableAsync(guild).ConfigureAwait(false);
        }
    }
    
    /// <inheritdoc />
    public override SocketGuild GetGuild(ulong id)
        => State.GetGuild(id);
    /// <inheritdoc />
    public override SocketChannel GetChannel(ulong id)
        => State.GetChannel(id);
    /// <summary>
    ///     Gets a generic channel from the cache or does a rest request if unavailable.
    /// </summary>
    /// <example>
    ///     <code language="cs" title="Example method">
    ///     var channel = await _client.GetChannelAsync(381889909113225237);
    ///     if (channel != null &amp;&amp; channel is IMessageChannel msgChannel)
    ///     {
    ///         await msgChannel.SendMessageAsync($"{msgChannel} is created at {msgChannel.CreatedAt}");
    ///     }
    ///     </code>
    /// </example>
    /// <param name="id">The identifier of the channel (e.g. `381889909113225237`).</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the channel associated
    ///     with the identifier; <c>null</c> when the channel cannot be found.
    /// </returns>
    public async ValueTask<IChannel> GetChannelAsync(ulong id, RequestOptions options = null)
        => GetChannel(id) ?? (IChannel)await ClientHelper.GetChannelAsync(this, id, options).ConfigureAwait(false);
    
    /// <inheritdoc />
    public override SocketUser GetUser(ulong id)
        => State.GetUser(id);
    /// <inheritdoc />
    public override SocketUser GetUser(string username, string discriminator)
        => State.Users.FirstOrDefault(x => x.IdentifyNumber == discriminator && x.Username == username);
    
    internal SocketGlobalUser GetOrCreateUser(ClientState state, KaiHeiLa.API.User model)
    {
        return state.GetOrAddUser(model.Id, x => SocketGlobalUser.Create(this, state, model));
    }
    internal SocketUser GetOrCreateTemporaryUser(ClientState state, KaiHeiLa.API.User model)
    {
        return state.GetUser(model.Id) ?? (SocketUser)SocketUnknownUser.Create(this, state, model.Id);
    }
    internal SocketGlobalUser GetOrCreateSelfUser(ClientState state, KaiHeiLa.API.User model)
    {
        return state.GetOrAddUser(model.Id, x =>
        {
            var user = SocketGlobalUser.Create(this, state, model);
            user.GlobalUser.AddRef();
            return user;
        });
    }
    internal void RemoveUser(ulong id)
        => State.RemoveUser(id);
    
    public async Task DownloadUsersAsync(IEnumerable<IGuild> guilds)
    {
        if (ConnectionState == ConnectionState.Connected)
        {
            // EnsureGatewayIntent(GatewayIntents.GuildMembers);
    
            //Race condition leads to guilds being requested twice, probably okay
            await ProcessUserDownloadsAsync(guilds.Select(x => GetGuild(x.Id)).Where(x => x != null)).ConfigureAwait(false);
        }
    }
    private async Task ProcessUserDownloadsAsync(IEnumerable<SocketGuild> guilds)
    {
        foreach (SocketGuild socketGuild in guilds)
        {
            IReadOnlyCollection<GuildMember> guildMembers = await ApiClient.GetGuildMembersAsync(socketGuild.Id).ConfigureAwait(false);
            socketGuild.Update(State, guildMembers);
        }
    }
    
    #region ProcessMessageAsync
    
    private async Task ProcessMessageAsync(SocketFrameType socketFrameType, int? sequence, object payload)
    {
        if (sequence != null)
            _lastSeq = sequence.Value;
        _lastMessageTime = Environment.TickCount;

        try
        {
            switch (socketFrameType)
            {
                case SocketFrameType.Event:
                    GatewayEvent gatewayEvent =
                        ((JsonElement) payload).Deserialize<GatewayEvent>(_serializerOptions);

                    dynamic eventExtraData = gatewayEvent!.Type switch
                    {
                        MessageType.System => ((JsonElement) gatewayEvent.ExtraData)
                            .Deserialize<GatewaySystemEventExtraData>(_serializerOptions),
                        _  when gatewayEvent.ChannelType == "GROUP" => ((JsonElement) gatewayEvent.ExtraData).Deserialize<GatewayGroupMessageExtraData>(
                            _serializerOptions),
                        _  when gatewayEvent.ChannelType == "PERSON" => ((JsonElement) gatewayEvent.ExtraData).Deserialize<GatewayPersonMessageExtraData>(
                            _serializerOptions),
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
                            await _gatewayLogger.DebugAsync("Received Message (Text)").ConfigureAwait(false);
                            if (gatewayEvent.ChannelType == "GROUP")
                            {
                                GatewayGroupMessageExtraData extraData = eventExtraData as GatewayGroupMessageExtraData;
                            
                                var channel = GetChannel(gatewayEvent.TargetId) as ISocketMessageChannel;
                                SocketGuild guild = (channel as SocketGuildChannel)?.Guild;

                                SocketUser author;
                                if (guild != null)
                                    author = guild.GetUser(extraData.Author.Id);
                                else
                                    author = (channel as SocketChannel).GetUser(extraData.Author.Id);
                                if (author == null)
                                {
                                    if (guild != null)
                                    {
                                        author = guild.AddOrUpdateUser(extraData.Author);
                                    }
                                    else
                                    {
                                        await UnknownChannelUserAsync(gatewayEvent.Type.ToString(), extraData.Author.Id, channel.Id).ConfigureAwait(false);
                                        return;
                                    }
                                }
                            
                                var msg = SocketMessage.Create(this, State, author, channel, extraData, gatewayEvent);
                                SocketChannelHelper.AddMessage(channel, this, msg);
                                await TimedInvokeAsync(_messageReceivedEvent, nameof(MessageReceived), msg).ConfigureAwait(false);
                            }
                            else if (gatewayEvent.ChannelType == "PERSON")
                            {
                                GatewayPersonMessageExtraData extraData = eventExtraData as GatewayPersonMessageExtraData;
                                var channel = CreateDMChannel(extraData.Code, extraData.Author, State);
                                
                                SocketUser author = (channel as SocketChannel).GetUser(extraData.Author.Id);
                                if (author == null)
                                {
                                    await UnknownChannelUserAsync(gatewayEvent.Type.ToString(), extraData.Author.Id, extraData.Code).ConfigureAwait(false);
                                    return;
                                }
                                var msg = SocketMessage.Create(this, State, author, channel, extraData, gatewayEvent);
                                SocketChannelHelper.AddMessage(channel, this, msg);
                                await TimedInvokeAsync(_messageReceivedEvent, nameof(MessageReceived), msg).ConfigureAwait(false);
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
                                    
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.Reaction>(_serializerOptions);
                                    var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;
                                    
                                    var cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                    bool isMsgCached = cachedMsg is not null;
                                    var optionalMsg = !isMsgCached
                                        ? Optional.Create<SocketUserMessage>()
                                        : Optional.Create(cachedMsg);
                                    
                                    IUser user = channel is not null
                                        ? await channel.GetUserAsync(data.UserId, CacheMode.CacheOnly).ConfigureAwait(false)
                                        : GetUser(data.UserId);
                                    var optionalUser = user is null
                                        ? Optional.Create<IUser>()
                                        : Optional.Create(user);
                                    
                                    var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);
                                    var cacheableMsg = new Cacheable<IUserMessage, Guid>(cachedMsg, data.MessageId, isMsgCached, async () =>
                                    {
                                        var channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                        return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false) as IUserMessage;
                                    });
                                    var reaction = SocketReaction.Create(data, channel, optionalMsg, optionalUser);
                                    
                                    cachedMsg?.AddReaction(reaction);
                                    
                                    await TimedInvokeAsync(_reactionAddedEvent, nameof(ReactionAdded), cacheableMsg, cacheableChannel, reaction).ConfigureAwait(false);
                                }
                                    break;

                                #endregion

                                default:
                                    await _gatewayLogger.WarningAsync($"Unknown SystemEventType ({extraData.Type})")
                                        .ConfigureAwait(false);
                                    break;
                            }
                        }
                            break;
                        default:
                            await _gatewayLogger.WarningAsync($"Unknown Event Type ({gatewayEvent.Type})")
                                .ConfigureAwait(false);
                            break;
                    }

                    break;

                case SocketFrameType.Hello:
                {
                    // Process Hello
                    await _gatewayLogger.DebugAsync("Received Hello").ConfigureAwait(false);
                    try
                    {
                        GatewayHelloPayload gatewayHelloPayload =
                            ((JsonElement) payload).Deserialize<GatewayHelloPayload>(_serializerOptions);
                        _sessionId = gatewayHelloPayload?.SessionId ?? Guid.Empty;
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
                        SelfUser selfUser = await ApiClient.GetSelfUserAsync();
                        var currentUser = SocketSelfUser.Create(this, State, selfUser);
                        ApiClient.CurrentUserId = currentUser.Id;
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
                        IReadOnlyCollection<Guild> guilds = await ApiClient.GetGuildsAsync();
                        var state = new ClientState(guilds.Count, 0);
                        int unavailableGuilds = 0;
                        foreach (Guild guild in guilds)
                        {
                            var model = guild;
                            var socketGuild = AddGuild(model, state);
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
                    
                    _lastGuildAvailableTime = Environment.TickCount;
                    _guildDownloadTask = WaitForGuildsAsync(_connection.CancelToken, _gatewayLogger)
                        .ContinueWith(async x =>
                        {
                            if (x.IsFaulted)
                            {
                                _connection.Error(x.Exception);
                                return;
                            }
                            else if (_connection.CancelToken.IsCancellationRequested)
                                return;
                            
                            foreach (SocketGuild socketGuild in State.Guilds)
                                socketGuild.MemberCount = await ApiClient.GetGuildMemberCountAsync(socketGuild.Id);
                            
                            if (BaseConfig.AlwaysDownloadUsers)
                                _ = DownloadUsersAsync(Guilds.Where(x => x.IsAvailable && !x.HasAllMembers));

                            await TimedInvokeAsync(_readyEvent, nameof(Ready)).ConfigureAwait(false);
                            await _gatewayLogger.InfoAsync("Ready").ConfigureAwait(false);
                        });

                    _ = _connection.CompleteAsync();
                    
                    // Download extended guild data
                    try
                    {
                        foreach (SocketGuild socketGuild in State.Guilds)
                        {
                            ExtendedGuild model = await ApiClient.GetGuildAsync(socketGuild.Id);
                            if (model is not null)
                            {
                                socketGuild.Update(State, model);
                                
                                if (_unavailableGuildCount != 0)
                                    _unavailableGuildCount--;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _connection.CriticalError(new Exception("Processing Guilds failed", ex));
                        return;
                    }
                }
                    break;

                case SocketFrameType.Pong:
                {
                    await _gatewayLogger.DebugAsync("Received Pong").ConfigureAwait(false);
                    if (_heartbeatTimes.TryDequeue(out long time))
                    {
                        int latency = (int) (Environment.TickCount - time);
                        int before = Latency;
                        Latency = latency;

                        await TimedInvokeAsync(_latencyUpdatedEvent, nameof(LatencyUpdated), before, latency)
                            .ConfigureAwait(false);
                    }
                }
                    break;

                case SocketFrameType.Reconnect:
                {
                    await _gatewayLogger.DebugAsync("Received Reconnect").ConfigureAwait(false);
                    _connection.Error(new GatewayReconnectException("Server requested a reconnect"));
                }
                    break;

                case SocketFrameType.ResumeAck:
                {
                    await _gatewayLogger.DebugAsync("Received ResumeAck").ConfigureAwait(false);
                    _ = _connection.CompleteAsync();

                    //Notify the client that these guilds are available again
                    foreach (var guild in State.Guilds)
                    {
                        if (guild.IsAvailable)
                            await GuildAvailableAsync(guild).ConfigureAwait(false);
                    }

                    await _gatewayLogger.InfoAsync("Resumed previous session").ConfigureAwait(false);
                }
                    break;

                default:
                    await _gatewayLogger.WarningAsync($"Unknown Socket Frame Type ({socketFrameType})")
                        .ConfigureAwait(false);
                    break;
            }
        }
        catch (Exception ex)
        {
            await _gatewayLogger.ErrorAsync($"Error handling {socketFrameType}", ex).ConfigureAwait(false);
        }
    }

    #endregion

    public Task LoginAsync(TokenType tokenType, string token)
    {
        ApiClient.LoginAsync(tokenType, token);
        ApiClient.LoginState = LoginState.LoggedIn;
        return Task.CompletedTask;
    }

    public override async Task StartAsync() => await _connection.StartAsync().ConfigureAwait(false);
    public override async Task StopAsync() => await _connection.StopAsync().ConfigureAwait(false);

    private async Task RunHeartbeatAsync(CancellationToken cancelToken)
    {
        int intervalMillis = KaiHeiLaSocketConfig.HeartbeatIntervalMilliseconds;
        try
        {
            await _gatewayLogger.DebugAsync("Heartbeat Started").ConfigureAwait(false);
            while (!cancelToken.IsCancellationRequested)
            {
                int now = Environment.TickCount;

                //Did server respond to our last heartbeat, or are we still receiving messages (long load?)
                if (_heartbeatTimes.IsEmpty && (now - _lastMessageTime) > intervalMillis)
                {
                    if (ConnectionState == ConnectionState.Connected)
                    {
                        _connection.Error(new GatewayReconnectException("Server missed last heartbeat"));
                        return;
                    }
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
            while ((_unavailableGuildCount != 0) && (Environment.TickCount - _lastGuildAvailableTime < BaseConfig.MaxWaitBetweenGuildAvailablesBeforeReady))
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
    
    internal SocketGuild AddGuild(API.Guild model, ClientState state)
    {
        var guild = SocketGuild.Create(this, state, model);
        state.AddGuild(guild);
        return guild;
    }
    internal SocketGuild RemoveGuild(ulong id)
        => State.RemoveGuild(id);

    internal SocketDMChannel CreateDMChannel(Guid chatCode, API.User model, ClientState state)
    {
        return SocketDMChannel.Create(this, state, chatCode, model);
    }
    internal SocketDMChannel CreateDMChannel(Guid chatCode, SocketUser user, ClientState state)
    {
        return new SocketDMChannel(this, chatCode, user);
    }
    
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
            var timeoutTask = Task.Delay(HandlerTimeout.Value);
            var handlersTask = action();
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
    
    private async Task UnknownChannelUserAsync(string evnt, ulong userId, ulong channelId)
    {
        string details = $"{evnt} User={userId} Channel={channelId}";
        await _gatewayLogger.WarningAsync($"Unknown User ({details}).").ConfigureAwait(false);
    }
    private async Task UnknownChannelUserAsync(string evnt, ulong userId, Guid chatCode)
    {
        string details = $"{evnt} User={userId} ChatCode={chatCode}";
        await _gatewayLogger.WarningAsync($"Unknown User ({details}).").ConfigureAwait(false);
    }
    private async Task UnsyncedGuildAsync(string evnt, ulong guildId)
    {
        string details = $"{evnt} Guild={guildId}";
        await _gatewayLogger.DebugAsync($"Unsynced Guild ({details}).").ConfigureAwait(false);
    }
}