using System.Collections.Concurrent;
using Kook.Audio;
using Kook.Audio.WebRtc;
using Kook.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kook.Net.Samples.WebRTC.Services;

/// <summary>
///     管理机器人所在的语音频道：连接与断开，点歌队列，以及对每个用户的收流录音。
/// </summary>
public class MusicService : IHostedService
{
    private sealed record Session(
        SocketVoiceChannel VoiceChannel,
        IWebRtcAudioClient AudioClient,
        MusicClient MusicClient,
        RecordingService Recording);

    private readonly WebRtcSampleOptions _options;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<MusicService> _logger;
    private readonly ConcurrentDictionary<ulong, Session> _sessions = new();

    public MusicService(WebRtcSampleOptions options, ILoggerFactory loggerFactory)
    {
        _options = options;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<MusicService>();
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken) =>
        await Task.WhenAll(_sessions.Keys.Select(DisconnectAsync));

    public async Task ConnectAsync(SocketVoiceChannel voiceChannel)
    {
        if (_sessions.ContainsKey(voiceChannel.Id))
            throw new InvalidOperationException("I'm already connected to this voice channel.");

        // 每个语音频道的录音单独放在一个目录里
        RecordingService recording = new(Path.Combine(_options.RecordingDirectory, $"channel-{voiceChannel.Id}"),
            _loggerFactory.CreateLogger<RecordingService>());
        // 连接过程中，已经在发言的用户的音频流会被立即创建，因此需要在连接之前订阅事件
        IWebRtcAudioClient audioClient = await voiceChannel.ConnectWebRtcAsync(
            client => Subscribe(client, voiceChannel, recording));
        Session session = new(voiceChannel, audioClient,
            new MusicClient(voiceChannel, audioClient, _options, _loggerFactory.CreateLogger<MusicClient>()), recording);
        if (!_sessions.TryAdd(voiceChannel.Id, session))
        {
            await DisposeSessionAsync(session);
            throw new InvalidOperationException("I'm already connected to this voice channel.");
        }

        LogChannelUsers(voiceChannel, "Users in the voice channel after connecting");
    }

    public async Task DisconnectAsync(ulong voiceChannelId)
    {
        if (!_sessions.TryRemove(voiceChannelId, out Session? session))
            return;
        await DisposeSessionAsync(session);
    }

    public void Enqueue(SocketVoiceChannel voiceChannel, MusicTrack track) =>
        GetSession(voiceChannel).MusicClient.Enqueue(track);

    public bool Skip(SocketVoiceChannel voiceChannel) => GetSession(voiceChannel).MusicClient.Skip();

    public (MusicTrack? Current, IReadOnlyList<MusicTrack> Queue) GetQueue(SocketVoiceChannel voiceChannel)
    {
        MusicClient musicClient = GetSession(voiceChannel).MusicClient;
        return (musicClient.CurrentPlaying, musicClient.Queue);
    }

    private Session GetSession(SocketVoiceChannel voiceChannel) =>
        _sessions.TryGetValue(voiceChannel.Id, out Session? session)
            ? session
            : throw new InvalidOperationException("I'm not connected to this voice channel.");

    private async Task DisposeSessionAsync(Session session)
    {
        await session.MusicClient.DisposeAsync();
        try
        {
            await session.AudioClient.StopAsync();
        }
        finally
        {
            session.AudioClient.Dispose();
        }

        // 录音在音频客户端停止之后再结束，保证最后一段音频也被写入文件
        await session.Recording.CompleteAsync();
        LogChannelUsers(session.VoiceChannel, "Users in the voice channel after disconnecting");
    }

