using Kook.Net;

namespace Kook;

/// <summary>
///     表示发送请求时要使用的选项。
/// </summary>
public class RequestOptions
{
    /// <summary>
    ///     使用默认设置创建一个新的 <see cref="RequestOptions" /> 类的实例。
    /// </summary>
    public static RequestOptions Default => new();

    /// <summary>
    ///     获取或设置等待此请求完成的最大时间，以毫秒为单位。
    /// </summary>
    /// <remarks>
    ///     获取或设置等待此请求完成的最大时间，以毫秒为单位。如果为 <c>null</c>，则请求不会超时。
    ///     如果此请求的桶触发了速率限制并且在超时前不会恢复，此请求将立即失败。
    /// </remarks>
    public int? Timeout { get; set; }

    /// <summary>
    ///     获取或设置此请求的取消令牌。
    /// </summary>
    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

    /// <summary>
    ///     获取或设置请求失败时的重试行为；如果为 <c>null</c>，则使用配置的默认的重试行为。
    /// </summary>
    /// <seealso cref="P:Kook.KookConfig.DefaultRetryMode"/>
    public RetryMode? RetryMode { get; set; }

    /// <summary>
    ///     获取或设置要写入到服务器审计日志中的操作原因。
    /// </summary>
    /// <remarks>
    ///     默认的 API 客户端不支持设置此属性。
    /// </remarks>
    public string? AuditLogReason { get; set; }

    /// <summary>
    ///     获取或设置此请求触发速率限制时要执行的回调委托。
    /// </summary>
    /// <seealso cref="P:Kook.KookConfig.DefaultRatelimitCallback"/>
    public Func<IRateLimitInfo, Task>? RatelimitCallback { get; set; }

    internal bool IgnoreState { get; set; }
    internal BucketId? BucketId { get; set; }
    internal bool IsClientBucket { get; set; }
    internal bool IsGatewayBucket { get; set; }

    internal IDictionary<string, IEnumerable<string>>? RequestHeaders { get; }

    internal static RequestOptions CreateOrClone(RequestOptions? options) =>
        options == null
            ? new RequestOptions()
            : options.Clone();

    internal void ExecuteRatelimitCallback(IRateLimitInfo info)
    {
        if (RatelimitCallback != null)
            _ = Task.Run(async () => await RatelimitCallback(info).ConfigureAwait(false), CancellationToken.None);
    }

    /// <summary>
    ///     使用默认设置创建一个新的 <see cref="RequestOptions" /> 类的实例。
    /// </summary>
    /// <remarks>
    ///     默认的请求超时时间是 <see cref="F:Kook.KookConfig.DefaultRequestTimeout"/>。
    /// </remarks>
    public RequestOptions()
    {
        Timeout = KookConfig.DefaultRequestTimeout;
        RequestHeaders = new Dictionary<string, IEnumerable<string>>();
    }

    /// <inheritdoc cref="M:System.Object.MemberwiseClone" />
    public RequestOptions Clone() => (RequestOptions) MemberwiseClone();
}
