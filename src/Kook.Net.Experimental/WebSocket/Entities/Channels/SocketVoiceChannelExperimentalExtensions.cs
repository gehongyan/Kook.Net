using Kook.Rest;

namespace Kook.WebSocket;

/// <summary>
///     Provides extension methods of experimental functionalities for <see cref="SocketVoiceChannel"/>s.
/// </summary>
public static class SocketVoiceChannelExperimentalExtensions
{
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
    public static Task DisconnectUserAsync(this SocketVoiceChannel channel, IGuildUser user, RequestOptions? options = null)
        => ExperimentalChannelHelper.DisconnectUserAsync(channel, channel.Kook, user, options);
}
