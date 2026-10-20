using Concentus;
using ConcentusApplication = Concentus.Enums.OpusApplication;
using ConcentusSignal = Concentus.Enums.OpusSignal;

namespace Kook.Audio.WebRtc.Streams;

/// <summary>
///     表示一个使用 Concentus 的 Opus 编码音频流，将 PCM 数据按 <see cref="WebRtcAudioFormat.FrameMillis"/> 毫秒一帧编码后写入下一个音频流。
/// </summary>
/// <remarks>
///     与 <see cref="Kook.Audio.Streams.OpusEncodeStream"/> 的职责相同，区别是使用 Concentus，
///     它优先使用平台上可用的原生 Opus 库，不可用时回退到纯托管的实现。
/// </remarks>
internal sealed class WebRtcOpusEncodeStream : AudioOutStream
{
    private const int MaxBitrate = 510000;

    private readonly AudioStream _next;
    private readonly IOpusEncoder _encoder;
    private readonly byte[] _pcmBuffer;
    private readonly short[] _samples;
    private readonly byte[] _packetBuffer;
    private int _partialFramePos;

    internal WebRtcOpusEncodeStream(AudioStream next, int bitrate, AudioApplication application, int packetLoss)
    {
        if (bitrate is < 1 or > MaxBitrate)
            throw new ArgumentOutOfRangeException(nameof(bitrate));

        (ConcentusApplication opusApplication, ConcentusSignal opusSignal) = application switch
        {
            AudioApplication.Mixed => (ConcentusApplication.OPUS_APPLICATION_AUDIO, ConcentusSignal.OPUS_SIGNAL_AUTO),
            AudioApplication.Music => (ConcentusApplication.OPUS_APPLICATION_AUDIO, ConcentusSignal.OPUS_SIGNAL_MUSIC),
            AudioApplication.Voice => (ConcentusApplication.OPUS_APPLICATION_VOIP, ConcentusSignal.OPUS_SIGNAL_VOICE),
            _ => throw new ArgumentOutOfRangeException(nameof(application))
        };

        if (packetLoss is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(packetLoss));

        _next = next;
        _encoder = OpusCodecFactory.CreateEncoder(WebRtcAudioFormat.SamplingRate, WebRtcAudioFormat.Channels, opusApplication);
        _encoder.SignalType = opusSignal;
        _encoder.Bitrate = bitrate;
        // 实测在 32 kbps 下启用带内 FEC 并预期 10% 以上丢包时，编码器会收窄带宽以腾出码率给冗余数据，
        // 同一段音乐的 SNR 下降约 2.4 dB，高频能量只剩一半左右。因此仅在调用方明确预期丢包时才启用 FEC。
        _encoder.UseInbandFEC = packetLoss > 0;
        _encoder.PacketLossPercent = packetLoss;
        _encoder.UseDTX = false;

        _pcmBuffer = new byte[WebRtcAudioFormat.FrameBytes];
        _samples = new short[WebRtcAudioFormat.FrameSamplesPerChannel * WebRtcAudioFormat.Channels];
        _packetBuffer = new byte[WebRtcAudioFormat.MaxPacketBytes];
    }

    /// <inheritdoc />
    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        // Assume thread-safe
        while (count > 0)
        {
            if (_partialFramePos == 0 && count >= WebRtcAudioFormat.FrameBytes)
            {
                // We have enough data and no partial frames. Encode directly from the caller's buffer
                await EncodeAndWriteAsync(buffer, offset, cancellationToken).ConfigureAwait(false);
                offset += WebRtcAudioFormat.FrameBytes;
                count -= WebRtcAudioFormat.FrameBytes;
            }
            else if (_partialFramePos + count >= WebRtcAudioFormat.FrameBytes)
            {
                // We have enough data to complete a previous partial frame
                int partialSize = WebRtcAudioFormat.FrameBytes - _partialFramePos;
                Buffer.BlockCopy(buffer, offset, _pcmBuffer, _partialFramePos, partialSize);
                await EncodeAndWriteAsync(_pcmBuffer, 0, cancellationToken).ConfigureAwait(false);
                offset += partialSize;
                count -= partialSize;
                _partialFramePos = 0;
            }
            else
            {
                // Not enough data to build a complete frame, store this part for later
                Buffer.BlockCopy(buffer, offset, _pcmBuffer, _partialFramePos, count);
                _partialFramePos += count;
                break;
            }
        }
    }

    private Task EncodeAndWriteAsync(byte[] pcm, int offset, CancellationToken cancellationToken)
    {
        Buffer.BlockCopy(pcm, offset, _samples, 0, WebRtcAudioFormat.FrameBytes);
        int length = _encoder.Encode(_samples, WebRtcAudioFormat.FrameSamplesPerChannel, _packetBuffer, _packetBuffer.Length);
        return _next.WriteAsync(_packetBuffer, 0, length, cancellationToken);
    }

    /// <inheritdoc />
    public override Task FlushAsync(CancellationToken cancellationToken) =>
        _next.FlushAsync(cancellationToken);

    /// <inheritdoc />
    public override Task ClearAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _partialFramePos = 0;
        return _next.ClearAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _encoder.Dispose();
            _next.Dispose();
        }
        base.Dispose(disposing);
    }
}
