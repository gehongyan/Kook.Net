using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Kook.Net.Webhooks.AspNet;

internal class AspNetWebhookClient : IAspNetWebhookClient
{
    private bool _isDisposed;

    /// <inheritdoc />
    public event Func<byte[], int, int, Task<string?>>? BinaryMessage;

    /// <inheritdoc />
    public event Func<string, Task<string?>>? TextMessage;

    /// <inheritdoc />
    public event Func<Exception, Task>? Closed;

    /// <inheritdoc />
    public async Task HandleRequestAsync(HttpContext httpContext)
    {
        string? messageResponse;
        if (httpContext.Request.Query.TryGetValue("compress", out StringValues compressValues)
            && compressValues.Any(x => x?.StartsWith('0') is true))
        {
            using StreamReader streamReader = new(httpContext.Request.Body);
            string requestBody = await streamReader.ReadToEndAsync();
            messageResponse = await HandleTextMessageAsync(requestBody);
        }
        else
        {
            using MemoryStream stream = new();
            await httpContext.Request.Body.CopyToAsync(stream);
            byte[] bytes = stream.ToArray();
            messageResponse = await HandleBinaryMessageAsync(bytes, 0, bytes.Length);
        }

        if (messageResponse is not null)
            await httpContext.Response.WriteAsync(messageResponse);
    }

    /// <inheritdoc />
    public Task<string?> HandleTextMessageAsync(string requestBody) =>
        TextMessage is not null ? TextMessage(requestBody) : Task.FromResult<string?>(null);

    /// <inheritdoc />
    public Task<string?> HandleBinaryMessageAsync(byte[] data, int index, int count) =>
        BinaryMessage is not null ? BinaryMessage(data, index, count) : Task.FromResult<string?>(null);

    internal void OnClosed(Exception ex) => Closed?.Invoke(ex);

    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing)
        {
        }
        _isDisposed = true;
    }

    public void Dispose() => Dispose(true);
}