    private void Subscribe(IWebRtcAudioClient audioClient, SocketVoiceChannel voiceChannel, RecordingService recording)
    {
        audioClient.Connected += () =>
        {
            _logger.LogInformation("[Connected] The audio client connected [Channel ID] {ChannelId} [State] {State} [Muted] {Muted}",
                audioClient.ChannelId, audioClient.ConnectionState, audioClient.IsMicrophoneMuted);
            return Task.CompletedTask;
        };
        audioClient.Disconnected += ex =>
        {
            _logger.LogWarning(ex, "[Disconnected] The audio client disconnected [Channel ID] {ChannelId}", audioClient.ChannelId);
            // 被服务器断开（例如被移出频道）时，清理这个频道对应的点歌队列与录音
            _ = DisconnectAsync(voiceChannel.Id);
            return Task.CompletedTask;
        };
        audioClient.UdpLatencyUpdated += (previous, current) =>
        {
            _logger.LogInformation("[UdpLatencyUpdated] The UDP latency was updated [Previous] {Previous} [Current] {Current}",
                previous, current);
            return Task.CompletedTask;
        };
        audioClient.StreamCreated += (ssrc, stream) =>
        {
            // WebRTC 语音通道在下发音频流时同时下发了其所属的用户，不需要像 UDP 版本那样仅凭 SSRC 区分
            bool resolved = audioClient.TryGetStreamUserId(ssrc, out ulong userId);
            string? displayName = resolved ? GetDisplayName(voiceChannel, userId) : null;
            _logger.LogInformation(
                "[StreamCreated] An audio stream was created [SSRC] {Ssrc} [User ID] {UserId} [Display Name] {DisplayName} [Available Streams] {AvailableStreams}",
                ssrc, resolved ? userId : null, displayName, audioClient.GetStreams().Count);
            // 只有被指定为录制目标的用户的音频流才会被读取，其余的音频流不会被消费
            if (resolved)
                recording.TryAttach(userId, displayName, ssrc, stream);
            return Task.CompletedTask;
        };
        audioClient.StreamDestroyed += async ssrc =>
        {
            _logger.LogInformation("[StreamDestroyed] An audio stream was destroyed [SSRC] {Ssrc}", ssrc);
            await recording.StopStreamAsync(ssrc);
        };

        // 频道内其他成员的状态变化。显示名称来自服务器缓存，成员不在缓存里时为 null
        audioClient.PeerJoined += peer =>
        {
            _logger.LogInformation(
                "[PeerJoined] A user joined the voice channel [User ID] {UserId} [Display Name] {DisplayName} [Device] {Device} [Microphone Muted] {MicrophoneMuted} [Headset Muted] {HeadsetMuted} [Server Muted] {ServerMuted} [Can Speak] {CanSpeak}",
                peer.Id, peer.DisplayName, peer.Device, peer.IsMicrophoneMuted, peer.IsHeadsetMuted,
                peer.Permissions.IsServerMuted, peer.Permissions.CanSpeak);
            return Task.CompletedTask;
        };
        audioClient.PeerLeft += userId =>
        {
            // 主动离开和被管理员移出语音频道，服务端下发的通知完全相同，无法区分
            _logger.LogInformation("[PeerLeft] A user left the voice channel [User ID] {UserId} [Display Name] {DisplayName}",
                userId, GetDisplayName(voiceChannel, userId));
            return Task.CompletedTask;
        };
        audioClient.PeerMicrophoneMuteChanged += (userId, isMuted) =>
        {
            _logger.LogInformation(
                "[PeerMicrophoneMuteChanged] The microphone state of a user changed [User ID] {UserId} [Display Name] {DisplayName} [Muted] {Muted}",
                userId, GetDisplayName(voiceChannel, userId), isMuted);
            return Task.CompletedTask;
        };
        audioClient.PeerHeadsetMuteChanged += (userId, isMuted) =>
        {
            _logger.LogInformation(
                "[PeerHeadsetMuteChanged] The headset state of a user changed [User ID] {UserId} [Display Name] {DisplayName} [Muted] {Muted}",
                userId, GetDisplayName(voiceChannel, userId), isMuted);
            return Task.CompletedTask;
        };
        audioClient.PeerPermissionChanged += (userId, permissions) =>
        {
            _logger.LogInformation(
                "[PeerPermissionChanged] The voice permissions of a user changed [User ID] {UserId} [Display Name] {DisplayName} [Is Self] {IsSelf} [Server Muted] {ServerMuted} [Server Headset Muted] {ServerHeadsetMuted} [Can Free Mic] {CanFreeMic} [Can Link] {CanLink} [Can Speak] {CanSpeak} [Can Manage Voice] {CanManageVoice}",
                userId, GetDisplayName(voiceChannel, userId), userId == voiceChannel.Guild.CurrentUser?.Id,
                permissions.IsServerMuted, permissions.IsServerHeadsetMuted, permissions.CanFreeMic, permissions.CanLink,
                permissions.CanSpeak, permissions.CanManageVoice);
            return Task.CompletedTask;
        };
        audioClient.AtmosphereReceived += (userId, itemId) =>
        {
            _logger.LogInformation("[AtmosphereReceived] A user used an atmosphere item [User ID] {UserId} [Display Name] {DisplayName} [Item ID] {ItemId}",
                userId, GetDisplayName(voiceChannel, userId), itemId);
            return Task.CompletedTask;
        };
        audioClient.AccompanimentStarted += (userId, accompaniment) =>
        {
            _logger.LogInformation(
                "[AccompanimentStarted] A user started an accompaniment [User ID] {UserId} [Display Name] {DisplayName} [Music Name] {MusicName} [Singer] {Singer} [Music Software] {MusicSoftware}",
                userId, GetDisplayName(voiceChannel, userId), accompaniment.MusicName, accompaniment.Singer, accompaniment.MusicSoftware);
            return Task.CompletedTask;
        };
        audioClient.AccompanimentStopped += userId =>
        {
            _logger.LogInformation("[AccompanimentStopped] A user stopped the accompaniment [User ID] {UserId} [Display Name] {DisplayName}",
                userId, GetDisplayName(voiceChannel, userId));
            return Task.CompletedTask;
        };

        // 服务端要求断开的通知。这三个事件之后都会紧接着引发 Disconnected
        audioClient.ServerDisconnected += (errorCode, errorReason) =>
        {
            // 实测中错误码 41106 对应多种原因：被管理员移出、同一账号用 WebRTC 连接了另一个频道、同一账号用 UDP 加入了同一个频道，
            // 后两种和前一种的 errorReason 文案不同，其中一种为空，不能只靠它们精确判断
            _logger.LogWarning("[ServerDisconnected] The server disconnected the audio client [Error Code] {ErrorCode} [Error Reason] {ErrorReason}",
                errorCode, errorReason);
            return Task.CompletedTask;
        };
        audioClient.ClosedByOtherDevice += () =>
        {
            _logger.LogWarning("[ClosedByOtherDevice] The audio client was closed because the user joined from another device");
            return Task.CompletedTask;
        };
        audioClient.ReloadRequested += () =>
        {
            _logger.LogWarning("[ReloadRequested] The server requested the audio client to reload. Reconnect to the voice channel to continue");
            return Task.CompletedTask;
        };
    }

