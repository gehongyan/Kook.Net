using Kook.Rest;
using System.Collections.Immutable;
using System.Diagnostics;
using Kook.API.Rest;
using Kook.Audio;
using Model = Kook.API.Channel;

namespace Kook.WebSocket;

/// <summary>
///     表示服务器中的一个基于网关的具有语音聊天能力的频道。
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

    /// <inheritdoc />
    public IAudioClient? AudioClient => _audioClient;

    /// <inheritdoc />
    /// <seealso cref="SocketVoiceChannel.ConnectedUsers"/>
    public override IReadOnlyCollection<SocketGuildUser> Users =>
        Guild.Users.Where(x => Permissions.GetValue(
            Permissions.ResolveChannel(Guild, x, this, Permissions.ResolveGuild(Guild, x)),
            ChannelPermission.ViewChannel)).ToImmutableArray();

    /// <summary>
    ///     获取当前连接到此语音频道的所有用户。
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         此属性可能不会始终返回连接到此语音频道的所有成员，因为用户可能在 Bot 连接到网关之前就连接到了此语音频道。
    ///         如需准确获取所有连接到此语音频道的成员，可能需要启用
    ///         <see cref="Kook.WebSocket.KookSocketConfig.AlwaysDownloadVoiceStates"/>，这可以让 Bot
    ///         在启动连接到网关时获取完整的语音状态。也可以调用方法
    ///         <see cref="Kook.WebSocket.SocketVoiceChannel.GetConnectedUsersAsync(Kook.CacheMode,Kook.RequestOptions)"/>
    ///         访问 API 获取连接到此语音频道的用户。
    ///     </note>
    /// </remarks>
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

    /// <inheritdoc cref="Kook.IVoiceChannel.GetConnectedUsersAsync(Kook.CacheMode,Kook.RequestOptions)" />
    public async Task<IReadOnlyCollection<SocketGuildUser>> GetConnectedUsersAsync(
        CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) =>
        mode is CacheMode.AllowDownload
            ? await SocketChannelHelper.GetConnectedUsersAsync(this, Guild, Kook, options).ConfigureAwait(false)
            : ConnectedUsers;

    #endregion

    #region TextOverrides

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持在语音频道中获取消息。 </exception>
    public override IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        throw new NotSupportedException("Getting messages from a voice channel is not supported.");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持在语音频道中获取消息。 </exception>
    public override IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(Guid referenceMessageId, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        throw new NotSupportedException("Getting messages from a voice channel is not supported.");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持在语音频道中获取消息。 </exception>
    public override IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage referenceMessage, Direction dir,
        int limit = KookConfig.MaxMessagesPerBatch, RequestOptions? options = null) =>
        throw new NotSupportedException("Getting messages from a voice channel is not supported.");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"> 不支持在语音频道中获取消息。 </exception>
    Task<IReadOnlyCollection<IMessage>> ITextChannel.GetPinnedMessagesAsync(RequestOptions? options) =>
        Task.FromException<IReadOnlyCollection<IMessage>>(
            new NotSupportedException("Getting messages from a voice channel is not supported."));

    #endregion

    #region IVoiceChannel

    /// <inheritdoc />
    async Task<IReadOnlyCollection<IGuildUser>> IVoiceChannel.GetConnectedUsersAsync(
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
    /// <seealso cref="Kook.IVoiceChannel.GetConnectedUsersAsync(Kook.CacheMode,Kook.RequestOptions)"/>
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
