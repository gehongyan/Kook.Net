namespace Kook;

/// <summary>
///     表示一个通用的限速信息。
/// </summary>
public interface IRateLimitInfo
{
    /// <summary>
    ///     获取此限速信息是否为全局限速。
    /// </summary>
    bool IsGlobal { get; }

    /// <summary>
    ///     获取在更新时限内可以进行的请求数量。
    /// </summary>
    int? Limit { get; }

    /// <summary>
    ///     获取目前可以立即进行的请求数量。
    /// </summary>
    int? Remaining { get; }

    // /// <summary>
    // ///     获取当前限速桶将在何时重置的总时间（以秒为单位）。
    // /// </summary>
    // int? RetryAfter { get; }

    // /// <summary>
    // ///     获取此限速重置的绝对时间。
    // /// </summary>
    // DateTimeOffset? Reset { get; }

    /// <summary>
    ///     获取相对于此刻此限速重置的相对时间间隔。
    /// </summary>
    TimeSpan? ResetAfter { get; }

    /// <summary>
    ///     获取一个唯一的字符串，表示所遇到的限速桶（不包括路由路径中的主要参数）。
    /// </summary>
    string? Bucket { get; }

    /// <summary>
    ///     获取请求的延迟，用于支持计算限速重置的精确时间。
    /// </summary>
    TimeSpan? Lag { get; }

    /// <summary>
    ///     获取此限速信息所属的终结点。
    /// </summary>
    string Endpoint { get; }
}
