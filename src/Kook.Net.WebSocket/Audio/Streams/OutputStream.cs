using System.Threading;
using System.Threading.Tasks;

namespace Kook.Audio.Streams;

///<summary> Wraps an IAudioClient, sending voice data on write. </summary>
public class OutputStream : AudioOutStream
{
    private readonly KookVoiceAPIClient _client;
    public OutputStream(IAudioClient client)
        : this((client as AudioClient).ApiClient) { }
    internal OutputStream(KookVoiceAPIClient client)
    {
        _client = client;
    }

    public override void WriteHeader(ushort seq, uint timestamp, bool missed) { } //Ignore
    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancelToken)
    {
        cancelToken.ThrowIfCancellationRequested();
        await _client.SendAsync(buffer, offset, count).ConfigureAwait(false);
    }
}
