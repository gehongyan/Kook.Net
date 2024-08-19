using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Kook;

/// <summary>
///     提供一组用于生成与解析提及标签的辅助方法。
/// </summary>
public static class MentionUtils
{
    internal static readonly Regex PlainTextUserRegex = new(@"@[^#]+?#(?<id>\d{1,20})",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex PlainTextRoleRegex = new(@"@role:(?<id>\d{1,10}?);",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex PlainTextChannelRegex = new(@"#channel:(?<id>\d{1,20});",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex PlainTextTagRegex = new(
        @"(@[^#]+?#\d{1,20})|(@role:\d{1,10};)|(#channel:\d{1,20}?;)|(\[:[^:]{1,32}?:[\w\/]{1,40}?\])",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex KMarkdownUserRegex = new(@"(\(met\))(?<id>\d{1,20}?)\1",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex KMarkdownRoleRegex = new(@"(\(rol\))(?<id>\d{1,10}?)\1",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex KMarkdownChannelRegex = new(@"(\(chn\))(?<id>\d{1,20}?)\1",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex KMarkdownTagRegex = new(
        @"(\((met|rol|chn)\)\d{1,20}?\(\2\))|(\(emj\)[^\(\)]{1,32}?\(emj\)\[[\w\/]{1,40}?\])",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    private const char SanitizeChar = '\u200b';

    internal static string KMarkdownMentionUser(string id) => $"(met){id}(met)";

    /// <summary>
    ///     返回基于用户 ID 的 KMarkdown 格式化用户提及字符串。
    /// </summary>
    /// <returns> 格式化为 KMarkdown 的用户提及字符串。 </returns>
    public static string KMarkdownMentionUser(ulong id) => KMarkdownMentionUser(id.ToString());

    internal static string KMarkdownMentionChannel(string id) => $"(chn){id}(chn)";

    /// <summary>
    ///     返回基于频道 ID 的 KMarkdown 格式化频道提及字符串。
    /// </summary>
    /// <returns> 格式化为 KMarkdown 的频道提及字符串。 </returns>
    public static string KMarkdownMentionChannel(ulong id) => KMarkdownMentionChannel(id.ToString());

    internal static string KMarkdownMentionRole(string id) => $"(rol){id}(rol)";

    /// <summary>
    ///     返回基于角色 ID 的 KMarkdown 格式化角色提及字符串。
    /// </summary>
    /// <returns> 格式化为 KMarkdown 的角色提及字符串。 </returns>
    public static string KMarkdownMentionRole(uint id) => KMarkdownMentionRole(id.ToString());

    internal static string PlainTextMentionUser(string username, string id) => $"@{username}#{id}";

    /// <summary>
    ///     返回基于用户名称与用户 ID 的纯文本格式化用户提及字符串。
    /// </summary>
    /// <returns> 格式化为纯文本的用户提及字符串。 </returns>
    public static string PlainTextMentionUser(string username, ulong id) =>
        PlainTextMentionUser(username, id.ToString());

    internal static string PlainTextMentionChannel(string id) => $"#channel:{id};";

    /// <summary>
    ///     返回基于频道 ID 的纯文本格式化频道提及字符串。
    /// </summary>
    /// <returns> 格式化为纯文本的频道提及字符串。 </returns>
    public static string PlainTextMentionChannel(ulong id) => PlainTextMentionChannel(id.ToString());

    internal static string PlainTextMentionRole(string id) => $"@role:{id};";

    /// <summary>
    ///     返回基于角色 ID 的纯文本格式化角色提及字符串。
    /// </summary>
    /// <returns> 格式化为纯文本的角色提及字符串。 </returns>
    public static string PlainTextMentionRole(uint id) => PlainTextMentionRole(id.ToString());

    /// <summary>
    ///     将指定的用户提及字符串解析为用户 ID。
    /// </summary>
    /// <param name="text"> 要解析的用户提及字符串。 </param>
    /// <param name="tagMode"> 提及标签的语法模式。 </param>
    /// <returns> 解析的用户 ID。 </returns>
    /// <exception cref="ArgumentException"> 无效的用户提及字符串格式。 </exception>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="tagMode"/> 不是有效的标签语法模式。 </exception>
    public static ulong ParseUser(string text, TagMode tagMode)
    {
        if (TryParseUser(text, out ulong id, tagMode))
            return id;

        throw new ArgumentException("Invalid mention format.", nameof(text));
    }

    /// <summary>
    ///     尝试解析指定的用户提及字符串。
    /// </summary>
    /// <param name="text"> 要解析的用户提及字符串。 </param>
    /// <param name="userId"> 如果解析成功，则为用户 ID；否则为 <c>0</c>。 </param>
    /// <param name="tagMode"> 提及标签的语法模式。 </param>
    /// <returns> 如果解析成功，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="tagMode"/> 不是有效的标签语法模式。 </exception>
    public static bool TryParseUser(string text, out ulong userId, TagMode tagMode)
    {
        Match match = tagMode switch
        {
            TagMode.PlainText => PlainTextUserRegex.Match(text),
            TagMode.KMarkdown => KMarkdownUserRegex.Match(text),
            _ => throw new ArgumentOutOfRangeException(nameof(tagMode), tagMode, null)
        };

        if (match.Success
            && ulong.TryParse(match.Groups["id"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out userId))
            return true;

        userId = 0;
        return false;
    }

    /// <summary>
    ///     解析指定的频道提及字符串。
    /// </summary>
    /// <param name="text"> 要解析的频道提及字符串。 </param>
    /// <param name="tagMode"> 提及标签的语法模式。 </param>
    /// <returns> 解析的频道 ID。 </returns>
    /// <exception cref="ArgumentException"> 无效的频道提及字符串格式。 </exception>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="tagMode"/> 不是有效的标签语法模式。 </exception>
    public static ulong ParseChannel(string text, TagMode tagMode)
    {
        if (TryParseChannel(text, out ulong id, tagMode))
            return id;

        throw new ArgumentException("Invalid mention format.", nameof(text));
    }

    /// <summary>
    ///     尝试解析指定的频道提及字符串。
    /// </summary>
    /// <param name="text"> 要解析的频道提及字符串。 </param>
    /// <param name="channelId"> 如果解析成功，则为频道 ID；否则为 <c>0</c>。 </param>
    /// <param name="tagMode"> 提及标签的语法模式。 </param>
    /// <returns> 如果解析成功，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="tagMode"/> 不是有效的标签语法模式。 </exception>
    public static bool TryParseChannel(string text, out ulong channelId, TagMode tagMode)
    {
        Match match = tagMode switch
        {
            TagMode.PlainText => PlainTextChannelRegex.Match(text),
            TagMode.KMarkdown => KMarkdownChannelRegex.Match(text),
            _ => throw new ArgumentOutOfRangeException(nameof(tagMode), tagMode, null)
        };

        if (match.Success
            && ulong.TryParse(match.Groups["id"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out channelId))
            return true;

        channelId = 0;
        return false;
    }

    /// <summary>
    ///     解析指定的角色提及字符串。
    /// </summary>
    /// <param name="text"> 要解析的角色提及字符串。 </param>
    /// <param name="tagMode"> 提及标签的语法模式。 </param>
    /// <returns> 解析的角色 ID。 </returns>
    /// <exception cref="ArgumentException"> 无效的角色提及字符串格式。 </exception>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="tagMode"/> 不是有效的标签语法模式。 </exception>
    public static ulong ParseRole(string text, TagMode tagMode)
    {
        if (TryParseRole(text, out uint id, tagMode)) return id;

        throw new ArgumentException("Invalid mention format.", nameof(text));
    }

    /// <summary>
    ///     尝试解析指定的角色提及字符串。
    /// </summary>
    /// <param name="text">T 要解析的角色提及字符串。 </param>
    /// <param name="roleId"> 如果解析成功，则为角色 ID；否则为 <c>0</c>。 </param>
    /// <param name="tagMode"> 提及标签的语法模式。 </param>
    /// <returns> 如果解析成功，则为 <c>true</c>；否则为 <c>false</c>。 </returns>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="tagMode"/> 不是有效的标签语法模式。 </exception>
    public static bool TryParseRole(string text, out uint roleId, TagMode tagMode)
    {
        Match match = tagMode switch
        {
            TagMode.PlainText => PlainTextRoleRegex.Match(text),
            TagMode.KMarkdown => KMarkdownRoleRegex.Match(text),
            _ => throw new ArgumentOutOfRangeException(nameof(tagMode), tagMode, null)
        };

        if (match.Success
            && uint.TryParse(match.Groups["id"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out roleId))
            return true;

        roleId = 0;
        return false;
    }

    internal static string Resolve(IMessage msg, int startIndex, TagHandling userHandling, TagHandling channelHandling,
        TagHandling roleHandling, TagHandling everyoneHandling, TagHandling emojiHandling)
    {
        StringBuilder text = new(msg.Content[startIndex..]);
        IReadOnlyCollection<ITag> tags = msg.Tags;
        int indexOffset = -startIndex;

        foreach (ITag tag in tags)
        {
            if (tag.Index < startIndex) continue;

            string newText;
            switch (tag.Type)
            {
                case TagType.UserMention:
                    if (userHandling == TagHandling.Ignore)
                        continue;
                    newText = ResolveUserMention(tag, userHandling);
                    break;
                case TagType.ChannelMention:
                    if (channelHandling == TagHandling.Ignore)
                        continue;
                    newText = ResolveChannelMention(tag, channelHandling);
                    break;
                case TagType.RoleMention:
                    if (roleHandling == TagHandling.Ignore)
                        continue;
                    newText = ResolveRoleMention(tag, roleHandling);
                    break;
                case TagType.EveryoneMention:
                    if (everyoneHandling == TagHandling.Ignore)
                        continue;
                    newText = ResolveEveryoneMention(tag, everyoneHandling);
                    break;
                case TagType.HereMention:
                    if (everyoneHandling == TagHandling.Ignore)
                        continue;
                    newText = ResolveHereMention(tag, everyoneHandling);
                    break;
                case TagType.Emoji:
                    if (emojiHandling == TagHandling.Ignore)
                        continue;
                    newText = ResolveEmoji(tag, emojiHandling);
                    break;
                default:
                    newText = string.Empty;
                    break;
            }

            text.Remove(tag.Index + indexOffset, tag.Length);
            text.Insert(tag.Index + indexOffset, newText);
            indexOffset += newText.Length - tag.Length;
        }

        return text.ToString();
    }

    internal static string ResolveUserMention(ITag tag, TagHandling mode)
    {
        if (mode == TagHandling.Remove)
            return string.Empty;
        if (tag.Value is not IUser user)
            return string.Empty;

        IGuildUser? guildUser = user as IGuildUser;
        return mode switch
        {
            TagHandling.Name => $"@{guildUser?.Nickname ?? user.Username}",
            TagHandling.NameNoPrefix => $"{guildUser?.Nickname ?? user.Username}",
            TagHandling.FullName => $"@{user.Username}#{user.IdentifyNumber}",
            TagHandling.FullNameNoPrefix => $"{user.Username}#{user.IdentifyNumber}",
            TagHandling.Sanitize => KMarkdownMentionUser($"{SanitizeChar}{tag.Key}"),
            _ => string.Empty
        };
    }

    internal static string ResolveChannelMention(ITag tag, TagHandling mode)
    {
        if (mode == TagHandling.Remove)
            return string.Empty;
        if (tag.Value is not IChannel channel)
            return string.Empty;
        return mode switch
        {
            TagHandling.Name or TagHandling.FullName => $"#{channel.Name}",
            TagHandling.NameNoPrefix or TagHandling.FullNameNoPrefix => $"{channel.Name}",
            TagHandling.Sanitize => KMarkdownMentionChannel($"{SanitizeChar}{tag.Key}"),
            _ => string.Empty
        };
    }

    internal static string ResolveRoleMention(ITag tag, TagHandling mode)
    {
        if (mode == TagHandling.Remove)
            return string.Empty;
        if (tag.Value is not IRole role)
            return string.Empty;
        return mode switch
        {
            TagHandling.Name or TagHandling.FullName => $"@{role.Name}",
            TagHandling.NameNoPrefix or TagHandling.FullNameNoPrefix => $"{role.Name}",
            TagHandling.Sanitize => KMarkdownMentionRole($"{SanitizeChar}{tag.Key}"),
            _ => string.Empty
        };
    }

    internal static string ResolveEveryoneMention(ITag tag, TagHandling mode)
    {
        if (mode == TagHandling.Remove)
            return string.Empty;
        return mode switch
        {
            TagHandling.Name or TagHandling.FullName => "@全体成员",
            TagHandling.NameNoPrefix or TagHandling.FullNameNoPrefix => "全体成员",
            TagHandling.Sanitize => $"@{SanitizeChar}all",
            _ => string.Empty
        };
    }

    internal static string ResolveHereMention(ITag tag, TagHandling mode)
    {
        if (mode == TagHandling.Remove)
            return string.Empty;
        return mode switch
        {
            TagHandling.Name or TagHandling.FullName => "@在线成员",
            TagHandling.NameNoPrefix or TagHandling.FullNameNoPrefix => "在线成员",
            TagHandling.Sanitize => $"@{SanitizeChar}here",
            _ => ""
        };
    }

    internal static string ResolveEmoji(ITag tag, TagHandling mode)
    {
        if (mode == TagHandling.Remove)
            return string.Empty;
        if (tag.Value is not Emote emoji)
            return string.Empty;

        //Remove if its name contains any bad chars (prevents a few tag exploits)
        if (emoji.Name.Any(c => !char.IsLetterOrDigit(c) && c != '_' && c != '-'))
            return string.Empty;

        return mode switch
        {
            TagHandling.Name or TagHandling.FullName => $":{emoji.Name}:",
            TagHandling.NameNoPrefix or TagHandling.FullNameNoPrefix => $"{emoji.Name}",
            TagHandling.Sanitize => $"(emj){SanitizeChar}{emoji.Name}(emj)[{SanitizeChar}{emoji.Id}]",
            _ => ""
        };
    }
}
