using System.Diagnostics;
using Kook.Audio;
using Kook.Audio.WebRtc;
using Kook.WebSocket;
using Microsoft.Extensions.Logging;

namespace Kook.Net.Samples.WebRTC.Services;

/// <summary>
///     一个语音频道的点歌队列和播放循环：依次取出队列里的歌，用 ffmpeg 转换为 PCM 后写入音频流。
/// </summary>
public sealed class MusicClient : IAsyncDisposable
{
    private readonly SocketVoiceChannel _voiceChannel;
    private readonly IWebRtcAudioClient _audioClient;
    private readonly WebRtcSampleOptions _options;
    private readonly ILogger<MusicClient> _logger;
    private readonly List<MusicTrack> _pending = [];
    private readonly SemaphoreSlim _signal = new(0);
    private readonly Lock _gate = new();
    private readonly CancellationTokenSource _lifetime = new();
    private readonly Task _loop;
    private CancellationTokenSource? _trackCancellation;

    public MusicClient(SocketVoiceChannel voiceChannel, IWebRtcAudioClient audioClient, WebRtcSampleOptions options,
        ILogger<MusicClient> logger)
    {
        _voiceChannel = voiceChannel;
        _audioClient = audioClient;
        _options = options;
        _logger = logger;
        _loop = Task.Run(RunAsync);
    }

    /// <summary> 正在播放的歌。 </summary>
    public MusicTrack? CurrentPlaying { get; private set; }

    /// <summary> 还在排队的歌。 </summary>
    public IReadOnlyList<MusicTrack> Queue
    {
        get
        {
            lock (_gate)
                return [.. _pending];
        }
    }

    public void Enqueue(MusicTrack track)
    {
        lock (_gate)
            _pending.Add(track);
        _signal.Release();
        _logger.LogInformation("[Music] Enqueued a track [Channel ID] {ChannelId} [Title] {Title} [Source] {Source}",
            _voiceChannel.Id, track.Title, track.Source);
    }

    /// <summary> 跳过正在播放的歌。 </summary>
    /// <returns> 是否有歌被跳过。 </returns>
    public bool Skip()
    {
        lock (_gate)
        {
            if (_trackCancellation is null)
                return false;
            _trackCancellation.Cancel();
            return true;
        }
    }

