using Kook.API;
using Kook.API.Rest;
using System.Collections.Immutable;
using System.Text.Json;
using Model = Kook.API.Channel;

namespace Kook.Rest;

internal static class ChannelHelper
{
    #region General

    public static async Task DeleteGuildChannelAsync(IGuildChannel channel, BaseKookClient client, RequestOptions? options) =>
        await client.ApiClient.DeleteGuildChannelAsync(channel.Id, options).ConfigureAwait(false);

    public static async Task<Model> ModifyAsync(IGuildChannel channel, BaseKookClient client,
        Action<ModifyGuildChannelProperties> func, RequestOptions? options)
    {
        ModifyGuildChannelProperties args = new();
        func(args);
        ModifyGuildChannelParams apiArgs = new()
        {
            ChannelId = channel.Id,
            Name = args.Name,
            Position = args.Position,
            CategoryId = args.CategoryId
        };
        return await client.ApiClient.ModifyGuildChannelAsync(channel.Id, apiArgs, options).ConfigureAwait(false);
    }

    public static async Task<Model> ModifyAsync(ITextChannel channel, BaseKookClient client,
        Action<ModifyTextChannelProperties> func, RequestOptions? options)
    {
        ModifyTextChannelProperties args = new();
        func(args);
        ModifyTextChannelParams apiArgs = new()
        {
            ChannelId = channel.Id,
            Name = args.Name,
            Position = args.Position,
            CategoryId = args.CategoryId,
            Topic = args.Topic,
            SlowModeInterval = (int?)args.SlowModeInterval * 1000
        };
        return await client.ApiClient.ModifyGuildChannelAsync(channel.Id, apiArgs, options).ConfigureAwait(false);
    }

    public static async Task<Model> ModifyAsync(IVoiceChannel channel, BaseKookClient client,
        Action<ModifyVoiceChannelProperties> func, RequestOptions? options)
    {
        ModifyVoiceChannelProperties args = new();
        func(args);
        ModifyVoiceChannelParams apiArgs = new()
        {
            ChannelId = channel.Id,
            Name = args.Name,
            Position = args.Position,
            CategoryId = args.CategoryId,
            VoiceQuality = args.VoiceQuality,
            UserLimit = args.UserLimit,
            Password = args.Password,
            OverwriteVoiceRegion = args.OverwriteVoiceRegion,
            VoiceRegion = args.VoiceRegion,
            Topic = args.Topic,
            SlowModeInterval = (int?)args.SlowModeInterval * 1000
        };
        return await client.ApiClient.ModifyGuildChannelAsync(channel.Id, apiArgs, options).ConfigureAwait(false);
    }

    public static async Task<Model> ModifyAsync(IThreadChannel channel, BaseKookClient client,
        Action<ModifyThreadChannelProperties> func, RequestOptions? options)
    {
        ModifyThreadChannelProperties args = new();
        func(args);
        ModifyThreadChannelParams apiArgs = new()
        {
            ChannelId = channel.Id,
            Name = args.Name,
            Position = args.Position,
            CategoryId = args.CategoryId,
            Topic = args.Topic,
            SlowModeInternal = (int?)args.PostCreationInterval * 1000,
            SlowModeReply = (int?)args.ReplyInterval * 1000
        };
        return await client.ApiClient.ModifyGuildChannelAsync(channel.Id, apiArgs, options).ConfigureAwait(false);
    }

    public static async Task DeleteDMChannelAsync(IDMChannel channel, BaseKookClient client, RequestOptions? options) =>
        await client.ApiClient.DeleteUserChatAsync(channel.ChatCode, options).ConfigureAwait(false);

    public static async Task UpdateAsync(RestChannel channel, BaseKookClient client, RequestOptions? options)
    {
        Model model = await client.ApiClient.GetGuildChannelAsync(channel.Id, options).ConfigureAwait(false);
        channel.Update(model);
    }

    #endregion

    #region Messages

