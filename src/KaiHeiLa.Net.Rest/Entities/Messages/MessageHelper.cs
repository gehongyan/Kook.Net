using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace KaiHeiLa.Rest;
using Model = KaiHeiLa.API.Message;
using UserModel = KaiHeiLa.API.User;

internal static class MessageHelper
{
    /// <summary>
    /// Regex used to check if some text is formatted as inline code.
    /// </summary>
    private static readonly Regex InlineCodeRegex = new Regex(@"[^\\]?(`).+?[^\\](`)", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    /// <summary>
    /// Regex used to check if some text is formatted as a code block.
    /// </summary>
    private static readonly Regex BlockCodeRegex = new Regex(@"[^\\]?(```).+?[^\\](```)", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);
    
    private static Regex PlainTextUserRegex => MentionUtils.PlainTextUserRegex;
    private static Regex PlainTextRoleRegex => MentionUtils.PlainTextRoleRegex;
    private static Regex PlainTextChannelRegex => MentionUtils.PlainTextChannelRegex;
    private static Regex PlainTextEmojiRegex => Emote.PlainTextEmojiRegex;
    private static Regex PlainTextTagRegex => MentionUtils.PlainTextTagRegex;
    private static Regex KMarkdownUserRegex => MentionUtils.KMarkdownUserRegex;
    private static Regex KMarkdownRoleRegex => MentionUtils.KMarkdownRoleRegex;
    private static Regex KMarkdownChannelRegex => MentionUtils.KMarkdownChannelRegex;
    private static Regex KMarkdownEmojiRegex => Emote.KMarkdownEmojiRegex;
    private static Regex KMarkdownTagRegex => MentionUtils.KMarkdownTagRegex;
    
    public static Task DeleteAsync(IMessage msg, BaseKaiHeiLaClient client, RequestOptions options)
        => DeleteAsync(msg.Channel.Id, msg.Id, client, options);

    public static async Task DeleteAsync(ulong channelId, Guid msgId, BaseKaiHeiLaClient client,
        RequestOptions options)
    {
        await client.ApiClient.DeleteMessageAsync(msgId, options).ConfigureAwait(false);
    }

    public static string SanitizeMessage(IMessage message)
    {
        var newContent = MentionUtils.Resolve(message, 0, TagHandling.FullName, TagHandling.FullName,
            TagHandling.FullName, TagHandling.FullName, TagHandling.FullName);
        newContent = Format.StripMarkDown(newContent);
        return newContent;
    }

    public static ImmutableArray<ITag> ParseTags(string text, IMessageChannel channel, IGuild guild,
        IReadOnlyCollection<IUser> userMentions, TagMode tagMode)
    {
        var tags = ImmutableArray.CreateBuilder<ITag>();
        int index = 0;
        var codeIndex = 0;

        // checks if the tag being parsed is wrapped in code blocks
        bool CheckWrappedCode()
        {
            // util to check if the index of a tag is within the bounds of the codeblock
            bool EnclosedInBlock(Match m)
                => m.Groups[1].Index < index && index < m.Groups[2].Index;
            
            // PlainText mode
            if (tagMode == TagMode.PlainText)
                return false;
            
            // loop through all code blocks that are before the start of the tag
            while (codeIndex < index)
            {
                var blockMatch = BlockCodeRegex.Match(text, codeIndex);
                if (blockMatch.Success)
                {
                    if (EnclosedInBlock(blockMatch))
                        return true;
                    // continue if the end of the current code was before the start of the tag
                    codeIndex += blockMatch.Groups[2].Index + blockMatch.Groups[2].Length;
                    if (codeIndex < index)
                        continue;
                    return false;
                }

                var inlineMatch = InlineCodeRegex.Match(text, codeIndex);
                if (inlineMatch.Success)
                {
                    if (EnclosedInBlock(inlineMatch))
                        return true;
                    // continue if the end of the current code was before the start of the tag
                    codeIndex += inlineMatch.Groups[2].Index + inlineMatch.Groups[2].Length;
                    if (codeIndex < index)
                        continue;
                    return false;
                }

                return false;
            }

            return false;
        }

        var matchCollection = tagMode switch
        {
            TagMode.PlainText => PlainTextTagRegex.Matches(text),
            TagMode.KMarkdown => KMarkdownTagRegex.Matches(text),
        };

        foreach (Match match in matchCollection.Where(m => m.Success))
        {
            index = match.Index;
            if (CheckWrappedCode()) break;
            string content = match.Groups[0].Value;
            if (MentionUtils.TryParseUser(content, out ulong id, tagMode))
            {
                IUser mentionedUser = null;
                foreach (var mention in userMentions)
                {
                    if (mention.Id == id)
                    {
                        mentionedUser = channel?.GetUserAsync(id, CacheMode.CacheOnly).GetAwaiter().GetResult();
                        if (mentionedUser == null)
                            mentionedUser = mention;
                        break;
                    }
                }

                tags.Add(new Tag<IUser>(TagType.UserMention, index, content.Length, id, mentionedUser));
            }
            else if (MentionUtils.TryParseChannel(content, out id, tagMode))
            {
                IChannel mentionedChannel = null;
                if (guild != null)
                    mentionedChannel = guild.GetChannelAsync(id, CacheMode.CacheOnly).GetAwaiter().GetResult();
                tags.Add(new Tag<IChannel>(TagType.ChannelMention, index, content.Length, id, mentionedChannel));
            }
            else if (MentionUtils.TryParseRole(content, out uint roleId, tagMode))
            {
                IRole mentionedRole = null;
                if (guild != null)
                    mentionedRole = guild.GetRole(roleId);
                tags.Add(new Tag<IRole>(TagType.RoleMention, index, content.Length, roleId, mentionedRole));
            }
            else if (Emote.TryParse(content, out Emote emoji, tagMode))
                tags.Add(new Tag<Emote>(TagType.Emoji, index, content.Length, emoji.Id, emoji));
            else //Bad Tag
                continue;
        }

        index = 0;
        codeIndex = 0;
        while (true)
        {
            index = text.IndexOf("@全体成员", index, StringComparison.Ordinal);
            if (index == -1) break;
            if (CheckWrappedCode()) break;
            var tagIndex = FindIndex(tags, index);
            if (tagIndex.HasValue)
                tags.Insert(tagIndex.Value,
                    new Tag<IRole>(TagType.EveryoneMention, index, "@全体成员".Length, 0, guild?.EveryoneRole));
            index++;
        }

        index = 0;
        codeIndex = 0;
        while (true)
        {
            index = text.IndexOf("@在线成员", index, StringComparison.Ordinal);
            if (index == -1) break;
            if (CheckWrappedCode()) break;
            var tagIndex = FindIndex(tags, index);
            if (tagIndex.HasValue)
                tags.Insert(tagIndex.Value,
                    new Tag<IRole>(TagType.HereMention, index, "@在线成员".Length, 0, guild?.EveryoneRole));
            index++;
        }

        return tags.ToImmutable();
    }

    private static int? FindIndex(IReadOnlyList<ITag> tags, int index)
    {
        int i = 0;
        for (; i < tags.Count; i++)
        {
            var tag = tags[i];
            if (index < tag.Index)
                break; //Position before this tag
        }
        if (i > 0 && index < tags[i - 1].Index + tags[i - 1].Length)
            return null; //Overlaps tag before this
        return i;
    }
    
    public static MessageSource GetSource(Model msg)
    {
        if (msg.Author.Bot ?? false)
            return MessageSource.Bot;
        if (msg.Type == MessageType.System)
            return MessageSource.System;
        return MessageSource.User;
    }

    public static IUser GetAuthor(BaseKaiHeiLaClient client, IGuild guild, UserModel model)
    {
        IUser author = null;
        if (guild != null)
            author = guild.GetUserAsync(model.Id, CacheMode.CacheOnly).Result;
        if (author == null)
            author = RestUser.Create(client, model);
        return author;
    }
}