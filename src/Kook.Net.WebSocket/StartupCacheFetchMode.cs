namespace Kook.WebSocket;

/// <summary>
///     Represents the mode in which the socket client fetches the cache when starting up.
/// </summary>
public enum StartupCacheFetchMode
{
    /// <summary>
    ///     Automatically selects the best mode based on the number of guilds.
    /// </summary>
    /// <remarks>
    ///     If the number of guilds is greater than or equal to the value of
    ///     <see cref="P:Kook.WebSocket.KookSocketConfig.LargeNumberOfGuildsThreshold"/>,
    ///     the mode will be <see cref="F:Kook.WebSocket.StartupCacheFetchMode.Lazy"/>.
    ///     Otherwise, the mode will be <see cref="F:Kook.WebSocket.StartupCacheFetchMode.Asynchronous"/>.
    /// </remarks>
    /// <seealso cref="P:Kook.WebSocket.KookSocketConfig.LargeNumberOfGuildsThreshold"/>.
    Auto,

    /// <summary>
    ///     Fetches the cache synchronously.
    /// </summary>
    /// <remarks>
    ///     When the socket client starts up, it will proactively fetch all the basic data from the KOOK API.
    ///     The <see cref="E:Kook.WebSocket.KookSocketClient.Ready"/> event will be triggered
    ///     after the data fetching is complete.
    /// </remarks>
    Synchronous,

    /// <summary>
    ///     Fetches the cache asynchronously.
    /// </summary>
    /// <remarks>
    ///     When the socket client starts up, it will trigger the <see cref="E:Kook.WebSocket.KookSocketClient.Ready"/>
    ///     event as soon as possible, then start a background task to fetch all the basic data from the KOOK API.
    ///     If an event related to a guild is received during this period and the basic data for that guild has not
    ///     yet been fetched, the event handler will fetch the basic data for that server before triggering
    ///     the event subscribed to by the user code.
    /// </remarks>
    Asynchronous,

    /// <summary>
    ///     Fetches the cache lazily.
    /// </summary>
    /// <remarks>
    ///     When the socket client starts up, it will trigger the <see cref="E:Kook.WebSocket.KookSocketClient.Ready"/>
    ///     event as soon as possible. When an event related to a guild is received and the basic data for that guild
    ///     has not yet been fetched, the event handler will fetch the basic data for that server before triggering
    ///     the event subscribed to by the user code.
    /// </remarks>
    Lazy
}
