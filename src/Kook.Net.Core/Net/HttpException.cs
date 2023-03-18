using System.Collections.Immutable;
using System.Net;

namespace Kook.Net;

/// <summary>
///     The exception that is thrown if an error occurs while processing an Kook HTTP request.
/// </summary>
public class HttpException : Exception
{
    /// <summary>
    ///     Gets the HTTP status code returned by Kook.
    /// </summary>
    /// <returns>
    ///     An HTTP status code from Kook.
    /// </returns>
    public HttpStatusCode HttpCode { get; }

    /// <summary>
    ///     Gets the JSON error code returned by Kook.
    /// </summary>
    /// <returns>
    ///     A JSON error code from Kook, or <c>null</c> if none.
    /// </returns>
    public KookErrorCode? KookCode { get; }

    /// <summary>
    ///     Gets the reason of the exception.
    /// </summary>
    public string Reason { get; }

    /// <summary>
    ///     Gets the request object used to send the request.
    /// </summary>
    public IRequest Request { get; }

    /// <summary>
    ///     Gets a collection of json errors describing what went wrong with the request.
    /// </summary>
    public IReadOnlyCollection<KookJsonError> Errors { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpException" /> class.
    /// </summary>
    /// <param name="httpCode"> The HTTP status code returned. </param>
    /// <param name="request"> The request that was sent prior to the exception. </param>
    /// <param name="kookCode"> The Kook status code returned. </param>
    /// <param name="reason"> The reason behind the exception. </param>
    /// <param name="errors"> A collection of json errors describing what went wrong with the request. </param>
    public HttpException(HttpStatusCode httpCode, IRequest request, KookErrorCode? kookCode = null, string reason = null,
        KookJsonError[] errors = null)
        : base(CreateMessage(httpCode, (int?)kookCode, reason))
    {
        HttpCode = httpCode;
        Request = request;
        KookCode = kookCode;
        Reason = reason;
        Errors = errors?.ToImmutableArray() ?? ImmutableArray<KookJsonError>.Empty;
    }

    private static string CreateMessage(HttpStatusCode httpCode, int? kookCode = null, string reason = null)
    {
        string msg;
        if (kookCode != null && kookCode != 0)
        {
            if (reason != null)
                msg = $"The server responded with error {(int)kookCode}: {reason}";
            else
                msg = $"The server responded with error {(int)kookCode}: {httpCode}";
        }
        else
        {
            if (reason != null)
                msg = $"The server responded with error {(int)httpCode}: {reason}";
            else
                msg = $"The server responded with error {(int)httpCode}: {httpCode}";
        }

        return msg;
    }
}
