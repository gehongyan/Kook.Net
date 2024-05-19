using System.Collections.Immutable;
using System.Net;
using System.Text.Json;
using Kook.API;
using Kook.API.Gateway;
using Kook.API.Rest;
using Kook.Net;
using Kook.Rest;
using Reaction = Kook.API.Gateway.Reaction;

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
                            await _gatewayLogger
                                .WarningAsync("Downloading voice states failed", ex)
                                .ConfigureAwait(false);
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
                            await _gatewayLogger
                                .WarningAsync("Downloading boost subscriptions failed", ex)
                                .ConfigureAwait(false);
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

        string reason = reconnectPayload?.Message is not null && !string.IsNullOrWhiteSpace(reconnectPayload.Message)
            ? $": {reconnectPayload.Message}"
            : ".";
        GatewayReconnectException exception = new($"Server requested a reconnect, resuming session failed{reason}");
        _connection.Error(exception);
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

    private async Task HandleGroupMessage(GatewayEvent<GatewayGroupMessageExtraData> gatewayEvent)
    {
        if (GetGuild(gatewayEvent.ExtraData.GuildId) is not { } guild)
        {
            await UnknownGuildAsync($"{gatewayEvent.Type.ToString()}: {gatewayEvent.ExtraData.Type}",
                    gatewayEvent.ExtraData.GuildId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }
        if (guild.GetTextChannel(gatewayEvent.TargetId) is not { } channel)
        {
            await UnknownChannelAsync($"{gatewayEvent.Type.ToString()}: {gatewayEvent.ExtraData.Type}",
                    gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }
        SocketGuildUser author = guild.GetUser(gatewayEvent.ExtraData.Author.Id)
            ?? guild.AddOrUpdateUser(gatewayEvent.ExtraData.Author);
        SocketMessage msg = SocketMessage.Create(this, State, author, channel, gatewayEvent);
        SocketChannelHelper.AddMessage(channel, this, msg);
        await TimedInvokeAsync(_messageReceivedEvent, nameof(MessageReceived),
            msg, author, channel).ConfigureAwait(false);
    }

    private async Task HandlePersonMessage(GatewayEvent<GatewayPersonMessageExtraData> gatewayEvent)
    {
        SocketUser author = State.GetOrAddUser(gatewayEvent.ExtraData.Author.Id,
            _ => SocketGlobalUser.Create(this, State, gatewayEvent.ExtraData.Author));
        SocketDMChannel channel = GetDMChannel(gatewayEvent.ExtraData.Code)
            ?? AddDMChannel(gatewayEvent.ExtraData.Code, gatewayEvent.ExtraData.Author, State);
        if (author is null)
        {
            await UnknownChannelUserAsync($"{gatewayEvent.Type.ToString()}: {gatewayEvent.ExtraData.Type}",
                    gatewayEvent.ExtraData.Author.Id, gatewayEvent.ExtraData.Code, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }
        SocketMessage msg = SocketMessage.Create(this, State, author, channel, gatewayEvent);
        SocketChannelHelper.AddMessage(channel, this, msg);

        await TimedInvokeAsync(_directMessageReceivedEvent, nameof(DirectMessageReceived),
            msg, author, channel).ConfigureAwait(false);
    }

    #endregion

    #region Channels

    /// <remarks>
    ///     "GROUP", "added_reaction"
    /// </remarks>
    private async Task HandleAddedReaction(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<Reaction>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
        {
            await UnknownChannelAsync(gatewayEvent.ExtraData.Type, data.ChannelId, gatewayEvent.ExtraData.Body)
                .ConfigureAwait(false);
            return;
        }

        SocketUserMessage? cachedMsg = channel.GetCachedMessage(data.MessageId) as SocketUserMessage;
        SocketUser? user = GetUser(data.UserId);
        SocketGuildUser? socketGuildUser = channel.GetUser(data.UserId);
        SocketReaction reaction = SocketReaction.Create(data, channel, cachedMsg, socketGuildUser ?? user);
        Cacheable<SocketGuildUser, ulong> cacheableUser =
            new(socketGuildUser, data.UserId, socketGuildUser is not null,
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
            cacheableMsg, channel, cacheableUser, reaction).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "deleted_reaction"
    /// </remarks>
    private async Task HandleDeletedReaction(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<Reaction>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
        {
            await UnknownChannelAsync(gatewayEvent.ExtraData.Type, data.ChannelId, gatewayEvent).ConfigureAwait(false);
            return;
        }

        SocketUserMessage? cachedMsg = channel.GetCachedMessage(data.MessageId) as SocketUserMessage;
        SocketUser? user = GetUser(data.UserId);
        SocketGuildUser? socketGuildUser = channel.GetUser(data.UserId);
        SocketReaction reaction = SocketReaction.Create(data, channel, cachedMsg, socketGuildUser ?? user);
        Cacheable<SocketGuildUser, ulong> cacheableUser =
            new(socketGuildUser, data.UserId, socketGuildUser is not null,
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
            cacheableMsg, channel, cacheableUser, reaction).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "updated_message"
    /// </remarks>
    private async Task HandleUpdatedMessage(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<MessageUpdateEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
        {
            await UnknownChannelAsync(gatewayEvent.ExtraData.Type, data.ChannelId, gatewayEvent).ConfigureAwait(false);
            return;
        }

        SocketMessage? cachedMsg = channel.GetCachedMessage(data.MessageId);
        SocketMessage? before = cachedMsg?.Clone();
        cachedMsg?.Update(State, data);
        Cacheable<IMessage, Guid> cacheableBefore = new(before, data.MessageId, cachedMsg is not null,
            () => Task.FromResult<IMessage?>(null));
        Cacheable<IMessage, Guid> cacheableAfter = new(cachedMsg, data.MessageId, cachedMsg is not null,
            async () => await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false) as SocketMessage);

        await TimedInvokeAsync(_messageUpdatedEvent, nameof(MessageUpdated),
            cacheableBefore, cacheableAfter, channel).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "embeds_append"
    /// </remarks>
    private async Task HandleEmbedsAppend(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<EmbedsAppendEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
        {
            await UnknownChannelAsync(gatewayEvent.ExtraData.Type, data.ChannelId, gatewayEvent).ConfigureAwait(false);
            return;
        }

        SocketMessage? cachedMsg = channel.GetCachedMessage(data.MessageId);
        SocketMessage? before = cachedMsg?.Clone();
        cachedMsg?.Update(State, data);
        Cacheable<IMessage, Guid> cacheableBefore = new(before, data.MessageId, cachedMsg is not null,
            () => Task.FromResult<IMessage?>(null));
        Cacheable<IMessage, Guid> cacheableAfter = new(cachedMsg, data.MessageId, cachedMsg is not null,
            async () => await channel.GetMessageAsync(data.MessageId).ConfigureAwait(false) as SocketMessage);

        await TimedInvokeAsync(_messageUpdatedEvent, nameof(MessageUpdated),
            cacheableBefore, cacheableAfter, channel).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "deleted_message"
    /// </remarks>
    private async Task HandleDeletedMessage(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<MessageDeleteEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
        {
            await UnknownChannelAsync(gatewayEvent.ExtraData.Type, data.ChannelId, gatewayEvent).ConfigureAwait(false);
            return;
        }

        SocketMessage? msg = SocketChannelHelper.RemoveMessage(channel, this, data.MessageId);
        Cacheable<IMessage, Guid> cacheableMsg = new(msg, data.MessageId, msg is not null,
            () => Task.FromResult<IMessage?>(null));
        await TimedInvokeAsync(_messageDeletedEvent, nameof(MessageDeleted),
            cacheableMsg, channel).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "added_channel"
    /// </remarks>
    private async Task HandleAddedChannel(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<Channel>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, data.GuildId, gatewayEvent).ConfigureAwait(false);
            return;
        }

        SocketChannel channel = guild.AddChannel(State, data);

        await TimedInvokeAsync(_channelCreatedEvent, nameof(ChannelCreated), channel).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "updated_channel"
    /// </remarks>
    private async Task HandleUpdatedChannel(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<Channel>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (GetChannel(data.Id) is not { } channel)
        {
            await UnknownChannelAsync(gatewayEvent.ExtraData.Type, data.Id, gatewayEvent).ConfigureAwait(false);
            return;
        }

        SocketChannel before = channel.Clone();
        channel.Update(State, data);
        await TimedInvokeAsync(_channelUpdatedEvent, nameof(ChannelUpdated), before, channel).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "sort_channel"
    /// </remarks>
    private async Task HandleSortChannel(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (!BaseConfig.AutoUpdateChannelPositions) return;
        if (DeserializePayload<ChannelSortEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, data.GuildId, gatewayEvent).ConfigureAwait(false);
            return;
        }

        IEnumerable<Channel> models = await ApiClient.GetGuildChannelsAsync(guild.Id).FlattenAsync();
        List<ulong> existingChannelIds = [];
        foreach (Channel model in models)
        {
            existingChannelIds.Add(model.Id);
            if (guild.GetChannel(model.Id) is not { } guildChannel)
            {
                await UnknownChannelAsync(gatewayEvent.ExtraData.Type, model.Id, gatewayEvent).ConfigureAwait(false);
                continue;
            }

            SocketGuildChannel before = guildChannel.Clone();
            guildChannel.Update(State, model);
            if (before.Position != guildChannel.Position)
            {
                await TimedInvokeAsync(_channelUpdatedEvent, nameof(ChannelUpdated), before, guildChannel)
                    .ConfigureAwait(false);
            }
        }

        IEnumerable<SocketGuildChannel> missingChannels = guild.Channels
            .Where(x => !existingChannelIds.Contains(x.Id))
            .Select(x => guild.RemoveChannel(State, x.Id))
            .OfType<SocketGuildChannel>();
        foreach (SocketGuildChannel missingChannel in missingChannels)
        {
            await TimedInvokeAsync(_channelDestroyedEvent, nameof(ChannelDestroyed), missingChannel)
                .ConfigureAwait(false);
        }
    }

    /// <remarks>
    ///     "GROUP", "deleted_channel"
    /// </remarks>
    private async Task HandleDeletedChannel(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<ChannelDeleteEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (GetChannel(data.ChannelId) is not { } channel)
        {
            await UnknownChannelAsync(gatewayEvent.ExtraData.Type, data.ChannelId, gatewayEvent).ConfigureAwait(false);
            return;
        }

        State.RemoveChannel(channel.Id);
        if (channel is SocketVoiceChannel voiceChannel)
        {
            IEnumerable<Cacheable<SocketGuildUser, ulong>> connectedUsers = voiceChannel.Guild.VoiceStates
                .Where(x => x.Value.VoiceChannel?.Id == voiceChannel.Id)
                .Select(x => x.Key)
                .Select(x => GetCacheableSocketGuildUser(voiceChannel.Guild.GetUser(x), x, voiceChannel.Guild));
            foreach (Cacheable<SocketGuildUser, ulong> user in connectedUsers)
            {
                SocketVoiceState after = voiceChannel.Guild.RemoveVoiceState(user.Id) ?? SocketVoiceState.Default;
                SocketVoiceState before = after.Clone();
                after.Update(null);
                await TimedInvokeAsync(_userDisconnectedEvent, nameof(UserDisconnected),
                    user, voiceChannel, gatewayEvent.MessageTimestamp).ConfigureAwait(false);
                await TimedInvokeAsync(_userVoiceStateUpdatedEvent, nameof(UserVoiceStateUpdated),
                    user, before, after).ConfigureAwait(false);
            }
        }

        await TimedInvokeAsync(_channelDestroyedEvent, nameof(ChannelDestroyed), channel).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "batch_added_channel"
    /// </remarks>
    private async Task HandleBatchAddChannel(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<Channel[]>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent).ConfigureAwait(false);
            return;
        }

        foreach (Channel model in data)
        {
            SocketChannel channel = guild.AddChannel(State, model);
            await TimedInvokeAsync(_channelCreatedEvent, nameof(ChannelCreated), channel).ConfigureAwait(false);
        }
    }

    /// <remarks>
    ///     "GROUP", "batch_updated_channel"
    /// </remarks>
    private async Task HandleBatchUpdateChannel(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<ChannelBatchUpdateEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent).ConfigureAwait(false);
            return;
        }

        if (data.AddedChannel is { } addedChannel)
        {
            SocketChannel channel = guild.AddChannel(State, addedChannel);
            await TimedInvokeAsync(_channelCreatedEvent, nameof(ChannelCreated), channel).ConfigureAwait(false);
        }

        foreach (Channel updatedChannel in data.UpdatedChannels)
        {
            if (GetChannel(updatedChannel.Id) is not { } channel)
            {
                await UnknownChannelAsync(gatewayEvent.ExtraData.Type, updatedChannel.Id, gatewayEvent).ConfigureAwait(false);
                return;
            }
            SocketChannel before = channel.Clone();
            channel.Update(State, updatedChannel);
            await TimedInvokeAsync(_channelUpdatedEvent, nameof(ChannelUpdated), before, channel).ConfigureAwait(false);
        }
    }

    /// <remarks>
    ///     "GROUP", "batch_deleted_channel"
    /// </remarks>
    private async Task HandleBatchDeleteChannel(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {

        if (DeserializePayload<ChannelBatchDeleteEventItem[]>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent).ConfigureAwait(false);
            return;
        }

        foreach (ChannelBatchDeleteEventItem model in data)
        {
            if (guild.RemoveChannel(State, model.Id) is not { } channel)
            {
                await UnknownChannelAsync(gatewayEvent.ExtraData.Type, model.Id, gatewayEvent).ConfigureAwait(false);
                return;
            }
            await TimedInvokeAsync(_channelDestroyedEvent, nameof(ChannelDestroyed), channel).ConfigureAwait(false);
        }
    }

    /// <remarks>
    ///     "GROUP", "pinned_message"
    /// </remarks>
    private async Task HandlePinnedMessage(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<MessagePinEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
        {
            await UnknownChannelAsync(gatewayEvent.ExtraData.Type, data.ChannelId, gatewayEvent).ConfigureAwait(false);
            return;
        }

        SocketGuild guild = channel.Guild;
        SocketGuildUser? operatorUser = guild.GetUser(data.OperatorUserId);
        Cacheable<SocketGuildUser, ulong> cacheableOperatorUser =
            new(operatorUser, data.OperatorUserId, operatorUser is not null,
                async () =>
                {
                    GuildMember model = await ApiClient
                        .GetGuildMemberAsync(guild.Id, data.OperatorUserId).ConfigureAwait(false);
                    return guild.AddOrUpdateUser(model);
                });

        SocketUserMessage? cachedMsg = channel.GetCachedMessage(data.MessageId) as SocketUserMessage;
        SocketMessage? before = cachedMsg?.Clone();
        if (cachedMsg is not null)
            cachedMsg.IsPinned = true;

        Cacheable<IMessage, Guid> cacheableBefore = new(before, data.MessageId, before is not null,
            () => Task.FromResult<IMessage?>(null));
        Cacheable<IMessage, Guid> cacheableAfter = GetCacheableSocketMessage(cachedMsg, data.MessageId, channel);

        await TimedInvokeAsync(_messagePinnedEvent, nameof(MessagePinned),
            cacheableBefore, cacheableAfter, channel, cacheableOperatorUser).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "unpinned_message"
    /// </remarks>
    private async Task HandleUnpinnedMessage(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<MessagePinEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (GetChannel(data.ChannelId) is not SocketTextChannel channel)
        {
            await UnknownChannelAsync(gatewayEvent.ExtraData.Type, data.ChannelId, gatewayEvent).ConfigureAwait(false);
            return;
        }

        SocketGuild guild = channel.Guild;
        SocketGuildUser? operatorUser = guild.GetUser(data.OperatorUserId);
        Cacheable<SocketGuildUser, ulong> cacheableOperatorUser =
            GetCacheableSocketGuildUser(operatorUser, data.OperatorUserId, guild);
        SocketUserMessage? cachedMsg = channel.GetCachedMessage(data.MessageId) as SocketUserMessage;
        SocketMessage? before = cachedMsg?.Clone();
        if (cachedMsg is not null)
            cachedMsg.IsPinned = false;

        Cacheable<IMessage, Guid> cacheableBefore = new(before, data.MessageId, before is not null,
            () => Task.FromResult<IMessage?>(null));
        Cacheable<IMessage, Guid> cacheableAfter = GetCacheableSocketMessage(cachedMsg, data.MessageId, channel);

        await TimedInvokeAsync(_messageUnpinnedEvent, nameof(MessageUnpinned),
            cacheableBefore, cacheableAfter, channel, cacheableOperatorUser).ConfigureAwait(false);
    }

    #endregion

    #region Direct Messages

    /// <remarks>
    ///     "PERSON", "updated_private_message"
    /// </remarks>
    private async Task HandleUpdatedPrivateMessage(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<DirectMessageUpdateEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        SocketDMChannel? channel = GetDMChannel(data.ChatCode);
        Cacheable<SocketDMChannel, Guid> cacheableChannel =
            GetCacheableDMChannel(channel, data.ChatCode, data.AuthorId);
        SocketMessage? cachedMsg = channel?.GetCachedMessage(data.MessageId);
        SocketMessage? before = cachedMsg?.Clone();
        cachedMsg?.Update(State, data);
        Cacheable<IMessage, Guid> cacheableBefore = new(before, data.MessageId, before is not null,
            () => Task.FromResult<IMessage?>(null));
        Cacheable<IMessage, Guid> cacheableAfter = new(cachedMsg, data.MessageId, cachedMsg is not null,
            async () =>
            {
                DirectMessage msg = await ApiClient
                    .GetDirectMessageAsync(data.MessageId, data.ChatCode)
                    .ConfigureAwait(false);
                User userModel = msg.Author ?? await ApiClient.GetUserAsync(data.AuthorId).ConfigureAwait(false);
                SocketGlobalUser author = State.AddOrUpdateUser(data.AuthorId,
                    _ => SocketGlobalUser.Create(this, State, userModel),
                    (_, x) =>
                    {
                        x.Update(State, userModel);
                        return x;
                    });
                SocketDMChannel dmChannel = CreateDMChannel(data.ChatCode, author, State);
                return SocketMessage.Create(this, State, author, dmChannel, msg);
            });
        SocketUser? user = State.GetUser(data.AuthorId);
        Cacheable<SocketUser, ulong> cacheableUser = GetCacheableSocketUser(user, data.AuthorId);

        await TimedInvokeAsync(_directMessageUpdatedEvent, nameof(DirectMessageUpdated),
            cacheableBefore, cacheableAfter, cacheableUser, cacheableChannel).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "PERSON", "deleted_private_message"
    /// </remarks>
    private async Task HandleDeletedPrivateMessage(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<DirectMessageDeleteEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        SocketDMChannel? channel = GetDMChannel(data.ChatCode);
        Cacheable<SocketDMChannel, Guid> cacheableChannel =
            GetCacheableDMChannel(channel, data.ChatCode, data.AuthorId);
        SocketMessage? cachedMsg = channel?.GetCachedMessage(data.MessageId);
        Cacheable<IMessage, Guid> cacheableMsg = new(cachedMsg, data.MessageId, cachedMsg is not null,
            () => Task.FromResult<IMessage?>(null));
        SocketUser? user = State.GetUser(data.AuthorId);
        Cacheable<SocketUser, ulong> cacheableUser = GetCacheableSocketUser(user, data.AuthorId);

        await TimedInvokeAsync(_directMessageDeletedEvent, nameof(DirectMessageDeleted),
            cacheableMsg, cacheableUser, cacheableChannel).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "PERSON", "private_added_reaction"
    /// </remarks>
    private async Task HandlePrivateAddedReaction(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<PrivateReaction>(gatewayEvent.ExtraData.Body) is not { } data) return;
        SocketDMChannel? channel = GetDMChannel(data.ChatCode);
        SocketUserMessage? cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
        SocketUser? operatorUser = GetUser(data.UserId);
        Cacheable<SocketUser, ulong> cacheableUser = GetCacheableSocketUser(operatorUser, data.UserId);
        Cacheable<SocketDMChannel, Guid> cacheableChannel = GetCacheableDMChannel(channel, data.ChatCode);
        Cacheable<IMessage, Guid> cacheableMsg = new(cachedMsg, data.MessageId, cachedMsg is not null, async () =>
        {
            SocketDMChannel dmChannel;
            if (channel is not null)
                dmChannel = channel;
            else
            {
                UserChat userChat = await ApiClient.GetUserChatAsync(data.ChatCode).ConfigureAwait(false);
                dmChannel = CreateDMChannel(data.ChatCode, userChat.Recipient, State);
            }
            return await dmChannel.GetMessageAsync(data.MessageId).ConfigureAwait(false);
        });
        SocketReaction reaction = SocketReaction.Create(data, channel, cachedMsg, operatorUser);
        cachedMsg?.AddReaction(reaction);

        await TimedInvokeAsync(_directReactionAddedEvent, nameof(DirectReactionAdded),
            cacheableMsg, cacheableChannel, cacheableUser, reaction).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "PERSON", "private_deleted_reaction"
    /// </remarks>
    private async Task HandlePrivateDeletedReaction(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<PrivateReaction>(gatewayEvent.ExtraData.Body) is not { } data) return;
        SocketDMChannel? channel = GetDMChannel(data.ChatCode);
        SocketUserMessage? cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
        SocketUser? operatorUser = GetUser(data.UserId);
        Cacheable<SocketUser, ulong> cacheableUser = GetCacheableSocketUser(operatorUser, data.UserId);
        Cacheable<SocketDMChannel, Guid> cacheableChannel = GetCacheableDMChannel(channel, data.ChatCode);
        Cacheable<IMessage, Guid> cacheableMsg = new(cachedMsg, data.MessageId, cachedMsg is not null, async () =>
        {
            SocketDMChannel dmChannel;
            if (channel is not null)
                dmChannel = channel;
            else
            {
                UserChat userChat = await ApiClient.GetUserChatAsync(data.ChatCode).ConfigureAwait(false);
                dmChannel = CreateDMChannel(data.ChatCode, userChat.Recipient, State);
            }
            return await dmChannel.GetMessageAsync(data.MessageId).ConfigureAwait(false);
        });
        SocketReaction reaction = SocketReaction.Create(data, channel, cachedMsg, operatorUser);
        cachedMsg?.RemoveReaction(reaction);

        await TimedInvokeAsync(_directReactionRemovedEvent, nameof(DirectReactionRemoved),
            cacheableMsg, cacheableChannel, cacheableUser, reaction).ConfigureAwait(false);
    }

    #endregion

    #region Guild Members

    /// <remarks>
    ///     "GROUP", "joined_guild"
    /// </remarks>
    private async Task HandleJoinedGuild(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<GuildMemberAddEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }
        SocketGuildUser? user = AlwaysDownloadUsers ? await DownloadUserAsync() : null;
        Cacheable<SocketGuildUser, ulong> cacheableUser = new(user, data.UserId, user is not null, DownloadUserAsync);
        guild.MemberCount++;

        await TimedInvokeAsync(_userJoinedEvent, nameof(UserJoined),
            cacheableUser, data.JoinedAt).ConfigureAwait(false);
        return;

        async Task<SocketGuildUser?> DownloadUserAsync()
        {
            GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, data.UserId).ConfigureAwait(false);
            SocketGuildUser member = guild.AddOrUpdateUser(model);
            return member;
        }
    }

    /// <remarks>
    ///     "GROUP", "exited_guild"
    /// </remarks>
    private async Task HandleExitedGuild(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<GuildMemberRemoveEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }
        SocketUser? user = guild.RemoveUser(data.UserId) ?? State.GetUser(data.UserId) as SocketUser;
        guild.MemberCount--;
        GetCacheableSocketUser(user, data.UserId);
        Cacheable<SocketUser, ulong> cacheableUser = GetCacheableSocketUser(user, data.UserId);

        await TimedInvokeAsync(_userLeftEvent, nameof(UserLeft),
            guild, cacheableUser, data.ExitedAt).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "updated_guild_member"
    /// </remarks>
    private async Task HandleUpdatedGuildMember(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<GuildMemberUpdateEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        SocketGuildUser? user = guild.GetUser(data.UserId);
        SocketGuildUser? before = user?.Clone();
        user?.Update(State, data);
        Cacheable<SocketGuildUser, ulong> cacheableBefore = new(before, data.UserId, before is not null,
            () => Task.FromResult<SocketGuildUser?>(null));
        Cacheable<SocketGuildUser, ulong> cacheableAfter = GetCacheableSocketGuildUser(user, data.UserId, guild);

        await TimedInvokeAsync(_guildMemberUpdatedEvent, nameof(GuildMemberUpdated),
            cacheableBefore, cacheableAfter).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "PERSON", "guild_member_online"
    /// </remarks>
    private async Task HandleGuildMemberOnline(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<GuildMemberOnlineOfflineEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        List<Cacheable<SocketGuildUser, ulong>> users = [];
        foreach (ulong guildId in data.CommonGuilds)
        {
            if (State.GetGuild(guildId) is not { } guild)
            {
                await UnknownGuildAsync(gatewayEvent.ExtraData.Type, guildId, gatewayEvent).ConfigureAwait(false);
                return;
            }

            SocketGuildUser? user = guild.GetUser(data.UserId);
            user?.Presence.Update(true);
            users.Add(new Cacheable<SocketGuildUser, ulong>(user, data.UserId, user is not null,
                async () =>
                {
                    GuildMember model = await ApiClient
                        .GetGuildMemberAsync(guild.Id, data.UserId).ConfigureAwait(false);
                    SocketGuildUser member = guild.AddOrUpdateUser(model);
                    member.Presence.Update(true);
                    return user;
                }));
        }

        await TimedInvokeAsync(_guildMemberOnlineEvent, nameof(GuildMemberOnline),
            users, data.EventTime).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "PERSON", "guild_member_offline"
    /// </remarks>
    private async Task HandleGuildMemberOffline(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<GuildMemberOnlineOfflineEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        List<Cacheable<SocketGuildUser, ulong>> users = [];
        foreach (ulong guildId in data.CommonGuilds)
        {
            if (State.GetGuild(guildId) is not { } guild)
            {
                await UnknownGuildAsync(gatewayEvent.ExtraData.Type, guildId, gatewayEvent).ConfigureAwait(false);
                return;
            }

            SocketGuildUser? user = guild.GetUser(data.UserId);
            user?.Presence.Update(false);
            users.Add(new Cacheable<SocketGuildUser, ulong>(user, data.UserId, user is not null,
                async () =>
                {
                    GuildMember model = await ApiClient
                        .GetGuildMemberAsync(guild.Id, data.UserId).ConfigureAwait(false);
                    SocketGuildUser member = guild.AddOrUpdateUser(model);
                    member.Presence.Update(false);
                    return user;
                }));
        }

        await TimedInvokeAsync(_guildMemberOnlineEvent, nameof(GuildMemberOffline),
            users, data.EventTime).ConfigureAwait(false);
    }

    #endregion

    #region Guild Roles

    /// <remarks>
    ///     "GROUP", "added_role"
    /// </remarks>
    private async Task HandleAddedRole(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<Role>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        SocketRole role = guild.AddRole(data);
        await TimedInvokeAsync(_roleCreatedEvent, nameof(RoleCreated), role).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "deleted_role"
    /// </remarks>
    private async Task HandleDeletedRole(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<Role>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        SocketRole role = guild.RemoveRole(data.Id) ?? SocketRole.Create(guild, State, data);
        await TimedInvokeAsync(_roleDeletedEvent, nameof(RoleDeleted), role).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "updated_role"
    /// </remarks>
    private async Task HandleUpdatedRole(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<Role>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        SocketRole? role = guild.GetRole(data.Id);
        if (role is null)
        {
            await UnknownRoleAsync(gatewayEvent.ExtraData.Type, data.Id, guild.Id, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        SocketRole before = role.Clone();
        role.Update(State, data);

        await TimedInvokeAsync(_roleUpdatedEvent, nameof(RoleUpdated), before, role).ConfigureAwait(false);
    }

    #endregion

    #region Guild Emojis

    /// <remarks>
    ///     "GROUP", "added_emoji"
    /// </remarks>
    private async Task HandleAddedRmoji(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<GuildEmojiEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        GuildEmote emote = guild.AddOrUpdateEmote(data);

        await TimedInvokeAsync(_emoteCreatedEvent, nameof(EmoteCreated), emote, guild).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "updated_emoji"
    /// </remarks>
    private async Task HandleUpdatedEmoji(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<GuildEmojiEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        GuildEmote? emote = guild.GetEmote(data.Id);
        GuildEmote? before = emote?.Clone();
        GuildEmote after = guild.AddOrUpdateEmote(data);

        await TimedInvokeAsync(_emoteUpdatedEvent, nameof(EmoteUpdated), before, after, guild).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "deleted_emoji"
    /// </remarks>
    private async Task HandleDeletedEmoji(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<GuildEmojiEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        GuildEmote emote = guild.RemoveEmote(data.Id)
            ?? new GuildEmote(data.Id, data.Name, data.Type is EmojiType.Animated, guild.Id, null);
        await TimedInvokeAsync(_emoteDeletedEvent, nameof(EmoteDeleted), emote, guild).ConfigureAwait(false);
    }

    #endregion

    #region Guilds

    /// <remarks>
    ///     "GROUP", "updated_guild"
    /// </remarks>
    private async Task HandleUpdatedGuild(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<GuildEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        SocketGuild before = guild.Clone();
        guild.Update(State, data);
        if (AlwaysDownloadBoostSubscriptions
            && (before.BoostSubscriptionCount != guild.BoostSubscriptionCount
                || before.BufferBoostSubscriptionCount != guild.BufferBoostSubscriptionCount))
            await guild.DownloadBoostSubscriptionsAsync().ConfigureAwait(false);

        if (BaseConfig.AutoUpdateRolePositions)
        {
            IEnumerable<Role> models;
            if (guild.CurrentUser?.GuildPermissions.Has(GuildPermission.ManageRoles) is true)
            {
                models = await ApiClient.GetGuildRolesAsync(guild.Id)
                    .FlattenAsync()
                    .ConfigureAwait(false);
            }
            else
            {
                ExtendedGuild guildModel = await ApiClient.GetGuildAsync(guild.Id)
                    .ConfigureAwait(false);
                models = guildModel.Roles ?? [];
            }

            foreach (Role model in models)
            {
                SocketRole? role = guild.GetRole(model.Id);
                if (role is null)
                {
                    await UnknownRoleAsync(gatewayEvent.ExtraData.Type, model.Id, guild.Id, gatewayEvent)
                        .ConfigureAwait(false);
                    continue;
                }

                SocketRole roleBefore = role.Clone();
                role.Update(State, model);
                if (roleBefore.Position != role.Position)
                {
                    await TimedInvokeAsync(_roleUpdatedEvent, nameof(RoleUpdated), roleBefore, role)
                        .ConfigureAwait(false);
                }
            }
        }

        await TimedInvokeAsync(_guildUpdatedEvent, nameof(GuildUpdated), before, guild).ConfigureAwait(false);
    }

    private async Task HandleUpdatedGuildSelf(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<GuildUpdateSelfEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(data.GuildId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        SocketGuildUser? user = guild.CurrentUser;
        SocketGuildUser? before = user?.Clone();
        user?.Update(State, data);
        Cacheable<SocketGuildUser, ulong> cacheableBefore = new(before, gatewayEvent.TargetId, before is not null,
            () => Task.FromResult<SocketGuildUser?>(null));
        Cacheable<SocketGuildUser, ulong> cacheableAfter = GetCacheableSocketGuildUser(user, gatewayEvent.TargetId, guild);

        await TimedInvokeAsync(_guildMemberUpdatedEvent, nameof(GuildMemberUpdated),
            cacheableBefore, cacheableAfter).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "deleted_guild"
    /// </remarks>
    private async Task HandleDeletedGuild(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<GuildEvent>(gatewayEvent.ExtraData.Body) is null) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        await GuildUnavailableAsync(guild).ConfigureAwait(false);
        await TimedInvokeAsync(_leftGuildEvent, nameof(LeftGuild), guild).ConfigureAwait(false);
        ((IDisposable)guild).Dispose();
    }

    /// <remarks>
    ///     "GROUP", "added_block_list"
    /// </remarks>
    private async Task HandleAddedBlockList(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<GuildBanEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        SocketGuildUser? operatorUser = guild.GetUser(data.OperatorUserId);
        Cacheable<SocketGuildUser, ulong> cacheableOperatorUser =
            GetCacheableSocketGuildUser(operatorUser, data.OperatorUserId, guild);
        IReadOnlyCollection<Cacheable<SocketUser, ulong>> bannedUsers = data.UserIds
            .Select(userId => GetCacheableSocketUser(guild.GetUser(userId), userId))
            .ToImmutableArray();

        await TimedInvokeAsync(_userBannedEvent, nameof(UserBanned),
            bannedUsers, cacheableOperatorUser, guild, data.Reason).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "deleted_block_list"
    /// </remarks>
    private async Task HandleDeletedBlockList(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<GuildBanEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        SocketGuildUser? operatorUser = guild.GetUser(data.OperatorUserId);
        Cacheable<SocketGuildUser, ulong> cacheableOperatorUser =
            GetCacheableSocketGuildUser(operatorUser, data.OperatorUserId, guild);
        IReadOnlyCollection<Cacheable<SocketUser, ulong>> bannedUsers = data.UserIds
            .Select(userId => GetCacheableSocketUser(guild.GetUser(userId), userId))
            .ToImmutableArray();

        await TimedInvokeAsync(_userUnbannedEvent, nameof(UserUnbanned),
            bannedUsers, cacheableOperatorUser, guild).ConfigureAwait(false);
    }

    #endregion

    #region Users

    /// <remarks>
    ///     "GROUP", "joined_channel"
    /// </remarks>
    private async Task HandleJoinedChannel(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<UserVoiceEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        if (GetChannel(data.ChannelId) is not SocketVoiceChannel channel)
        {
            await UnknownChannelAsync(gatewayEvent.ExtraData.Type, data.ChannelId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        SocketGuildUser? user = guild.GetUser(data.UserId);
        Cacheable<SocketGuildUser, ulong> cacheableUser = GetCacheableSocketGuildUser(user, data.UserId, guild);
        SocketVoiceState before = guild.GetVoiceState(data.UserId)?.Clone() ?? SocketVoiceState.Default;
        SocketVoiceState after = guild.AddOrUpdateVoiceState(data.UserId, channel.Id);

        await TimedInvokeAsync(_userConnectedEvent, nameof(UserConnected),
            cacheableUser, channel, data.At).ConfigureAwait(false);
        await TimedInvokeAsync(_userVoiceStateUpdatedEvent, nameof(UserVoiceStateUpdated),
            cacheableUser, before, after).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "exited_channel"
    /// </remarks>
    private async Task HandleExitedChannel(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<UserVoiceEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        if (GetChannel(data.ChannelId) is not SocketVoiceChannel channel)
        {
            await UnknownChannelAsync(gatewayEvent.ExtraData.Type, data.ChannelId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        SocketGuildUser? user = guild.GetUser(data.UserId);
        Cacheable<SocketGuildUser, ulong> cacheableUser = GetCacheableSocketGuildUser(user, data.UserId, guild);
        SocketVoiceState before = guild.GetVoiceState(data.UserId)?.Clone() ?? SocketVoiceState.Default;
        SocketVoiceState after = guild.AddOrUpdateVoiceState(data.UserId, null);

        await TimedInvokeAsync(_userDisconnectedEvent, nameof(UserDisconnected),
            cacheableUser, channel, data.At).ConfigureAwait(false);
        await TimedInvokeAsync(_userVoiceStateUpdatedEvent, nameof(UserVoiceStateUpdated),
            cacheableUser, before, after).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "GROUP", "live_status_changed"
    /// </remarks>
    private async Task HandleLiveStatusChanged(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<LiveStatusChangeEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(gatewayEvent.TargetId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }
        if (GetChannel(data.Channel.Id) is not SocketVoiceChannel channel)
        {
            await UnknownChannelAsync(gatewayEvent.ExtraData.Type, data.Channel.Id, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        SocketVoiceState before = guild.GetVoiceState(data.User.Id)?.Clone() ?? SocketVoiceState.Default;
        SocketVoiceState after = guild.AddOrUpdateVoiceState(data.User.Id, channel, data.User.LiveStreamStatus);

        SocketGuildUser? user = guild.GetUser(data.User.Id);
        Cacheable<SocketGuildUser, ulong> cacheableUser = GetCacheableSocketGuildUser(user, data.User.Id, guild);
        guild.AddOrUpdateVoiceState(data.User.Id, channel, data.User.LiveStreamStatus);

        if (data.User.LiveStreamStatus.InLive)
        {
            await TimedInvokeAsync(_livestreamBeganEvent, nameof(LivestreamBegan),
                cacheableUser, channel).ConfigureAwait(false);
        }
        else
        {
            await TimedInvokeAsync(_livestreamEndedEvent, nameof(LivestreamEnded),
                cacheableUser, channel).ConfigureAwait(false);
        }

        await TimedInvokeAsync(_userVoiceStateUpdatedEvent, nameof(UserVoiceStateUpdated),
            cacheableUser, before, after).ConfigureAwait(false);
    }

    private async Task HandleAddGuildMute(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<GuildMuteDeafEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(data.GuildId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        SocketGuildUser? user = guild.GetUser(data.UserId);
        Cacheable<SocketGuildUser, ulong> cacheableUser = GetCacheableSocketGuildUser(user, data.UserId, guild);
        SocketVoiceState before = guild.GetVoiceState(data.UserId)?.Clone() ?? SocketVoiceState.Default;
        (bool? isMuted, bool? isDeafened) = data.Type switch
        {
            MuteOrDeafType.Mute => (true, null),
            MuteOrDeafType.Deaf => (null, true),
            _ => ((bool?)null, (bool?)null)
        };
        SocketVoiceState after = guild.AddOrUpdateVoiceState(data.UserId, isMuted, isDeafened);

        await TimedInvokeAsync(_userVoiceStateUpdatedEvent, nameof(UserVoiceStateUpdated),
            cacheableUser, before, after).ConfigureAwait(false);
    }

    private async Task HandleDeleteGuildMute(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<GuildMuteDeafEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.GetGuild(data.GuildId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        SocketGuildUser? user = guild.GetUser(data.UserId);
        Cacheable<SocketGuildUser, ulong> cacheableUser = GetCacheableSocketGuildUser(user, data.UserId, guild);
        SocketVoiceState before = guild.GetVoiceState(data.UserId)?.Clone() ?? SocketVoiceState.Default;
        (bool? isMuted, bool? isDeafened) = data.Type switch
        {
            MuteOrDeafType.Mute => (false, null),
            MuteOrDeafType.Deaf => (null, false),
            _ => ((bool?)null, (bool?)null)
        };
        SocketVoiceState after = guild.AddOrUpdateVoiceState(data.UserId, isMuted, isDeafened);

        await TimedInvokeAsync(_userVoiceStateUpdatedEvent, nameof(UserVoiceStateUpdated),
            cacheableUser, before, after).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "PERSON", "user_updated"
    /// </remarks>
    private async Task HandleUserUpdated(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<UserUpdateEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (data.UserId == CurrentUser?.Id)
        {
            SocketSelfUser selfBefore = CurrentUser.Clone();
            CurrentUser.Update(State, data);
            await TimedInvokeAsync(_currentUserUpdatedEvent, nameof(CurrentUserUpdated),
                selfBefore, CurrentUser).ConfigureAwait(false);
            return;
        }

        SocketUser? user = GetUser(data.UserId);
        SocketUser? before = user?.Clone();
        user?.Update(State, data);
        Cacheable<SocketUser, ulong> cacheableBefore = new(before, data.UserId, before is not null,
            () => Task.FromResult<SocketUser?>(null));
        Cacheable<SocketUser, ulong> cacheableUser = GetCacheableSocketUser(user, data.UserId);
        await TimedInvokeAsync(_userUpdatedEvent, nameof(UserUpdated),
            cacheableBefore, cacheableUser).ConfigureAwait(false);
    }

    /// <remarks>
    ///     "PERSON", "self_joined_guild"
    /// </remarks>
    private async Task HandleSelfJoinedGuild(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<SelfGuildEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        await Task.Yield();
        _ = Task.Run(async () =>
        {
            try
            {
                int remainingRetryTimes = BaseConfig.MaxJoinedGuildDataFetchingRetryTimes;
                while (true)
                {
                    try
                    {
                        return await ApiClient.GetGuildAsync(data.GuildId).ConfigureAwait(false);
                    }
                    catch (HttpException ex)
                        when (ex is { HttpCode: HttpStatusCode.OK, KookCode: KookErrorCode.GeneralError })
                    {
                        if (remainingRetryTimes < 0)
                            throw;
                    }

                    double retryDelay = BaseConfig.JoinedGuildDataFetchingRetryDelay / 1000D;
                    await _gatewayLogger
                        .WarningAsync($"Failed to get guild {data.GuildId} after joining. "
                            + $"Retrying in {retryDelay:F3} second{(retryDelay is 1 ? string.Empty : "s")} "
                            + $"for {remainingRetryTimes} more time{(remainingRetryTimes is 1 ? string.Empty : "s")}.")
                        .ConfigureAwait(false);
                    remainingRetryTimes--;
                    await Task.Delay(TimeSpan.FromMilliseconds(retryDelay)).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                string payloadJson = SerializePayload(gatewayEvent);
                await _gatewayLogger
                    .ErrorAsync($"Error handling {gatewayEvent.ExtraData.Type}. Payload: {payloadJson}", e)
                    .ConfigureAwait(false);
                throw;
            }
        }).ContinueWith(async x =>
        {
            ExtendedGuild model = x.Result;
            SocketGuild guild = AddGuild(model, State);
            guild.Update(State, model);
            await TimedInvokeAsync(_joinedGuildEvent, nameof(JoinedGuild), guild).ConfigureAwait(false);
            await GuildAvailableAsync(guild).ConfigureAwait(false);
        }, TaskContinuationOptions.OnlyOnRanToCompletion);
    }

    /// <remarks>
    ///     "PERSON", "self_exited_guild"
    /// </remarks>
    private async Task HandleSelfExitedGuild(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<SelfGuildEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (State.RemoveGuild(data.GuildId) is not { } guild)
        {
            await UnknownGuildAsync(gatewayEvent.ExtraData.Type, data.GuildId, gatewayEvent)
                .ConfigureAwait(false);
            return;
        }

        await GuildUnavailableAsync(guild).ConfigureAwait(false);
        await TimedInvokeAsync(_leftGuildEvent, nameof(LeftGuild), guild).ConfigureAwait(false);
        ((IDisposable)guild).Dispose();
    }

    #endregion

    #region Interactions

    /// <remarks>
    ///     "PERSON", "message_btn_click"
    /// </remarks>
    private async Task HandleMessageButtonClick(GatewayEvent<GatewaySystemEventExtraData> gatewayEvent)
    {
        if (DeserializePayload<MessageButtonClickEvent>(gatewayEvent.ExtraData.Body) is not { } data) return;
        if (data.GuildId.HasValue)
        {
            if (GetGuild(data.GuildId.Value) is not { } guild)
            {
                await UnknownGuildAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                    .ConfigureAwait(false);
                return;
            }

            if (guild.GetTextChannel(data.ChannelId) is not { } channel)
            {
                await UnknownChannelAsync(gatewayEvent.ExtraData.Type, gatewayEvent.TargetId, gatewayEvent)
                    .ConfigureAwait(false);
                return;
            }

            SocketGuildUser? user = channel.GetUser(data.UserId);
            Cacheable<SocketGuildUser, ulong> cacheableUser = GetCacheableSocketGuildUser(user, data.UserId, guild);
            SocketMessage? cachedMsg = channel.GetCachedMessage(data.MessageId);
            Cacheable<IMessage, Guid> cacheableMessage = GetCacheableSocketMessage(cachedMsg, data.MessageId, channel);

            await TimedInvokeAsync(_messageButtonClickedEvent, nameof(MessageButtonClicked),
                data.Value, cacheableUser, cacheableMessage, channel).ConfigureAwait(false);
        }
        else
        {
            SocketUser? user = GetUser(data.UserId);
            Cacheable<SocketUser, ulong> cacheableUser = GetCacheableSocketUser(user, data.UserId);
            SocketDMChannel? channel = GetDMChannel(data.UserId);
            if (channel is null)
            {
                CreateUserChatParams createUserChatParams = new()
                {
                    UserId = data.UserId
                };
                UserChat model = await ApiClient.CreateUserChatAsync(createUserChatParams).ConfigureAwait(false);
                channel = CreateDMChannel(model.Code, model.Recipient, State);
            }
            Cacheable<IMessage, Guid> cacheableMessage = GetCacheableSocketMessage(null, data.MessageId, channel);
            await TimedInvokeAsync(_directMessageButtonClickedEvent, nameof(DirectMessageButtonClicked),
                data.Value, cacheableUser, cacheableMessage, channel).ConfigureAwait(false);
        }
    }

    #endregion

    #region Cacheable

    private Cacheable<SocketUser, ulong> GetCacheableSocketUser(SocketUser? value, ulong id)
    {
        return new Cacheable<SocketUser, ulong>(value, id, value is not null,
            async () =>
            {
                User model = await ApiClient.GetUserAsync(id).ConfigureAwait(false);
                return State.AddOrUpdateUser(id,
                    _ => SocketGlobalUser.Create(this, State, model),
                    (_, x) =>
                    {
                        x.Update(State, model);
                        x.UpdatePresence(model.Online, model.OperatingSystem);
                        return x;
                    });
            });
    }

    private Cacheable<SocketGuildUser, ulong> GetCacheableSocketGuildUser(SocketGuildUser? value,
        ulong id, SocketGuild guild)
    {
        return new Cacheable<SocketGuildUser, ulong>(value, id, value is not null,
            async () =>
            {
                GuildMember model = await ApiClient.GetGuildMemberAsync(guild.Id, id).ConfigureAwait(false);
                return guild.AddOrUpdateUser(model);
            });
    }

    private Cacheable<SocketDMChannel, Guid> GetCacheableDMChannel(SocketDMChannel? value,
        Guid chatCode, ulong userId)
    {
        return new Cacheable<SocketDMChannel, Guid>(value, chatCode, value is not null,
            async () =>
            {
                User user = await ApiClient.GetUserAsync(userId).ConfigureAwait(false);
                return CreateDMChannel(chatCode, user, State);
            });
    }

    private Cacheable<SocketDMChannel, Guid> GetCacheableDMChannel(SocketDMChannel? value, Guid chatCode)
    {
        return new Cacheable<SocketDMChannel, Guid>(value, chatCode, value is not null,
            async () =>
            {
                UserChat userChat = await ApiClient.GetUserChatAsync(chatCode).ConfigureAwait(false);
                return CreateDMChannel(chatCode, userChat.Recipient, State);
            });
    }

    private Cacheable<IMessage, Guid> GetCacheableSocketMessage(SocketMessage? value,
        Guid messageId, IMessageChannel channel)
    {
        return new Cacheable<IMessage, Guid>(value, messageId, value is not null,
            async () => await channel.GetMessageAsync(messageId).ConfigureAwait(false) as SocketMessage);
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

    private string SerializePayload(object jsonElement) =>
        JsonSerializer.Serialize(jsonElement, _serializerOptions);

    #endregion

}
