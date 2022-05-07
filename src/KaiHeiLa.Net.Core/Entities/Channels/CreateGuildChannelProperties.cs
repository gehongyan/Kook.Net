namespace KaiHeiLa;

/// <summary>
///     Properties that are used to create an <see cref="IGuildChannel" /> with the specified properties.
/// </summary>
public class CreateGuildChannelProperties
{
    /// <summary>
    ///     Gets or sets the category ID for this channel.
    /// </summary>
    /// <remarks>
    ///     Setting this value to a category's snowflake identifier will change or set this channel's parent to the
    ///     specified channel; setting this value to <see langword="null"/> will detach this channel from its parent if one
    ///     is set.
    /// </remarks>
    public ulong? CategoryId { get; set; }
}