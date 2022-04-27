namespace KaiHeiLa;

/// <summary>
///     Specifies the type of a guild channel.
/// </summary>
public enum ChannelType
{
    /// <summary>
    ///     Specifies that the type of the channel is unknown.
    /// </summary>
    Unspecified = -1,
    
    /// <summary>
    ///     Specifies that the channel is a guild category channel.
    /// </summary>
    Category = 0,
    
    /// <summary>
    ///     Specifies that the channel is a guild text channel.
    /// </summary>
    Text = 1,
    
    /// <summary>
    ///     Specifies that the channel is a guild voice channel.
    /// </summary>
    Voice = 2
}