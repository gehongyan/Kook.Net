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
    ///     Setting this value to a category's identifier will set this channel's parent to the
    ///     specified channel; setting this value to <see langword="null"/> will leave this channel alone
    ///     from any parents.
    /// </remarks>
    public ulong? CategoryId { get; set; }
}