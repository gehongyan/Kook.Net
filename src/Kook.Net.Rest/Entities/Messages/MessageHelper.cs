using Kook.API;
using Kook.API.Rest;
using Kook.Net.Converters;
using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Kook.Rest;

using UserModel = User;

internal static class MessageHelper
{
    /// <summary>
    /// Regex used to check if some text is formatted as inline code.
    /// </summary>
    private static readonly Regex InlineCodeRegex =
        new(@"[^\\]?(`).+?[^\\](`)",
            RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

    /// <summary>
    /// Regex used to check if some text is formatted as a code block.
    /// </summary>
    private static readonly Regex BlockCodeRegex =
        new(@"[^\\]?(```).+?[^\\](```)",
            RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);

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

    public static Task DeleteAsync(IMessage msg, BaseKookClient client, RequestOptions? options) =>
        DeleteAsync(msg.Id, client, options);

    public static async Task DeleteAsync(Guid msgId, BaseKookClient client, RequestOptions? options) =>
        await client.ApiClient.DeleteMessageAsync(msgId, options).ConfigureAwait(false);

    public static Task DeleteDirectAsync(IMessage msg, BaseKookClient client, RequestOptions? options) =>
        DeleteDirectAsync(msg.Id, client, options);

    public static async Task DeleteDirectAsync(Guid msgId, BaseKookClient client, RequestOptions? options) =>
        await client.ApiClient.DeleteDirectMessageAsync(msgId, options).ConfigureAwait(false);

    public static async Task AddReactionAsync(Guid messageId, IEmote emote,
        BaseKookClient client, RequestOptions? options)
    {
        AddReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = messageId
        };
        await client.ApiClient.AddReactionAsync(args, options).ConfigureAwait(false);
    }

