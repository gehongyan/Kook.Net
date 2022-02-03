using System.Collections.Immutable;
using System.Net;

namespace KaiHeiLa.Net
{
    /// <summary>
    ///     The exception that is thrown if an error occurs while processing an KaiHeiLa HTTP request.
    /// </summary>
    public class HttpException : Exception
    {
        /// <summary>
        ///     Gets the HTTP status code returned by KaiHeiLa.
        /// </summary>
        /// <returns>
        ///     An HTTP status code from KaiHeiLa.
        /// </returns>
        public HttpStatusCode HttpCode { get; }
        /// <summary>
        ///     Gets the JSON error code returned by KaiHeiLa.
        /// </summary>
        /// <returns>
        ///     A JSON error code from KaiHeiLa, or <c>null</c> if none.
        /// </returns>
        public KaiHeiLaErrorCode? KaiHeiLaCode { get; }
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
        public IReadOnlyCollection<KaiHeiLaJsonError> Errors { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpException" /> class.
        /// </summary>
        /// <param name="httpCode">The HTTP status code returned.</param>
        /// <param name="request">The request that was sent prior to the exception.</param>
        /// <param name="kaiHeiLaCode">The KaiHeiLa status code returned.</param>
        /// <param name="reason">The reason behind the exception.</param>
        public HttpException(HttpStatusCode httpCode, IRequest request, KaiHeiLaErrorCode? kaiHeiLaCode = null, string reason = null, KaiHeiLaJsonError[] errors = null)
            : base(CreateMessage(httpCode, (int?)kaiHeiLaCode, reason))
        {
            HttpCode = httpCode;
            Request = request;
            KaiHeiLaCode = kaiHeiLaCode;
            Reason = reason;
            Errors = errors?.ToImmutableArray() ?? ImmutableArray<KaiHeiLaJsonError>.Empty;
        }

        private static string CreateMessage(HttpStatusCode httpCode, int? kaiHeiLaCode = null, string reason = null)
        {   
            string msg;
            if (kaiHeiLaCode != null && kaiHeiLaCode != 0)
            {
                if (reason != null)
                    msg = $"The server responded with error {(int)kaiHeiLaCode}: {reason}";
                else
                    msg = $"The server responded with error {(int)kaiHeiLaCode}: {httpCode}";
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
}
