using System.Text.Json;
using Kook.API.Gateway;
using Kook.API.Rest;
using Kook.Rest;

namespace Kook.WebSocket;

public partial class KookSocketClient
{
    #region Gateway

    private async Task HandleGatewayHelloAsync(JsonElement payload)
    {
        if (DeserializePayload<GatewayHelloPayload>(payload) is not { } gatewayHelloPayload) return;
        await _gatewayLogger.DebugAsync("Received Hello").ConfigureAwait(false);
        try
        {
            _sessionId = gatewayHelloPayload.SessionId;
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
            _unavailableGuildCount = 0;
            foreach (RichGuild guild in guilds)
            {
                SocketGuild socketGuild = AddGuild(guild, state);
                if (!socketGuild.IsAvailable)
                    _unavailableGuildCount++;
                else
                    await GuildAvailableAsync(socketGuild).ConfigureAwait(false);
            }

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
                    Exception exception = task.Exception
                        ?? new Exception("Waiting for guilds failed without an exception");
                    _connection.Error(exception);
                    return;
                }

                if (_connection.CancellationToken.IsCancellationRequested) return;

                // Download user list if enabled
                if (BaseConfig.AlwaysDownloadUsers)
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

                if (BaseConfig.AlwaysDownloadVoiceStates)
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

                if (BaseConfig.AlwaysDownloadBoostSubscriptions)
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

    private async Task HandlePongAsync()
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

    private async Task HandleReconnectAsync(JsonElement payload)
    {
        GatewayReconnectPayload? reconnectPayload = DeserializePayload<GatewayReconnectPayload>(payload);
        await _gatewayLogger.DebugAsync("Received Reconnect").ConfigureAwait(false);
        if (reconnectPayload?.Code is KookErrorCode.MissingResumeArgument
            or KookErrorCode.SessionExpired
            or KookErrorCode.InvalidSequenceNumber)
        {
            _sessionId = null;
            _lastSeq = 0;
        }

        string reason = reconnectPayload?.Message != null && !string.IsNullOrWhiteSpace(reconnectPayload.Message)
            ? $": {reconnectPayload.Message}"
            : ".";
        _connection.Error(new GatewayReconnectException($"Server requested a reconnect, resuming session failed{reason}"));
    }

    private async Task HandleResumeAckAsync()
    {
        await _gatewayLogger.DebugAsync("Received ResumeAck").ConfigureAwait(false);
        _ = _connection.CompleteAsync();

        //Notify the client that these guilds are available again
        foreach (SocketGuild guild in State.Guilds)
        {
            if (guild.IsAvailable)
                await GuildAvailableAsync(guild).ConfigureAwait(false);
        }

        await _gatewayLogger.InfoAsync("Resumed previous session").ConfigureAwait(false);
    }

    #endregion

    #region Messages

    private async Task HandleGroupMessage(JsonElement payload)
    {
        const string channelType = "GROUP";
        if (DeserializePayload<GatewayEvent<GatewayGroupMessageExtraData>>(payload) is not { } gatewayEvent) return;
        if (GetGuild(gatewayEvent.ExtraData.GuildId) is not { } guild)
        {
            await UnknownGuildAsync($"{gatewayEvent.Type.ToString()}: {channelType}", gatewayEvent.ExtraData.GuildId, payload)
                .ConfigureAwait(false);
            return;
        }
        if (guild.GetTextChannel(gatewayEvent.TargetId) is not { } channel)
        {
            await UnknownChannelAsync($"{gatewayEvent.Type.ToString()}: {channelType}", gatewayEvent.TargetId, payload)
                .ConfigureAwait(false);
            return;
        }
        SocketGuildUser author = guild.GetUser(gatewayEvent.ExtraData.Author.Id)
            ?? guild.AddOrUpdateUser(gatewayEvent.ExtraData.Author);
        SocketMessage msg = SocketMessage.Create(this, State, author, channel, gatewayEvent);
        SocketChannelHelper.AddMessage(channel, this, msg);
        await TimedInvokeAsync(_messageReceivedEvent, nameof(MessageReceived), msg, author, channel).ConfigureAwait(false);
    }

    private async Task HandlePersonMessage(JsonElement payload)
    {
        const string channelType = "PERSON";
        if (DeserializePayload<GatewayEvent<GatewayPersonMessageExtraData>>(payload) is not { } gatewayEvent) return;
        SocketUser author = State.GetOrAddUser(gatewayEvent.ExtraData.Author.Id,
            _ => SocketGlobalUser.Create(this, State, gatewayEvent.ExtraData.Author));
        SocketDMChannel channel = GetDMChannel(gatewayEvent.ExtraData.Code)
            ?? AddDMChannel(gatewayEvent.ExtraData.Code, gatewayEvent.ExtraData.Author, State);
        if (author == null)
        {
            await UnknownChannelUserAsync($"{gatewayEvent.Type.ToString()}: {channelType}",
                    gatewayEvent.ExtraData.Author.Id, gatewayEvent.ExtraData.Code, payload)
                .ConfigureAwait(false);
            return;
        }
        SocketMessage msg = SocketMessage.Create(this, State, author, channel, gatewayEvent);
        SocketChannelHelper.AddMessage(channel, this, msg);
        await TimedInvokeAsync(_directMessageReceivedEvent, nameof(DirectMessageReceived), msg, author, channel).ConfigureAwait(false);
    }

    #endregion

    private async Task HandleAddedReaction(JsonElement payload)
    {
        if (DeserializePayload<Reaction>(payload) is not { } data) return;
        if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
        {
            await UnknownChannelAsync("added_reaction", data.ChannelId, payload).ConfigureAwait(false);
            return;
        }

        SocketUserMessage? cachedMsg = channel.GetCachedMessage(data.MessageId) as SocketUserMessage;
        SocketUser? user = GetUser(data.UserId);
        SocketGuildUser? socketGuildUser = channel.GetUser(data.UserId);
        SocketReaction reaction = SocketReaction.Create(data, channel, cachedMsg, socketGuildUser ?? user);
        Cacheable<SocketGuildUser, ulong> cacheableUser =
            new(socketGuildUser, data.UserId, socketGuildUser != null,
                async () =>
                {
                    GuildMember model = await ApiClient
                        .GetGuildMemberAsync(channel.Guild.Id, data.UserId)
                        .ConfigureAwait(false);
                    SocketGuildUser updatedUser = channel.Guild.AddOrUpdateUser(model);
                    reaction.User = updatedUser;
                    return updatedUser;
                });
        Cacheable<IMessage, Guid> cacheableMsg =
            new(cachedMsg, data.MessageId, cachedMsg is not null,
                async () =>
                {
                    IMessage message = await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false);
                    reaction.Message = message;
                    return message;
                });
        cachedMsg?.AddReaction(reaction);

        await TimedInvokeAsync(_reactionAddedEvent, nameof(ReactionAdded),
                cacheableMsg, channel, cacheableUser, reaction)
            .ConfigureAwait(false);
    }