    public static async Task<RestMessage> GetMessageAsync(IMessageChannel channel, BaseKookClient client,
        Guid id, RequestOptions? options)
    {
        IGuild? guild = channel is IGuildChannel { GuildId: var guildId }
            ? await (client as IKookClient).GetGuildAsync(guildId, CacheMode.CacheOnly)
            : null;
        Message model = await client.ApiClient.GetMessageAsync(id, options).ConfigureAwait(false);
        IUser author = await MessageHelper.GetAuthorAsync(client, guild, model.Author);
        return RestMessage.Create(client, channel, author, model);
    }

    public static IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(
        IMessageChannel channel, BaseKookClient client,
        Guid? referenceMessageId, Direction direction, int limit, bool includeReferenceMessage,
        RequestOptions? options)
    {
        IGuild? guild = channel is IGuildChannel { GuildId: var guildId }
            ? (client as IKookClient).GetGuildAsync(guildId, CacheMode.CacheOnly).GetAwaiter().GetResult()
            : null;

        if (direction == Direction.Around)
        {
            int around = limit / 2;
            if (referenceMessageId.HasValue)
            {
                IAsyncEnumerable<IReadOnlyCollection<RestMessage>> before = GetMessagesAsync(
                    channel, client, referenceMessageId, Direction.Before,
                    around, includeReferenceMessage, options);
                return before.Concat(GetMessagesAsync(
                    channel, client, referenceMessageId, Direction.After,
                    around, false, options));
            }

            return GetMessagesAsync(channel, client, null, Direction.Before,
                around + 1, includeReferenceMessage, options);
        }

        return new PagedAsyncEnumerable<RestMessage, Guid?>(
            KookConfig.MaxMessagesPerBatch,
            async (info, _) =>
            {
                IReadOnlyCollection<Message> models = await client.ApiClient
                    .QueryMessagesAsync(channel.Id, info.Position, null, direction, info.PageSize, options)
                    .ConfigureAwait(false);
                ImmutableArray<RestMessage>.Builder builder = ImmutableArray.CreateBuilder<RestMessage>();

                // Insert the reference message before query results
                if (includeReferenceMessage && info.Position.HasValue && direction == Direction.After)
                {
                    Message currentMessage = await client.ApiClient.GetMessageAsync(info.Position.Value, options);
                    IUser currentMessageAuthor = await MessageHelper.GetAuthorAsync(client, guild, currentMessage.Author);
                    builder.Add(RestMessage.Create(client, channel, currentMessageAuthor, currentMessage));
                }

                foreach (Message model in models)
                {
                    IUser author = await MessageHelper.GetAuthorAsync(client, guild, model.Author);
                    builder.Add(RestMessage.Create(client, channel, author, model));
                }

                // Append the reference message after query results
                if (includeReferenceMessage && info.Position.HasValue && direction == Direction.Before)
                {
                    Message currentMessage = await client.ApiClient.GetMessageAsync(info.Position.Value, options);
                    IUser currentMessageAuthor = await MessageHelper.GetAuthorAsync(client, guild, currentMessage.Author);
                    builder.Add(RestMessage.Create(client, channel, currentMessageAuthor, currentMessage));
                }

                return builder.ToImmutable();
            },
            (info, lastPage) =>
            {
                if (lastPage.Count < KookConfig.MaxMessagesPerBatch)
                    return false;

                info.Position = direction == Direction.Before
                    ? lastPage.MinBy(x => x.Timestamp)?.Id
                    : lastPage.MaxBy(x => x.Timestamp)?.Id;
                return info.Position.HasValue;
            },
            referenceMessageId,
            limit
        );
    }

