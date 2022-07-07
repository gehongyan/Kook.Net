namespace KaiHeiLa;

/// <summary>
///     Represents the logged-in KaiHeiLa user.
/// </summary>
public interface ISelfUser : IUser
{
    /// <summary>
    ///     Gets the mobile prefix of the logged-in user.
    /// </summary>
    string MobilePrefix { get; }
    /// <summary>
    ///     Gets the mobile number of the logged-in user.
    /// </summary>
    string Mobile { get; }
    /// <summary>
    ///     TODO: To be documented.
    /// </summary>
    int InvitedCount { get; }
    /// <summary>
    ///     Gets whether the mobile number of the logged-in user is verified.
    /// </summary>
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
    ///     Starts a new music activity. After this operation, a music activity will be displayed on the currently connected user's profile.
    /// </summary>
    /// <param name="music">The music being listened to be showed.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for starting a music activity.
    /// </returns>
    Task StartPlayingAsync(Music music, RequestOptions options = null);

    /// <summary>
    ///     Stops an activity. After this operation, the activity on the currently connected user's profile will disappear.
    /// </summary>
    /// <param name="type">The type of the activity to stop</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for stopping an activity.
    /// </returns>
    Task StopPlayingAsync(ActivityType type, RequestOptions options = null);
}