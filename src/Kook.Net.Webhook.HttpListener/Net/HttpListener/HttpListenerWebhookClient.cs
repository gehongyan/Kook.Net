using System.Net;

namespace Kook.Net.Webhooks.HttpListener;

internal class HttpListenerWebhookClient : IHttpListenerWebhookClient
{
    private readonly SemaphoreSlim _lock;
    private System.Net.HttpListener? _httpListener;
    private Task? _task;
    private CancellationTokenSource _disconnectTokenSource;
    private CancellationTokenSource? _cancellationTokenSource;
    private CancellationToken _cancellationToken;
    private CancellationToken _parentToken;

    private bool _isDisposed, _isStopping;

    /// <inheritdoc />
    public event Func<byte[], int, int, Task<string?>>? BinaryMessage;

    /// <inheritdoc />
    public event Func<string, Task<string?>>? TextMessage;

    /// <inheritdoc />
    public event Func<Exception, Task>? Closed;

    public HttpListenerWebhookClient()
    {
        _lock = new SemaphoreSlim(1, 1);
        _disconnectTokenSource = new CancellationTokenSource();
        _cancellationToken = CancellationToken.None;
        _parentToken = CancellationToken.None;
    }

    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing)
        {
            StopInternalAsync(isDisposing: true).GetAwaiter().GetResult();
            _disconnectTokenSource?.Dispose();
            _cancellationTokenSource?.Dispose();
            _lock?.Dispose();
        }
        _isDisposed = true;
    }

    public void Dispose() => Dispose(true);

    /// <inheritdoc />
    public void SetCancellationToken(CancellationToken cancellationToken)
    {
        _cancellationTokenSource?.Dispose();

        _parentToken = cancellationToken;
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _disconnectTokenSource.Token);
        _cancellationToken = _cancellationTokenSource.Token;
    }

    /// <inheritdoc />
    public async Task StartAsync(IEnumerable<string> uriPrefixes)
    {
        await _lock.WaitAsync(CancellationToken.None).ConfigureAwait(false);
        try
        {
            await StartInternalAsync(uriPrefixes).ConfigureAwait(false);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task StartInternalAsync(IEnumerable<string> uriPrefixes)
    {
        await StopInternalAsync().ConfigureAwait(false);

        _disconnectTokenSource?.Dispose();
        _cancellationTokenSource?.Dispose();

        _disconnectTokenSource = new CancellationTokenSource();
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _disconnectTokenSource.Token);
        _cancellationToken = _cancellationTokenSource.Token;

        _httpListener?.Close();
        _httpListener?.Stop();
        _httpListener = new System.Net.HttpListener();
        foreach (string uriPrefix in uriPrefixes)
            _httpListener.Prefixes.Add(uriPrefix);
        _httpListener.Start();
        _task = RunAsync(_cancellationToken);
    }

    /// <inheritdoc />
    public async Task StopAsync()
    {
        await _lock.WaitAsync(CancellationToken.None).ConfigureAwait(false);
        try
        {
            await StopInternalAsync().ConfigureAwait(false);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task StopInternalAsync(bool isDisposing = false)
    {
        _isStopping = true;
        if (_httpListener != null)
        {
            if (!isDisposing)
            {
                try
                {
                    _httpListener.Close();
                    _httpListener.Stop();
                }
                catch
                {
                    // ignored
                }
            }

            try
            {
                _disconnectTokenSource.Cancel(false);
            }
            catch
            {
                // ignored
            }

            _httpListener = null;
        }

        try
        {
            if (_task is not null)
            {
                await _task.ConfigureAwait(false);
                _task = null;
            }
        }
        finally
        {
            _isStopping = false;
        }
    }

    private async Task OnClosed(Exception ex)
    {
        if (_isStopping)
            return; //Ignore, this disconnect was requested.

        await _lock.WaitAsync(CancellationToken.None).ConfigureAwait(false);
        try
        {
            await StopInternalAsync(isDisposing: false);
        }
        finally
        {
            _lock.Release();
        }

        if (Closed is not null)
            await Closed(ex);
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_httpListener is null)
                    throw new InvalidOperationException("The client is not created.");
                HttpListenerContext context = await _httpListener.GetContextAsync();
                await HandleRequestAsync(context);
            }
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
        catch (Exception ex)
        {
            _ = OnClosed(ex);
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context)
    {
        try
        {
            string? messageResponse;
            if (context.Request.QueryString["compress"]?.Split(',').Any(x => x.StartsWith("0")) is true)
            {
                using StreamReader streamReader = new(context.Request.InputStream);
                string requestBody = await streamReader.ReadToEndAsync(_cancellationToken);
                messageResponse = await HandleTextMessageAsync(requestBody);
            }
            else
            {
                using MemoryStream stream = new();
#if NETSTANDARD2_0 || NETFRAMEWORK
                await context.Request.InputStream.CopyToAsync(stream);
#else
                await context.Request.InputStream.CopyToAsync(stream, _cancellationToken);
#endif
                byte[] bytes = stream.ToArray();
                messageResponse = await HandleBinaryMessageAsync(bytes, 0, bytes.Length);
            }

            if (messageResponse is not null)
            {
#if SUPPORTS_ASYNC_DISPOSABLE
                await
#endif
                using StreamWriter writer = new(context.Response.OutputStream);
                await writer.WriteAsync(messageResponse);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                await context.Response.OutputStream.WriteAsync(ReadOnlyMemory<byte>.Empty, _cancellationToken);
            }
        }
        catch
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.OutputStream.Close();
        }
    }

    /// <inheritdoc />
    public Task<string?> HandleTextMessageAsync(string requestBody) =>
        TextMessage is not null ? TextMessage(requestBody) : Task.FromResult<string?>(null);

    /// <inheritdoc />
    public Task<string?> HandleBinaryMessageAsync(byte[] data, int index, int count) =>
        BinaryMessage is not null ? BinaryMessage(data, index, count) : Task.FromResult<string?>(null);
}
