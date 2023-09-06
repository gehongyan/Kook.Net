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
    string VoiceRegion { get; }

    /// <summary>
    ///     Gets the server url that clients should connect to to join this voice channel.
    /// </summary>
    /// <returns>
    ///     A string representing the url that clients should connect to to join this voice channel.
    /// </returns>
    string ServerUrl { get; }
}
