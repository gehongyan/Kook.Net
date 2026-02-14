namespace Kook.Net.Rest;

/// <summary>
///     表示一个通用的基于 RESTful API 的客户端。
/// </summary>
public interface IRestClient : IDisposable
{
    /// <summary>
    ///     设置此客户端的 HTTP 头部，这将应用于所有请求。
    /// </summary>
    /// <param name="key"> HTTP 头部的键。 </param>
    /// <param name="value"> HTTP 头部的值。 </param>
    void SetHeader(string key, string? value);

    /// <summary>
    ///     设置此客户端的取消令牌。
    /// </summary>
    /// <param name="cancellationToken"> 用于取消任务的取消令牌。 </param>
    void SetCancellationToken(CancellationToken cancellationToken);

    /// <summary>
    ///     发送一个 RESTful API 请求。
    /// </summary>
    /// <param name="method"> 用于发送此请求的方法。 </param>
    /// <param name="endpoint"> 要发送此请求的端点。 </param>
    /// <param name="cancellationToken"> 用于取消任务的取消令牌。 </param>
    /// <param name="reason"> 用于审计日志的操作原因。 </param>
    /// <param name="requestHeaders"> 要随请求一起发送的附加标头。 </param>
    /// <returns> 表示一个异步发送操作的任务。任务的结果包含请求的响应。 </returns>
    Task<RestResponse> SendAsync(HttpMethod method, string endpoint,
        CancellationToken cancellationToken, string? reason = null,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>>? requestHeaders = null);

    /// <summary>
    ///     发送一个带有 JSON 请求体的 RESTful API 请求。
    /// </summary>
    /// <param name="method"> 用于发送此请求的方法。 </param>
    /// <param name="endpoint"> 要发送此请求的端点。 </param>
    /// <param name="json"> 要发送的 JSON 请求体。 </param>
    /// <param name="cancellationToken"> 用于取消任务的取消令牌。 </param>
    /// <param name="reason"> 用于审计日志的操作原因。 </param>
    /// <param name="requestHeaders"> 要随请求一起发送的附加标头。 </param>
    /// <returns> 表示一个异步发送操作的任务。任务的结果包含请求的响应。 </returns>
    Task<RestResponse> SendAsync(HttpMethod method, string endpoint, string json,
        CancellationToken cancellationToken, string? reason = null,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>>? requestHeaders = null);

    /// <summary>
    ///     发送一个带有多部分数据参数的 RESTful API 请求。
    /// </summary>
    /// <param name="method"> 用于发送此请求的方法。 </param>
    /// <param name="endpoint"> 要发送此请求的端点。 </param>
    /// <param name="multipartParams"> 要发送的多部分数据参数。 </param>
    /// <param name="cancellationToken"> 用于取消任务的取消令牌。 </param>
    /// <param name="reason"> 用于审计日志的操作原因。 </param>
    /// <param name="requestHeaders"> 要随请求一起发送的附加标头。 </param>
    /// <returns> 表示一个异步发送操作的任务。任务的结果包含请求的响应。 </returns>
    Task<RestResponse> SendAsync(HttpMethod method, string endpoint, IReadOnlyDictionary<string, object> multipartParams,
        CancellationToken cancellationToken, string? reason = null,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>>? requestHeaders = null);
}
