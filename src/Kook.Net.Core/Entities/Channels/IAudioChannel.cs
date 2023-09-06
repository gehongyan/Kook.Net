namespace Kook;

/// <summary>
///     Represents a generic audio channel.
/// </summary>
public interface IAudioChannel : IChannel
{
    /// <summary>
    ///     Gets whether the voice region of this audio channel is overwritten.
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         This property may be <see langword="null" />
    ///         due to the lack of fields of KOOK API.
    ///     </note>
    /// </remarks>
    bool? IsVoiceRegionOverwritten { get; }

    /// <summary>
    ///     Gets the voice region for this audio channel.
    /// </summary>
    /// <remarks>
    ///     <note type="warning">
    ///         This property may be <see langword="null" />
    ///         due to the lack of fields of KOOK API.
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
