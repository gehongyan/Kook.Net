#if NET462
using System.Net.Http;
#endif

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
using System.Net.Mime;
#endif

using System.Collections.Immutable;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kook.Net.Rest;

internal sealed class DefaultRestClient : IRestClient, IDisposable
{
    private const int HR_SECURECHANNELFAILED = -2146233079;

    private readonly HttpClient _client;
    private readonly string _baseUrl;
    private CancellationToken _cancellationToken;
    private bool _isDisposed;

    private readonly JsonSerializerOptions _serializerOptions;
    private static int _nextId;

    public DefaultRestClient(string baseUrl,
        bool useProxy = false, IWebProxy? webProxy = null,
        Func<HttpClient>? httpClientFactory = null)
    {
        _baseUrl = baseUrl;

        if (httpClientFactory is not null)
        {
            _client = httpClientFactory();
            if (_client.BaseAddress is not null)
                throw new ArgumentException("The HttpClient provided by the factory must not have a BaseAddress set.", nameof(httpClientFactory));
        }
        else
        {
            _client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = false,
                UseProxy = useProxy,
                Proxy = webProxy
            });
            SetHeader("accept-encoding", "gzip, deflate");
        }

        _cancellationToken = CancellationToken.None;

        _serializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
                _client.Dispose();
            _isDisposed = true;
        }
    }

    public void Dispose() => Dispose(true);

    public void SetHeader(string key, string? value)
    {
        _client.DefaultRequestHeaders.Remove(key);
        if (value != null) _client.DefaultRequestHeaders.Add(key, value);
    }

    public void SetCancellationToken(CancellationToken cancellationToken) => _cancellationToken = cancellationToken;

    public async Task<RestResponse> SendAsync(HttpMethod method, string endpoint, CancellationToken cancellationToken,
        string? reason = null,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>>? requestHeaders = null)
    {
        string uri = Path.Combine(_baseUrl, endpoint);
        using (HttpRequestMessage restRequest = new(method, uri))
        {
            if (reason != null)
                restRequest.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(reason));

            if (requestHeaders != null)
                foreach (KeyValuePair<string, IEnumerable<string>> header in requestHeaders)
                    restRequest.Headers.Add(header.Key, header.Value);

            return await SendInternalAsync(restRequest, cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task<RestResponse> SendAsync(HttpMethod method, string endpoint, string json,
        CancellationToken cancellationToken, string? reason = null,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>>? requestHeaders = null)
    {
        string uri = Path.Combine(_baseUrl, endpoint);
        using HttpRequestMessage restRequest = new(method, uri);
        if (reason != null)
            restRequest.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(reason));

        if (requestHeaders != null)
            foreach (KeyValuePair<string, IEnumerable<string>> header in requestHeaders)
                restRequest.Headers.Add(header.Key, header.Value);
        restRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
        return await SendInternalAsync(restRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <exception cref="InvalidOperationException">Unsupported param type.</exception>
    public async Task<RestResponse> SendAsync(HttpMethod method, string endpoint,
        IReadOnlyDictionary<string, object> multipartParams,
        CancellationToken cancellationToken, string? reason = null,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>>? requestHeaders = null)
    {
        string uri = Path.Combine(_baseUrl, endpoint);

        // HttpRequestMessage implements IDisposable but we do not need to dispose it as it merely disposes of its Content property,
        // which we can do as needed. And regarding that, we do not want to take responsibility for disposing of content provided by
        // the caller of this function, since it's possible that the caller wants to reuse it or is forced to reuse it because of a
        // 429 response. Therefore, by convention, we only dispose the content objects created in this function (if any).
        //
        // See this comment explaining why this is safe: https://github.com/aspnet/Security/issues/886#issuecomment-229181249
        // See also the source for HttpRequestMessage: https://github.com/microsoft/referencesource/blob/master/System/net/System/Net/Http/HttpRequestMessage.cs
        var restRequest = new HttpRequestMessage(method, uri);

        if (reason != null)
            restRequest.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(reason));
        if (requestHeaders != null)
            foreach (var header in requestHeaders)
                restRequest.Headers.Add(header.Key, header.Value);
        var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));

        static StreamContent GetStreamContent(Stream stream)
        {
            if (stream.CanSeek)
            {
                // Reset back to the beginning; it may have been used elsewhere or in a previous request.
                stream.Position = 0;
            }

            return new StreamContent(stream);
        }

        foreach (KeyValuePair<string, object> p in multipartParams ?? ImmutableDictionary<string, object>.Empty)
        {
            switch (p.Value)
            {
                case string stringValue:
                    { content.Add(new StringContent(stringValue, Encoding.UTF8, "text/plain"), p.Key); continue; }
                case byte[] byteArrayValue:
                    { content.Add(new ByteArrayContent(byteArrayValue), p.Key); continue; }
                case Stream streamValue:
                    { content.Add(GetStreamContent(streamValue), p.Key); continue; }
                case MultipartFile fileValue:
                    {
                        StreamContent streamContent = GetStreamContent(fileValue.Stream);
                        if (fileValue.ContentType != null)
                            streamContent.Headers.ContentType = new MediaTypeHeaderValue(fileValue.ContentType);
                        if (fileValue.Filename is not null)
                            content.Add(streamContent, p.Key, fileValue.Filename);
                        else
                            content.Add(streamContent, p.Key);
                        continue;
                    }
                default:
                    throw new InvalidOperationException($"Unsupported param type \"{p.Value.GetType().Name}\".");
            }
        }

        restRequest.Content = content;
        return await SendInternalAsync(restRequest, cancellationToken).ConfigureAwait(false);
    }

    private async Task<RestResponse> SendInternalAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        int id = Interlocked.Increment(ref _nextId);
        using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken);
        KookDebugger.DebugRest($"[REST] [{id}] {request.Method} {request.RequestUri} {request.Content?.Headers.ContentType?.MediaType}");
        if (request.Content?.Headers.ContentType?.MediaType == "application/json")
            KookDebugger.DebugRest($"[REST] [{id}] {await request.Content.ReadAsStringAsync().ConfigureAwait(false)}");
        HttpResponseMessage response = await _client.SendAsync(request, cancellationTokenSource.Token).ConfigureAwait(false);

        Dictionary<string, string?> headers = [];
        foreach (KeyValuePair<string, IEnumerable<string>> kvp in response.Headers)
        {
            string? value = kvp.Value.FirstOrDefault();
            headers[kvp.Key] = value;
        }
        Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        if (KookDebugger.IsDebuggingRest)
        {
            KookDebugger.DebugRest($"[REST] [{id}] {response.StatusCode} {response.ReasonPhrase}");
            if (response.Content?.Headers.ContentType?.MediaType == "application/json")
            {
                string body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                KookDebugger.DebugRest($"[REST] [{id}] {body}");
            }
        }
        return new RestResponse(response.StatusCode, headers, stream, response.Content?.Headers.ContentType);
    }

    // private static readonly HttpMethod Patch = new HttpMethod("PATCH");
    // private HttpMethod GetMethod(string method)
    // {
    //     return method switch
    //     {
    //         "DELETE" => HttpMethod.Delete,
    //         "GET" => HttpMethod.Get,
    //         "PATCH" => Patch,
    //         "POST" => HttpMethod.Post,
    //         "PUT" => HttpMethod.Put,
    //         _ => throw new ArgumentOutOfRangeException(nameof(method), $"Unknown HttpMethod: {method}"),
    //     };
    // }
}
