namespace Kook
{
    /// <summary>
    ///     Defines the types of clients a user can be active on.
    /// </summary>
    public enum ClientType
    {
        /// <summary>
        ///     The user is active using a WebSocket connection to the server.
        /// </summary>
        WebSocket,
        /// <summary>
        ///     The user is active using the Android application.
        /// </summary>
        Android,
        /// <summary>
        ///     The user is active using the iOS application.
        /// </summary>
        iOS
    }
}
