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
    ///     Starts the connection between KaiHeiLa and the client..
    /// </summary>
    /// <remarks>
    ///     This method will initialize the connection between the client and KaiHeiLa.
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
    ///     Stops the connection between KaiHeiLa and the client.
    /// </summary>
    /// <returns>
    ///     A task that represents the asynchronous stop operation.
    /// </returns>
    Task StopAsync();
    
    /// <summary>
    ///     Gets a guild.
    /// </summary>
    /// <param name="id">The guild identifier.</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the guild associated
    ///     with the identifier; <c>null</c> when the guild cannot be found.
    /// </returns>
    Task<IGuild> GetGuildAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    
    /// <summary>
    ///     Gets a user.
    /// </summary>
    /// <param name="id">The identifier of the user (e.g. `168693960628371456`).</param>
    /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains the user associated with
    ///     the identifier; <c>null</c> if the user is not found.
    /// </returns>
    Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
}