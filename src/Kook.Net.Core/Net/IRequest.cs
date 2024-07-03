namespace Kook.Net;

/// <summary>
///     表示一个要发送到 KOOK 的通用请求。
/// </summary>
public interface IRequest
{
    /// <summary>
    ///     获取请求在超时之前应等待的时间。
    /// </summary>
    DateTimeOffset? TimeoutAt { get; }

    /// <summary>
    ///     获取发送请求时要使用的选项。
    /// </summary>
    RequestOptions Options { get; }
}
