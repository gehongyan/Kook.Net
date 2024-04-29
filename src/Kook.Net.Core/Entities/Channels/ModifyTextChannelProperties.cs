namespace Kook;

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
    public string? Topic { get; set; }

    /// <summary>
    ///     Gets or sets the slow-mode ratelimit in seconds for this channel.
    /// </summary>
    /// <remarks>
    ///     Setting this value will require each user to wait before sending another message; setting this value to
    ///     <see cref="Kook.SlowModeInterval.None"/> will disable slow-mode for this channel;
    ///     if this value is set to <c>null</c>, the slow-mode interval will not be modified.
    ///     <note>
    ///         Users with <see cref="Kook.ChannelPermission.ManageMessages"/> or
    ///         <see cref="ChannelPermission.ManageChannels"/> will be exempt from slow-mode.
    ///     </note>
    /// </remarks>
    public SlowModeInterval? SlowModeInterval { get; set; }
}
