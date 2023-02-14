namespace Kook;

/// <summary>
///     Provides properties that are used to modify an <see cref="IGuild" /> with the specified changes.
/// </summary>
/// <remarks>
///     <note type="warning">
///         This entity is still in experimental state, which means that it is not for official API implementation
///         usage, may violate the developer rules or policies, not guaranteed to be stable, and may be changed or removed in the future.
///     </note>
/// </remarks>
public class GuildProperties
{
    /// <summary>
    ///     Gets or sets the identifier of the guild to modify.
    /// </summary>
    public ulong GuildId { get; set; }
    /// <summary>
    ///     Gets or sets the region for the guild's voice connections.
    /// </summary>
    public IVoiceRegion Region { get; set; }
    /// <summary>
    ///     Gets or sets the ID of the region for the guild's voice connections.
    /// </summary>
    public string RegionId { get; set; }
    /// <summary>
    ///     Gets or sets the ID of the default channel.
    /// </summary>
    /// <returns>
    ///     An <see langword="ulong"/> representing the identifier of the default channel.
    ///     <c>null</c> if nothing changes. <c>0</c> if set to none.
    /// </returns>
    public ulong? DefaultChannelId { get; set; }
    /// <summary>
    ///     Gets or sets the default channel.
    /// </summary>
    /// <returns>
    ///     An <see cref="ITextChannel"/> which is the default channel; <c>null</c> if nothing changes.
    ///     To clear the manually assigned default channel, set <see cref="DefaultChannelId"/> to <c>0</c> instead.
    /// </returns>
    public ITextChannel DefaultChannel { get; set; }
    /// <summary>
    ///     Gets or sets the ID of welcome channel.
    /// </summary>
    /// <returns>
    ///     An <see langword="ulong"/> representing the identifier of the channel where welcome messages are sent;
    ///     <c>null</c> if nothing changes. <c>0</c> if set to none.
    /// </returns>
    public ulong? WelcomeChannelId { get; set; }
    /// <summary>
    ///     Gets or sets the welcome channel.
    /// </summary>
    /// <returns>
    ///     An <see cref="ITextChannel"/> where welcome messages are sent; <c>null</c> if nothing changes.
    ///     To clear the welcome channel, set <see cref="WelcomeChannelId"/> to <c>0</c> instead.
    /// </returns>
    public ITextChannel WelcomeChannel { get; set; }
    /// <summary>
    ///     Gets or sets whether the guild is open.
    /// </summary>
    public bool? EnableOpen { get; set; }
    /// <summary>
    ///     Gets the ID of the channel assigned to the widget of this guild.
    /// </summary>
    /// <returns>
    ///     A <see langword="ulong"/> representing the identifier of the channel assigned to the widget found
    ///     within the widget settings of this guild; <see langword="null" /> if nothing changes; <c>0</c> if
    ///     set to none.
    /// </returns>
    public ulong? WidgetChannelId { get; set; }
    /// <summary>
    ///     Gets the channel assigned to the widget of this guild.
    /// </summary>
    /// <returns>
    ///     An <see cref="ITextChannel"/> assigned to the widget found within the widget settings of this guild;
    ///     <see langword="null" /> if nothing changes; To clear the widget channel,
    ///     set <see cref="WidgetChannelId"/> to <c>0</c> instead.
    /// </returns>
    public ITextChannel WidgetChannel { get; set; }
}
