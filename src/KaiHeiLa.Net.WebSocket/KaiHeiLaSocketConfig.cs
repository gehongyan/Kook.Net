using KaiHeiLa.Rest;

namespace KaiHeiLa.WebSocket;

public class KaiHeiLaSocketConfig : KaiHeiLaRestConfig
{
    /// <summary>
    ///     超时毫秒
    /// </summary>
    public int ConnectionTimeout { get; set; } = 6000;
    
    /// <summary>
    ///     心跳间隔毫秒
    /// </summary>
    public const int HeartbeatIntervalMilliseconds = 30000;
    
    /// <summary>
    ///     处理程序超时毫秒
    /// </summary>
    public int? HandlerTimeout { get; set; } = 3000;
}