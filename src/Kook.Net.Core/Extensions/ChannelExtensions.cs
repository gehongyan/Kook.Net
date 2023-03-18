namespace Kook;

/// <summary>
///     Provides extension methods for <see cref="IChannel"/>.
/// </summary>
public static class ChannelExtensions
{
    /// <summary>
    ///     Attempts to get the <see cref="ChannelType"/> based off of the channel's interfaces.
    /// </summary>
    /// <param name="channel">The channel to get the type of.</param>
    /// <returns>The <see cref="ChannelType"/> of the channel if found, otherwise <see langword="null"/>.</returns>
    public static ChannelType? GetChannelType(this IChannel channel) =>
        channel switch
        {
            ICategoryChannel => ChannelType.Category,
            IDMChannel => ChannelType.DM,
            IVoiceChannel => ChannelType.Voice,
            ITextChannel => ChannelType.Text,
            _ => null
        };

    /// <summary>
    ///     Gets a URL that jumps to the channel.
    /// </summary>
    /// <param name="channel">The channel to jump to.</param>
    /// <returns>
    ///     A string that contains a URL for jumping to the message in chat.
    /// </returns>
    public static string GetJumpUrl(this IChannel channel) =>
        channel switch
        {
            IDMChannel dmChannel => $"https://www.kookapp.cn/app/home/privatemessage/{dmChannel.Recipient.Id}",
            IGuildChannel guildChannel => $"https://https://www.kookapp.cn/app/channels/{guildChannel.GuildId}/{channel.Id}",
            _ => throw new ArgumentException("Channel must be a guild or a DM channel.", nameof(channel))
        };
}