    /// <summary> 开始录制一个用户的音频，每个用户保存为单独的文件。 </summary>
    /// <returns> 如果用户已经在被录制，则为 <c>false</c>。 </returns>
    public bool StartRecording(SocketVoiceChannel voiceChannel, ulong userId)
    {
        Session session = GetSession(voiceChannel);
        bool started = session.Recording.StartUser(userId);
        AttachExistingStreams(session);
        return started;
    }

    /// <summary> 开始录制频道里的每一个人，每个人的音频分别保存为单独的文件。 </summary>
    /// <returns> 如果已经在录制每一个人，则为 <c>false</c>。 </returns>
    public bool StartRecordingAll(SocketVoiceChannel voiceChannel)
    {
        Session session = GetSession(voiceChannel);
        bool started = session.Recording.StartAll();
        AttachExistingStreams(session);
        return started;
    }

    /// <summary> 停止录制一个用户的音频并保存文件。 </summary>
    /// <returns> 如果用户本来就不在被录制，则为 <c>false</c>。 </returns>
    public Task<bool> StopRecordingAsync(SocketVoiceChannel voiceChannel, ulong userId) =>
        GetSession(voiceChannel).Recording.StopUserAsync(userId);

    /// <summary> 停止所有录制并保存文件。 </summary>
    public Task StopRecordingAllAsync(SocketVoiceChannel voiceChannel) =>
        GetSession(voiceChannel).Recording.CompleteAsync();

    public (bool All, IReadOnlyCollection<ulong> Targets) GetRecordingStatus(SocketVoiceChannel voiceChannel)
    {
        RecordingService recording = GetSession(voiceChannel).Recording;
        return (recording.RecordAll, recording.Targets);
    }

    /// <summary>
    ///     用户已经在说话时，他的音频流在开始录制之前就已经存在，不会再触发创建事件，因此需要主动读取现有的音频流。
    /// </summary>
    private void AttachExistingStreams(Session session)
    {
        foreach ((uint ssrc, AudioInStream stream) in session.AudioClient.GetStreams())
        {
            if (session.AudioClient.TryGetStreamUserId(ssrc, out ulong userId))
                session.Recording.TryAttach(userId, GetDisplayName(session.VoiceChannel, userId), ssrc, stream);
        }
    }

    private static string? GetDisplayName(SocketVoiceChannel voiceChannel, ulong userId) =>
        voiceChannel.Guild.GetUser(userId)?.DisplayName;

    private void LogChannelUsers(SocketVoiceChannel voiceChannel, string message) =>
        _logger.LogInformation("[Channel] {Message} [Channel ID] {ChannelId} [Count] {Count} [Users] {Users}",
            message, voiceChannel.Id, voiceChannel.ConnectedUsers.Count,
            string.Join(", ", voiceChannel.ConnectedUsers.Select(x => $"{x.DisplayName}({x.Id})")));
}
