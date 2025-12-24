namespace Kook;

/// <summary>
///     提供用于各种频道实体的扩展方法。
/// </summary>
public static class ChannelExtensions
{
    /// <summary>
    ///     尝试基于频道所实现的接口类型获取频道的实际类型。
    /// </summary>
    /// <param name="channel"> 要获取其类型的频道。 </param>
    /// <returns> 如果此频道的实际类型已知，则返回其类型；否则，返回 <c>null</c>。 </returns>
    public static ChannelType? GetChannelType(this IChannel channel) =>
        channel switch
        {
            ICategoryChannel => ChannelType.Category,
            IDMChannel => ChannelType.DM,
            IVoiceChannel => ChannelType.Voice,
            ITextChannel => ChannelType.Text,
            IThreadChannel => ChannelType.Thread,
            _ => null
        };

    /// <summary>
    ///     获取一个跳转到频道的 URL。
    /// </summary>
    /// <param name="channel"> 要获取跳转 URL 的频道。 </param>
    /// <returns> 一个包含用于在聊天中跳转到频道的 URL 的字符串。 </returns>
    public static string GetJumpUrl(this IChannel channel) =>
        channel switch
        {
            IDMChannel dmChannel => $"https://www.kookapp.cn/app/home/privatemessage/{dmChannel.Recipient.Id}",
            IGuildChannel guildChannel => DirectLinks.Channel(guildChannel.Guild.Id, guildChannel.Id),
            _ => throw new ArgumentException("Channel must be a guild or a DM channel.", nameof(channel))
        };
}
