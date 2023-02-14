namespace Kook;

/// <summary> Specifies the connection state of a client. </summary>
public enum ConnectionState : byte
{
    /// <summary> The client has disconnected from Kook. </summary>
    Disconnected,
    /// <summary> The client is connecting to Kook. </summary>
    Connecting,
    /// <summary> The client has established a connection to Kook. </summary>
    Connected,
    /// <summary> The client is disconnecting from Kook. </summary>
    Disconnecting
}
