using System.Net;
using System.Net.Http.Headers;

namespace Kook.Net.Rest;

/// <summary>
///     Represents a REST response.
/// </summary>
public struct RestResponse
{
    /// <summary>
    ///     Gets the status code of the response.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    ///     Gets the headers of the response.
    /// </summary>
    public Dictionary<string, string> Headers { get; }

    /// <summary>
    ///     Gets the stream of the response.
    /// </summary>
    public Stream Stream { get; }

    /// <summary>
    ///     Gets the media type header of the response.
    /// </summary>
    public MediaTypeHeaderValue? MediaTypeHeader { get; }

    internal RestResponse(HttpStatusCode statusCode, Dictionary<string, string> headers, Stream stream,
        MediaTypeHeaderValue? mediaTypeHeader)
    {
        StatusCode = statusCode;
        Headers = headers;
        Stream = stream;
        MediaTypeHeader = mediaTypeHeader;
    }
}
