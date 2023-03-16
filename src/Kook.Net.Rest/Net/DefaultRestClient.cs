#if DEBUG_REST
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
#endif
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
    private CancellationToken _cancelToken;
    private bool _isDisposed;

#if DEBUG_REST
    private readonly JsonSerializerOptions _serializerOptions;
    private int _requestId;
    private readonly SemaphoreSlim _requestIdLock = new SemaphoreSlim(1, 1);
#endif

    public DefaultRestClient(string baseUrl, bool useProxy = false)
    {
        _baseUrl = baseUrl;

#pragma warning disable IDISP014
        _client = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            UseCookies = false,
            UseProxy = useProxy,
        });
#pragma warning restore IDISP014
        SetHeader("accept-encoding", "gzip, deflate");

        _cancelToken = CancellationToken.None;

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
            if (disposing)
                _client.Dispose();
            _isDisposed = true;
        }
    }
    public void Dispose()
    {
        Dispose(true);
    }

    public void SetHeader(string key, string value)
    {
        _client.DefaultRequestHeaders.Remove(key);
        if (value != null)
            _client.DefaultRequestHeaders.Add(key, value);
    }
    public void SetCancelToken(CancellationToken cancelToken)
    {
        _cancelToken = cancelToken;
    }

    public async Task<RestResponse> SendAsync(HttpMethod method, string endpoint, CancellationToken cancelToken, string reason = null,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders = null)
    {
        string uri = Path.Combine(_baseUrl, endpoint);
        using (var restRequest = new HttpRequestMessage(method, uri))
        {
            if (reason != null)
                restRequest.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(reason));
            if (requestHeaders != null)
                foreach (KeyValuePair<string, IEnumerable<string>> header in requestHeaders)
                    restRequest.Headers.Add(header.Key, header.Value);
            return await SendInternalAsync(restRequest, cancelToken).ConfigureAwait(false);
        }
    }
    public async Task<RestResponse> SendAsync(HttpMethod method, string endpoint, string json, CancellationToken cancelToken, string reason = null,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders = null)
    {
        string uri = Path.Combine(_baseUrl, endpoint);
        using (var restRequest = new HttpRequestMessage(method, uri))
        {
            if (reason != null)
                restRequest.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(reason));
            if (requestHeaders != null)
                foreach (KeyValuePair<string, IEnumerable<string>> header in requestHeaders)
                    restRequest.Headers.Add(header.Key, header.Value);
            restRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return await SendInternalAsync(restRequest, cancelToken).ConfigureAwait(false);
        }
    }

    /// <exception cref="InvalidOperationException">Unsupported param type.</exception>
    public async Task<RestResponse> SendAsync(HttpMethod method, string endpoint, IReadOnlyDictionary<string, object> multipartParams, CancellationToken cancelToken, string reason = null,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders = null)
    {
        string uri = Path.Combine(_baseUrl, endpoint);
        using (var restRequest = new HttpRequestMessage(method, uri))
        {
            if (reason != null)
                restRequest.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(reason));
            if (requestHeaders != null)
                foreach (KeyValuePair<string, IEnumerable<string>> header in requestHeaders)
                    restRequest.Headers.Add(header.Key, header.Value);
            var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            MemoryStream memoryStream = null;
            if (multipartParams != null)
            {
                foreach (var p in multipartParams)
                {
                    switch (p.Value)
                    {
#pragma warning disable IDISP004
                        case string stringValue:
                            { content.Add(new StringContent(stringValue, Encoding.UTF8, "text/plain"), p.Key); continue; }
                        case byte[] byteArrayValue:
                            { content.Add(new ByteArrayContent(byteArrayValue), p.Key); continue; }
                        case Stream streamValue:
                            { content.Add(new StreamContent(streamValue), p.Key); continue; }
                        case MultipartFile fileValue:
                            {
                                var stream = fileValue.Stream;
                                if (!stream.CanSeek)
                                {
                                    memoryStream = new MemoryStream();
                                    await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
                                    memoryStream.Position = 0;
#pragma warning disable IDISP001
                                    stream = memoryStream;
#pragma warning restore IDISP001
                                }

                                var streamContent = new StreamContent(stream);
                                var extension = fileValue.Filename.Split('.').Last();

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
            }
            restRequest.Content = content;
            var result = await SendInternalAsync(restRequest, cancelToken).ConfigureAwait(false);
            memoryStream?.Dispose();
            return result;
        }
    }

    private async Task<RestResponse> SendInternalAsync(HttpRequestMessage request, CancellationToken cancelToken)
    {
#if DEBUG_REST
        await _requestIdLock.WaitAsync(1, cancelToken);
        _requestId++;
        int requestId = _requestId;
        _requestIdLock.Release();
#endif
        using (var cancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancelToken, cancelToken))
        {
#if DEBUG_REST
            Debug.WriteLine($"[REST] [{requestId}] {request.Method} {request.RequestUri} {request.Content?.Headers.ContentType?.MediaType}");
            if (request.Content?.Headers.ContentType?.MediaType == "application/json")
                Debug.WriteLine($"[REST] {await request.Content.ReadAsStringAsync().ConfigureAwait(false)}");
#endif
            cancelToken = cancelTokenSource.Token;
            HttpResponseMessage response = await _client.SendAsync(request, cancelToken).ConfigureAwait(false);

            var headers = response.Headers.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault(), StringComparer.OrdinalIgnoreCase);
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

#if DEBUG_REST
                Debug.WriteLine($"[REST] [{requestId}] {response.StatusCode} {response.ReasonPhrase}");
                if (response.Content?.Headers.ContentType?.MediaType == "application/json")
                    Debug.WriteLine($"[REST] [{requestId}] {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
#endif
            return new RestResponse(response.StatusCode, headers, stream, response.Content?.Headers.ContentType);
        }
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
