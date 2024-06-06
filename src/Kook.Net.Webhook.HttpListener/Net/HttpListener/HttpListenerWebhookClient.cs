using System.Net;

namespace Kook.Net.Webhooks.HttpListener;

internal class HttpListenerWebhookClient : IHttpListenerWebhookClient
{
    private bool _isDisposed;

    private readonly System.Net.HttpListener _httpListener;

    public event Func<byte[], int, int, Task<string?>>? BinaryMessage;
    public event Func<string, Task<string?>>? TextMessage;

    public HttpListenerWebhookClient()
    {
        _httpListener = new System.Net.HttpListener();
        _httpListener.Prefixes.Add("http://localhost:5043/");
        _httpListener.Prefixes.Add("http://127.0.0.1:5043/");
        _httpListener.Start();
        _ = Task.Run(async () =>
        {
            while (true)
            {
                HttpListenerContext context = await _httpListener.GetContextAsync();
                await HandleRequestAsync(context);
            }
        }, CancellationToken.None);
    }

    private async Task HandleRequestAsync(HttpListenerContext context)
    {
        try
        {
            string? messageResponse;
            if (context.Request.QueryString["compress"]?.Split(',').Any(x => x.StartsWith("0")) is true)
            {
                using StreamReader streamReader = new(context.Request.InputStream);
                string requestBody = await streamReader.ReadToEndAsync();
                messageResponse = await HandleTextMessageAsync(requestBody);
            }
            else
            {
                using MemoryStream stream = new();
                await context.Request.InputStream.CopyToAsync(stream);
                byte[] bytes = stream.ToArray();
                messageResponse = await HandleBinaryMessageAsync(bytes, 0, bytes.Length);
            }

            if (messageResponse is not null)
            {
                using StreamWriter writer = new(context.Response.OutputStream);
                await writer.WriteAsync(messageResponse);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.OutputStream.Close();
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

    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing)
        {
            _httpListener.Stop();
            _httpListener.Close();
        }
        _isDisposed = true;
    }

    public void Dispose() => Dispose(true);
}
