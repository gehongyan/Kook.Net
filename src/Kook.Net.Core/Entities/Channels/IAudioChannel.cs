using Kook.Audio;

namespace Kook;

/// <summary>
///     Represents a generic audio channel.
/// </summary>
public interface IAudioChannel : IChannel
{
    /// <summary>
    ///     Gets whether the voice region of this audio channel is overwritten.
    /// </summary>
    bool? IsVoiceRegionOverwritten { get; }

    /// <summary>
    ///     Gets the voice region for this audio channel.
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         This property may be empty if the voice channel is created before this feature was released.
    ///     </note>
    /// </remarks>
    string? VoiceRegion { get; }

    /// <summary>
    ///     Gets the server url that clients should connect to to join this voice channel.
    /// </summary>
    /// <returns>
    ///     A string representing the url that clients should connect to to join this voice channel.
    /// </returns>
    string? ServerUrl { get; }

    // /// <param name="selfDeaf">Determines whether the client should deaf itself upon connection.</param>
    // /// <param name="selfMute">Determines whether the client should mute itself upon connection.</param>
    /// <summary>
    ///     Connects to this audio channel.
    /// </summary>
    /// <param name="external">Determines whether the audio client is an external one or not.</param>
    /// <param name="disconnect">Determines whether the client should send a disconnect call before connecting to a new voice channel.</param>
    /// <returns>
    ///     A task representing the asynchronous connection operation. The task result contains the
    ///     <see cref="IAudioClient"/> responsible for the connection.
    /// </returns>
    Task<IAudioClient?> ConnectAsync( /*bool selfDeaf = false, bool selfMute = false, */
        bool external = false, bool disconnect = true);

    /// <summary>
    ///     Disconnects from this audio channel.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous operation for disconnecting from the audio channel.
    /// </returns>
    Task DisconnectAsync();
}
