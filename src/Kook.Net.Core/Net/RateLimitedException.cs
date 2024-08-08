namespace Kook.Net;

/// <summary>
///     表示一个由 KOOK 限制请求频率时引发的异常。
/// </summary>
public class RateLimitedException : TimeoutException
{
    /// <summary>
    ///     获取引发此异常的请求对象。
    /// </summary>
    public IRequest Request { get; }

    /// <summary>
    ///     使用发送的 <paramref name="request"/> 初始化 <see cref="RateLimitedException" /> 类的新实例。
    /// </summary>
    /// <param name="request"> 引发异常的请求。 </param>
    public RateLimitedException(IRequest request)
        : base("You are being rate limited.")
    {
        Request = request;
    }
}
