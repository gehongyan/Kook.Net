namespace Kook.Rest;

/// <summary>
///     Provides extension methods of experimental functionalities for <see cref="RestVoiceChannel"/>s.
/// </summary>
public static class RestVoiceChannelExperimentalExtensions
{
    /// <summary>
    ///     Syncs the permissions of this nested channel with its parent's.
    /// </summary>
    /// <param name="channel">The nested channel whose permissions will be synced.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for syncing channel permissions with its parent's.
    /// </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         This method is still in experimental state, which means that it is not for official API implementation
    ///         usage, may violate the developer rules or policies, not guaranteed to be stable, and may be changed or removed in the future.
    ///     </note>
    /// </remarks>
    public static Task SyncPermissionsAsync(this RestVoiceChannel channel, RequestOptions? options = null) =>
        ExperimentalChannelHelper.SyncPermissionsAsync(channel, channel.Kook, options);

    /// <summary>
    ///     Disconnects the specified user from the voice channel.
    /// </summary>
    /// <param name="channel">The voice channel where the use is connected to.</param>
    /// <param name="user">The user to disconnect.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation for disconnecting the user from the voice channel.
    /// </returns>
    /// <remarks>
    ///     <note type="warning">
    ///         This method is still in experimental state, which means that it is not for official API implementation
    ///         usage, may violate the developer rules or policies, not guaranteed to be stable, and may be changed or removed in the future.
    ///     </note>
    /// </remarks>
    public static Task DisconnectUserAsync(this RestVoiceChannel channel, IGuildUser user, RequestOptions? options = null) =>
        ExperimentalChannelHelper.DisconnectUserAsync(channel, channel.Kook, user, options);
}
