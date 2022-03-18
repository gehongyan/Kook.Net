using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Net.Sockets;
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
        
        LeftGuild += async g => await _gatewayLogger.InfoAsync($"Left {g.Name}").ConfigureAwait(false);
        JoinedGuild += async g => await _gatewayLogger.InfoAsync($"Joined {g.Name}").ConfigureAwait(false);
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
    public SocketDMChannel GetDMChannel(Guid chatCode)
        => State.GetDMChannel(chatCode);
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
    
    public async ValueTask<IDMChannel> GetDMChannelAsync(Guid chatCode, RequestOptions options = null)
        => GetDMChannel(chatCode) ?? (IDMChannel)await ClientHelper.GetDMChannelAsync(this, chatCode, options).ConfigureAwait(false);
    
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

                                // 频道内用户取消 reaction
                                case ("GROUP", "deleted_reaction"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (deleted_reaction)").ConfigureAwait(false);
                                    
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
                                    
                                    cachedMsg?.RemoveReaction(reaction);
                                    
                                    await TimedInvokeAsync(_reactionRemovedEvent, nameof(ReactionRemoved), cacheableMsg, cacheableChannel, reaction).ConfigureAwait(false);
                                }
                                    break;
                                
                                // 频道消息更新
                                case ("GROUP", "updated_message"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (updated_message)").ConfigureAwait(false);
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.MessageUpdateEvent>(_serializerOptions);
                                    var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;
                                    var guild = (channel as SocketGuildChannel)?.Guild;

                                    SocketMessage before = null, after = null;
                                    SocketMessage cachedMsg = channel?.GetCachedMessage(data.MessageId);
                                    bool isCached = cachedMsg != null;
                                    if (isCached)
                                    {
                                        before = cachedMsg.Clone();
                                        cachedMsg.Update(State, data);
                                        after = cachedMsg;
                                    }
                                    else
                                    {
                                        Message msg = await ApiClient.GetMessageAsync(data.MessageId).ConfigureAwait(false);
                                        
                                        SocketUser author = guild.GetUser(msg.Author.Id) 
                                                            ?? (SocketUser) new SocketUnknownUser(this, id: 0);

                                        if (channel == null)
                                        {
                                            await UnknownChannelAsync(extraData.Type, data.ChannelId).ConfigureAwait(false);
                                            return;
                                        }

                                        after = SocketMessage.Create(this, State, author, channel, msg);
                                    }
                                    var cacheableBefore = new Cacheable<IMessage, Guid>(before, data.MessageId, isCached, async () => await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false));

                                    await TimedInvokeAsync(_messageUpdatedEvent, nameof(MessageUpdated), cacheableBefore, after, channel).ConfigureAwait(false);
                                }
                                    break;
                                
                                // 频道消息被删除
                                case ("GROUP", "deleted_message"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (updated_message)").ConfigureAwait(false);
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.MessageDeleteEvent>(_serializerOptions);
                                    var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;
                                    var guild = (channel as SocketGuildChannel)?.Guild;
                                    
                                    SocketMessage msg = null;
                                    if (channel != null)
                                        msg = SocketChannelHelper.RemoveMessage(channel, this, data.MessageId);
                                    var cacheableMsg = new Cacheable<IMessage, Guid>(msg, data.MessageId, msg != null, () => Task.FromResult((IMessage)null));
                                    var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);

                                    await TimedInvokeAsync(_messageDeletedEvent, nameof(MessageDeleted), cacheableMsg, cacheableChannel).ConfigureAwait(false);
                                }
                                    break;
                                
                                // 新增频道
                                case ("GROUP", "added_channel"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (added_channel)").ConfigureAwait(false);
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Channel>(_serializerOptions);
                                    SocketChannel channel = null;
                                    var guild = State.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        channel = guild.AddChannel(State, data);
                                    }
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
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Channel>(_serializerOptions);
                                    var channel = State.GetChannel(data.Id);
                                    if (channel != null)
                                    {
                                        var before = channel.Clone();
                                        channel.Update(State, data);

                                        var guild = (channel as SocketGuildChannel)?.Guild;
                                        await TimedInvokeAsync(_channelUpdatedEvent, nameof(ChannelUpdated), before, channel).ConfigureAwait(false);
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
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.ChannelDeleteEvent>(_serializerOptions);
                                    var channel = State.GetChannel(data.ChannelId);
                                    
                                    var guild = (channel as SocketGuildChannel)?.Guild;
                                    if (guild != null)
                                    {
                                        channel = guild.RemoveChannel(State, data.ChannelId);

                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, guildId: 0).ConfigureAwait(false);
                                        return;
                                    }

                                    if (channel != null)
                                        await TimedInvokeAsync(_channelDestroyedEvent, nameof(ChannelDestroyed), channel).ConfigureAwait(false);
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
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.PinnedMessageEvent>(_serializerOptions);
                                    var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;
                                    
                                    if (channel == null)
                                    {
                                        await UnknownChannelAsync(extraData.Type, data.ChannelId).ConfigureAwait(false);
                                        return;
                                    }
                                    
                                    var guild = (channel as SocketGuildChannel)?.Guild;
                                    if (guild != null)
                                    {
                                        SocketGuildUser operatorUser = guild.GetUser(data.OperatorUserId);
                                        if (operatorUser == null)
                                        {
                                            GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, data.OperatorUserId).ConfigureAwait(false);
                                            operatorUser = guild.AddOrUpdateUser(model);
                                        }    
                                        
                                        Cacheable<IMessage, Guid> cacheableBefore;
                                        SocketMessage after;
                                        
                                        SocketUserMessage cachedMsg = channel.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                        bool isCached = cachedMsg != null;
                                        if (isCached)
                                        {
                                            SocketMessage before = cachedMsg.Clone();
                                            cachedMsg.IsPinned = true;
                                            after = cachedMsg;
                                            
                                            cacheableBefore = new Cacheable<IMessage, Guid>(before, data.MessageId, true, async () => await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false));
                                            
                                        }
                                        else
                                        {
                                            Message msg = await ApiClient.GetMessageAsync(data.MessageId).ConfigureAwait(false);
                                            SocketUser author = guild.GetUser(msg.Author.Id) 
                                                                ?? (SocketUser) new SocketUnknownUser(this, id: 0);
                                            after = SocketMessage.Create(this, State, author, channel, msg);
                                            
                                            cacheableBefore = new Cacheable<IMessage, Guid>(null, data.MessageId, false, async () => await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false));
                                        }
                                        await TimedInvokeAsync(_messagePinnedEvent, nameof(MessagePinned), cacheableBefore, after, channel, operatorUser).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                    break;
                                
                                // 取消频道置顶消息
                                case ("GROUP", "unpinned_message"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (unpinned_message)").ConfigureAwait(false);
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.UnpinnedMessageEvent>(_serializerOptions);
                                    var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;
                                    
                                    if (channel == null)
                                    {
                                        await UnknownChannelAsync(extraData.Type, data.ChannelId).ConfigureAwait(false);
                                        return;
                                    }
                                    
                                    var guild = (channel as SocketGuildChannel)?.Guild;
                                    if (guild != null)
                                    {
                                        SocketGuildUser operatorUser = guild.GetUser(data.OperatorUserId);
                                        if (operatorUser == null)
                                        {
                                            GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, data.OperatorUserId).ConfigureAwait(false);
                                            operatorUser = guild.AddOrUpdateUser(model);
                                        }    
                                        
                                        Cacheable<IMessage, Guid> cacheableBefore;
                                        SocketMessage after;
                                        
                                        SocketUserMessage cachedMsg = channel.GetCachedMessage(data.MessageId) as SocketUserMessage;
                                        bool isCached = cachedMsg != null;
                                        if (isCached)
                                        {
                                            SocketMessage before = cachedMsg.Clone();
                                            cachedMsg.IsPinned = false;
                                            after = cachedMsg;
                                            
                                            cacheableBefore = new Cacheable<IMessage, Guid>(before, data.MessageId, true, async () => await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false));
                                            
                                        }
                                        else
                                        {
                                            Message msg = await ApiClient.GetMessageAsync(data.MessageId).ConfigureAwait(false);
                                            SocketUser author = guild.GetUser(msg.Author.Id) 
                                                                ?? (SocketUser) new SocketUnknownUser(this, id: 0);
                                            after = SocketMessage.Create(this, State, author, channel, msg);
                                            
                                            cacheableBefore = new Cacheable<IMessage, Guid>(null, data.MessageId, false, async () => await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false));
                                        }
                                        await TimedInvokeAsync(_messageUnpinnedEvent, nameof(MessageUnpinned), cacheableBefore, after, channel, operatorUser).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                    break;
                                
                                #endregion

                                #region Direct Messages

                                // 私聊消息更新
                                case ("PERSON", "updated_private_message"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (updated_private_message)").ConfigureAwait(false);
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.DirectMessageUpdateEvent>(_serializerOptions);
                                    var user = await ApiClient.GetUserAsync(data.AuthorId).ConfigureAwait(false);
                                    var channel = CreateDMChannel(data.ChatCode, user, State);
                                    
                                    SocketMessage before = null, after = null;
                                    SocketMessage cachedMsg = channel?.GetCachedMessage(data.MessageId);
                                    bool isCached = cachedMsg != null;
                                    if (isCached)
                                    {
                                        before = cachedMsg.Clone();
                                        cachedMsg.Update(State, data);
                                        after = cachedMsg;
                                    }
                                    else
                                    {
                                        var msg = await ApiClient.GetDirectMessageAsync(data.MessageId, chatCode: data.ChatCode).ConfigureAwait(false);
                                        SocketUser author = State.GetUser(msg?.AuthorId ?? 0) 
                                                            ?? (SocketUser) new SocketUnknownUser(this, id: 0);
                                        
                                        if (channel == null)
                                        {
                                            await UnknownPrivateChannelAsync(extraData.Type, data.ChatCode).ConfigureAwait(false);
                                            return;
                                        }
                                        
                                        after = SocketMessage.Create(this, State, author, channel, msg);
                                    }
                                    var cacheableBefore = new Cacheable<IMessage, Guid>(before, data.MessageId, isCached, async () => await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false));

                                    await TimedInvokeAsync(_directMessageUpdatedEvent, nameof(DirectMessageUpdated), cacheableBefore, after, channel).ConfigureAwait(false);
                                }
                                    break;
                                
                                // 私聊消息被删除
                                case ("PERSON", "deleted_private_message"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (deleted_private_message)").ConfigureAwait(false);
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.DirectMessageDeleteEvent>(_serializerOptions);
                                    var user = await ApiClient.GetUserAsync(data.AuthorId).ConfigureAwait(false);
                                    var channel = CreateDMChannel(data.ChatCode, user, State);
                                    
                                    SocketMessage msg = null;
                                    if (channel != null)
                                        msg = SocketChannelHelper.RemoveMessage(channel, this, data.MessageId);
                                    var cacheableMsg = new Cacheable<IMessage, Guid>(msg, data.MessageId, msg != null, () => Task.FromResult((IMessage)null));
                                    var cacheableChannel = new Cacheable<IDMChannel, Guid>(channel, data.ChatCode, channel != null, async () => await GetDMChannelAsync(data.ChatCode).ConfigureAwait(false));

                                    await TimedInvokeAsync(_directMessageDeletedEvent, nameof(DirectMessageDeleted), cacheableMsg, cacheableChannel).ConfigureAwait(false);
                                }
                                    break;
                                
                                // 私聊内用户添加 reaction
                                case ("PERSON", "private_added_reaction"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (private_added_reaction)").ConfigureAwait(false);
                                    
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.PrivateReaction>(_serializerOptions);
                                    var channel = GetDMChannel(data.ChatCode) as ISocketMessageChannel;
                                    
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
                                    
                                    var cacheableChannel = new Cacheable<IDMChannel, Guid>((IDMChannel) channel, data.ChatCode, channel != null, async () => await GetDMChannelAsync(data.ChatCode).ConfigureAwait(false));
                                    var cacheableMsg = new Cacheable<IUserMessage, Guid>(cachedMsg, data.MessageId, isMsgCached, async () =>
                                    {
                                        var channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                        return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false) as IUserMessage;
                                    });
                                    var reaction = SocketReaction.Create(data, (IDMChannel) channel, optionalMsg, optionalUser);
                                    
                                    cachedMsg?.AddReaction(reaction);

                                    await TimedInvokeAsync(_directReactionAddedEvent, nameof(DirectReactionAdded), cacheableMsg, cacheableChannel, reaction).ConfigureAwait(false);
                                }
                                    break;
                                
                                // 私聊内用户取消 reaction
                                case ("PERSON", "private_deleted_reaction"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (private_deleted_reaction)").ConfigureAwait(false);
                                    
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.PrivateReaction>(_serializerOptions);
                                    var channel = GetDMChannel(data.ChatCode) as ISocketMessageChannel;
                                    
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
                                    
                                    var cacheableChannel = new Cacheable<IDMChannel, Guid>((IDMChannel) channel, data.ChatCode, channel != null, async () => await GetDMChannelAsync(data.ChatCode).ConfigureAwait(false));
                                    var cacheableMsg = new Cacheable<IUserMessage, Guid>(cachedMsg, data.MessageId, isMsgCached, async () =>
                                    {
                                        var channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                        return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false) as IUserMessage;
                                    });
                                    var reaction = SocketReaction.Create(data, (IDMChannel) channel, optionalMsg, optionalUser);
                                    
                                    cachedMsg?.RemoveReaction(reaction);
                                    
                                    await TimedInvokeAsync(_directReactionRemovedEvent, nameof(DirectReactionRemoved), cacheableMsg, cacheableChannel, reaction).ConfigureAwait(false);
                                }
                                    break;

                                #endregion

                                #region Guild Members

                                // 新成员加入服务器
                                case ("GROUP", "joined_guild"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (joined_guild)").ConfigureAwait(false);
                                    var guild = State.GetGuild(gatewayEvent.TargetId);
                                    if (guild != null)
                                    {
                                        var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.GuildMemberAddEvent>(_serializerOptions);
                                        GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, data.UserId).ConfigureAwait(false);
                                        SocketGuildUser user = guild.AddOrUpdateUser(model);
                                        guild.MemberCount++;
                                        await TimedInvokeAsync(_userJoinedEvent, nameof(UserJoined), user, data.JoinedAt).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                    break;
                                
                                // 服务器成员退出
                                case ("GROUP", "exited_guild"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (exited_guild)").ConfigureAwait(false);
                                    var guild = State.GetGuild(gatewayEvent.TargetId);
                                    if (guild != null)
                                    {
                                        var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.GuildMemberRemoveEvent>(_serializerOptions);
                                        SocketUser user = guild.RemoveUser(data.UserId);
                                        guild.MemberCount--;
                                        
                                        user ??= State.GetUser(data.UserId);

                                        User model = await ApiClient.GetUserAsync(data.UserId).ConfigureAwait(false);
                                        if (user != null)
                                            user.Update(State, model);
                                        else
                                            user = State.GetOrAddUser(data.UserId, _ => SocketGlobalUser.Create(this, State, model));
                                        
                                        await TimedInvokeAsync(_userLeftEvent, nameof(UserLeft), guild, user, data.ExitedAt).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                    break;
                                
                                // 服务器成员信息更新
                                case ("GROUP", "updated_guild_member"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (updated_guild_member)").ConfigureAwait(false);
                                    var guild = State.GetGuild(gatewayEvent.TargetId);
                                    if (guild != null)
                                    {
                                        var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.GuildMemberUpdateEvent>(_serializerOptions);
                                        SocketGuildUser user = guild.GetUser(data.UserId);

                                        if (user != null)
                                        {
                                            // var globalBefore = user.GlobalUser.Clone();
                                            // if (user.GlobalUser.Update(State, model))
                                            // {
                                            //     //Global data was updated, trigger UserUpdated
                                            //     await TimedInvokeAsync(_userUpdatedEvent, nameof(UserUpdated), globalBefore, user).ConfigureAwait(false);
                                            // }
                                            
                                            var before = user.Clone();
                                            user.Update(State, data);
                                            
                                            var cacheableBefore = new Cacheable<SocketGuildUser, ulong>(before, user.Id, true, () => null);
                                            await TimedInvokeAsync(_guildMemberUpdatedEvent, nameof(GuildMemberUpdated), cacheableBefore, user).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, data.UserId).ConfigureAwait(false);
                                            user = guild.AddOrUpdateUser(model);
                                            var cacheableBefore = new Cacheable<SocketGuildUser, ulong>(null, user.Id, false, () => null);
                                            await TimedInvokeAsync(_guildMemberUpdatedEvent, nameof(GuildMemberUpdated), cacheableBefore, user).ConfigureAwait(false);
                                        }
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                    break;
                                
                                // 服务器成员上线
                                case ("GROUP", "guild_member_online"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (guild_member_online)").ConfigureAwait(false);
                                    var guild = State.GetGuild(gatewayEvent.TargetId);
                                    if (guild != null)
                                    {
                                        var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.GuildMemberOnlineEvent>(_serializerOptions);
                                        SocketGuildUser user = guild.GetUser(data.UserId);
                                        if (user != null)
                                        {
                                            if (user.IsOnline != true)
                                            {
                                                var before = user.Clone();
                                                user.IsOnline = true;
                                            
                                                var cacheableBefore = new Cacheable<SocketGuildUser, ulong>(before, user.Id, true, () => null);
                                                await TimedInvokeAsync(_guildMemberOnlineEvent, nameof(GuildMemberOnline), cacheableBefore, user, data.OnlineAt).ConfigureAwait(false);
                                            }
                                        }
                                        else
                                        {
                                            GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, data.UserId).ConfigureAwait(false);
                                            user = guild.AddOrUpdateUser(model);
                                            var cacheableBefore = new Cacheable<SocketGuildUser, ulong>(null, user.Id, false, () => null);
                                            await TimedInvokeAsync(_guildMemberOnlineEvent, nameof(GuildMemberOnline), cacheableBefore, user, data.OnlineAt).ConfigureAwait(false);
                                        }
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                    break;
                                
                                // 服务器成员下线
                                case ("GROUP", "guild_member_offline"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (guild_member_offline)").ConfigureAwait(false);
                                    var guild = State.GetGuild(gatewayEvent.TargetId);
                                    if (guild != null)
                                    {
                                        var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.GuildMemberOfflineEvent>(_serializerOptions);
                                        SocketGuildUser user = guild.GetUser(data.UserId);
                                        if (user != null)
                                        {
                                            if (user.IsOnline != true)
                                            {
                                                var before = user.Clone();
                                                user.IsOnline = false;
                                            
                                                var cacheableBefore = new Cacheable<SocketGuildUser, ulong>(before, user.Id, true, () => null);
                                                await TimedInvokeAsync(_guildMemberOfflineEvent, nameof(GuildMemberOffline), cacheableBefore, user, data.OfflineAt).ConfigureAwait(false);
                                            }
                                        }
                                        else
                                        {
                                            GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, data.UserId).ConfigureAwait(false);
                                            user = guild.AddOrUpdateUser(model);
                                            var cacheableBefore = new Cacheable<SocketGuildUser, ulong>(null, user.Id, false, () => null);
                                            await TimedInvokeAsync(_guildMemberOfflineEvent, nameof(GuildMemberOffline), cacheableBefore, user, data.OfflineAt).ConfigureAwait(false);
                                        }
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                    break;

                                #endregion

                                #region Guild Roles

                                // 服务器角色增加
                                case ("GROUP", "added_role"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (added_role)").ConfigureAwait(false);
                                    
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Role>(_serializerOptions);
                                    var guild = State.GetGuild(gatewayEvent.TargetId);
                                    if (guild != null)
                                    {
                                        var role = guild.AddRole(data);
                                        await TimedInvokeAsync(_roleCreatedEvent, nameof(RoleCreated), role).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                    break;
                                
                                // 服务器角色删除
                                case ("GROUP", "deleted_role"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (deleted_role)").ConfigureAwait(false);
                                    
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Role>(_serializerOptions);
                                    var guild = State.GetGuild(gatewayEvent.TargetId);
                                    if (guild != null)
                                    {
                                        var role = guild.RemoveRole(data.Id);
                                        await TimedInvokeAsync(_roleDeletedEvent, nameof(RoleDeleted), role).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                    break;
                                
                                // 服务器角色更新
                                case ("GROUP", "updated_role"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (updated_role)").ConfigureAwait(false);

                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Role>(_serializerOptions);
                                    var guild = State.GetGuild(gatewayEvent.TargetId);
                                    if (guild != null)
                                    {
                                        var role = guild.GetRole(data.Id);
                                        if (role != null)
                                        {
                                            var before = role.Clone();
                                            role.Update(State, data);

                                            await TimedInvokeAsync(_roleUpdatedEvent, nameof(RoleUpdated), before, role).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            await UnknownRoleAsync(extraData.Type, data.Id, guild.Id).ConfigureAwait(false);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                    break;

                                #endregion

                                #region Guilds

                                // 服务器信息更新
                                case ("GROUP", "updated_guild"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (updated_guild)").ConfigureAwait(false);

                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.GuildEvent>(_serializerOptions);
                                    var guild = State.GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        var before = guild.Clone();
                                        guild.Update(State, data);
                                        await TimedInvokeAsync(_guildUpdatedEvent, nameof(GuildUpdated), before, guild).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                    break;
                                
                                // 服务器删除
                                case ("GROUP", "deleted_guild"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (deleted_guild)").ConfigureAwait(false);
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.GuildEvent>(_serializerOptions);

                                    var guild = RemoveGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        await GuildUnavailableAsync(guild).ConfigureAwait(false);
                                        await TimedInvokeAsync(_leftGuildEvent, nameof(LeftGuild), guild).ConfigureAwait(false);
                                        ((IDisposable) guild).Dispose();
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                    break;
                                
                                // 服务器封禁用户
                                case ("GROUP", "added_block_list"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (added_block_list)").ConfigureAwait(false);
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.GuildBanEvent>(_serializerOptions);
                                    var guild = State.GetGuild(gatewayEvent.TargetId);
                                    if (guild != null)
                                    {
                                        SocketUser operatorUser = guild.GetUser(data.OperatorUserId) 
                                                                  ?? (SocketUser) SocketUnknownUser.Create(this, State, data.OperatorUserId);
                                        var bannedUsers = data.UserIds.Select(id => guild.GetUser(id) 
                                            ?? (SocketUser) SocketUnknownUser.Create(this, State, data.OperatorUserId))
                                            .ToReadOnlyCollection(() => data.UserIds.Length);
                                        await TimedInvokeAsync(_userBannedEvent, nameof(UserBanned), bannedUsers, operatorUser, guild).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                    break;
                                
                                // 服务器取消封禁用户
                                case ("GROUP", "deleted_block_list"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (deleted_block_list)").ConfigureAwait(false);
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.GuildBanEvent>(_serializerOptions);
                                    var guild = State.GetGuild(gatewayEvent.TargetId);
                                    if (guild != null)
                                    {
                                        SocketUser operatorUser = guild.GetUser(data.OperatorUserId) 
                                                                  ?? (SocketUser) SocketUnknownUser.Create(this, State, data.OperatorUserId);
                                        var bannedUsers = data.UserIds.Select(id => guild.GetUser(id) 
                                                ?? (SocketUser) SocketUnknownUser.Create(this, State, data.OperatorUserId))
                                            .ToReadOnlyCollection(() => data.UserIds.Length);
                                        await TimedInvokeAsync(_userBannedEvent, nameof(UserBanned), bannedUsers, operatorUser, guild).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                    break;

                                #endregion

                                #region Users

                                // 用户加入语音频道
                                case ("GROUP", "joined_channel"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (joined_channel)").ConfigureAwait(false);
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.UserVoiceEvent>(_serializerOptions);
                                    var guild = State.GetGuild(gatewayEvent.TargetId);
                                    if (guild != null)
                                    {
                                        var channel = GetChannel(gatewayEvent.TargetId) as SocketVoiceChannel;
                                        
                                        if (channel == null)
                                        {
                                            await UnknownChannelAsync(extraData.Type, data.ChannelId).ConfigureAwait(false);
                                            return;
                                        }
                                        
                                        var user = guild.GetUser(data.UserId) 
                                                    ?? SocketUnknownUser.Create(this, State, data.UserId) as SocketUser;
                                        
                                        await TimedInvokeAsync(_userConnectedEvent, nameof(UserConnected), user, channel, guild, data.At).ConfigureAwait(false);
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
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.UserVoiceEvent>(_serializerOptions);
                                    var guild = State.GetGuild(gatewayEvent.TargetId);
                                    if (guild != null)
                                    {
                                        var channel = GetChannel(gatewayEvent.TargetId) as SocketVoiceChannel;
                                        
                                        if (channel == null)
                                        {
                                            await UnknownChannelAsync(extraData.Type, data.ChannelId).ConfigureAwait(false);
                                            return;
                                        }
                                        
                                        var user = guild.GetUser(data.UserId) 
                                                   ?? SocketUnknownUser.Create(this, State, data.UserId) as SocketUser;
                                        
                                        await TimedInvokeAsync(_userDisconnectedEvent, nameof(UserDisconnected), user, channel, guild, data.At).ConfigureAwait(false);
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
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.UserUpdateEvent>(_serializerOptions);
                                    if (data.UserId == CurrentUser.Id)
                                    {
                                        var before = CurrentUser.Clone();
                                        CurrentUser.Update(State, data);
                                        await TimedInvokeAsync(_selfUpdatedEvent, nameof(CurrentUserUpdated), before, CurrentUser).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        SocketUser user = GetUser(data.UserId);
                                        var before = user.Clone();
                                        user.Update(State, data);
                                        await TimedInvokeAsync(_userUpdatedEvent, nameof(UserUpdated), before, CurrentUser).ConfigureAwait(false);
                                    }
                                }
                                    break;
                                
                                // 自己新加入服务器
                                case ("PERSON", "self_joined_guild"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (self_joined_guild)").ConfigureAwait(false);
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.SelfGuildEvent>(_serializerOptions);

                                    ExtendedGuild model = await ApiClient.GetGuildAsync(data.GuildId).ConfigureAwait(false);
                                    var guild = AddGuild(model, State);
                                    guild.Update(State, model);
                                    await TimedInvokeAsync(_joinedGuildEvent, nameof(JoinedGuild), guild).ConfigureAwait(false);
                                    await GuildAvailableAsync(guild).ConfigureAwait(false);
                                }
                                    break;
                                
                                // 自己退出服务器
                                case ("PERSON", "self_exited_guild"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (self_exited_guild)").ConfigureAwait(false);
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.SelfGuildEvent>(_serializerOptions);

                                    var guild = RemoveGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        await GuildUnavailableAsync(guild).ConfigureAwait(false);
                                        await TimedInvokeAsync(_leftGuildEvent, nameof(LeftGuild), guild).ConfigureAwait(false);
                                        ((IDisposable) guild).Dispose();
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                    break;

                                #endregion

                                #region Interactions

                                // Card 消息中的 Button 点击事件
                                case ("PERSON", "message_btn_click"):
                                {
                                    await _gatewayLogger.DebugAsync("Received Event (message_btn_click)").ConfigureAwait(false);
                                    var data = ((JsonElement) extraData.Body).Deserialize<API.Gateway.MessageButtonClickEvent>(_serializerOptions);
                                    SocketTextChannel channel = GetChannel(data.ChannelId) as SocketTextChannel;
                                    SocketGuild guild = GetGuild(data.GuildId);
                                    if (guild != null)
                                    {
                                        if (channel == null)
                                        {
                                            await UnknownChannelAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                            return;
                                        }
                                        SocketUser user = channel.GetUser(data.UserId) 
                                                          ?? SocketUnknownUser.Create(this, State, data.UserId) as SocketUser;
                                        IMessage msg = await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false);
                                        await TimedInvokeAsync(_messageButtonClickedEvent, nameof(MessageButtonClicked), data.Value, user, msg, channel, guild).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await UnknownGuildAsync(extraData.Type, gatewayEvent.TargetId).ConfigureAwait(false);
                                        return;
                                    }
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
                        SelfUser selfUser = await ApiClient.GetSelfUserAsync().ConfigureAwait(false);
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
                        IReadOnlyCollection<Guild> guilds = await ApiClient.GetGuildsAsync().ConfigureAwait(false);
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
                                socketGuild.MemberCount = await ApiClient.GetGuildMemberCountAsync(socketGuild.Id).ConfigureAwait(false);
                            
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
                            ExtendedGuild model = await ApiClient.GetGuildAsync(socketGuild.Id).ConfigureAwait(false);
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

    // public async Task LoginAsync(TokenType tokenType, string token)
    // {
    //     await ApiClient.LoginAsync(tokenType, token);
    //     ApiClient.LoginState = LoginState.LoggedIn;
    //     // return Task.CompletedTask;
    // }

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
}