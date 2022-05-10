namespace KaiHeiLa;

/// <summary>
///     Provides properties that are used to modify an <see cref="ITextChannel"/> with the specified properties.
/// </summary>
/// <seealso cref="ITextChannel.ModifyAsync(System.Action{ModifyTextChannelProperties}, RequestOptions)"/>
public class ModifyTextChannelProperties : ModifyGuildChannelProperties
{
    /// <summary>
    ///     Gets or sets the topic of the channel.
    /// </summary>
    /// <remarks>
    ///     Setting this value to any string other than <c>null</c> or <see cref="string.Empty"/> will set the
    ///     channel topic or description to the desired value.
    /// </remarks>
    public string Topic { get; set; }
    /// <summary>
    ///     Gets or sets the slow-mode ratelimit in seconds for this channel.
    /// </summary>
    /// <remarks>
    ///     Setting this value to anything above zero will require each user to wait X seconds before
    ///     sending another message; setting this value to <c>0</c> will disable slow-mode for this channel.
    ///     <note>
    ///         Users with <see cref="KaiHeiLa.ChannelPermission.ManageMessages"/> or 
    ///         <see cref="ChannelPermission.ManageChannels"/> will be exempt from slow-mode.
    ///     </note>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value does not fall within [0, 21600].</exception>
    public int? SlowModeInterval { get; set; }
}