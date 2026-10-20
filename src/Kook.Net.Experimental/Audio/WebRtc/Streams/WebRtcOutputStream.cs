using Kook.Audio.Streams;

namespace Kook.Audio.WebRtc.Streams;

/// <summary>
///     表示 WebRTC 输出音频流，用于在写入时通过 WebRTC 媒体传输向语音服务器发送一个 Opus 包。
/// </summary>
/// <remarks>
///     RTP 头部的序列号、时间戳与 SSRC 由底层 WebRTC 传输生成，因此这个流会忽略
///     <see cref="Kook.Audio.AudioStream.WriteHeader(System.UInt16,System.UInt32,System.Boolean)"/> 传入的头部。
/// </remarks>
internal sealed class WebRtcOutputStream : AudioOutStream
{
    private readonly WebRtcAudioClient _client;

    internal WebRtcOutputStream(WebRtcAudioClient client)
    {
        _client = client;
    }

    /// <inheritdoc />
    public override void WriteHeader(ushort seq, uint timestamp, bool missed)
    {
        // Ignore
    }

    /// <inheritdoc />
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        byte[] payload = new byte[count];
        Buffer.BlockCopy(buffer, offset, payload, 0, count);
        _client.SendOpusPacket(payload);
        return Task.CompletedTask;
    }
}
