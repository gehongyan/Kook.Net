namespace KaiHeiLa;

public interface IKaiHeiLaClient : IDisposable
{
    /// <summary>
    ///     Gets the current state of connection.
    /// </summary>
    ConnectionState ConnectionState { get; }
    /// <summary>
    ///     Gets the currently logged-in user.
    /// </summary>
    ISelfUser CurrentUser { get; }
    /// <summary>
    ///     Gets the token type of the logged-in user.
    /// </summary>
    TokenType TokenType { get; }

    /// <summary>
    ///     Starts the connection between Discord and the client..
    /// </summary>
    /// <remarks>
    ///     This method will initialize the connection between the client and Discord.
    ///     <note type="important">
    ///         This method will immediately return after it is called, as it will initialize the connection on
    ///         another thread.
    ///     </note>
    /// </remarks>
    /// <returns>
    ///     A task that represents the asynchronous start operation.
    /// </returns>
    Task StartAsync();
    /// <summary>
    ///     Stops the connection between Discord and the client.
    /// </summary>
    /// <returns>
    ///     A task that represents the asynchronous stop operation.
    /// </returns>
    Task StopAsync();
}