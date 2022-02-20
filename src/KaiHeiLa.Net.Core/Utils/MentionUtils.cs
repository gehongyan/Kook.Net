using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace KaiHeiLa;

public static class MentionUtils
{
    internal static readonly Regex PlainTextUserRegex = new Regex(@"@[^#]+?#(?<id>\d{1,20})",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex PlainTextRoleRegex = new Regex(@"@role:(?<id>\d{1,10}?);",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);
    
    internal static readonly Regex PlainTextChannelRegex = new Regex(@"#channel:(?<id>\d{1,20});",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex PlainTextTagRegex = new Regex(@"(@[^#]+?#\d{1,20})|(@role:\d{1,10};)|(#channel:\d{1,20}?;)|(\[:[^:]+?:\d{1,20}\/\w{1,20}?\])",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    
    internal static readonly Regex KMarkdownUserRegex = new Regex(@"(\(met\))(?<id>\d{1,20}?)\1",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex KMarkdownRoleRegex = new Regex(@"(\(rol\))(?<id>\d{1,10}?)\1",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);
    
    internal static readonly Regex KMarkdownChannelRegex = new Regex(@"(\(chn\))(?<id>\d{1,20}?)\1",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    internal static readonly Regex KMarkdownTagRegex = new Regex(@"(\((met|rol|chn)\)\d{1,20}?\(\2\))|(\(emj\)[^\(\)]{1,20}?(emj)\[\d{1,20}\/\w{1,20}\])",
        RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);


    private const char SanitizeChar = '\x200b';
    
    internal static string MentionUser(string id) => $"(met){id}(met)";
    public static string MentionUser(ulong id) => MentionUser(id.ToString());
    
    internal static string MentionChannel(string id) => $"(chn){id}(chn)";
    public static string MentionChannel(ulong id) => MentionChannel(id.ToString());
    
    internal static string MentionRole(string id) => $"(rol){id}(rol)";
    public static string MentionRole(ulong id) => MentionRole(id.ToString());


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
            TagMode.KMarkdown => KMarkdownUserRegex.Match(text)
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
            TagMode.KMarkdown => KMarkdownChannelRegex.Match(text)
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
            TagMode.KMarkdown => KMarkdownRoleRegex.Match(text)
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
        var text = new StringBuilder(msg.Content[startIndex..]);
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
                    if (userHandling == TagHandling.Ignore) continue;
                    newText = ResolveUserMention(tag, userHandling);
                    break;
                case TagType.ChannelMention:
                    if (channelHandling == TagHandling.Ignore) continue;
                    newText = ResolveChannelMention(tag, channelHandling);
                    break;
                case TagType.RoleMention:
                    if (roleHandling == TagHandling.Ignore) continue;
                    newText = ResolveRoleMention(tag, roleHandling);
                    break;
                case TagType.EveryoneMention:
                    if (everyoneHandling == TagHandling.Ignore) continue;
                    newText = ResolveEveryoneMention(tag, everyoneHandling);
                    break;
                case TagType.HereMention:
                    if (everyoneHandling == TagHandling.Ignore) continue;
                    newText = ResolveHereMention(tag, everyoneHandling);
                    break;
                case TagType.Emoji:
                    if (emojiHandling == TagHandling.Ignore) continue;
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
                    if (guildUser != null && guildUser.Nickname == null)
                        return MentionUser($"{SanitizeChar}{tag.Key}");
                    else
                        return MentionUser($"{SanitizeChar}{tag.Key}");
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
                    return MentionChannel($"{SanitizeChar}{tag.Key}");
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
                        return MentionRole($"{SanitizeChar}{tag.Key}");
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