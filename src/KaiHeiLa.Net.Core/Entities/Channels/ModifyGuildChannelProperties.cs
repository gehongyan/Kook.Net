namespace KaiHeiLa;

/// <summary>
///     Properties that are used to modify an <see cref="IGuildChannel" /> with the specified properties.
/// </summary>
public class ModifyGuildChannelProperties
{
    /// <summary>
    ///     Gets or sets the channel to this name to be modified.
    /// </summary>
    /// <remarks>
    ///     This property defines the new name for this channel.
    /// </remarks>
    public string Name { get; set; }
}