using Concentus;

namespace Kook.Audio.WebRtc.Streams;

/// <summary>
///     表示一个使用 Concentus 的 Opus 解码音频流，将 Opus 包解码为 PCM 数据后写入下一个音频流。
/// </summary>
/// <remarks>
///     与 <see cref="Kook.Audio.Streams.OpusDecodeStream"/> 的职责相同，区别是使用 Concentus，
///     并且支持 2.5 到 120 毫秒的任意帧长（KOOK 客户端发送的是 60 毫秒的包），解码结果始终是 48 kHz 的双声道。
/// </remarks>
internal sealed class WebRtcOpusDecodeStream : AudioOutStream
{
    private readonly AudioStream _next;
    private readonly IOpusDecoder _decoder;
    private readonly short[] _samples;
    private readonly byte[] _pcmBuffer;
    private bool _nextMissed;
    private bool _hasHeader;

    internal WebRtcOpusDecodeStream(AudioStream next)
    {
        _next = next;
        _decoder = OpusCodecFactory.CreateDecoder(WebRtcAudioFormat.SamplingRate, WebRtcAudioFormat.Channels);
        _samples = new short[WebRtcAudioFormat.MaxDecodedSamplesPerChannel * WebRtcAudioFormat.Channels];
        _pcmBuffer = new byte[_samples.Length * sizeof(short)];
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException"> 该流接收到没有有效负载的 RTP 头部。 </exception>
    public override void WriteHeader(ushort seq, uint timestamp, bool missed)
    {
        if (_hasHeader)
        {
            _hasHeader = false;
            throw new InvalidOperationException("Header received with no payload");
        }

        _hasHeader = true;
        _nextMissed = missed;
        _next.WriteHeader(seq, timestamp, missed);
    }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException"> 该流接收到没有 RTP 头部的有效负载。 </exception>
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (!_hasHeader)
            throw new InvalidOperationException("Received payload without an RTP header.");
        _hasHeader = false;

        int samplesPerChannel = !_nextMissed || count > 0
            ? _decoder.Decode(new ReadOnlySpan<byte>(buffer, offset, count), _samples,
                WebRtcAudioFormat.MaxDecodedSamplesPerChannel, false)
            : _decoder.Decode(ReadOnlySpan<byte>.Empty, _samples, WebRtcAudioFormat.FrameSamplesPerChannel, false);
        int bytes = samplesPerChannel * WebRtcAudioFormat.SampleBytes;
        Buffer.BlockCopy(_samples, 0, _pcmBuffer, 0, bytes);
        return _next.WriteAsync(_pcmBuffer, 0, bytes, cancellationToken);
    }

    /// <inheritdoc />
    public override Task FlushAsync(CancellationToken cancellationToken) =>
        _next.FlushAsync(cancellationToken);

    /// <inheritdoc />
    public override Task ClearAsync(CancellationToken cancellationToken) =>
        _next.ClearAsync(cancellationToken);

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _decoder.Dispose();
            _next.Dispose();
        }
        base.Dispose(disposing);
    }
}