    private async Task RunAsync()
    {
        CancellationToken cancellationToken = _lifetime.Token;
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _signal.WaitAsync(cancellationToken).ConfigureAwait(false);
                MusicTrack track;
                using CancellationTokenSource trackCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                lock (_gate)
                {
                    track = _pending[0];
                    _pending.RemoveAt(0);
                    _trackCancellation = trackCancellation;
                }

                CurrentPlaying = track;
                try
                {
                    await PlayAsync(track, trackCancellation.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("[Music] Skipped a track [Channel ID] {ChannelId} [Title] {Title}",
                        _voiceChannel.Id, track.Title);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (InvalidOperationException e)
                {
                    _logger.LogWarning("[Music] A track is not playable [Channel ID] {ChannelId} [Title] {Title} [Error] {Error}",
                        _voiceChannel.Id, track.Title, e.Message);
                    _ = _voiceChannel.SendTextAsync($"Can't play {track.Title}: {e.Message}");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "[Music] Failed to play a track [Channel ID] {ChannelId} [Title] {Title}",
                        _voiceChannel.Id, track.Title);
                }
                finally
                {
                    lock (_gate)
                        _trackCancellation = null;
                    CurrentPlaying = null;
                }

                bool idle;
                lock (_gate)
                    idle = _pending.Count == 0;
                if (idle && !_audioClient.IsMicrophoneMuted)
                    await _audioClient.SetMicrophoneMutedAsync(true).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Music] The playback loop stopped unexpectedly [Channel ID] {ChannelId}", _voiceChannel.Id);
        }
    }

    private async Task PlayAsync(MusicTrack track, CancellationToken cancellationToken)
    {
        if (_audioClient.ConnectionState != ConnectionState.Connected)
            return;

        // 客户端加入频道时处于静音状态，发送前需要取消静音
        if (_audioClient.IsMicrophoneMuted)
            await _audioClient.SetMicrophoneMutedAsync(false).ConfigureAwait(false);

        _logger.LogInformation("[Music] Now playing [Channel ID] {ChannelId} [Title] {Title}", _voiceChannel.Id, track.Title);
        _ = _voiceChannel.SendTextAsync($"Now playing {track.Title}");

        // 网络状况良好时不需要带内 FEC 与丢包补偿，packetLoss 设为 0 可以把全部码率用于音质
        await using AudioOutStream kook = _audioClient.CreatePcmStream(AudioApplication.Music, packetLoss: 0);
        try
        {
            await StreamFromFfmpegAsync(track, kook, cancellationToken).ConfigureAwait(false);
            await kook.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // 被跳过或停止时丢弃还在缓冲区里的音频，而不是把它们播完
            await kook.ClearAsync(CancellationToken.None).ConfigureAwait(false);
            throw;
        }
    }

    private async Task StreamFromFfmpegAsync(MusicTrack track, AudioOutStream kook, CancellationToken cancellationToken)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = _options.FfmpegPath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        // -nostdin：避免 ffmpeg 读取控制台输入；-protocol_whitelist：只允许读取这首歌所需的协议，防止播放列表引用其他资源；
        // -vn：忽略封面图等视频流；48 kHz 双声道 16 位 PCM 是 CreatePcmStream 约定的输入格式。
        string source = track.Source;
        List<string> arguments = ["-nostdin", "-hide_banner", "-loglevel", "error"];
        if (track.IsRemote)
        {
            // 重定向由这里手动跟随并逐跳检查，ffmpeg 拿到的是最终地址。歌曲链接在加入队列之后可能已经失效，因此在播放前才解析
            source = (await MusicSourceResolver.ResolvePlayableUriAsync(new Uri(track.Source), cancellationToken)
                .ConfigureAwait(false)).AbsoluteUri;
            arguments.AddRange(["-protocol_whitelist", "http,https,tcp,tls,crypto", "-user_agent", MusicSourceResolver.UserAgent]);
        }
        else
            arguments.AddRange(["-protocol_whitelist", "file"]);
        arguments.AddRange(["-i", source, "-vn", "-ac", "2", "-f", "s16le", "-ar", "48000", "-"]);
        foreach (string argument in arguments)
            startInfo.ArgumentList.Add(argument);

        using Process ffmpeg = Process.Start(startInfo)
            ?? throw new InvalidOperationException($"Failed to start ffmpeg ({_options.FfmpegPath}).");
        _logger.LogInformation("[Music] Started ffmpeg [Channel ID] {ChannelId} [Process ID] {ProcessId}", _voiceChannel.Id, ffmpeg.Id);
        Task errorLogging = LogFfmpegErrorsAsync(ffmpeg);
        await using CancellationTokenRegistration kill = cancellationToken.Register(() =>
        {
            try
            {
                ffmpeg.Kill(true);
            }
            catch (InvalidOperationException)
            {
                // 进程已经退出
            }
        });
        try
        {
            await ffmpeg.StandardOutput.BaseStream.CopyToAsync(kook, cancellationToken).ConfigureAwait(false);
            await ffmpeg.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("[Music] ffmpeg exited [Channel ID] {ChannelId} [Exit Code] {ExitCode}", _voiceChannel.Id, ffmpeg.ExitCode);
        }
        finally
        {
            await errorLogging.ConfigureAwait(false);
        }
    }

    private async Task LogFfmpegErrorsAsync(Process ffmpeg)
    {
        try
        {
            while (await ffmpeg.StandardError.ReadLineAsync().ConfigureAwait(false) is { } line)
                _logger.LogWarning("[ffmpeg] {Line}", line);
        }
        catch (ObjectDisposedException)
        {
            // ignored
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _lifetime.CancelAsync().ConfigureAwait(false);
        await _loop.ConfigureAwait(false);
        _lifetime.Dispose();
        _signal.Dispose();
    }
}
