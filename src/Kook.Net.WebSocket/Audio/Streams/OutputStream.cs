namespace Kook.Audio.Streams;

/// <summary>
///     表示输出音频流，用于包装一个 <see cref="IAudioClient"/>，在写入时向语音服务器发送语音数据。
/// </summary>
public class OutputStream : AudioOutStream
{
    private readonly KookVoiceAPIClient _client;

    internal OutputStream(KookVoiceAPIClient client)
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
        return _client.SendAsync(buffer, offset, count);
    }
}
