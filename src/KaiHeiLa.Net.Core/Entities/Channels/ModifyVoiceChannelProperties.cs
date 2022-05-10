namespace KaiHeiLa;

/// <summary>
///     Provides properties that are used to modify an <see cref="IVoiceChannel"/> with the specified properties.
/// </summary>
/// <seealso cref="IVoiceChannel.ModifyAsync(System.Action{ModifyVoiceChannelProperties}, RequestOptions)"/>
public class ModifyVoiceChannelProperties : ModifyGuildChannelProperties
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