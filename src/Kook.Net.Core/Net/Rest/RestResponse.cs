using System.Net;
using System.Net.Http.Headers;

namespace Kook.Net.Rest;

/// <summary>
///     表示一个 RESTful API 请求的响应。
/// </summary>
public struct RestResponse
{
    /// <summary>
    ///     获取响应的 HTTP 状态码。
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    ///     获取响应的头部。
    /// </summary>
    public Dictionary<string, string?> Headers { get; }

    /// <summary>
    ///     获取响应的流。
    /// </summary>
    public Stream Stream { get; }

    /// <summary>
    ///     获取响应的媒体类型头部。
    /// </summary>
    public MediaTypeHeaderValue? MediaTypeHeader { get; }

    internal RestResponse(HttpStatusCode statusCode, Dictionary<string, string?> headers, Stream stream,
        MediaTypeHeaderValue? mediaTypeHeader)
    {
        StatusCode = statusCode;
        Headers = headers;
        Stream = stream;
        MediaTypeHeader = mediaTypeHeader;
    }
}
