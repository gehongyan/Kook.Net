using Kook.API;
using Kook.API.Rest;
using Kook.Utils;
using System.Collections.Immutable;
using Model = Kook.API.Channel;

namespace Kook.Rest;

internal static class ChannelHelper
{
    #region General

    public static async Task DeleteGuildChannelAsync(IGuildChannel channel, BaseKookClient client,
        RequestOptions options) =>
        await client.ApiClient.DeleteGuildChannelAsync(channel.Id, options).ConfigureAwait(false);

    public static async Task<Model> ModifyAsync(IGuildChannel channel, BaseKookClient client,
        Action<ModifyGuildChannelProperties> func,
        RequestOptions options)
    {
        ModifyGuildChannelProperties args = new();
        func(args);
        ModifyGuildChannelParams apiArgs = new() { ChannelId = channel.Id, Name = args.Name, Position = args.Position, CategoryId = args.CategoryId };
        return await client.ApiClient.ModifyGuildChannelAsync(channel.Id, apiArgs, options).ConfigureAwait(false);
    }

    public static async Task<Model> ModifyAsync(ITextChannel channel, BaseKookClient client,
        Action<ModifyTextChannelProperties> func,
        RequestOptions options)
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
        Action<ModifyVoiceChannelProperties> func,
        RequestOptions options)
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

    public static async Task DeleteDMChannelAsync(IDMChannel channel, BaseKookClient client,
        RequestOptions options) =>
        await client.ApiClient.DeleteUserChatAsync(channel.ChatCode, options).ConfigureAwait(false);

    public static async Task UpdateAsync(RestChannel channel, BaseKookClient client, RequestOptions options)
    {
        Channel model = await client.ApiClient.GetGuildChannelAsync(channel.Id, options).ConfigureAwait(false);
        channel.Update(model);
    }

    #endregion

    #region Messages

    public static async Task<RestMessage> GetMessageAsync(IMessageChannel channel, BaseKookClient client,
        Guid id, RequestOptions options)
    {
        ulong? guildId = (channel as IGuildChannel)?.GuildId;
        IGuild guild = guildId != null
            ? await (client as IKookClient).GetGuildAsync(guildId.Value, CacheMode.CacheOnly).ConfigureAwait(false)
            : null;
        Message model = await client.ApiClient.GetMessageAsync(id, options).ConfigureAwait(false);
        if (model == null) return null;

        IUser author = await MessageHelper.GetAuthorAsync(client, guild, model.Author);
        return RestMessage.Create(client, channel, author, model);
    }

    public static IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessageChannel channel,
        BaseKookClient client,
        Guid? referenceMessageId, Direction dir, int limit, bool includeReferenceMessage, RequestOptions options)
    {
        ulong? guildId = (channel as IGuildChannel)?.GuildId;
        IGuild guild = guildId != null
            ? (client as IKookClient).GetGuildAsync(guildId.Value, CacheMode.CacheOnly).GetAwaiter().GetResult()
            : null;

        if (dir
            == Direction
                .Around) //  && limit > KookConfig.MaxMessagesPerBatch // Around mode returns error messages from endpoint
        {
            int around = limit / 2;
            if (referenceMessageId.HasValue)
            {
                IAsyncEnumerable<IReadOnlyCollection<RestMessage>> messages = GetMessagesAsync(channel, client, referenceMessageId, Direction.Before,
                    around,
                    includeReferenceMessage, options);
                messages = messages.Concat(GetMessagesAsync(channel, client, referenceMessageId, Direction.After,
                    around, false, options));
                return messages;
            }
            else // Shouldn't happen since there's no public overload for Guid? and Direction
                return GetMessagesAsync(channel, client, null, Direction.Before, around + 1, includeReferenceMessage,
                    options);
        }

        return new PagedAsyncEnumerable<RestMessage>(
            KookConfig.MaxMessagesPerBatch,
            async (info, ct) =>
            {
                IReadOnlyCollection<Message> models = await client.ApiClient.QueryMessagesAsync(channel.Id, info.Position,
                    dir: dir, count: limit, options: options).ConfigureAwait(false);
                ImmutableArray<RestMessage>.Builder builder = ImmutableArray.CreateBuilder<RestMessage>();
                // Insert the reference message before query results
                if (includeReferenceMessage && info.Position.HasValue && dir == Direction.After)
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
                if (includeReferenceMessage && info.Position.HasValue && dir == Direction.Before)
                {
                    Message currentMessage = await client.ApiClient.GetMessageAsync(info.Position.Value, options);
                    IUser currentMessageAuthor = await MessageHelper.GetAuthorAsync(client, guild, currentMessage.Author);
                    builder.Add(RestMessage.Create(client, channel, currentMessageAuthor, currentMessage));
                }

                return builder.ToImmutable();
            },
            (info, lastPage) =>
            {
                if (lastPage.Count != KookConfig.MaxMessagesPerBatch) return false;

                if (dir == Direction.Before)
#if NET6_0_OR_GREATER
                    info.Position = lastPage.MinBy(x => x.Timestamp)?.Id;
#else
                    info.Position = lastPage.OrderBy(x => x.Timestamp).FirstOrDefault()?.Id;
#endif
                else

#if NET6_0_OR_GREATER
                    info.Position = lastPage.MaxBy(x => x.Timestamp)?.Id;
#else
                    info.Position = lastPage.OrderByDescending(x => x.Timestamp).FirstOrDefault()?.Id;
#endif
                return true;
            },
            referenceMessageId,
            limit
        );
    }

    public static async Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(IMessageChannel channel,
        BaseKookClient client,
        RequestOptions options)
    {
        ulong? guildId = (channel as IGuildChannel)?.GuildId;
        IGuild guild = guildId != null
            ? await (client as IKookClient).GetGuildAsync(guildId.Value, CacheMode.CacheOnly).ConfigureAwait(false)
            : null;
        IReadOnlyCollection<Message> models = await client.ApiClient.QueryMessagesAsync(channel.Id, queryPin: true, options: options)
            .ConfigureAwait(false);
        ImmutableArray<RestMessage>.Builder builder = ImmutableArray.CreateBuilder<RestMessage>();
        foreach (Message model in models)
        {
            IUser author = await MessageHelper.GetAuthorAsync(client, guild, model.Author);
            RestMessage message = RestMessage.Create(client, channel, author, model);
            if (message is RestUserMessage userMessage) userMessage.IsPinned = true;

            builder.Add(message);
        }

        return builder.ToImmutable();
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendMessageAsync(IMessageChannel channel,
        BaseKookClient client, MessageType messageType, string content, RequestOptions options, IQuote quote = null,
        IUser ephemeralUser = null)
    {
        CreateMessageParams args = new(messageType, channel.Id, content)
        {
            QuotedMessageId = quote?.QuotedMessageId, EphemeralUserId = ephemeralUser?.Id
        };
        CreateMessageResponse model = await client.ApiClient.CreateMessageAsync(args, options).ConfigureAwait(false);
        return new Cacheable<IUserMessage, Guid>(null, model.MessageId, false,
            async () => await channel.GetMessageAsync(model.MessageId) as IUserMessage);
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendCardsAsync(IMessageChannel channel,
        BaseKookClient client, IEnumerable<ICard> cards, RequestOptions options, IQuote quote = null,
        IUser ephemeralUser = null)
    {
        string json = MessageHelper.SerializeCards(cards);
        return await SendMessageAsync(channel, client, MessageType.Card, json, options, quote,
            ephemeralUser);
    }

    public static Task<Cacheable<IUserMessage, Guid>> SendCardAsync(IMessageChannel channel,
        BaseKookClient client, ICard card, RequestOptions options, IQuote quote = null,
        IUser ephemeralUser = null)
        => SendCardsAsync(channel, client, new[] { card }, options, quote, ephemeralUser);

    public static async Task<Cacheable<IUserMessage, Guid>> SendFileAsync(IMessageChannel channel,
        BaseKookClient client, string path, string fileName, AttachmentType type, RequestOptions options,
        IQuote quote = null, IUser ephemeralUser = null)
    {
        using FileAttachment file = new(path, fileName, type);
        return await SendFileAsync(channel, client, file, options, quote, ephemeralUser);
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendFileAsync(IMessageChannel channel,
        BaseKookClient client, Stream stream, string fileName, AttachmentType type, RequestOptions options,
        IQuote quote = null, IUser ephemeralUser = null)
    {
        using FileAttachment file = new(stream, fileName, type);
        return await SendFileAsync(channel, client, file, options, quote, ephemeralUser);
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendFileAsync(IMessageChannel channel,
        BaseKookClient client, FileAttachment attachment, RequestOptions options,
        IQuote quote = null, IUser ephemeralUser = null)
    {
        switch (attachment.Mode)
        {
            case CreateAttachmentMode.FilePath:
            case CreateAttachmentMode.Stream:
                if (attachment.Uri != null)
                    break;
                CreateAssetResponse assetResponse = await client.ApiClient
                    .CreateAssetAsync(new CreateAssetParams { File = attachment.Stream, FileName = attachment.FileName }, options);
                attachment.Uri = new Uri(assetResponse.Url);
                break;
            case CreateAttachmentMode.AssetUri:
                if (!UrlValidation.ValidateKookAssetUrl(attachment.Uri.OriginalString))
                    throw new ArgumentException("The uri cannot be blank.", nameof(attachment.Uri));

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attachment.Mode), attachment.Mode, "Unknown attachment mode");
        }

        return attachment.Type switch
        {
            AttachmentType.File => await SendMessageAsync(channel, client, MessageType.File,
                attachment.Uri.OriginalString, options, quote, ephemeralUser),
            AttachmentType.Image => await SendMessageAsync(channel, client, MessageType.Image,
                attachment.Uri.OriginalString, options, quote, ephemeralUser),
            AttachmentType.Video => await SendMessageAsync(channel, client, MessageType.Video,
                attachment.Uri.OriginalString, options, quote, ephemeralUser),
            AttachmentType.Audio => await SendCardAsync(channel, client,
                new CardBuilder().WithSize(CardSize.Large)
                    .WithTheme(CardTheme.None)
                    .AddModule<AudioModuleBuilder>(x => x
                        .WithSource(attachment.Uri.OriginalString)
                        .WithTitle(attachment.FileName))
                    .Build(), options, quote, ephemeralUser),
            _ => throw new ArgumentOutOfRangeException(nameof(attachment.Type), attachment.Type,
                "Unknown attachment type")
        };
    }

    public static Task DeleteMessageAsync(IMessageChannel channel, Guid messageId, BaseKookClient client,
        RequestOptions options)
        => MessageHelper.DeleteAsync(messageId, client, options);

    public static Task DeleteDirectMessageAsync(IMessageChannel channel, Guid messageId, BaseKookClient client,
        RequestOptions options)
        => MessageHelper.DeleteDirectAsync(messageId, client, options);


    public static async Task ModifyMessageAsync(IMessageChannel channel, Guid messageId, Action<MessageProperties> func,
        BaseKookClient client, RequestOptions options)
        => await MessageHelper.ModifyAsync(messageId, client, func, options).ConfigureAwait(false);

    #endregion

    #region Direct Messages

    public static async Task<RestMessage> GetDirectMessageAsync(IDMChannel channel, BaseKookClient client,
        Guid id, RequestOptions options)
    {
        DirectMessage model = await client.ApiClient.GetDirectMessageAsync(id, channel.Id, options: options)
            .ConfigureAwait(false);
        if (model == null) return null;

        // User userModel = await client.ApiClient.GetUserAsync(model.AuthorId, options);
        // var author = MessageHelper.GetAuthor(client, null, userModel);
        return RestMessage.Create(client, channel, channel.Recipient, model);
    }

    public static IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetDirectMessagesAsync(IDMChannel channel,
        BaseKookClient client,
        Guid? referenceMessageId, Direction dir, int limit, bool includeReferenceMessage, RequestOptions options)
    {
        if (dir == Direction.Around) //  && limit > KookConfig.MaxMessagesPerBatch // Around mode returns error messages from endpoint
        {
            int around = limit / 2;
            if (referenceMessageId.HasValue)
            {
                IAsyncEnumerable<IReadOnlyCollection<RestMessage>> messages = GetDirectMessagesAsync(channel, client, referenceMessageId,
                    Direction.Before, around,
                    includeReferenceMessage, options);
                messages = messages.Concat(GetDirectMessagesAsync(channel, client, referenceMessageId, Direction.After,
                    around, false, options));
                return messages;
            }
            else // Shouldn't happen since there's no public overload for Guid? and Direction
                return GetDirectMessagesAsync(channel, client, null, Direction.Before, around + 1,
                    includeReferenceMessage, options);
        }

        return new PagedAsyncEnumerable<RestMessage>(
            KookConfig.MaxMessagesPerBatch,
            async (info, ct) =>
            {
                IReadOnlyCollection<DirectMessage> models = await client.ApiClient.QueryDirectMessagesAsync(channel.ChatCode,
                    referenceMessageId: info.Position, dir: dir, count: limit, options: options).ConfigureAwait(false);
                ImmutableArray<RestMessage>.Builder builder = ImmutableArray.CreateBuilder<RestMessage>();
                // Insert the reference message before query results
                if (includeReferenceMessage && info.Position.HasValue && dir == Direction.After)
                {
                    DirectMessage currentMessage = await client.ApiClient.GetDirectMessageAsync(info.Position.Value,
                        channel.ChatCode, options: options);
                    builder.Add(RestMessage.Create(client, channel, channel.Recipient, currentMessage));
                }

                foreach (DirectMessage model in models) builder.Add(RestMessage.Create(client, channel, channel.Recipient, model));

                // Append the reference message after query results
                if (includeReferenceMessage && info.Position.HasValue && dir == Direction.Before)
                {
                    DirectMessage currentMessage = await client.ApiClient.GetDirectMessageAsync(info.Position.Value,
                        channel.ChatCode, options: options);
                    builder.Add(RestMessage.Create(client, channel, channel.Recipient, currentMessage));
                }

                return builder.ToImmutable();
            },
            (info, lastPage) =>
            {
                if (lastPage.Count != KookConfig.MaxMessagesPerBatch) return false;

                if (dir == Direction.Before)
#if NET6_0_OR_GREATER
                    info.Position = lastPage.MinBy(x => x.Timestamp)?.Id;
#else
                    info.Position = lastPage.OrderBy(x => x.Timestamp).FirstOrDefault()?.Id;
#endif
                else

#if NET6_0_OR_GREATER
                    info.Position = lastPage.MaxBy(x => x.Timestamp)?.Id;
#else
                    info.Position = lastPage.OrderByDescending(x => x.Timestamp).FirstOrDefault()?.Id;
#endif
                return true;
            },
            referenceMessageId,
            limit
        );
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendDirectMessageAsync(IDMChannel channel,
        BaseKookClient client, MessageType messageType, string content, RequestOptions options, IQuote quote = null)
    {
        CreateDirectMessageParams args = new(messageType, channel.Recipient.Id, content) { QuotedMessageId = quote?.QuotedMessageId };
        CreateDirectMessageResponse model = await client.ApiClient.CreateDirectMessageAsync(args, options).ConfigureAwait(false);
        return new Cacheable<IUserMessage, Guid>(null, model.MessageId, false,
            async () => await channel.GetMessageAsync(model.MessageId) as IUserMessage);
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendDirectCardsAsync(IDMChannel channel,
        BaseKookClient client, IEnumerable<ICard> cards, RequestOptions options, IQuote quote = null)
    {
        string json = MessageHelper.SerializeCards(cards);
        return await SendDirectMessageAsync(channel, client, MessageType.Card, json, options, quote);
    }

    public static Task<Cacheable<IUserMessage, Guid>> SendDirectCardAsync(IDMChannel channel,
        BaseKookClient client, ICard card, RequestOptions options, IQuote quote = null)
        => SendDirectCardsAsync(channel, client, new[] { card }, options, quote);

    public static async Task<Cacheable<IUserMessage, Guid>> SendDirectFileAsync(IDMChannel channel,
        BaseKookClient client, string path, string fileName, AttachmentType type, RequestOptions options,
        IQuote quote = null)
    {
        using FileAttachment file = new(path, fileName, type);
        return await SendDirectFileAsync(channel, client, file, options, quote);
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendDirectFileAsync(IDMChannel channel,
        BaseKookClient client, Stream stream, string fileName, AttachmentType type, RequestOptions options,
        IQuote quote = null)
    {
        using FileAttachment file = new(stream, fileName, type);
        return await SendDirectFileAsync(channel, client, file, options, quote);
    }

    public static async Task<Cacheable<IUserMessage, Guid>> SendDirectFileAsync(IDMChannel channel,
        BaseKookClient client, FileAttachment attachment, RequestOptions options, IQuote quote = null)
    {
        switch (attachment.Mode)
        {
            case CreateAttachmentMode.FilePath:
            case CreateAttachmentMode.Stream:
                CreateAssetResponse assetResponse = await client.ApiClient
                    .CreateAssetAsync(new CreateAssetParams { File = attachment.Stream, FileName = attachment.FileName }, options);
                attachment.Uri = new Uri(assetResponse.Url);
                break;
            case CreateAttachmentMode.AssetUri:
                if (!UrlValidation.ValidateKookAssetUrl(attachment.Uri.OriginalString))
                    throw new ArgumentException("The uri cannot be blank.", nameof(attachment.Uri));

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attachment.Mode), attachment.Mode, "Unknown attachment mode");
        }

        return attachment.Type switch
        {
            AttachmentType.File => await SendDirectMessageAsync(channel, client, MessageType.File,
                attachment.Uri.OriginalString, options, quote),
            AttachmentType.Image => await SendDirectMessageAsync(channel, client, MessageType.Image,
                attachment.Uri.OriginalString, options, quote),
            AttachmentType.Video => await SendDirectMessageAsync(channel, client, MessageType.Video,
                attachment.Uri.OriginalString, options, quote),
            AttachmentType.Audio => await SendDirectCardAsync(channel, client,
                new CardBuilder().WithSize(CardSize.Large)
                    .WithTheme(CardTheme.None)
                    .AddModule<AudioModuleBuilder>(x => x
                        .WithSource(attachment.Uri.OriginalString)
                        .WithTitle(attachment.FileName))
                    .Build(), options, quote),
            _ => throw new ArgumentOutOfRangeException(nameof(attachment.Type), attachment.Type,
                "Unknown attachment type")
        };
    }

    public static async Task ModifyDirectMessageAsync(IDMChannel channel, Guid messageId,
        Action<MessageProperties> func,
        BaseKookClient client, RequestOptions options)
        => await MessageHelper.ModifyDirectAsync(messageId, client, func, options).ConfigureAwait(false);

    #endregion

    #region Permission Overwrites

    public static async Task<UserPermissionOverwrite> AddPermissionOverwriteAsync(IGuildChannel channel,
        BaseKookClient client,
        IUser user, RequestOptions options)
    {
        CreateOrRemoveChannelPermissionOverwriteParams args = new(channel.Id,
            PermissionOverwriteTargetType.User,
            user.Id);
        CreateOrModifyChannelPermissionOverwriteResponse resp = await client.ApiClient.CreateChannelPermissionOverwriteAsync(args, options)
            .ConfigureAwait(false);
        return new UserPermissionOverwrite(user, new OverwritePermissions(resp.Allow, resp.Deny));
    }

    public static async Task<RolePermissionOverwrite> AddPermissionOverwriteAsync(IGuildChannel channel,
        BaseKookClient client,
        IRole role, RequestOptions options)
    {
        CreateOrRemoveChannelPermissionOverwriteParams args = new(channel.Id, PermissionOverwriteTargetType.Role, role.Id);
        CreateOrModifyChannelPermissionOverwriteResponse resp = await client.ApiClient.CreateChannelPermissionOverwriteAsync(args, options)
            .ConfigureAwait(false);
        return new RolePermissionOverwrite(role.Id, new OverwritePermissions(resp.Allow, resp.Deny));
    }

    public static async Task RemovePermissionOverwriteAsync(IGuildChannel channel, BaseKookClient client,
        IUser user, RequestOptions options)
    {
        CreateOrRemoveChannelPermissionOverwriteParams args = new(channel.Id,
            PermissionOverwriteTargetType.User,
            user.Id);
        await client.ApiClient.RemoveChannelPermissionOverwriteAsync(args, options).ConfigureAwait(false);
    }

    public static async Task RemovePermissionOverwriteAsync(IGuildChannel channel, BaseKookClient client,
        IRole role, RequestOptions options)
    {
        CreateOrRemoveChannelPermissionOverwriteParams args = new(channel.Id,
            PermissionOverwriteTargetType.Role,
            role.Id);
        await client.ApiClient.RemoveChannelPermissionOverwriteAsync(args, options).ConfigureAwait(false);
    }

    public static async Task<UserPermissionOverwrite> ModifyPermissionOverwriteAsync(IGuildChannel channel,
        BaseKookClient client,
        IGuildUser user, Func<OverwritePermissions, OverwritePermissions> func, RequestOptions options)
    {
        OverwritePermissions? perms = channel.UserPermissionOverwrites.SingleOrDefault(x => x.Target.Id == user.Id)?.Permissions;
        if (!perms.HasValue)
            throw new ArgumentNullException(nameof(user),
                "The user does not have any permission overwrites on this channel.");

        perms = func(perms.Value);
        ModifyChannelPermissionOverwriteParams args = new(channel.Id, PermissionOverwriteTargetType.User,
            user.Id,
            perms.Value.AllowValue, perms.Value.DenyValue);
        CreateOrModifyChannelPermissionOverwriteResponse resp = await client.ApiClient.ModifyChannelPermissionOverwriteAsync(args, options)
            .ConfigureAwait(false);
        return new UserPermissionOverwrite(user, new OverwritePermissions(resp.Allow, resp.Deny));
    }

    public static async Task<RolePermissionOverwrite> ModifyPermissionOverwriteAsync(IGuildChannel channel,
        BaseKookClient client,
        IRole role, Func<OverwritePermissions, OverwritePermissions> func, RequestOptions options)
    {
        OverwritePermissions? perms = channel.RolePermissionOverwrites.SingleOrDefault(x => x.Target == role.Id)?.Permissions;
        if (!perms.HasValue)
            throw new ArgumentNullException(nameof(role),
                "The role does not have any permission overwrites on this channel.");

        perms = func(perms.Value);
        ModifyChannelPermissionOverwriteParams args = new(channel.Id, PermissionOverwriteTargetType.Role,
            role.Id,
            perms.Value.AllowValue, perms.Value.DenyValue);
        CreateOrModifyChannelPermissionOverwriteResponse resp = await client.ApiClient.ModifyChannelPermissionOverwriteAsync(args, options)
            .ConfigureAwait(false);
        return new RolePermissionOverwrite(role.Id, new OverwritePermissions(resp.Allow, resp.Deny));
    }

    #endregion

    #region Invites

    public static async Task<IReadOnlyCollection<RestInvite>> GetInvitesAsync(IGuildChannel channel,
        BaseKookClient client,
        RequestOptions options)
    {
        IEnumerable<Invite> models = await client.ApiClient.GetGuildInvitesAsync(channel.GuildId, channel.Id, options: options)
            .FlattenAsync().ConfigureAwait(false);
        return models.Select(x => RestInvite.Create(client, channel.Guild, channel, x)).ToImmutableArray();
    }

    /// <exception cref="ArgumentException">
    /// <paramref name="channel.Id"/> may not be equal to zero.
    /// <paramref name="maxAge"/> and <paramref name="maxUses"/> must be greater than zero.
    /// <paramref name="maxAge"/> must be lesser than 604800.
    /// </exception>
    public static async Task<RestInvite> CreateInviteAsync(IGuildChannel channel, BaseKookClient client,
        int? maxAge, int? maxUses, RequestOptions options)
    {
        CreateGuildInviteParams args = new()
        {
            GuildId = channel.GuildId, ChannelId = channel.Id, MaxAge = (InviteMaxAge)(maxAge ?? 0), MaxUses = (InviteMaxUses)(maxUses ?? -1)
        };
        CreateGuildInviteResponse model = await client.ApiClient.CreateGuildInviteAsync(args, options).ConfigureAwait(false);
        IEnumerable<Invite> invites = await client.ApiClient.GetGuildInvitesAsync(channel.GuildId, channel.Id, options: options)
            .FlattenAsync().ConfigureAwait(false);
        return RestInvite.Create(client, channel.Guild, channel, invites.SingleOrDefault(x => x.Url == model.Url));
    }

    public static Task<RestInvite> CreateInviteAsync(IGuildChannel channel, BaseKookClient client,
        InviteMaxAge maxAge, InviteMaxUses maxUses, RequestOptions options) =>
        CreateInviteAsync(channel, client, (int?)maxAge, (int?)maxUses, options);

    #endregion

    #region Categories

    public static async Task<ICategoryChannel> GetCategoryAsync(INestedChannel channel, BaseKookClient client,
        RequestOptions options)
    {
        // if no category id specified, return null
        if (!channel.CategoryId.HasValue) return null;

        // CategoryId will contain a value here
        Channel model = await client.ApiClient.GetGuildChannelAsync(channel.CategoryId.Value, options)
            .ConfigureAwait(false);
        return RestChannel.Create(client, model) as ICategoryChannel;
    }

    /// <exception cref="InvalidOperationException">This channel does not have a parent channel.</exception>
    public static async Task SyncPermissionsAsync(INestedChannel channel, BaseKookClient client,
        RequestOptions options)
    {
        var category = await GetCategoryAsync(channel, client, options).ConfigureAwait(false);
        if (category == null)
            throw new InvalidOperationException("This channel does not have a parent channel.");

        var args = new SyncChannelPermissionsParams(channel.Id);
        await client.ApiClient.SyncChannelPermissionsAsync(args, options).ConfigureAwait(false);
    }

    #endregion

    #region Users

    /// <exception cref="InvalidOperationException">Resolving permissions requires the parent guild to be downloaded.</exception>
    public static async Task<RestGuildUser> GetUserAsync(IGuildChannel channel, IGuild guild, BaseKookClient client,
        ulong id, RequestOptions options)
    {
        GuildMember model = await client.ApiClient.GetGuildMemberAsync(channel.GuildId, id, options).ConfigureAwait(false);
        if (model == null) return null;

        RestGuildUser user = RestGuildUser.Create(client, guild, model);
        if (!user.GetPermissions(channel).ViewChannel) return null;

        return user;
    }

    /// <exception cref="InvalidOperationException">Resolving permissions requires the parent guild to be downloaded.</exception>
    public static IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> GetUsersAsync(IGuildChannel channel,
        IGuild guild, BaseKookClient client,
        int limit, int fromPage, RequestOptions options) =>
        client.ApiClient.GetGuildMembersAsync(guild.Id, limit: limit, fromPage: fromPage, options: options)
            .Select(x => x.Select(y => RestGuildUser.Create(client, guild, y))
                .Where(y => y.GetPermissions(channel).ViewChannel)
                .ToImmutableArray() as IReadOnlyCollection<RestGuildUser>);

    public static async Task<IReadOnlyCollection<RestGuildUser>> GetConnectedUsersAsync(IVoiceChannel channel,
        IGuild guild, BaseKookClient client, RequestOptions options)
    {
        IReadOnlyCollection<User> model = await client.ApiClient.GetConnectedUsersAsync(channel.Id, options);
        return model?.Select(x => RestGuildUser.Create(client, guild, x)).ToImmutableArray();
    }

    #endregion
}
