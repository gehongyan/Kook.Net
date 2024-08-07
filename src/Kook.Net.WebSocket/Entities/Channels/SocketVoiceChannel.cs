using Kook.Rest;
using System.Collections.Immutable;
using System.Diagnostics;
using Kook.API.Rest;
using Kook.Audio;
using Model = Kook.API.Channel;

namespace Kook.WebSocket;

/// <summary>
///     Represents a WebSocket-based voice channel in a guild.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SocketVoiceChannel : SocketTextChannel, IVoiceChannel, ISocketAudioChannel, IDisposable
{
    private AudioClient? _audioClient;
    private readonly SemaphoreSlim _audioLock;
    private TaskCompletionSource<AudioClient?>? _audioConnectPromise;

    #region SocketVoiceChannel

    /// <inheritdoc />
    public VoiceQuality? VoiceQuality { get; private set; }

    /// <inheritdoc />
    public int UserLimit { get; private set; }

    /// <inheritdoc />
    public string? ServerUrl { get; private set; }

    /// <inheritdoc />
    public bool? IsVoiceRegionOverwritten { get; private set; }

    /// <inheritdoc />
    public string? VoiceRegion { get; private set; }

    /// <inheritdoc />
    public bool HasPassword { get; private set; }

    /// <summary>
    ///     Gets the <see cref="IAudioClient" /> associated with this guild.
    /// </summary>
    public IAudioClient? AudioClient => _audioClient;

    /// <inheritdoc />
    /// <seealso cref="SocketVoiceChannel.ConnectedUsers"/>
    public override IReadOnlyCollection<SocketGuildUser> Users =>
        Guild.Users.Where(x => Permissions.GetValue(
            Permissions.ResolveChannel(Guild, x, this, Permissions.ResolveGuild(Guild, x)),
            ChannelPermission.ViewChannel)).ToImmutableArray();

    /// <summary>
    ///     Gets a collection of users that are currently connected to this voice channel.
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         This property may not always return all the members that are connected to this voice channel,
    ///         because uses may connected this voice channel before the bot has connected to the gateway.
    ///         To ensure accuracy, you may need to enable <see cref="KookSocketConfig.AlwaysDownloadVoiceStates"/>
    ///         to fetch the full voice states upon startup, or use <see cref="SocketGuild.DownloadVoiceStatesAsync"/>
    ///         on the guild this voice channel belongs to to manually download the users voice states,
    ///         or use <see cref="GetConnectedUsersAsync"/> to fetch the connected users from the API.
    ///     </note>
    /// </remarks>
    /// <returns>
    ///     A read-only collection of users that are currently connected to this voice channel.
    /// </returns>
    public IReadOnlyCollection<SocketGuildUser> ConnectedUsers =>
        Guild.Users.Where(x => x.VoiceChannel?.Id == Id).ToImmutableArray();

    internal SocketVoiceChannel(KookSocketClient kook, ulong id, SocketGuild guild)
        : base(kook, id, guild)
    {
        Type = ChannelType.Voice;
        _audioLock = new SemaphoreSlim(1, 1);
    }

    internal static new SocketVoiceChannel Create(SocketGuild guild, ClientState state, Model model)
    {
        SocketVoiceChannel entity = new(guild.Kook, model.Id, guild);
        entity.Update(state, model);
        return entity;
    }

    /// <inheritdoc />
    internal override void Update(ClientState state, Model model)
    {
        base.Update(state, model);
        VoiceQuality = model.VoiceQuality;
        UserLimit = model.UserLimit ?? 0;
        ServerUrl = model.ServerUrl;
        VoiceRegion = model.VoiceRegion;
        HasPassword = model.HasPassword;
        IsVoiceRegionOverwritten = model.OverwriteVoiceRegion;
    }

    /// <inheritdoc />
    public override SocketGuildUser? GetUser(ulong id)
    {
        SocketGuildUser? user = Guild.GetUser(id);
        return user?.VoiceChannel?.Id == Id ? user : null;
    }

    /// <inheritdoc />
    public Task ModifyAsync(Action<ModifyVoiceChannelProperties> func, RequestOptions? options = null) =>
        ChannelHelper.ModifyAsync(this, Kook, func, options);

    /// <summary>
    ///     Gets a collection of users that are currently connected to this voice channel.
    /// </summary>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection of users
    ///     that are currently connected to this voice channel.
    /// </returns>
    public async Task<IReadOnlyCollection<SocketGuildUser>> GetConnectedUsersAsync(
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
        mode is CacheMode.AllowDownload
            ? await SocketChannelHelper.GetConnectedUsersAsync(this, Guild, Kook, options).ConfigureAwait(false)
            : ConnectedUsers;

    #endregion

    #region TextOverrides

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> Getting messages from a voice channel is not supported. </exception>
    public override IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        throw new NotSupportedException("Getting messages from a voice channel is not supported.");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> Getting messages from a voice channel is not supported. </exception>
    public override IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        throw new NotSupportedException("Getting messages from a voice channel is not supported.");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> Getting messages from a voice channel is not supported. </exception>
    public override IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        throw new NotSupportedException("Getting messages from a voice channel is not supported.");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> Getting messages from a voice channel is not supported. </exception>
    Task<IReadOnlyCollection<IMessage>> ITextChannel.GetPinnedMessagesAsync(RequestOptions? options) =>
        Task.FromException<IReadOnlyCollection<IMessage>>(
            new NotSupportedException("Getting messages from a voice channel is not supported."));

    #endregion

    #region IVoiceChannel

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IUser>> IVoiceChannel.GetConnectedUsersAsync(
        CacheMode mode, RequestOptions? options) =>
        await GetConnectedUsersAsync(mode, options).ConfigureAwait(false);

    #endregion

    private string DebuggerDisplay => $"{Name} ({Id}, Voice{(AudioClient is not null ? ", Connected" : string.Empty)})";

    #region IAudioChannel

    /// <inheritdoc />
    public async Task<IAudioClient?> ConnectAsync( /*bool selfDeaf = false, bool selfMute = false, */
        bool external = false, bool disconnect = true, string? password = null)
    {
        TaskCompletionSource<AudioClient?> promise;
        await _audioLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (disconnect || !external)
                await DisconnectAudioInternalAsync().ConfigureAwait(false);
            promise = new TaskCompletionSource<AudioClient?>();
            _audioConnectPromise = promise;

            if (external)
            {
                _ = promise.TrySetResultAsync(null);
                // await UpdateSelfVoiceStateAsync(channelId, selfDeaf, selfMute).ConfigureAwait(false);
                return null;
            }

            if (_audioClient is null)
            {
                AudioClient audioClient = new(this, Kook.GetAudioId(), password);
                audioClient.Disconnected += async ex =>
                {
                    if (!promise.Task.IsCompleted && audioClient.IsFinished)
                    {
                        try
                        {
                            audioClient.Dispose();
                        }
                        catch
                        {
                            // ignored
                        }
                        _audioClient = null;
                        if (ex is not null)
                            await promise.TrySetExceptionAsync(ex);
                        else
                            await promise.TrySetCanceledAsync();
                    }
                };
                audioClient.Connected += () =>
                {
                    _ = promise.TrySetResultAsync(_audioClient);
                    return Task.CompletedTask;
                };
                _audioClient = audioClient;
            }

            // await UpdateSelfVoiceStateAsync(channelId, selfDeaf, selfMute).ConfigureAwait(false);
            await _audioClient.StartAsync().ConfigureAwait(false);
        }
        catch
        {
            await DisconnectAudioInternalAsync().ConfigureAwait(false);
            throw;
        }
        finally
        {
            _audioLock.Release();
        }

        try
        {
            Task timeoutTask = Task.Delay(15000);
            Task completedTask = await Task.WhenAny(promise.Task, timeoutTask).ConfigureAwait(false);
            if (completedTask == timeoutTask)
                throw new TimeoutException("The audio client failed to connect within 15 seconds.");
            return await promise.Task.ConfigureAwait(false);
        }
        catch
        {
            await DisconnectAsync().ConfigureAwait(false);
            throw;
        }
    }

    private async Task UpdateSelfVoiceStateAsync(bool selfDeaf, bool selfMute)
    {
        SocketGuildUser? selfUser = Users
            .SingleOrDefault(x => x.Id == Kook.CurrentUser?.Id);
        if (selfUser is null)
            return;
        if (selfDeaf)
            await selfUser.DeafenAsync();
        else
            await selfUser.UndeafenAsync();
        if (selfMute)
            await selfUser.MuteAsync();
        else
            await selfUser.UnmuteAsync();
    }

    /// <inheritdoc />
    public async Task DisconnectAsync()
    {
        await _audioLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await DisconnectAudioInternalAsync().ConfigureAwait(false);
        }
        finally
        {
            _audioLock.Release();
        }
    }

    private async Task DisconnectAudioInternalAsync()
    {
        _audioConnectPromise?.TrySetCanceledAsync(); //Cancel any previous audio connection
        _audioConnectPromise = null;
        if (_audioClient is not null)
            await _audioClient.StopAsync().ConfigureAwait(false);
        await Kook.ApiClient
            .DisposeVoiceGatewayAsync(new DisposeVoiceGatewayParams { ChannelId = Id })
            .ConfigureAwait(false);
        _audioClient?.Dispose();
        _audioClient = null;
    }

    /// <inheritdoc />
    void IDisposable.Dispose()
    {
        DisconnectAsync().GetAwaiter().GetResult();
        _audioLock?.Dispose();
        _audioClient?.Dispose();
    }

    #endregion

    #region IGuildChannel

    /// <inheritdoc />
    Task<IGuildUser?> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions? options) =>
        Task.FromResult<IGuildUser?>(GetUser(id));

    /// <inheritdoc />
    /// <seealso cref="IVoiceChannel.GetConnectedUsersAsync"/>
    IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(
        CacheMode mode, RequestOptions? options) =>
        mode == CacheMode.AllowDownload
            ? ChannelHelper.GetUsersAsync(this, Guild, Kook, KookConfig.MaxUsersPerBatch, 1, options)
            : ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();

    #endregion

    #region INestedChannel

    /// <inheritdoc />
    Task<ICategoryChannel?> INestedChannel.GetCategoryAsync(CacheMode mode, RequestOptions? options) =>
        Task.FromResult(Category);

    #endregion
}
