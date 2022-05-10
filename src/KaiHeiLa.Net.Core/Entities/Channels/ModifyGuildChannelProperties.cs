namespace KaiHeiLa;

/// <summary>
///     Properties that are used to modify an <see cref="IGuildChannel" /> with the specified properties.
/// </summary>
/// <seealso cref="IGuildChannel.ModifyAsync(System.Action{ModifyGuildChannelProperties}, RequestOptions)"/>
public class ModifyGuildChannelProperties
{
    /// <summary>
    ///     Gets or sets the channel to this name to be modified.
    /// </summary>
    /// <remarks>
    ///     This property defines the new name for this channel;
    ///     if this is <c>null</c>, the name will not be modified.
    /// </remarks>
    public string Name { get; set; }
    /// <summary>
    ///     Moves the channel to the following position. This property is one-based.
    /// </summary>
    /// <remarks>
    ///     If this is <c>null</c>, the position will not be modified.
    /// </remarks>
    public int? Position { get; set; }
    /// <summary>
    ///     Gets or sets the category ID for this channel.
    /// </summary>
    /// <remarks>
    ///     Setting this value to a category's identifier will change this channel's parent to the
    ///     specified channel; setting this value to <c>0</c> will detach this channel from its parent if one
    ///     is set; if this is <c>null</c>, the parent of this channel will not be modified.
    /// </remarks>
    public ulong? CategoryId { get; set; }
}