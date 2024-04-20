#if DEBUG_REST
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
#endif

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

namespace Kook.Net.Rest;

internal sealed class DefaultRestClient : IRestClient, IDisposable
{
    private const int HR_SECURECHANNELFAILED = -2146233079;

    private readonly HttpClient _client;
    private readonly string _baseUrl;
    private CancellationToken _cancellationToken;
    private bool _isDisposed;

#if DEBUG_REST
    private readonly JsonSerializerOptions _serializerOptions;
    private static int _nextId;
#endif

    public DefaultRestClient(string baseUrl, bool useProxy = false)
    {
        _baseUrl = baseUrl;

#pragma warning disable IDISP014
        _client = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate, UseCookies = false, UseProxy = useProxy
        });
#pragma warning restore IDISP014
        SetHeader("accept-encoding", "gzip, deflate");

        _cancellationToken = CancellationToken.None;

#if DEBUG_REST
        _serializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
#endif
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing) _client.Dispose();

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
            if (reason != null) restRequest.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(reason));

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
        if (reason != null) restRequest.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(reason));

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
#pragma warning disable IDISP004
        var restRequest = new HttpRequestMessage(method, uri);
#pragma warning restore IDISP004

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

#pragma warning disable IDISP004
            return new StreamContent(stream);
#pragma warning restore IDISP004
        }

        foreach (var p in multipartParams ?? ImmutableDictionary<string, object>.Empty)
        {
            switch (p.Value)
            {
#pragma warning disable IDISP004
                case string stringValue:
                    { content.Add(new StringContent(stringValue, Encoding.UTF8, "text/plain"), p.Key); continue; }
                case byte[] byteArrayValue:
                    { content.Add(new ByteArrayContent(byteArrayValue), p.Key); continue; }
                case Stream streamValue:
                    { content.Add(GetStreamContent(streamValue), p.Key); continue; }
                case MultipartFile fileValue:
                    {
                        var streamContent = GetStreamContent(fileValue.Stream);

                        if (fileValue.ContentType != null)
                            streamContent.Headers.ContentType = new MediaTypeHeaderValue(fileValue.ContentType);

                        content.Add(streamContent, p.Key, fileValue.Filename);
#pragma warning restore IDISP004

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
#if DEBUG_REST
        int id = Interlocked.Increment(ref _nextId);
#endif
        using CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken);
#if DEBUG_REST
        Debug.WriteLine($"[REST] [{id}] {request.Method} {request.RequestUri} {request.Content?.Headers.ContentType?.MediaType}");
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        if (request.Content?.Headers.ContentType?.MediaType == MediaTypeNames.Application.Json)
#else
        if (request.Content?.Headers.ContentType?.MediaType == "application/json")
#endif
            Debug.WriteLine($"[REST] {await request.Content.ReadAsStringAsync().ConfigureAwait(false)}");
#endif
        cancellationToken = cancellationTokenSource.Token;
        HttpResponseMessage response = await _client.SendAsync(request, cancellationToken).ConfigureAwait(false);

        Dictionary<string, string?> headers = [];
        foreach (KeyValuePair<string, IEnumerable<string>> kvp in response.Headers)
        {
            string? value = kvp.Value.FirstOrDefault();
            headers[kvp.Key] = value;
        }
        // ReSharper disable once MethodSupportsCancellation
        Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

#if DEBUG_REST
        Debug.WriteLine($"[REST] [{id}] {response.StatusCode} {response.ReasonPhrase}");
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        if (response.Content?.Headers.ContentType?.MediaType == MediaTypeNames.Application.Json)
#else
        if (response.Content?.Headers.ContentType?.MediaType == "application/json")
#endif
            Debug.WriteLine($"[REST] [{id}] {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
#endif
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
