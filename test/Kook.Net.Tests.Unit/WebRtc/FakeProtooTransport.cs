using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kook.Audio.WebRtc.Signaling;

namespace Kook.Audio.WebRtc;

/// <summary>
///     An in-memory <see cref="IProtooTransport"/> that records outgoing frames and lets tests inject incoming frames.
/// </summary>
internal sealed class FakeProtooTransport : IProtooTransport
{
    private readonly ConcurrentQueue<string> _sent = new();

    public event Func<string, Task>? TextReceived;

    public event Func<ProtooCloseInfo, Task>? Closed;

    public IReadOnlyCollection<string> Sent => _sent.ToArray();

    public Uri? ConnectedUri { get; private set; }

    public string? SubProtocol { get; private set; }

    public Task ConnectAsync(Uri uri, string subProtocol, CancellationToken cancellationToken)
    {
        ConnectedUri = uri;
        SubProtocol = subProtocol;
        return Task.CompletedTask;
    }

    public Task SendTextAsync(string text, CancellationToken cancellationToken)
    {
        _sent.Enqueue(text);
        return Task.CompletedTask;
    }

    public Task CloseAsync(CancellationToken cancellationToken) =>
        CloseFromServerAsync(1000, "normal closure");

    public Task ReceiveAsync(string text) => TextReceived?.Invoke(text) ?? Task.CompletedTask;

    public Task CloseFromServerAsync(int code, string reason) =>
        Closed?.Invoke(new ProtooCloseInfo(code, reason, null)) ?? Task.CompletedTask;

    public async Task<string> WaitForSentAsync(Func<string, bool>? predicate = null, int timeoutMilliseconds = 2000)
    {
        DateTime deadline = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);
        while (DateTime.UtcNow < deadline)
        {
            foreach (string frame in _sent)
            {
                if (predicate?.Invoke(frame) ?? true)
                    return frame;
            }
            await Task.Delay(5);
        }
        throw new TimeoutException("No matching frame was sent in time.");
    }

    public void Dispose()
    {
    }
}
