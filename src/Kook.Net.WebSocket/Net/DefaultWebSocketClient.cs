using System.ComponentModel;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace Kook.Net.WebSockets;

internal class DefaultWebSocketClient : IWebSocketClient, IDisposable
{
    public const int ReceiveChunkSize = 16 * 1024; //16KB
    public const int SendChunkSize = 4 * 1024;     //4KB
    private const int HR_TIMEOUT = -2147012894;

    public event Func<byte[], int, int, Task>? BinaryMessage;
    public event Func<string, Task>? TextMessage;
    public event Func<Exception, Task>? Closed;

    private readonly SemaphoreSlim _lock;
    private readonly Dictionary<string, string> _headers;
    private readonly IWebProxy _proxy;
    private TimeSpan _keepAliveInterval;
    private ClientWebSocket _client;
    private Task _task;
    private CancellationTokenSource _disconnectTokenSource, _cancellationTokenSource;
    private CancellationToken _cancellationToken, _parentToken;
    private bool _isDisposed, _isDisconnecting;

    public DefaultWebSocketClient(IWebProxy proxy = null)
    {
        _lock = new SemaphoreSlim(1, 1);
        _disconnectTokenSource = new CancellationTokenSource();
        _cancellationToken = CancellationToken.None;
        _parentToken = CancellationToken.None;
        _headers = new Dictionary<string, string>();
        _proxy = proxy;
        _keepAliveInterval = System.Net.WebSockets.WebSocket.DefaultKeepAliveInterval;
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                DisconnectInternalAsync(isDisposing: true).GetAwaiter().GetResult();
                _disconnectTokenSource?.Dispose();
                _cancellationTokenSource?.Dispose();
                _lock?.Dispose();
            }

            _isDisposed = true;
        }
    }

    public void Dispose() => Dispose(true);

    public async Task ConnectAsync(string host)
    {
        await _lock.WaitAsync().ConfigureAwait(false);
        try
        {
            await ConnectInternalAsync(host).ConfigureAwait(false);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task ConnectInternalAsync(string host)
    {
        await DisconnectInternalAsync().ConfigureAwait(false);

        _disconnectTokenSource?.Dispose();
        _cancellationTokenSource?.Dispose();

        _disconnectTokenSource = new CancellationTokenSource();
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _disconnectTokenSource.Token);
        _cancellationToken = _cancellationTokenSource.Token;

        _client?.Dispose();
        _client = new ClientWebSocket();
        _client.Options.Proxy = _proxy;
        _client.Options.KeepAliveInterval = _keepAliveInterval;
        foreach (KeyValuePair<string, string> header in _headers)
            if (header.Value != null)
                _client.Options.SetRequestHeader(header.Key, header.Value);

        await _client.ConnectAsync(new Uri(host), _cancellationToken).ConfigureAwait(false);
        _task = RunAsync(_cancellationToken);
    }

    public async Task DisconnectAsync(int closeCode = 1000)
    {
        await _lock.WaitAsync().ConfigureAwait(false);
        try
        {
            await DisconnectInternalAsync(closeCode).ConfigureAwait(false);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task DisconnectInternalAsync(int closeCode = 1000, bool isDisposing = false)
    {
        _isDisconnecting = true;

        if (_client != null)
        {
            if (!isDisposing)
            {
                WebSocketCloseStatus status = (WebSocketCloseStatus)closeCode;
                try
                {
                    await _client.CloseOutputAsync(status, "", new CancellationToken());
                }
                catch
                {
                    // ignored
                }
            }

            try
            {
                _client.Dispose();
            }
            catch
            {
                // ignored
            }

            try
            {
                _disconnectTokenSource.Cancel(false);
            }
            catch
            {
                // ignored
            }

            _client = null;
        }

        try
        {
            await (_task ?? Task.Delay(0)).ConfigureAwait(false);
            _task = null;
        }
        finally
        {
            _isDisconnecting = false;
        }
    }

    private async Task OnClosed(Exception ex)
    {
        if (_isDisconnecting) return; //Ignore, this disconnect was requested.

        await _lock.WaitAsync().ConfigureAwait(false);
        try
        {
            await DisconnectInternalAsync(isDisposing: false);
        }
        finally
        {
            _lock.Release();
        }

        await Closed(ex);
    }

    public void SetHeader(string key, string value) => _headers[key] = value;

    public void SetKeepAliveInterval(TimeSpan keepAliveInterval)
    {
        _keepAliveInterval = keepAliveInterval;
    }

    public void SetCancellationToken(CancellationToken cancellationToken)
    {
        _cancellationTokenSource?.Dispose();

        _parentToken = cancellationToken;
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _disconnectTokenSource.Token);
        _cancellationToken = _cancellationTokenSource.Token;
    }

    public async Task SendAsync(byte[] data, int index, int count, bool isText)
    {
        try
        {
            await _lock.WaitAsync(_cancellationToken).ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        try
        {
            if (_client == null) return;

            int frameCount = (int)Math.Ceiling((double)count / SendChunkSize);

            for (int i = 0; i < frameCount; i++, index += SendChunkSize)
            {
                bool isLast = i == frameCount - 1;

                int frameSize;
                if (isLast)
                    frameSize = count - i * SendChunkSize;
                else
                    frameSize = SendChunkSize;

                WebSocketMessageType type = isText ? WebSocketMessageType.Text : WebSocketMessageType.Binary;
                await _client.SendAsync(new ArraySegment<byte>(data, index, count), type, isLast, _cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        ArraySegment<byte> buffer = new(new byte[ReceiveChunkSize]);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                WebSocketReceiveResult socketResult = await _client.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
                byte[] result;
                int resultCount;

                if (socketResult.MessageType == WebSocketMessageType.Close)
                    throw new WebSocketClosedException((int)socketResult.CloseStatus, socketResult.CloseStatusDescription);

                if (!socketResult.EndOfMessage)
                    //This is a large message (likely just READY), lets create a temporary expandable stream
                    using (MemoryStream stream = new())
                    {
                        stream.Write(buffer.Array, 0, socketResult.Count);
                        do
                        {
                            if (cancellationToken.IsCancellationRequested) return;

                            socketResult = await _client.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
                            stream.Write(buffer.Array, 0, socketResult.Count);
                        } while (socketResult == null || !socketResult.EndOfMessage);

                        //Use the internal buffer if we can get it
                        resultCount = (int)stream.Length;

                        result = stream.TryGetBuffer(out ArraySegment<byte> streamBuffer) ? streamBuffer.Array : stream.ToArray();
                    }
                else
                {
                    //Small message
                    resultCount = socketResult.Count;
                    result = buffer.Array;
                }

                if (socketResult.MessageType == WebSocketMessageType.Text)
                {
                    string text = Encoding.UTF8.GetString(result, 0, resultCount);
                    await TextMessage(text).ConfigureAwait(false);
                }
                else
                    await BinaryMessage(result, 0, resultCount).ConfigureAwait(false);
            }
        }
        catch (Win32Exception ex) when (ex.HResult == HR_TIMEOUT)
        {
            _ = OnClosed(new Exception("Connection timed out.", ex));
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
        catch (Exception ex)
        {
            //This cannot be awaited otherwise we'll deadlock when KookApiClient waits for this task to complete.
            _ = OnClosed(ex);
        }
    }
}
