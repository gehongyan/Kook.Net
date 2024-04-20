using Kook.Net.WebSockets;

namespace Kook.Net.Queue;

internal class WebSocketRequest : IRequest
{
    public IWebSocketClient? Client { get; }
    public byte[] Data { get; }
    public bool IsText { get; }
    public bool IgnoreLimit { get; }
    public DateTimeOffset? TimeoutAt { get; }
    public TaskCompletionSource<Stream> Promise { get; }
    public RequestOptions Options { get; }

    public WebSocketRequest(IWebSocketClient? client, byte[] data, bool isText, bool ignoreLimit, RequestOptions options)
    {
        Preconditions.NotNull(options, nameof(options));

        Client = client;
        Data = data;
        IsText = isText;
        IgnoreLimit = ignoreLimit;
        Options = options;
        TimeoutAt = options.Timeout.HasValue ? DateTimeOffset.UtcNow.AddMilliseconds(options.Timeout.Value) : (DateTimeOffset?)null;
        Promise = new TaskCompletionSource<Stream>();
    }

    public async Task SendAsync()
    {
        if (Client == null)
        {
            Promise.SetException(new InvalidOperationException("WebSocket client is not set."));
            return;
        }
        await Client.SendAsync(Data, 0, Data.Length, IsText).ConfigureAwait(false);
    }
}