    #region Channels

    private async Task HandleDeletedReaction(JsonElement payload)
    {
        if (DeserializePayload<Reaction>(payload) is not { } data) return;
        if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
        {
            await UnknownChannelAsync("deleted_reaction", data.ChannelId, payload).ConfigureAwait(false);
            return;
        }

        SocketUserMessage? cachedMsg = channel.GetCachedMessage(data.MessageId) as SocketUserMessage;
        SocketUser? user = GetUser(data.UserId);
        SocketGuildUser? socketGuildUser = channel.GetUser(data.UserId);
        SocketReaction reaction = SocketReaction.Create(data, channel, cachedMsg, socketGuildUser ?? user);
        Cacheable<SocketGuildUser, ulong> cacheableUser =
            new(socketGuildUser, data.UserId, socketGuildUser != null,
                async () =>
                {
                    GuildMember model = await ApiClient
                        .GetGuildMemberAsync(channel.Guild.Id, data.UserId)
                        .ConfigureAwait(false);
                    SocketGuildUser updatedUser = channel.Guild.AddOrUpdateUser(model);
                    reaction.User = updatedUser;
                    return updatedUser;
                });
        Cacheable<IMessage, Guid> cacheableMsg =
            new(cachedMsg, data.MessageId, cachedMsg is not null,
                async () =>
                {
                    IMessage message = await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false);
                    reaction.Message = message;
                    return message;
                });
        cachedMsg?.RemoveReaction(reaction);

        await TimedInvokeAsync(_reactionRemovedEvent, nameof(ReactionRemoved),
                cacheableMsg, channel, cacheableUser, reaction)
            .ConfigureAwait(false);
    }

    private async Task HandleUpdatedMessage(JsonElement payload)
    {
        if (DeserializePayload<MessageUpdateEvent>(payload) is not { } data) return;
        if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
        {
            await UnknownChannelAsync("updated_message", data.ChannelId, payload).ConfigureAwait(false);
            return;
        }
        SocketGuild guild = channel.Guild;

    }

    #endregion

    #region Helpers

    private T? DeserializePayload<T>(JsonElement jsonElement)
    {
        if (jsonElement.Deserialize<T>(_serializerOptions) is { } x) return x;
        string payloadJson = SerializePayload(jsonElement);
        _gatewayLogger.ErrorAsync($"Failed to deserialize JSON element to type {typeof(T).Name}: {payloadJson}");
        return default;
    }

    private string SerializePayload(JsonElement jsonElement) =>
        JsonSerializer.Serialize(jsonElement, _serializerOptions);

    #endregion

}
