namespace Kook;

/// <summary>
///     表示请求在发生错误时应如何处理。
/// </summary>
[Flags]
public enum RetryMode
{
    /// <summary>
    ///     如果请求失败，将立即引发异常。
    /// </summary>
    AlwaysFail = 0x0,

    /// <summary>
    ///     如果请求超时，则重试。
    /// </summary>
    RetryTimeouts = 0x1,

    // /// <summary>
    // ///     如果请求因网络错误而失败，则重试。
    // /// </summary>
    // RetryErrors = 0x2,

    /// <summary>
    ///     如果请求因速率限制而失败，则重试。
    /// </summary>
    RetryRatelimit = 0x4,

    /// <summary>
    ///     如果请求因 HTTP 状态码 502 而失败，则重试。
    /// </summary>
    /// <seealso cref="F:System.Net.HttpStatusCode.BadGateway"/>
    Retry502 = 0x8,

    /// <summary>
    ///     总是重试失败的请求，直到超时、取消令牌被触发或服务器响应非 502 错误。
    /// </summary>
    AlwaysRetry = RetryTimeouts | /*RetryErrors |*/ RetryRatelimit | Retry502
}