    public static async Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(IMessageChannel channel,
        BaseKookClient client, RequestOptions? options)
    {
        IGuild? guild = channel is IGuildChannel { GuildId: var guildId }
            ? await (client as IKookClient).GetGuildAsync(guildId, CacheMode.CacheOnly)
            : null;
        IReadOnlyCollection<Message> models = await client.ApiClient
            .QueryMessagesAsync(channel.Id, queryPin: true, options: options).ConfigureAwait(false);
        ImmutableArray<RestMessage>.Builder builder = ImmutableArray.CreateBuilder<RestMessage>();
        foreach (Message model in models)
        {
            IUser author = await MessageHelper.GetAuthorAsync(client, guild, model.Author);
            RestMessage message = RestMessage.Create(client, channel, author, model);
            if (message is RestUserMessage userMessage)
                userMessage.IsPinned = true;
            builder.Add(message);
        }

        return builder.ToImmutable();
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendMessageAsync(IMessageChannel channel,
        BaseKookClient client, MessageType messageType, string content, IQuote? quote, IUser? ephemeralUser,
        RequestOptions? options)
    {
        CreateMessageParams args = new()
        {
            Type = messageType,
            ChannelId = channel.Id,
            Content = content,
            QuotedMessageId = MessageHelper.QuoteToReferenceMessageId(quote),
            ReplyMessageId = MessageHelper.QuoteToReplyMessageId(quote),
            EphemeralUserId = ephemeralUser?.Id
        };
        CreateMessageResponse model = await client.ApiClient.CreateMessageAsync(args, options).ConfigureAwait(false);
        return new Cacheable<IUserMessage, Guid>(null, model.MessageId, false,
            async () => await channel.GetMessageAsync(model.MessageId) as IUserMessage);
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendMessageAsync<T>(IMessageChannel channel,
        BaseKookClient client, MessageType messageType, ulong templateId, T parameters, IQuote? quote,
        IUser? ephemeralUser, JsonSerializerOptions? jsonSerializerOptions, RequestOptions? options)
    {
        CreateMessageParams args = new()
        {
            Type = messageType,
            ChannelId = channel.Id,
            TemplateId = templateId,
            Content = JsonSerializer.Serialize(parameters, jsonSerializerOptions),
            QuotedMessageId = MessageHelper.QuoteToReferenceMessageId(quote),
            ReplyMessageId = MessageHelper.QuoteToReplyMessageId(quote),
            EphemeralUserId = ephemeralUser?.Id
        };
        CreateMessageResponse model = await client.ApiClient.CreateMessageAsync(args, options).ConfigureAwait(false);
        return new Cacheable<IUserMessage, Guid>(null, model.MessageId, false,
            async () => await channel.GetMessageAsync(model.MessageId) as IUserMessage);
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(IMessageChannel channel,
        BaseKookClient client, IEnumerable<ICard> cards, IQuote? quote, IUser? ephemeralUser, RequestOptions? options)
    {
        string json = MessageHelper.SerializeCards(cards);
        return await SendMessageAsync(channel, client, MessageType.Card, json, quote, ephemeralUser, options);
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendCardsAsync<T>(IMessageChannel channel,
        BaseKookClient client, ulong templateId, T parameters, IQuote? quote, IUser? ephemeralUser,
        JsonSerializerOptions? jsonSerializerOptions, RequestOptions? options)
    {
        string json = JsonSerializer.Serialize(parameters, jsonSerializerOptions);
        return await SendMessageAsync(channel, client, MessageType.Card, templateId, json, quote, ephemeralUser, jsonSerializerOptions, options);
    }

    public static Task<Cacheable<IUserMessage, Guid>> SendCardAsync(IMessageChannel channel,
        BaseKookClient client, ICard card, IQuote? quote, IUser? ephemeralUser, RequestOptions? options) =>
        SendCardsAsync(channel, client, [card], quote, ephemeralUser, options);

    public static async Task<Cacheable<IUserMessage, Guid>> SendFileAsync(IMessageChannel channel,
        BaseKookClient client, string path, string filename, AttachmentType type, IQuote? quote, IUser? ephemeralUser,
        RequestOptions? options)
    {
        using FileAttachment file = new(path, filename, type);
        return await SendFileAsync(channel, client, file, quote, ephemeralUser, options);
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendFileAsync(IMessageChannel channel,
        BaseKookClient client, Stream stream, string filename, AttachmentType type, IQuote? quote, IUser? ephemeralUser,
        RequestOptions? options)
    {
        using FileAttachment file = new(stream, filename, type);
        return await SendFileAsync(channel, client, file, quote, ephemeralUser, options);
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendFileAsync(IMessageChannel channel,
        BaseKookClient client, FileAttachment attachment, IQuote? quote, IUser? ephemeralUser, RequestOptions? options)
    {
        switch (attachment.Mode)
        {
            case CreateAttachmentMode.FilePath:
            case CreateAttachmentMode.Stream:
            {
                if (attachment.Uri is not null) break;
                if (attachment.Stream is null)
                    throw new ArgumentNullException(nameof(attachment.Stream), "The stream cannot be null.");
                CreateAssetParams createAssetParams = new()
                {
                    File = attachment.Stream,
                    FileName = attachment.FileName
                };
                CreateAssetResponse assetResponse = await client.ApiClient
                    .CreateAssetAsync(createAssetParams, options).ConfigureAwait(false);
                attachment.Uri = new Uri(assetResponse.Url);
            }
                break;
            case CreateAttachmentMode.AssetUri:
                if (attachment.Uri is null || !UrlValidation.ValidateKookAssetUrl(attachment.Uri.OriginalString))
                    throw new ArgumentException("The uri cannot be blank.", nameof(attachment.Uri));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attachment.Mode), attachment.Mode, "Unknown attachment mode");
        }

        return attachment.Type switch
        {
            AttachmentType.File => await SendAttachmentAsync(MessageType.File).ConfigureAwait(false),
            AttachmentType.Image => await SendAttachmentAsync(MessageType.Image).ConfigureAwait(false),
            AttachmentType.Video => await SendAttachmentAsync(MessageType.Video).ConfigureAwait(false),
            AttachmentType.Audio => await SendCardAsync(
                channel, client,
                new CardBuilder(theme: CardTheme.None, size: CardSize.Large)
                    .AddModule(new AudioModuleBuilder(attachment.Uri.OriginalString, attachment.FileName))
                    .Build(),
                quote, ephemeralUser, options).ConfigureAwait(false),
            _ => throw new ArgumentOutOfRangeException(nameof(attachment.Type), attachment.Type,
                "Unknown attachment type")
        };

        Task<Cacheable<IUserMessage, Guid>> SendAttachmentAsync(MessageType messageType) =>
            SendMessageAsync(channel, client, messageType, attachment.Uri.OriginalString, quote, ephemeralUser, options);
    }

    public static Task DeleteMessageAsync(IMessageChannel channel,
        Guid messageId, BaseKookClient client, RequestOptions? options) =>
        MessageHelper.DeleteAsync(messageId, client, options);

    public static Task DeleteDirectMessageAsync(IMessageChannel channel,
        Guid messageId, BaseKookClient client, RequestOptions? options) =>
        MessageHelper.DeleteDirectAsync(messageId, client, options);

    public static async Task ModifyMessageAsync<T>(IMessageChannel channel, Guid messageId,
        Action<MessageProperties<T>> func, BaseKookClient client, RequestOptions? options) =>
        await MessageHelper.ModifyAsync(messageId, client, func, options).ConfigureAwait(false);

    #endregion

    #region Direct Messages

    public static async Task<RestMessage> GetDirectMessageAsync(IDMChannel channel,
        BaseKookClient client, Guid id, RequestOptions? options)
    {
        DirectMessage model = await client.ApiClient.GetDirectMessageAsync(
            id, channel.Id, options: options).ConfigureAwait(false);
        return RestMessage.Create(client, channel, channel.Recipient, model);
    }

    public static IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetDirectMessagesAsync(IDMChannel channel,
        BaseKookClient client, Guid? referenceMessageId, Direction direction, int limit, bool includeReferenceMessage,
        RequestOptions? options)
    {
        if (direction == Direction.Around)
        {
            int around = limit / 2;
            if (referenceMessageId.HasValue)
            {
                IAsyncEnumerable<IReadOnlyCollection<RestMessage>> before = GetDirectMessagesAsync(
                    channel, client, referenceMessageId, Direction.Before,
                    around, includeReferenceMessage, options);
                return before.Concat(GetDirectMessagesAsync(
                    channel, client, referenceMessageId, Direction.After,
                    around, false, options));
            }

            return GetDirectMessagesAsync(channel, client, null, Direction.Before,
                around + 1, includeReferenceMessage, options);
        }

        return new PagedAsyncEnumerable<RestMessage, Guid?>(
            KookConfig.MaxMessagesPerBatch,
            async (info, _) =>
            {
                IReadOnlyCollection<DirectMessage> models = await client.ApiClient
                    .QueryDirectMessagesAsync(channel.ChatCode, null, info.Position, direction, limit, options)
                    .ConfigureAwait(false);
                ImmutableArray<RestMessage>.Builder builder = ImmutableArray.CreateBuilder<RestMessage>();
                // Insert the reference message before query results
                if (includeReferenceMessage && info.Position.HasValue && direction == Direction.After)
                {
                    DirectMessage currentMessage = await client.ApiClient.GetDirectMessageAsync(
                        info.Position.Value, channel.ChatCode, options);
                    IUser author = MessageHelper.GetAuthor(client, channel, currentMessage);
                    builder.Add(RestMessage.Create(client, channel, author, currentMessage));
                }

                foreach (DirectMessage model in models)
                {
                    IUser author = MessageHelper.GetAuthor(client, channel, model);
                    builder.Add(RestMessage.Create(client, channel, author, model));
                }

                // Append the reference message after query results
                if (includeReferenceMessage && info.Position.HasValue && direction == Direction.Before)
                {
                    DirectMessage currentMessage = await client.ApiClient.GetDirectMessageAsync(
                        info.Position.Value, channel.ChatCode, options);
                    IUser author = MessageHelper.GetAuthor(client, channel, currentMessage);
                    builder.Add(RestMessage.Create(client, channel, author, currentMessage));
                }

                return builder.ToImmutable();
            },
            (info, lastPage) =>
            {
                if (lastPage.Count != KookConfig.MaxMessagesPerBatch) return false;

                info.Position = direction == Direction.Before
                    ? lastPage.MinBy(x => x.Timestamp)?.Id
                    : lastPage.MaxBy(x => x.Timestamp)?.Id;
                return info.Position.HasValue;
            },
            referenceMessageId,
            limit
        );
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendDirectMessageAsync(IDMChannel channel,
        BaseKookClient client, MessageType messageType, string content, IQuote? quote, RequestOptions? options)
    {
        CreateDirectMessageParams args = new()
        {
            Type = messageType,
            UserId = channel.Recipient.Id,
            Content = content,
            QuotedMessageId = MessageHelper.QuoteToReferenceMessageId(quote),
            ReplyMessageId = MessageHelper.QuoteToReplyMessageId(quote)
        };
        CreateDirectMessageResponse model = await client.ApiClient
            .CreateDirectMessageAsync(args, options).ConfigureAwait(false);
        return new Cacheable<IUserMessage, Guid>(null, model.MessageId, false,
            async () => await channel.GetMessageAsync(model.MessageId) as IUserMessage);
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendDirectMessageAsync<T>(IDMChannel channel, BaseKookClient client,
        MessageType messageType, ulong templateId, T parameters, IQuote? quote, JsonSerializerOptions? jsonSerializerOptions, RequestOptions? options)
    {
        CreateDirectMessageParams args = new()
        {
            Type = messageType,
            UserId = channel.Recipient.Id,
            TemplateId = templateId,
            Content = JsonSerializer.Serialize(parameters, jsonSerializerOptions),
            QuotedMessageId = MessageHelper.QuoteToReferenceMessageId(quote),
            ReplyMessageId = MessageHelper.QuoteToReplyMessageId(quote)
        };
        CreateDirectMessageResponse model = await client.ApiClient
            .CreateDirectMessageAsync(args, options).ConfigureAwait(false);
        return new Cacheable<IUserMessage, Guid>(null, model.MessageId, false,
            async () => await channel.GetMessageAsync(model.MessageId) as IUserMessage);
    }

    public static Task<Cacheable<IUserMessage, Guid>> SendDirectCardsAsync(IDMChannel channel,
        BaseKookClient client, IEnumerable<ICard> cards, IQuote? quote, RequestOptions? options)
    {
        string json = MessageHelper.SerializeCards(cards);
        return SendDirectMessageAsync(channel, client, MessageType.Card, json, quote, options);
    }

    public static Task<Cacheable<IUserMessage, Guid>> SendDirectCardsAsync<T>(IDMChannel channel, BaseKookClient client,
        ulong templateId, T parameters, IQuote? quote, JsonSerializerOptions? jsonSerializerOptions, RequestOptions? options)
    {
        string json = JsonSerializer.Serialize(parameters, jsonSerializerOptions);
        return SendDirectMessageAsync(channel, client, MessageType.Card, templateId, json, quote, jsonSerializerOptions, options);
    }

    public static Task<Cacheable<IUserMessage, Guid>> SendDirectCardAsync(IDMChannel channel,
        BaseKookClient client, ICard card, IQuote? quote, RequestOptions? options) =>
        SendDirectCardsAsync(channel, client, [card], quote, options);

    public static async Task<Cacheable<IUserMessage, Guid>> SendDirectFileAsync(IDMChannel channel,
        BaseKookClient client, string path, string? filename, AttachmentType type, IQuote? quote,
        RequestOptions? options)
    {
        using FileAttachment file = new(path, filename, type);
        return await SendDirectFileAsync(channel, client, file, quote, options);
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendDirectFileAsync(IDMChannel channel,
        BaseKookClient client, Stream stream, string filename, AttachmentType type, IQuote? quote,
        RequestOptions? options)
    {
        using FileAttachment file = new(stream, filename, type);
        return await SendDirectFileAsync(channel, client, file, quote, options);
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendDirectFileAsync(IDMChannel channel,
        BaseKookClient client, FileAttachment attachment, IQuote? quote, RequestOptions? options)
    {
        switch (attachment.Mode)
        {
            case CreateAttachmentMode.FilePath:
            case CreateAttachmentMode.Stream:
            {
                if (attachment.Uri is not null) break;
                if (attachment.Stream is null)
                    throw new ArgumentNullException(nameof(attachment.Stream), "The stream cannot be null.");
                CreateAssetParams createAssetParams = new()
                {
                    File = attachment.Stream,
                    FileName = attachment.FileName
                };
                CreateAssetResponse assetResponse = await client.ApiClient
                    .CreateAssetAsync(createAssetParams, options).ConfigureAwait(false);
                attachment.Uri = new Uri(assetResponse.Url);
            }
                break;
            case CreateAttachmentMode.AssetUri:
                if (attachment.Uri is null || !UrlValidation.ValidateKookAssetUrl(attachment.Uri.OriginalString))
                    throw new ArgumentException("The uri cannot be blank.", nameof(attachment.Uri));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attachment.Mode), attachment.Mode, "Unknown attachment mode");
        }

        return attachment.Type switch
        {
            AttachmentType.File => await SendAttachmentAsync(MessageType.File).ConfigureAwait(false),
            AttachmentType.Image => await SendAttachmentAsync(MessageType.Image).ConfigureAwait(false),
            AttachmentType.Video => await SendAttachmentAsync(MessageType.Video).ConfigureAwait(false),
            AttachmentType.Audio => await SendDirectCardAsync(
                channel, client,
                new CardBuilder(theme: CardTheme.None, size: CardSize.Large)
                    .AddModule(new AudioModuleBuilder(attachment.Uri.OriginalString, attachment.FileName))
                    .Build(),
                quote, options).ConfigureAwait(false),
            _ => throw new ArgumentOutOfRangeException(nameof(attachment.Type), attachment.Type,
                "Unknown attachment type")
        };

        Task<Cacheable<IUserMessage, Guid>> SendAttachmentAsync(MessageType messageType) =>
            SendDirectMessageAsync(channel, client, messageType, attachment.Uri.OriginalString, quote, options);
    }

    public static Task ModifyDirectMessageAsync<T>(IDMChannel channel, Guid messageId,
        Action<MessageProperties<T>> func, BaseKookClient client, RequestOptions? options) =>
        MessageHelper.ModifyDirectAsync(messageId, client, func, options);

    #endregion

    #region Permission Overwrites

    public static async Task<UserPermissionOverwrite> AddPermissionOverwriteAsync(IGuildChannel channel,
        BaseKookClient client, IUser user, RequestOptions? options)
    {
        CreateOrRemoveChannelPermissionOverwriteParams args = new()
        {
            ChannelId = channel.Id,
            TargetType = PermissionOverwriteTarget.User,
            TargetId = user.Id
        };
        CreateOrModifyChannelPermissionOverwriteResponse response = await client.ApiClient
            .CreateChannelPermissionOverwriteAsync(args, options)
            .ConfigureAwait(false);
        return new UserPermissionOverwrite(user, new OverwritePermissions(response.Allow, response.Deny));
    }

    public static async Task<RolePermissionOverwrite> AddPermissionOverwriteAsync(IGuildChannel channel,
        BaseKookClient client, IRole role, RequestOptions? options)
    {
        CreateOrRemoveChannelPermissionOverwriteParams args = new()
        {
            ChannelId = channel.Id,
            TargetType = PermissionOverwriteTarget.Role,
            TargetId = role.Id
        };
        CreateOrModifyChannelPermissionOverwriteResponse resp = await client.ApiClient
            .CreateChannelPermissionOverwriteAsync(args, options)
            .ConfigureAwait(false);
        return new RolePermissionOverwrite(role, new OverwritePermissions(resp.Allow, resp.Deny));
    }

    public static async Task RemovePermissionOverwriteAsync(IGuildChannel channel,
        BaseKookClient client, IUser user, RequestOptions? options)
    {
        CreateOrRemoveChannelPermissionOverwriteParams args = new()
        {
            ChannelId = channel.Id,
            TargetType = PermissionOverwriteTarget.User,
            TargetId = user.Id
        };
        await client.ApiClient.RemoveChannelPermissionOverwriteAsync(args, options).ConfigureAwait(false);
    }

    public static async Task RemovePermissionOverwriteAsync(IGuildChannel channel,
        BaseKookClient client, IRole role, RequestOptions? options)
    {
        CreateOrRemoveChannelPermissionOverwriteParams args = new()
        {
            ChannelId = channel.Id,
            TargetType = PermissionOverwriteTarget.Role,
            TargetId = role.Id
        };
        await client.ApiClient.RemoveChannelPermissionOverwriteAsync(args, options).ConfigureAwait(false);
    }

    public static async Task<UserPermissionOverwrite> ModifyPermissionOverwriteAsync(IGuildChannel channel,
        BaseKookClient client, IGuildUser user, Func<OverwritePermissions, OverwritePermissions> func,
        RequestOptions? options)
    {
        OverwritePermissions? perms = channel.UserPermissionOverwrites.SingleOrDefault(x => x.TargetId == user.Id)?.Permissions;
        if (!perms.HasValue)
            throw new ArgumentNullException(nameof(user), "The user does not have any permission overwrites on this channel.");
        perms = func(perms.Value);
        ModifyChannelPermissionOverwriteParams args = new()
        {
            ChannelId = channel.Id,
            TargetType = PermissionOverwriteTarget.User,
            TargetId = user.Id,
            Allow = perms.Value.AllowValue,
            Deny = perms.Value.DenyValue
        };
        CreateOrModifyChannelPermissionOverwriteResponse resp = await client.ApiClient
            .ModifyChannelPermissionOverwriteAsync(args, options)
            .ConfigureAwait(false);
        return new UserPermissionOverwrite(user, new OverwritePermissions(resp.Allow, resp.Deny));
    }

    public static async Task<RolePermissionOverwrite> ModifyPermissionOverwriteAsync(IGuildChannel channel,
        BaseKookClient client, IRole role, Func<OverwritePermissions, OverwritePermissions> func,
        RequestOptions? options)
    {
        OverwritePermissions? perms = channel.RolePermissionOverwrites.SingleOrDefault(x => x.TargetId == role.Id)?.Permissions;
        if (!perms.HasValue)
            throw new ArgumentNullException(nameof(role), "The role does not have any permission overwrites on this channel.");
        perms = func(perms.Value);
        ModifyChannelPermissionOverwriteParams args = new()
        {
            ChannelId = channel.Id,
            TargetType = PermissionOverwriteTarget.Role,
            TargetId = role.Id,
            Allow = perms.Value.AllowValue,
            Deny = perms.Value.DenyValue
        };
        CreateOrModifyChannelPermissionOverwriteResponse resp = await client.ApiClient
            .ModifyChannelPermissionOverwriteAsync(args, options)
            .ConfigureAwait(false);
        return new RolePermissionOverwrite(role, new OverwritePermissions(resp.Allow, resp.Deny));
    }

    #endregion

    #region Invites

    public static async Task<IReadOnlyCollection<RestInvite>> GetInvitesAsync(IGuildChannel channel,
        BaseKookClient client, RequestOptions? options)
    {
        IEnumerable<Invite> models = await client.ApiClient
            .GetGuildInvitesAsync(channel.GuildId, channel.Id, options: options)
            .FlattenAsync()
            .ConfigureAwait(false);
        return models.Select(x => RestInvite.Create(client, channel.Guild, channel, x)).ToImmutableArray();
    }

    public static async Task<RestInvite> CreateInviteAsync(IGuildChannel channel, BaseKookClient client,
        int? maxAge, int? maxUses, RequestOptions? options)
    {
        CreateGuildInviteParams args = new()
        {
            GuildId = channel.GuildId,
            ChannelId = channel.Id,
            MaxAge = (InviteMaxAge)(maxAge ?? 0),
            MaxUses = (InviteMaxUses)(maxUses ?? -1)
        };
        CreateGuildInviteResponse model = await client.ApiClient
            .CreateGuildInviteAsync(args, options).ConfigureAwait(false);
        IEnumerable<Invite> invites = await client.ApiClient
            .GetGuildInvitesAsync(channel.GuildId, channel.Id, options: options)
            .FlattenAsync().ConfigureAwait(false);
        return RestInvite.Create(client, channel.Guild, channel, invites.Single(x => x.Url == model.Url));
    }

    public static Task<RestInvite> CreateInviteAsync(IGuildChannel channel, BaseKookClient client,
        InviteMaxAge maxAge, InviteMaxUses maxUses, RequestOptions? options) =>
        CreateInviteAsync(channel, client, (int?)maxAge, (int?)maxUses, options);

    #endregion

    #region Categories

    public static async Task<ICategoryChannel?> GetCategoryAsync(INestedChannel channel, BaseKookClient client,
        RequestOptions? options)
    {
        // if no category id specified, return null
        if (!channel.CategoryId.HasValue)
            return null;

        // CategoryId will contain a value here
        Model model = await client.ApiClient
            .GetGuildChannelAsync(channel.CategoryId.Value, options)
            .ConfigureAwait(false);
        return RestChannel.Create(client, model) as ICategoryChannel;
    }

    public static Task SyncPermissionsAsync(INestedChannel channel, BaseKookClient client,
        RequestOptions? options)
    {
        SyncChannelPermissionsParams args = new()
        {
            ChannelId = channel.Id
        };
        return client.ApiClient.SyncChannelPermissionsAsync(args, options);
    }

    #endregion

    #region Users

    /// <exception cref="InvalidOperationException">Resolving permissions requires the parent guild to be downloaded.</exception>
    public static async Task<RestGuildUser?> GetUserAsync(IGuildChannel channel,
        IGuild guild, BaseKookClient client, ulong id, RequestOptions? options)
    {
        GuildMember model = await client.ApiClient
            .GetGuildMemberAsync(channel.GuildId, id, options)
            .ConfigureAwait(false);

        RestGuildUser user = RestGuildUser.Create(client, guild, model);
        return user.GetPermissions(channel).ViewChannel
            ? user
            : null;
    }

    /// <exception cref="InvalidOperationException">Resolving permissions requires the parent guild to be downloaded.</exception>
    public static IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> GetUsersAsync(IGuildChannel channel,
        IGuild guild, BaseKookClient client, int limit, int fromPage, RequestOptions? options) =>
        client.ApiClient.GetGuildMembersAsync(guild.Id, limit: limit, fromPage: fromPage, options: options)
            .Select(x => x
                .Select(y => RestGuildUser.Create(client, guild, y))
                .Where(y => y.GetPermissions(channel).ViewChannel)
                .ToImmutableArray() as IReadOnlyCollection<RestGuildUser>);

    public static async Task<IReadOnlyCollection<RestGuildUser>> GetConnectedUsersAsync(IVoiceChannel channel,
        IGuild guild, BaseKookClient client, RequestOptions? options)
    {
        IReadOnlyCollection<User> model = await client.ApiClient.GetConnectedUsersAsync(channel.Id, options);
        return model.Select(x => RestGuildUser.Create(client, guild, x)).ToImmutableArray();
    }

    #endregion
}
