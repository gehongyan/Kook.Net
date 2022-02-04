namespace KaiHeiLa.Net
{
    /// <summary>
    ///     The exception that is thrown when the WebSocket session is closed by Discord.
    /// </summary>
    public class WebSocketClosedException : Exception
    {
        /// <summary>
        ///     Gets the close code sent by KaiHeiLa.
        /// </summary>
        /// <returns>
        ///     A 
        ///     <see href="https://developer.kaiheila.cn/doc/websocket">close code</see>
        ///     from KaiHeiLa.
        /// </returns>
        public int CloseCode { get; }
        /// <summary>
        ///     Gets the reason of the interruption.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WebSocketClosedException" /> using a Discord close code
        ///     and an optional reason.
        /// </summary>
        public WebSocketClosedException(int closeCode, string reason = null)
            : base($"The server sent close {closeCode}{(reason != null ? $": \"{reason}\"" : "")}")
        {
            CloseCode = closeCode;
            Reason = reason;
        }
    }
}