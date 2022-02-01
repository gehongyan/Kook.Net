using KaiHeiLa.Net.WebSockets;

namespace KaiHeiLa.WebSocket;

public class KaiHeiLaSocketConfig : KaiHeiLaConfig
{
    /// <summary>
    ///     超时毫秒
    /// </summary>
    public int ConnectionTimeout { get; set; } = 6000;
    
    /// <summary>
    ///     心跳间隔毫秒
    /// </summary>
    public const int HeartbeatIntervalMilliseconds = 30000;
}