using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Kook;

/// <summary>
///     Provides a series of helper methods for parsing mentions.
/// </summary>
public static class MentionUtils
{
    internal static readonly Regex PlainTextUserRegex = new Regex(@"@[^#]+?#(?<id>\d{1,20})",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex PlainTextRoleRegex = new Regex(@"@role:(?<id>\d{1,10}?);",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex PlainTextChannelRegex = new Regex(@"#channel:(?<id>\d{1,20});",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex PlainTextTagRegex = new Regex(
        @"(@[^#]+?#\d{1,20})|(@role:\d{1,10};)|(#channel:\d{1,20}?;)|(\[:[^:]{1,32}?:\d{1,20}\/\w{1,20}?\])",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);


    internal static readonly Regex KMarkdownUserRegex = new Regex(@"(\(met\))(?<id>\d{1,20}?)\1",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex KMarkdownRoleRegex = new Regex(@"(\(rol\))(?<id>\d{1,10}?)\1",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex KMarkdownChannelRegex = new Regex(@"(\(chn\))(?<id>\d{1,20}?)\1",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex KMarkdownTagRegex = new Regex(
        @"(\((met|rol|chn)\)\d{1,20}?\(\2\))|(\(emj\)[^\(\)]{1,32}?\(emj\)\[\d{1,20}\/\w{1,20}\])",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);


    private const char SanitizeChar = '\x200b';

    internal static string KMarkdownMentionUser(string id) => $"(met){id}(met)";

    /// <summary>
    ///     Returns a KMarkdown formatted mention string based on the user ID.
    /// </summary>
    /// <returns>
    ///     A user mention string formatted to KMarkdown.
    /// </returns>
    public static string KMarkdownMentionUser(ulong id) => KMarkdownMentionUser(id.ToString());

    internal static string KMarkdownMentionChannel(string id) => $"(chn){id}(chn)";

    /// <summary>
    ///     Returns a KMarkdown formatted mention string based on the channel ID.
    /// </summary>
    /// <returns>
    ///     A channel mention string formatted to KMarkdown.
    /// </returns>
    public static string KMarkdownMentionChannel(ulong id) => KMarkdownMentionChannel(id.ToString());

    internal static string KMarkdownMentionRole(string id) => $"(rol){id}(rol)";

    /// <summary>
    ///     Returns a KMarkdown formatted mention string based on the role ID.
    /// </summary>
    /// <returns>
    ///     A role mention string formatted to KMarkdown.
    /// </returns>
    public static string KMarkdownMentionRole(uint id) => KMarkdownMentionRole(id.ToString());

    internal static string PlainTextMentionUser(string username, string id) => $"@{username}#{id}";

    /// <summary>
    ///     Returns a plain text formatted mention string based on the user ID.
    /// </summary>
    /// <returns>
    ///     A user mention string formatted to plain text.
    /// </returns>
    public static string PlainTextMentionUser(string username, ulong id) =>
        PlainTextMentionUser(username, id.ToString());

    internal static string PlainTextMentionChannel(string id) => $"#channel:{id};";

    /// <summary>
    ///     Returns a plain text formatted mention string based on the channel ID.
    /// </summary>
    /// <returns>
    ///     A channel mention string formatted to plain text.
    /// </returns>
    public static string PlainTextMentionChannel(ulong id) => PlainTextMentionChannel(id.ToString());

    internal static string PlainTextMentionRole(string id) => $"@role:{id};";

    /// <summary>
    ///     Returns a plain text formatted mention string based on the role ID.
    /// </summary>
    /// <returns>
    ///     A role mention string formatted to plain text.
    /// </returns>
    public static string PlainTextMentionRole(uint id) => PlainTextMentionRole(id.ToString());


    /// <summary>
    ///     Parses a provided user mention string.
    /// </summary>
    /// <param name="text">The user mention.</param>
    /// <param name="tagMode"></param>
    /// <exception cref="ArgumentException">Invalid mention format.</exception>
    public static ulong ParseUser(string text, TagMode tagMode)
    {
        if (TryParseUser(text, out ulong id, tagMode))
            return id;
        throw new ArgumentException(message: "Invalid mention format.", paramName: nameof(text));
    }

    /// <summary>
    ///     Tries to parse a provided user mention string.
    /// </summary>
    /// <param name="text">The user mention.</param>
    /// <param name="userId">The UserId of the user.</param>
    /// <param name="tagMode">Parse as PlainText or KMarkdown.</param>
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
    ///     Parses a provided channel mention string.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid mention format.</exception>
    public static ulong ParseChannel(string text, TagMode tagMode)
    {
        if (TryParseChannel(text, out ulong id, tagMode))
            return id;
        throw new ArgumentException(message: "Invalid mention format.", paramName: nameof(text));
    }

    /// <summary>
    ///     Tries to parse a provided channel mention string.
    /// </summary>
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
    ///     Parses a provided role mention string.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid mention format.</exception>
    public static ulong ParseRole(string text, TagMode tagMode)
    {
        if (TryParseRole(text, out uint id, tagMode))
            return id;
        throw new ArgumentException(message: "Invalid mention format.", paramName: nameof(text));
    }

    /// <summary>
    ///     Tries to parse a provided role mention string.
    /// </summary>
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
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        var text = new StringBuilder(msg.Content[startIndex..]);
#else 
        var text = new StringBuilder(msg.Content.Substring(startIndex));
#endif
        var tags = msg.Tags;
        int indexOffset = -startIndex;

        foreach (var tag in tags)
        {
            if (tag.Index < startIndex)
                continue;

            string newText = "";
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
            }

            text.Remove(tag.Index + indexOffset, tag.Length);
            text.Insert(tag.Index + indexOffset, newText);
            indexOffset += newText.Length - tag.Length;
        }

        return text.ToString();
    }

    internal static string ResolveUserMention(ITag tag, TagHandling mode)
    {
        if (mode != TagHandling.Remove)
        {
            var user = tag.Value as IUser;
            var guildUser = user as IGuildUser;
            switch (mode)
            {
                case TagHandling.Name:
                    if (user != null)
                        return $"@{guildUser?.Nickname ?? user?.Username}";
                    else
                        return "";
                case TagHandling.NameNoPrefix:
                    if (user != null)
                        return $"{guildUser?.Nickname ?? user?.Username}";
                    else
                        return "";
                case TagHandling.FullName:
                    if (user != null)
                        return $"@{user.Username}#{user.IdentifyNumber}";
                    else
                        return "";
                case TagHandling.FullNameNoPrefix:
                    if (user != null)
                        return $"{user.Username}#{user.IdentifyNumber}";
                    else
                        return "";
                case TagHandling.Sanitize:
                    return KMarkdownMentionUser($"{SanitizeChar}{tag.Key}");
            }
        }

        return "";
    }

    internal static string ResolveChannelMention(ITag tag, TagHandling mode)
    {
        if (mode != TagHandling.Remove)
        {
            var channel = tag.Value as IChannel;
            switch (mode)
            {
                case TagHandling.Name:
                case TagHandling.FullName:
                    if (channel != null)
                        return $"#{channel.Name}";
                    else
                        return "";
                case TagHandling.NameNoPrefix:
                case TagHandling.FullNameNoPrefix:
                    if (channel != null)
                        return $"{channel.Name}";
                    else
                        return "";
                case TagHandling.Sanitize:
                    return KMarkdownMentionChannel($"{SanitizeChar}{tag.Key}");
            }
        }

        return "";
    }

    internal static string ResolveRoleMention(ITag tag, TagHandling mode)
    {
        if (mode != TagHandling.Remove)
        {
            var role = tag.Value as IRole;
            switch (mode)
            {
                case TagHandling.Name:
                case TagHandling.FullName:
                    if (role != null)
                        return $"@{role.Name}";
                    else
                        return "";
                case TagHandling.NameNoPrefix:
                case TagHandling.FullNameNoPrefix:
                    if (role != null)
                        return $"{role.Name}";
                    else
                        return "";
                case TagHandling.Sanitize:
                    return KMarkdownMentionRole($"{SanitizeChar}{tag.Key}");
            }
        }

        return "";
    }

    internal static string ResolveEveryoneMention(ITag tag, TagHandling mode)
    {
        if (mode != TagHandling.Remove)
        {
            switch (mode)
            {
                case TagHandling.Name:
                case TagHandling.FullName:
                    return "@全体成员";
                case TagHandling.NameNoPrefix:
                case TagHandling.FullNameNoPrefix:
                    return "全体成员";
                case TagHandling.Sanitize:
                    return $"@{SanitizeChar}全体成员";
            }
        }

        return "";
    }

    internal static string ResolveHereMention(ITag tag, TagHandling mode)
    {
        if (mode != TagHandling.Remove)
        {
            switch (mode)
            {
                case TagHandling.Name:
                case TagHandling.FullName:
                    return "@在线成员";
                case TagHandling.NameNoPrefix:
                case TagHandling.FullNameNoPrefix:
                    return "在线成员";
                case TagHandling.Sanitize:
                    return $"@{SanitizeChar}在线成员";
            }
        }

        return "";
    }

    internal static string ResolveEmoji(ITag tag, TagHandling mode)
    {
        if (mode != TagHandling.Remove)
        {
            Emote emoji = (Emote)tag.Value;

            //Remove if its name contains any bad chars (prevents a few tag exploits)
            for (int i = 0; i < emoji.Name.Length; i++)
            {
                char c = emoji.Name[i];
                if (!char.IsLetterOrDigit(c) && c != '_' && c != '-')
                    return "";
            }

            switch (mode)
            {
                case TagHandling.Name:
                case TagHandling.FullName:
                    return $":{emoji.Name}:";
                case TagHandling.NameNoPrefix:
                case TagHandling.FullNameNoPrefix:
                    return $"{emoji.Name}";
                case TagHandling.Sanitize:
                    return $"[{SanitizeChar}{emoji.Name}{SanitizeChar}:{emoji.Id}]";
            }
        }

        return "";
    }
}
