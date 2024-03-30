namespace Kook;

/// <summary>
///     Provides properties that are used to create an <see cref="IVoiceChannel"/> with the specified properties.
/// </summary>
/// <seealso cref="IGuild.CreateVoiceChannelAsync(string, System.Action{CreateVoiceChannelProperties}, RequestOptions)"/>
public class CreateVoiceChannelProperties : CreateTextChannelProperties
{
    /// <summary>
    ///     Gets or sets the voice quality that the clients in this voice channel are requested to use.
    /// </summary>
    public VoiceQuality? VoiceQuality { get; set; }

    /// <summary>
    ///     Gets or sets the maximum number of users that can be present in a channel, or <c>null</c> if none.
    /// </summary>
    public int? UserLimit { get; set; }
}
