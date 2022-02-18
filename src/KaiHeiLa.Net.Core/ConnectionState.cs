namespace KaiHeiLa
{
    /// <summary> Specifies the connection state of a client. </summary>
    public enum ConnectionState : byte
    {
        /// <summary> The client has disconnected from KaiHeiLa. </summary>
        Disconnected,
        /// <summary> The client is connecting to KaiHeiLa. </summary>
        Connecting,
        /// <summary> The client has established a connection to KaiHeiLa. </summary>
        Connected,
        /// <summary> The client is disconnecting from KaiHeiLa. </summary>
        Disconnecting
    }
}
