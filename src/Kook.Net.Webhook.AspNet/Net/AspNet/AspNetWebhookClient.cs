using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Kook.Net.Webhooks.AspNet;

internal class AspNetWebhookClient : IAspNetWebhookClient
{
    private bool _isDisposed;

    public event Func<byte[], int, int, Task<string?>>? BinaryMessage;
    public event Func<string, Task<string?>>? TextMessage;

    /// <summary>
    ///     Handles a Kook webhook request.
    /// </summary>
    public async Task HandleRequestAsync(HttpContext httpContext)
    {
        using StreamReader streamReader = new(httpContext.Request.Body);
        if (httpContext.Request.Query.TryGetValue("compress", out StringValues compressValues)
            && compressValues.Any(x => x?.StartsWith('0') is true))
        {
            string requestBody = await streamReader.ReadToEndAsync();
            string? textMessageResponse = await HandleTextMessageAsync(requestBody);
            if (textMessageResponse is not null)
                await httpContext.Response.WriteAsync(textMessageResponse);
        }

        using MemoryStream stream = new();
        await httpContext.Request.Body.CopyToAsync(stream);
        byte[] bytes = stream.ToArray();
        string? binaryMessageResponse = await HandleBinaryMessageAsync(bytes, 0, bytes.Length);
        if (binaryMessageResponse is not null)
            await httpContext.Response.WriteAsync(binaryMessageResponse);
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
        }
        _isDisposed = true;
    }

    public void Dispose() => Dispose(true);
}
