namespace Kook;

/// <summary>
///     提供用于转换 <see cref="T:Kook.ITag"/> 为强类型实现类的工具方法。
/// </summary>
public static class TagUtils
{
    /// <summary>
    ///     将标签转换为用户标签。
    /// </summary>
    /// <param name="tag"> 要转换的标签。 </param>
    /// <returns> 转换后的用户标签。 </returns>
    /// <exception cref="InvalidCastException"> 标签不是用户提及标签。 </exception>
    public static Tag<ulong, IUser> AsUserTag(this ITag tag)
    {
        if (tag.Type is not TagType.UserMention)
            throw new InvalidCastException("The tag is not a user mention tag.");
        return (Tag<ulong, IUser>)tag;
    }

    /// <summary>
    ///     将标签转换为频道标签。
    /// </summary>
    /// <param name="tag"> 要转换的标签。 </param>
    /// <returns> 转换后的频道标签。 </returns>
    /// <exception cref="InvalidCastException"> 标签不是频道提及标签。 </exception>
    public static Tag<ulong, IChannel> AsChannelTag(this ITag tag)
    {
        if (tag.Type is not TagType.ChannelMention)
            throw new InvalidCastException("The tag is not a channel mention tag.");
        return (Tag<ulong, IChannel>)tag;
    }

    /// <summary>
    ///     将标签转换为角色标签。
    /// </summary>
    /// <param name="tag"> 要转换的标签。 </param>
    /// <returns> 转换后的角色标签。 </returns>
    /// <exception cref="InvalidCastException"> 标签不是角色提及标签。 </exception>
    public static Tag<uint, IRole> AsRoleTag(this ITag tag)
    {
        if (tag.Type is not (TagType.RoleMention or TagType.HereMention or TagType.EveryoneMention))
            throw new InvalidCastException("The tag is not a role mention tag.");
        return (Tag<uint, IRole>)tag;
    }

    /// <summary>
    ///     将标签转换为表情符号标签。
    /// </summary>
    /// <param name="tag"> 要转换的标签。 </param>
    /// <returns> 转换后的表情符号标签。 </returns>
    /// <exception cref="InvalidCastException"> 标签不是表情符号标签。 </exception>
    public static Tag<string, IEmote> AsEmojiTag(this ITag tag)
    {
        if (tag.Type is not TagType.Emoji)
            throw new InvalidCastException("The tag is not an emoji tag.");
        return (Tag<string, IEmote>)tag;
    }
}
