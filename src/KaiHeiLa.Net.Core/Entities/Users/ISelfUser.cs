namespace KaiHeiLa;

public interface ISelfUser : IUser
{
    string MobilePrefix { get; }
    
    string Mobile { get; }
    
    int InvitedCount { get; }

    bool IsMobileVerified { get; }

    /// <summary>
    ///     Starts a new game activity. After this operation, a game activity will be displayed on the currently connected user's profile.
    /// </summary>
    /// <param name="game">The game to be played.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for starting a game activity.
    /// </returns>
    Task StartPlayingAsync(IGame game, RequestOptions options = null);

    /// <summary>
    ///     Stops a new game activity. After this operation, the game activity on the currently connected user's profile will disappear.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for stopping a game activity.
    /// </returns>
    Task StopPlayingAsync(RequestOptions options = null);
}