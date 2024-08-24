namespace Kook.WebSocket;

/// <summary>
///     当网关客户端被请求重新连接时引发的异常。
/// </summary>
public class GatewayReconnectException : Exception
{
    /// <summary>
    ///     初始化一个带有重新连接消息的 <see cref="GatewayReconnectException" /> 类的新实例。
    /// </summary>
    /// <param name="message"> 包含要求客户端重新连接原因的消息。 </param>
    public GatewayReconnectException(string message)
        : base(message)
    {
    }
}
