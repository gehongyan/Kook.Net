using Kook.Audio;

namespace Kook.Net.Samples.WebRTC.Services;

/// <summary>
///     把某一位用户的音频保存为一个 WAV 文件。
/// </summary>
/// <remarks>
///     <see cref="AudioInStream"/> 提供的是已经解码好的 48 kHz、16 位、双声道 PCM。对端静音或停止说话时服务端不会下发数据，
///     这里按 RTP 时间戳补齐静音，使文件的时间轴与真实时间一致。同一用户的 SSRC 变化时（例如重新开麦），
///     新的音频流会继续写入同一个文件。
/// </remarks>
public sealed class UserRecorder : IDisposable
{
    private const int SampleRate = 48000;
    private const int Channels = 2;
    private const int SampleBytes = sizeof(short) * Channels;
    private const int MaxGapSamples = SampleRate * 600;

    private readonly Lock _gate = new();
    private readonly WavWriter _wav;
    private bool _hasPrevious;
    private ushort _lastSequence;
    private uint _nextTimestamp;
    private bool _disposed;

    public UserRecorder(ulong userId, string? displayName, string path)
    {
        UserId = userId;
        DisplayName = displayName;
        _wav = new WavWriter(path, SampleRate, Channels);
    }

    public ulong UserId { get; }

    public string? DisplayName { get; }

    public string Path => _wav.Path;

    public int Segments { get; private set; }

    public long Frames { get; private set; }

    public long DroppedFrames { get; private set; }

    public double SilenceSeconds { get; private set; }

    public double DurationSeconds
    {
        get
        {
            lock (_gate)
                return _wav.DurationSeconds;
        }
    }

    /// <summary>
    ///     开始一个新的音频流段，RTP 的序列号与时间戳会从头开始计算。
    /// </summary>
    public void BeginSegment()
    {
        lock (_gate)
        {
            _hasPrevious = false;
            Segments++;
        }
    }

    public void WriteFrame(RtpFrame frame)
    {
        lock (_gate)
        {
            if (_disposed)
                return;
            Frames++;

            if (_hasPrevious)
            {
                if ((short)(frame.Sequence - _lastSequence) <= 0)
                {
                    DroppedFrames++;
                    return;
                }

                int gap = (int)(frame.Timestamp - _nextTimestamp);
                if (gap is > 0 and <= MaxGapSamples)
                {
                    _wav.WriteSilence(gap);
                    SilenceSeconds += (double)gap / SampleRate;
                }
            }

            _wav.Write(frame.Payload, 0, frame.Payload.Length);
            _hasPrevious = true;
            _lastSequence = frame.Sequence;
            _nextTimestamp = frame.Timestamp + (uint)(frame.Payload.Length / SampleBytes);
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
                return;
            _disposed = true;
            _wav.Dispose();
        }
    }
}
