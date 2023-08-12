#if NET462
using System.Net.Http;
#endif

namespace Kook.Net.Rest;

/// <summary>
///     Represents a generic REST-based client.
/// </summary>
public interface IRestClient : IDisposable
{
    /// <summary>
    ///     Sets the HTTP header of this client for all requests.
    /// </summary>
    /// <param name="key">The field name of the header.</param>
    /// <param name="value">The value of the header.</param>
    void SetHeader(string key, string value);

    /// <summary>
    ///     Sets the cancellation token for this client.
    /// </summary>
    /// <param name="cancelToken">The cancellation token.</param>
    void SetCancelToken(CancellationToken cancelToken);

    /// <summary>
    ///     Sends a REST request.
    /// </summary>
    /// <param name="method">The method used to send this request (see <see cref="HttpMethod"/>).</param>
    /// <param name="endpoint">The endpoint to send this request to.</param>
    /// <param name="cancelToken">The cancellation token used to cancel the task.</param>
    /// <param name="reason">The audit log reason.</param>
    /// <param name="requestHeaders">Additional headers to be sent with the request.</param>
    /// <returns> A task that represents an asynchronous send operation. The task result contains the REST response of the request. </returns>
    Task<RestResponse> SendAsync(HttpMethod method, string endpoint, CancellationToken cancelToken, string reason = null,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders = null);

    /// <summary>
    ///     Sends a REST request with a JSON body.
    /// </summary>
    /// <param name="method">The method used to send this request (see <see cref="HttpMethod"/>).</param>
    /// <param name="endpoint">The endpoint to send this request to.</param>
    /// <param name="json">The JSON body of the request.</param>
    /// <param name="cancelToken">The cancellation token used to cancel the task.</param>
    /// <param name="reason">The audit log reason.</param>
    /// <param name="requestHeaders">Additional headers to be sent with the request.</param>
    /// <returns> A task that represents an asynchronous send operation. The task result contains the REST response of the request. </returns>
    Task<RestResponse> SendAsync(HttpMethod method, string endpoint, string json, CancellationToken cancelToken, string reason = null,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders = null);

    /// <summary>
    ///     Sends a REST request with multipart parameters.
    /// </summary>
    /// <param name="method">The method used to send this request (see <see cref="HttpMethod"/>).</param>
    /// <param name="endpoint">The endpoint to send this request to.</param>
    /// <param name="multipartParams">The multipart parameters.</param>
    /// <param name="cancelToken">The cancellation token used to cancel the task.</param>
    /// <param name="reason">The audit log reason.</param>
    /// <param name="requestHeaders">Additional headers to be sent with the request.</param>
    /// <returns> A task that represents an asynchronous send operation. The task result contains the REST response of the request. </returns>
    Task<RestResponse> SendAsync(HttpMethod method, string endpoint, IReadOnlyDictionary<string, object> multipartParams,
        CancellationToken cancelToken, string reason = null,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders = null);
}
