using System.Net.WebSockets;
using System.Text;

namespace Kook.Audio.WebRtc.Signaling;

internal sealed class ClientWebSocketProtooTransport : IProtooTransport
{
    private const int ReceiveBufferSize = 16 * 1024;

    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private ClientWebSocket? _socket;
    private CancellationTokenSource? _receiveCts;
    private int _closedRaised;

    public event Func<string, Task>? TextReceived;

    public event Func<ProtooCloseInfo, Task>? Closed;

    public async Task ConnectAsync(Uri uri, string subProtocol, CancellationToken cancellationToken)
    {
        if (_socket is not null)
            throw new InvalidOperationException("The transport has already been connected.");

        ClientWebSocket socket = new();
        socket.Options.AddSubProtocol(subProtocol);
        try
        {
            await socket.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            socket.Dispose();
            throw;
        }

        _socket = socket;
        _receiveCts = new CancellationTokenSource();
        CancellationToken token = _receiveCts.Token;
        _ = Task.Run(() => ReceiveLoopAsync(socket, token), CancellationToken.None);
    }

    public async Task SendTextAsync(string text, CancellationToken cancellationToken)
    {
        ClientWebSocket socket = _socket ?? throw new InvalidOperationException("The transport is not connected.");
        byte[] payload = Encoding.UTF8.GetBytes(text);
        await _sendLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await socket.SendAsync(new ArraySegment<byte>(payload), WebSocketMessageType.Text, true, cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            _sendLock.Release();
        }
    }

    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        ClientWebSocket? socket = _socket;
        if (socket is null)
            return;
        try
        {
            if (socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken).ConfigureAwait(false);
        }
        catch (WebSocketException)
        {
            // 对端已断开时关闭握手会失败，此时接收循环会负责上报关闭
        }
    }

    private async Task ReceiveLoopAsync(ClientWebSocket socket, CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[ReceiveBufferSize];
        using MemoryStream message = new();
        int? closeCode = null;
        string? closeReason = null;
        Exception? error = null;
        try
        {
            while (!cancellationToken.IsCancellationRequested && socket.State is WebSocketState.Open or WebSocketState.CloseSent)
            {
                message.SetLength(0);
                WebSocketReceiveResult result;
                do
                {
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken).ConfigureAwait(false);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        closeCode = (int?)result.CloseStatus;
                        closeReason = result.CloseStatusDescription;
                        try
                        {
                            await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None)
                                .ConfigureAwait(false);
                        }
                        catch (WebSocketException)
                        {
                            // 对端已关闭，忽略
                        }
                        return;
                    }
                    message.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType != WebSocketMessageType.Text)
                    continue;

                string text = Encoding.UTF8.GetString(message.GetBuffer(), 0, (int)message.Length);
                Func<string, Task>? handler = TextReceived;
                if (handler is null)
                    continue;
                try
                {
                    await handler(text).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // 订阅者的异常不应终止接收循环
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // 本端主动释放
        }
        catch (Exception ex)
        {
            error = ex;
        }
        finally
        {
            await RaiseClosedAsync(new ProtooCloseInfo(closeCode, closeReason, error)).ConfigureAwait(false);
        }
    }

    private async Task RaiseClosedAsync(ProtooCloseInfo info)
    {
        if (Interlocked.Exchange(ref _closedRaised, 1) == 1)
            return;
        Func<ProtooCloseInfo, Task>? handler = Closed;
        if (handler is null)
            return;
        try
        {
            await handler(info).ConfigureAwait(false);
        }
        catch (Exception)
        {
            // 订阅者的异常不应向外传播
        }
    }

    public void Dispose()
    {
        _receiveCts?.Cancel();
        _receiveCts?.Dispose();
        _socket?.Dispose();
        _sendLock.Dispose();
    }
}
