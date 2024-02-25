namespace Kook.Audio.Streams;

///<summary> Wraps an IAudioClient, sending voice data on write. </summary>
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
