using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Kook.API;
using Kook.API.Rest;
using Kook.Net.Converters;

namespace Kook.Rest;
using Model = Kook.API.Message;
using UserModel = Kook.API.User;

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
    
    public static Task DeleteAsync(IMessage msg, BaseKookClient client, RequestOptions options)
        => DeleteAsync(msg.Id, client, options);
    public static async Task DeleteAsync(Guid msgId, BaseKookClient client,
        RequestOptions options)
    {
        await client.ApiClient.DeleteMessageAsync(msgId, options).ConfigureAwait(false);
    }

    public static Task DeleteDirectAsync(IMessage msg, BaseKookClient client, RequestOptions options)
        => DeleteDirectAsync(msg.Id, client, options);
    public static async Task DeleteDirectAsync(Guid msgId, BaseKookClient client,
        RequestOptions options)
    {
        await client.ApiClient.DeleteDirectMessageAsync(msgId, options).ConfigureAwait(false);
    }
    
    public static async Task AddReactionAsync(Guid messageId, IEmote emote, BaseKookClient client,
        RequestOptions options)
    {
        AddReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = messageId
        };
        await client.ApiClient.AddReactionAsync(args, options).ConfigureAwait(false);
    }
    public static async Task AddReactionAsync(IMessage msg, IEmote emote, BaseKookClient client, RequestOptions options)
    {
        AddReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = msg.Id
        };
        
        await client.ApiClient.AddReactionAsync(args, options).ConfigureAwait(false);
    }

    public static async Task AddDirectMessageReactionAsync(Guid messageId, IEmote emote, BaseKookClient client,
        RequestOptions options)
    {
        AddReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = messageId
        };
        await client.ApiClient.AddDirectMessageReactionAsync(args, options).ConfigureAwait(false);
    }
    public static async Task AddDirectMessageReactionAsync(IMessage msg, IEmote emote, BaseKookClient client, RequestOptions options)
    {
        AddReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = msg.Id
        };
        
        await client.ApiClient.AddDirectMessageReactionAsync(args, options).ConfigureAwait(false);
    }

    public static async Task RemoveReactionAsync(Guid messageId, ulong userId, IEmote emote, BaseKookClient client, RequestOptions options)
    {
        RemoveReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = messageId,
            UserId = userId == client.CurrentUser.Id ? null : userId
        };
        await client.ApiClient.RemoveReactionAsync(args, options).ConfigureAwait(false);
    }
    public static async Task RemoveReactionAsync(IMessage msg, ulong userId, IEmote emote, BaseKookClient client, RequestOptions options)
    {
        
        RemoveReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = msg.Id,
            UserId = userId == client.CurrentUser.Id ? null : userId
        };
        await client.ApiClient.RemoveReactionAsync(args, options).ConfigureAwait(false);
    }

    public static async Task RemoveDirectMessageReactionAsync(Guid messageId, ulong userId, IEmote emote, BaseKookClient client, RequestOptions options)
    {
        RemoveReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = messageId,
            UserId = userId == client.CurrentUser.Id ? null : userId
        };
        await client.ApiClient.RemoveDirectMessageReactionAsync(args, options).ConfigureAwait(false);
    }
    public static async Task RemoveDirectMessageReactionAsync(IMessage msg, ulong userId, IEmote emote, BaseKookClient client, RequestOptions options)
    {
        
        RemoveReactionParams args = new()
        {
            EmojiId = emote.Id,
            MessageId = msg.Id,
            UserId = userId == client.CurrentUser.Id ? null : userId
        };
        await client.ApiClient.RemoveDirectMessageReactionAsync(args, options).ConfigureAwait(false);
    }

    public static async Task<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IMessage msg, IEmote emote, BaseKookClient client, RequestOptions options)
    {
        var models = await client.ApiClient.GetReactionUsersAsync(msg.Id, emote.Id, options).ConfigureAwait(false);
        return models.Select(x => RestUser.Create(client, x)).ToImmutableArray();
    }
    public static async Task<IReadOnlyCollection<IUser>> GetDirectMessageReactionUsersAsync(IMessage msg, IEmote emote, BaseKookClient client, RequestOptions options)
    {
        var models = await client.ApiClient.GetDirectMessageReactionUsersAsync(msg.Id, emote.Id, options).ConfigureAwait(false);
        return models.Select(x => RestUser.Create(client, x)).ToImmutableArray();
    }
    
    public static async Task ModifyAsync(IUserMessage msg, BaseKookClient client, Action<MessageProperties> func,
        RequestOptions options)
    {
        if (msg.Type == MessageType.KMarkdown)
        {
            MessageProperties args = new MessageProperties()
            {
                Content = msg.Content,
                Quote = msg.Quote as Quote
            };
            func(args);
            Preconditions.NotNullOrEmpty(args.Content, nameof(args.Content));
            await ModifyAsync(msg.Id, client, args.Content, options, args.Quote, args.EphemeralUser);
            return;
        }

        if (msg.Type == MessageType.Card)
        {
            MessageProperties args = new MessageProperties()
            {
                Cards = msg.Cards,
                Quote = msg.Quote as Quote
            };
            func(args);
            if (args.Cards is null || !args.Cards.Any())
                throw new ArgumentNullException(nameof(args.Cards), "CardMessage must contains cards.");

            string json = SerializeCards(args.Cards);
            await ModifyAsync(msg.Id, client, json, options, args.Quote, args.EphemeralUser);
            return;
        }
        
        throw new NotSupportedException("Only the modification of KMarkdown and CardMessage are supported.");
    }

    public static async Task ModifyAsync(Guid msgId, BaseKookClient client, Action<MessageProperties> func,
        RequestOptions options, IQuote quote = null, IUser ephemeralUser = null)
    {
        var properties = new MessageProperties();
        func(properties);
        if (string.IsNullOrEmpty(properties.Content) ^ (properties.Cards is not null && properties.Cards.Any()))
            throw new InvalidOperationException("Only one of arguments can be set between Content and Cards");

        string content = string.Empty;
        if (!string.IsNullOrEmpty(properties.Content))
            content = properties.Content;
        if (properties.Cards is not null && properties.Cards.Any())
            content = SerializeCards(properties.Cards);
        
        await ModifyAsync(msgId, client, content, options, quote, ephemeralUser);
    }
    public static async Task ModifyAsync(Guid msgId, BaseKookClient client, string content,
        RequestOptions options, IQuote quote = null, IUser ephemeralUser = null)
    {
        ModifyMessageParams args = new(msgId, content)
        {
            QuotedMessageId = quote?.QuotedMessageId,
            EphemeralUserId = ephemeralUser?.Id
        };
        await client.ApiClient.ModifyMessageAsync(args, options).ConfigureAwait(false);
    }

    public static async Task ModifyDirectAsync(Guid msgId, BaseKookClient client, Action<MessageProperties> func,
        RequestOptions options, IQuote quote = null)
    {
        var properties = new MessageProperties();
        func(properties);
        if (string.IsNullOrEmpty(properties.Content) ^ (properties.Cards is not null && properties.Cards.Any()))
            throw new InvalidOperationException("Only one of arguments can be set between Content and Cards");

        string content = string.Empty;
        if (!string.IsNullOrEmpty(properties.Content))
            content = properties.Content;
        if (properties.Cards is not null && properties.Cards.Any())
            content = SerializeCards(properties.Cards);
        
        await ModifyDirectAsync(msgId, client, content, options, quote);
    }
    public static async Task ModifyDirectAsync(Guid msgId, BaseKookClient client, string content,
        RequestOptions options, IQuote quote = null)
    {
        ModifyDirectMessageParams args = new(msgId, content)
        {
            QuotedMessageId = quote?.QuotedMessageId,
        };
        await client.ApiClient.ModifyDirectMessageAsync(args, options).ConfigureAwait(false);
    }

    public static ImmutableArray<ICard> ParseCards(string json)
    {
        JsonSerializerOptions serializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters =
            {
                new CardConverter(),
                new ModuleConverter(),
                new ElementConverter()
            }
        };
        CardBase[] cardBases = JsonSerializer.Deserialize<CardBase[]>(json, serializerOptions);
            
        var cards = ImmutableArray.CreateBuilder<ICard>(cardBases.Length);
        foreach (CardBase cardBase in cardBases)
            cards.Add(cardBase.ToEntity());

        return cards.ToImmutable();
    }
    
    public static string SerializeCards(IEnumerable<ICard> cards)
    {
        const int maxModuleCount = 50;
        IEnumerable<ICard> enumerable = cards as ICard[] ?? cards.ToArray();
        Preconditions.AtMost(enumerable.Sum(c => c.ModuleCount), maxModuleCount, nameof(cards), 
            $"A max of {maxModuleCount} modules are allowed.");
        
        CardBase[] cardBases = enumerable.Select(c => c.ToModel()).ToArray();
        JsonSerializerOptions serializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters =
            {
                new CardConverter(),
                new ModuleConverter(),
                new ElementConverter()
            },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        return JsonSerializer.Serialize(cardBases, serializerOptions);
    }
    
    public static IReadOnlyCollection<Attachment> ParseAttachments(IEnumerable<Card> cards)
    {
        List<Attachment> attachments = new();
        IEnumerable<IModule> modules = cards.SelectMany(x => x.Modules);
        foreach (IModule module in modules)
        {
            switch (module)
            {
                case FileModule fileModule:
                    attachments.Add(new Attachment(AttachmentType.File, fileModule.Source, fileModule.Title, 
                        null, null, null, null, null));
                    break;
                case AudioModule audioModule:
                    attachments.Add(new Attachment(AttachmentType.Audio, audioModule.Source, audioModule.Title, 
                        null, null, null, null, null));
                    break;
                case VideoModule videoModule:
                    attachments.Add(new Attachment(AttachmentType.Video, videoModule.Source, videoModule.Title, 
                        null, null, null, null, null));
                    break;
                case ContainerModule containerModule:
                    attachments.AddRange(containerModule.Elements.Select(x => 
                        new Attachment(AttachmentType.Image, x.Source, x.Alternative, 
                        null, null, null, null, null)));
                    break;
                case ImageGroupModule imageGroupModule:
                    attachments.AddRange(imageGroupModule.Elements.Select(x => 
                        new Attachment(AttachmentType.Image, x.Source, x.Alternative, 
                        null, null, null, null, null)));
                    break;
                case ContextModule contextModule:
                    attachments.AddRange(contextModule.Elements
                        .OfType<ImageElement>()
                        .Select(x => new Attachment(AttachmentType.Image, x.Source, x.Alternative, 
                            null, null, null, null, null)));
                    break;
            }
        }
        return attachments;
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
            _ => throw new ArgumentOutOfRangeException(nameof(tagMode), tagMode, null)
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
            index = text.IndexOf("(met)all(met)", index, StringComparison.Ordinal);
            if (index == -1) break;
            if (CheckWrappedCode()) break;
            var tagIndex = FindIndex(tags, index);
            if (tagIndex.HasValue)
                tags.Insert(tagIndex.Value,
                    new Tag<IRole>(TagType.EveryoneMention, index, "(met)all(met)".Length, 0, guild?.EveryoneRole));
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
                    new Tag<IRole>(TagType.HereMention, index, "@在线成员".Length, 0, null));
            index++;
        }

        index = 0;
        codeIndex = 0;
        while (true)
        {
            index = text.IndexOf("(met)here(met)", index, StringComparison.Ordinal);
            if (index == -1) break;
            if (CheckWrappedCode()) break;
            var tagIndex = FindIndex(tags, index);
            if (tagIndex.HasValue)
                tags.Insert(tagIndex.Value,
                    new Tag<IRole>(TagType.HereMention, index, "(met)here(met)".Length, 0, guild?.EveryoneRole));
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
        if (msg.Author.Id == KookConfig.SystemMessageAuthorID)
            return MessageSource.System;
        return MessageSource.User;
    }
    public static MessageSource GetSource(DirectMessage msg, IUser author)
    {
        if (author.IsBot ?? false)
            return MessageSource.Bot;
        if (msg.AuthorId == KookConfig.SystemMessageAuthorID)
            return MessageSource.System;
        return MessageSource.User;
    }

    public static IUser GetAuthor(BaseKookClient client, IGuild guild, UserModel model)
    {
        IUser author = null;
        if (guild != null)
            author = guild.GetUserAsync(model.Id, CacheMode.CacheOnly).Result;
        if (author == null)
            author = RestUser.Create(client, model);
        return author;
    }
}