    public static async Task AddReactionAsync(IMessage msg, IEmote emote,
        BaseKookClient client, RequestOptions? options)
    {
        AddReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = msg.Id
        };
        await client.ApiClient.AddReactionAsync(args, options).ConfigureAwait(false);
    }

    public static async Task AddDirectMessageReactionAsync(Guid messageId, IEmote emote,
        BaseKookClient client, RequestOptions? options)
    {
        AddReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = messageId
        };
        await client.ApiClient.AddDirectMessageReactionAsync(args, options).ConfigureAwait(false);
    }

    public static async Task AddDirectMessageReactionAsync(IMessage msg, IEmote emote,
        BaseKookClient client, RequestOptions? options)
    {
        AddReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = msg.Id
        };
        await client.ApiClient.AddDirectMessageReactionAsync(args, options).ConfigureAwait(false);
    }

    public static async Task RemoveReactionAsync(Guid messageId, ulong userId, IEmote emote,
        BaseKookClient client, RequestOptions? options)
    {
        RemoveReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = messageId,
            UserId = userId == client.CurrentUser?.Id ? null : userId
        };
        await client.ApiClient.RemoveReactionAsync(args, options).ConfigureAwait(false);
    }

    public static async Task RemoveReactionAsync(IMessage msg, ulong userId, IEmote emote,
        BaseKookClient client, RequestOptions? options)
    {
        RemoveReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = msg.Id,
            UserId = userId == client.CurrentUser?.Id ? null : userId
        };
        await client.ApiClient.RemoveReactionAsync(args, options).ConfigureAwait(false);
    }

    public static async Task RemoveDirectMessageReactionAsync(Guid messageId, ulong userId, IEmote emote,
        BaseKookClient client, RequestOptions? options)
    {
        RemoveReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = messageId,
            UserId = userId == client.CurrentUser?.Id ? null : userId
        };
        await client.ApiClient.RemoveDirectMessageReactionAsync(args, options).ConfigureAwait(false);
    }

    public static async Task RemoveDirectMessageReactionAsync(IMessage msg, ulong userId, IEmote emote,
        BaseKookClient client, RequestOptions? options)
    {
        RemoveReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = msg.Id,
            UserId = userId == client.CurrentUser?.Id ? null : userId
        };
        await client.ApiClient.RemoveDirectMessageReactionAsync(args, options).ConfigureAwait(false);
    }

    public static async Task<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IMessage msg, IEmote emote,
        BaseKookClient client, RequestOptions? options)
    {
        IReadOnlyCollection<ReactionUserResponse> models = await client.ApiClient
            .GetReactionUsersAsync(msg.Id, emote.Id, options)
            .ConfigureAwait(false);
        return [..models.Select(x => RestUser.Create(client, x))];
    }

    public static async Task<IReadOnlyCollection<IUser>> GetDirectMessageReactionUsersAsync(
        IMessage msg, IEmote emote, BaseKookClient client, RequestOptions? options)
    {
        IReadOnlyCollection<ReactionUserResponse> models = await client.ApiClient
            .GetDirectMessageReactionUsersAsync(msg.Id, emote.Id, options)
            .ConfigureAwait(false);
        return [..models.Select(x => RestUser.Create(client, x))];
    }

    public static async Task ModifyAsync(IUserMessage msg, BaseKookClient client, Action<MessageProperties> func,
        RequestOptions? options)
    {
        if (msg.Type == MessageType.KMarkdown)
        {
            MessageProperties args = new()
            {
                Content = msg.Content,
                Quote = msg.Quote
            };
            func(args);
            Preconditions.NotNullOrEmpty(args.Content, nameof(args.Content));
            await ModifyAsync(msg.Id, client, args.Content, args.Quote, args.EphemeralUser, options);
            return;
        }

        if (msg.Type == MessageType.Card)
        {
            MessageProperties args = new()
            {
                Cards = msg.Cards,
                Quote = msg.Quote
            };
            func(args);
            if (args.Cards is null || !args.Cards.Any())
                throw new ArgumentNullException(nameof(args.Cards), "CardMessage must contains cards.");
            string json = SerializeCards(args.Cards);
            await ModifyAsync(msg.Id, client, json, args.Quote, args.EphemeralUser, options);
            return;
        }

        throw new NotSupportedException("Only the modification of KMarkdown and CardMessage are supported.");
    }

    public static async Task ModifyAsync(Guid msgId, BaseKookClient client,
        Action<MessageProperties> func, RequestOptions? options)
    {
        MessageProperties properties = new();
        func(properties);
        if (string.IsNullOrEmpty(properties.Content) ^ (properties.Cards is not null && properties.Cards.Any()))
            throw new InvalidOperationException("Only one of arguments can be set between Content and Cards");

        string content;
        if (properties.Content != null && !string.IsNullOrEmpty(properties.Content))
            content = properties.Content;
        else if (properties.Cards is not null && properties.Cards.Any())
            content = SerializeCards(properties.Cards);
        else
            content = string.Empty;

        await ModifyAsync(msgId, client, content, properties.Quote, properties.EphemeralUser, options);
    }

    public static async Task ModifyAsync(Guid msgId, BaseKookClient client, string content,
        IQuote? quote, IUser? ephemeralUser, RequestOptions? options)
    {
        ModifyMessageParams args = new()
        {
            MessageId = msgId,
            Content = content,
            QuotedMessageId = quote?.QuotedMessageId,
            EphemeralUserId = ephemeralUser?.Id
        };
        await client.ApiClient.ModifyMessageAsync(args, options).ConfigureAwait(false);
    }

    public static async Task ModifyDirectAsync(Guid msgId, BaseKookClient client,
        Action<MessageProperties> func, RequestOptions? options)
    {
        MessageProperties properties = new();
        func(properties);
        if (string.IsNullOrEmpty(properties.Content) ^ (properties.Cards is not null && properties.Cards.Any()))
            throw new InvalidOperationException("Only one of arguments can be set between Content and Cards");

        string content;
        if (properties.Content != null && !string.IsNullOrEmpty(properties.Content))
            content = properties.Content;
        else if (properties.Cards is not null && properties.Cards.Any())
            content = SerializeCards(properties.Cards);
        else
            content = string.Empty;

        await ModifyDirectAsync(msgId, client, content, properties.Quote, options);
    }

    public static async Task ModifyDirectAsync(Guid msgId, BaseKookClient client,
        string content, IQuote? quote, RequestOptions? options)
    {
        ModifyDirectMessageParams args = new()
        {
            MessageId = msgId,
            Content = content,
            QuotedMessageId = quote?.QuotedMessageId
        };
        await client.ApiClient.ModifyDirectMessageAsync(args, options).ConfigureAwait(false);
    }

    public static ImmutableArray<ICard> ParseCards(string json)
    {
        JsonSerializerOptions serializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters = { CardConverterFactory.Instance }
        };
        CardBase[]? cardBases = JsonSerializer.Deserialize<CardBase[]>(json, serializerOptions);
        if (cardBases is null)
            throw new InvalidOperationException("Failed to parse cards from the provided JSON.");
        return [..cardBases.Select(x => x.ToEntity())];
    }

    public static string SerializeCards(IEnumerable<ICard> cards)
    {
        const int maxModuleCount = 50;
        IEnumerable<ICard> enumerable = cards as ICard[] ?? cards.ToArray();
        Preconditions.AtMost(enumerable.Sum(c => c.ModuleCount), maxModuleCount, nameof(cards),
            $"A max of {maxModuleCount} modules can be included in a card.");

        JsonSerializerOptions serializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters = { CardConverterFactory.Instance },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        return JsonSerializer.Serialize(enumerable.Select(c => c.ToModel()), serializerOptions);
    }

    public static IReadOnlyCollection<Attachment> ParseAttachments(IEnumerable<Card> cards)
    {
        List<Attachment> attachments = [];
        IEnumerable<IModule> modules = cards.SelectMany(x => x.Modules);
        foreach (IModule module in modules)
        {
            switch (module)
            {
                case FileModule fileModule:
                    Attachment file = new(AttachmentType.File, fileModule.Source, fileModule.Title);
                    attachments.Add(file);
                    break;
                case AudioModule audioModule:
                    Attachment audio = new(AttachmentType.Audio, audioModule.Source, audioModule.Title);
                    attachments.Add(audio);
                    break;
                case VideoModule videoModule:
                    Attachment video = new(AttachmentType.Video, videoModule.Source, videoModule.Title);
                    attachments.Add(video);
                    break;
                case ContainerModule containerModule:
                    IEnumerable<Attachment> containerImages = containerModule.Elements
                        .Select(x => new Attachment(AttachmentType.Image, x.Source, x.Alternative));
                    attachments.AddRange(containerImages);
                    break;
                case ImageGroupModule imageGroupModule:
                    IEnumerable<Attachment> groupImages = imageGroupModule.Elements
                        .Select(x => new Attachment(AttachmentType.Image, x.Source, x.Alternative));
                    attachments.AddRange(groupImages);
                    break;
                case ContextModule contextModule:
                    IEnumerable<Attachment> contextImages = contextModule.Elements
                        .OfType<ImageElement>()
                        .Select(x => new Attachment(AttachmentType.Image, x.Source, x.Alternative));
                    attachments.AddRange(contextImages);
                    break;
            }
        }

        return attachments;
    }

    public static string SanitizeMessage(IMessage message)
    {
        string newContent = MentionUtils.Resolve(message, 0,
            TagHandling.FullName, TagHandling.FullName, TagHandling.FullName,
            TagHandling.FullName, TagHandling.FullName);
        return newContent.StripMarkDown();
    }

    public static ImmutableArray<ITag> ParseTags(string text, IMessageChannel? channel, IGuild? guild,
        IReadOnlyCollection<IUser> userMentions, TagMode tagMode)
    {
        ImmutableArray<ITag>.Builder tags = ImmutableArray.CreateBuilder<ITag>();
        int index;
        int codeIndex = 0;

        MatchCollection matchCollection = tagMode switch
        {
            TagMode.PlainText => PlainTextTagRegex.Matches(text),
            TagMode.KMarkdown => KMarkdownTagRegex.Matches(text),
            _ => throw new ArgumentOutOfRangeException(nameof(tagMode), tagMode, null)
        };

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        foreach (Match match in matchCollection.Where(m => m.Success))
#else
        foreach (Match match in matchCollection.Cast<Match>().Where(m => m.Success))
#endif
        {
            index = match.Index;
            if (CheckWrappedCode())
                break;

            string content = match.Groups[0].Value;
            if (MentionUtils.TryParseUser(content, out ulong userId, tagMode))
            {
                IUser? mentionedUser = userMentions.FirstOrDefault(x => x.Id == userId);
                IUser? channelUser = channel?.GetUserAsync(userId, CacheMode.CacheOnly).GetAwaiter().GetResult();
                tags.Add(new Tag<IUser>(TagType.UserMention, index, content.Length, userId, channelUser ?? mentionedUser));
            }
            else if (MentionUtils.TryParseChannel(content, out ulong channelId, tagMode))
            {
                IGuildChannel? mentionedChannel = guild?.GetChannelAsync(channelId, CacheMode.CacheOnly).GetAwaiter().GetResult();
                tags.Add(new Tag<IChannel>(TagType.ChannelMention, index, content.Length, channelId, mentionedChannel));
            }
            else if (MentionUtils.TryParseRole(content, out uint roleId, tagMode))
            {
                IRole? mentionedRole = guild?.GetRole(roleId);
                tags.Add(new Tag<IRole>(TagType.RoleMention, index, content.Length, roleId, mentionedRole));
            }
            else if (Emote.TryParse(content, out Emote? emoji, tagMode))
                tags.Add(new Tag<Emote>(TagType.Emoji, index, content.Length, emoji.Id, emoji));
            // Bad Tag
        }

        index = 0;
        codeIndex = 0;
        if (tagMode == TagMode.PlainText)
        {
            while (true)
            {
                index = text.IndexOf("@全体成员", index, StringComparison.Ordinal);
                if (index == -1) break;
                if (CheckWrappedCode()) break;
                int? tagIndex = FindIndex(tags, index);
                if (tagIndex.HasValue)
                {
                    Tag<IRole> everyoneMention = new(TagType.EveryoneMention, index, "@全体成员".Length, 0, guild?.EveryoneRole);
                    tags.Insert(tagIndex.Value, everyoneMention);
                }
                index++;
            }
        }
        else
        {
            while (true)
            {
                index = text.IndexOf("(met)all(met)", index, StringComparison.Ordinal);
                if (index == -1) break;
                if (CheckWrappedCode()) break;
                int? tagIndex = FindIndex(tags, index);
                if (tagIndex.HasValue)
                {
                    Tag<IRole> everyoneMention = new(TagType.EveryoneMention, index, "(met)all(met)".Length, 0, guild?.EveryoneRole);
                    tags.Insert(tagIndex.Value, everyoneMention);
                }
                index++;
            }
        }

        index = 0;
        codeIndex = 0;
        if (tagMode == TagMode.PlainText)
        {
            while (true)
            {
                index = text.IndexOf("@在线成员", index, StringComparison.Ordinal);
                if (index == -1) break;
                if (CheckWrappedCode()) break;
                int? tagIndex = FindIndex(tags, index);
                if (tagIndex.HasValue)
                {
                    Tag<IRole> hereMention = new(TagType.HereMention, index, "@在线成员".Length, 0, null);
                    tags.Insert(tagIndex.Value, hereMention);
                }
                index++;
            }
        }
        else
        {
            while (true)
            {
                index = text.IndexOf("(met)here(met)", index, StringComparison.Ordinal);
                if (index == -1) break;
                if (CheckWrappedCode()) break;
                int? tagIndex = FindIndex(tags, index);
                if (tagIndex.HasValue)
                {
                    Tag<IRole> hereMention = new(TagType.HereMention, index, "(met)here(met)".Length, 0, guild?.EveryoneRole);
                    tags.Insert(tagIndex.Value, hereMention);
                }
                index++;
            }
        }

        return tags.ToImmutable();

        // checks if the tag being parsed is wrapped in code blocks
        bool CheckWrappedCode()
        {
            // PlainText mode
            if (tagMode == TagMode.PlainText)
                return false;

            // loop through all code blocks that are before the start of the tag
            while (codeIndex < index)
            {
                Match blockMatch = BlockCodeRegex.Match(text, codeIndex);
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

                Match inlineMatch = InlineCodeRegex.Match(text, codeIndex);
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

            // util to check if the index of a tag is within the bounds of the codeblock
            bool EnclosedInBlock(Match m) => m.Groups[1].Index < index && index < m.Groups[2].Index;
        }
    }

    private static int? FindIndex(IReadOnlyList<ITag> tags, int index)
    {
        int i = 0;
        for (; i < tags.Count; i++)
        {
            ITag tag = tags[i];
            if (index < tag.Index)
                break; //Position before this tag
        }

        if (i > 0 && index < tags[i - 1].Index + tags[i - 1].Length)
            return null; //Overlaps tag before this

        return i;
    }

    public static MessageSource GetSource(Message msg)
    {
        if (msg.Author.IsSystemUser ?? msg.Author.Id == KookConfig.SystemMessageAuthorID)
            return MessageSource.System;
        if (msg.Author.Bot is true)
            return MessageSource.Bot;
        return MessageSource.User;
    }

    public static MessageSource GetSource(IUser author)
    {
        return author switch
        {
            { IsSystemUser: true } => MessageSource.System,
            { IsBot: true } => MessageSource.Bot,
            _ => MessageSource.User
        };
    }

    public static async Task<IUser> GetAuthorAsync(BaseKookClient client, IGuild? guild, UserModel model)
    {
        IUser? author = guild is not null
            ? await guild.GetUserAsync(model.Id, CacheMode.CacheOnly)
            : null;
        return author ?? RestUser.Create(client, model);
    }

    public static IUser GetAuthor(BaseKookClient client, IGuild? guild, UserModel model)
    {
        IUser? author = guild?.GetUserAsync(model.Id, CacheMode.CacheOnly).GetAwaiter().GetResult();
        return author ?? RestUser.Create(client, model);
    }

    public static IUser GetAuthor(BaseKookClient client, IDMChannel channel, DirectMessage model)
    {
        if (client.CurrentUser is null)
            throw new InvalidOperationException("The current user is not set well via login.");
        return model.AuthorId == channel.Recipient.Id
            ? channel.Recipient
            : client.CurrentUser;
    }
}
