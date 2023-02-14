using System.Net;
using System.Net.Http.Headers;

namespace Kook.Net.Rest;

public struct RestResponse
{
    public HttpStatusCode StatusCode { get; }
    public Dictionary<string, string> Headers { get; }
    public Stream Stream { get; }

    public MediaTypeHeaderValue MediaTypeHeader { get; }

    public RestResponse(HttpStatusCode statusCode, Dictionary<string, string> headers, Stream stream, MediaTypeHeaderValue mediaTypeHeader)
    {
        StatusCode = statusCode;
        Headers = headers;
        Stream = stream;
        MediaTypeHeader = mediaTypeHeader;
    }
}
