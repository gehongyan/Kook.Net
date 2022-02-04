using KaiHeiLa.Net.WebSockets;
using KaiHeiLa.Rest;

namespace KaiHeiLa.WebSocket;

public class KaiHeiLaSocketConfig : KaiHeiLaRestConfig
{
    /// <summary>
    ///    Returns the encoding gateway should use.
    /// </summary>
    public const string GatewayEncoding = "json";

    /// <summary>
    ///     Gets or sets the WebSocket host to connect to. If <c>null</c>, the client will use the
    ///     /gateway endpoint.
    /// </summary>
    public string GatewayHost { get; set; } = null;
    
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
    
    public int LargeThreshold { get; set; } = 250;

    /// <summary>
    ///     Gets or sets the provider used to generate new WebSocket connections.
    /// </summary>
    public WebSocketProvider WebSocketProvider { get; set; }
    
    
    /// <summary>
    ///     Gets or sets the maximum wait time in milliseconds between GUILD_AVAILABLE events before firing READY.
    ///     If zero, READY will fire as soon as it is received and all guilds will be unavailable.
    /// </summary>
    /// <remarks>
    ///     <para>This property is measured in milliseconds; negative values will throw an exception.</para>
    ///     <para>If a guild is not received before READY, it will be unavailable.</para>
    /// </remarks>
    /// <returns>
    ///     A <see cref="int"/> representing the maximum wait time in milliseconds between GUILD_AVAILABLE events
    ///     before firing READY.
    /// </returns>
    /// <exception cref="System.ArgumentException">Value must be at least 0.</exception>
    public int MaxWaitBetweenGuildAvailablesBeforeReady
    {
        get
        {
            return maxWaitForGuildAvailable;
        }

        set
        {
            Preconditions.AtLeast(value, 0, nameof(MaxWaitBetweenGuildAvailablesBeforeReady));
            maxWaitForGuildAvailable = value;
        }
    }

    private int maxWaitForGuildAvailable = 10000;
    
    public KaiHeiLaSocketConfig()
    {
        WebSocketProvider = DefaultWebSocketProvider.Instance;
    }
    
    internal KaiHeiLaSocketConfig Clone() => MemberwiseClone() as KaiHeiLaSocketConfig;
}