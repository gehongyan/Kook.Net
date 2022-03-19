using System.Collections.Immutable;
using KaiHeiLa.API;
using KaiHeiLa.API.Rest;

namespace KaiHeiLa.Rest;

internal static class ChannelHelper
{
    #region General

    public static async Task DeleteGuildChannelAsync(IGuildChannel channel, BaseKaiHeiLaClient client,
        RequestOptions options)
    {
        await client.ApiClient.DeleteGuildChannelAsync(channel.Id, options).ConfigureAwait(false);
    }
    
    public static async Task DeleteDMChannelAsync(IDMChannel channel, BaseKaiHeiLaClient client,
        RequestOptions options)
    {
        await client.ApiClient.DeleteUserChatAsync(channel.Recipient.Id, options).ConfigureAwait(false);
    }

    #endregion

    #region Messages
    
    public static async Task<RestMessage> GetMessageAsync(IMessageChannel channel, BaseKaiHeiLaClient client,
        Guid id, RequestOptions options)
    {
        var guildId = (channel as IGuildChannel)?.GuildId;
        var guild = guildId != null ? await (client as IKaiHeiLaClient).GetGuildAsync(guildId.Value, CacheMode.CacheOnly).ConfigureAwait(false) : null;
        var model = await client.ApiClient.GetMessageAsync(id, options).ConfigureAwait(false);
        if (model == null)
            return null;
        var author = MessageHelper.GetAuthor(client, guild, model.Author);
        return RestMessage.Create(client, channel, author, model);
    }
    
    public static IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessageChannel channel, BaseKaiHeiLaClient client,
        Guid? referenceMessageId, Direction dir, int limit, bool includeReferenceMessage, RequestOptions options)
    {
        var guildId = (channel as IGuildChannel)?.GuildId;
        var guild = guildId != null ? (client as IKaiHeiLaClient).GetGuildAsync(guildId.Value, CacheMode.CacheOnly).Result : null;

        if (dir == Direction.Around) //  && limit > KaiHeiLaConfig.MaxMessagesPerBatch // Around mode returns error messages from endpoint
        {
            int around = limit / 2;
            if (referenceMessageId.HasValue)
            {
                var messages = GetMessagesAsync(channel, client, referenceMessageId, Direction.Before, around, includeReferenceMessage, options);
                messages = messages.Concat(GetMessagesAsync(channel, client, referenceMessageId, Direction.After, around, false, options));
                return messages;
            }
            else // Shouldn't happen since there's no public overload for Guid? and Direction
                return GetMessagesAsync(channel, client, null, Direction.Before, around + 1, includeReferenceMessage, options);
        }

        return new PagedAsyncEnumerable<RestMessage>(
            KaiHeiLaConfig.MaxMessagesPerBatch,
            async (info, ct) =>
            {
                var models = await client.ApiClient.QueryMessagesAsync(channel.Id, referenceMessageId: info.Position, dir: dir, count: limit, options: options).ConfigureAwait(false);
                var builder = ImmutableArray.CreateBuilder<RestMessage>();
                // Insert the reference message before query results
                if (includeReferenceMessage && info.Position.HasValue && dir == Direction.After)
                {
                    Message currentMessage = await client.ApiClient.GetMessageAsync(info.Position.Value, options);
                    var currentMessageAuthor = MessageHelper.GetAuthor(client, guild, currentMessage.Author);
                    builder.Add(RestMessage.Create(client, channel, currentMessageAuthor, currentMessage));
                }
                foreach (var model in models)
                {
                    var author = MessageHelper.GetAuthor(client, guild, model.Author);
                    builder.Add(RestMessage.Create(client, channel, author, model));
                }
                // Append the reference message after query results
                if (includeReferenceMessage && info.Position.HasValue && dir == Direction.Before)
                {
                    Message currentMessage = await client.ApiClient.GetMessageAsync(info.Position.Value, options);
                    var currentMessageAuthor = MessageHelper.GetAuthor(client, guild, currentMessage.Author);
                    builder.Add(RestMessage.Create(client, channel, currentMessageAuthor, currentMessage));
                }
                return builder.ToImmutable();
            },
            nextPage: (info, lastPage) =>
            {
                if (lastPage.Count != KaiHeiLaConfig.MaxMessagesPerBatch)
                    return false;
                if (dir == Direction.Before)
                    info.Position = lastPage.MinBy(x => x.Timestamp)?.Id;
                else
                    info.Position = lastPage.MaxBy(x => x.Timestamp)?.Id;
                return true;
            },
            start: referenceMessageId,
            count: limit
        );
    }
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendMessageAsync(IMessageChannel channel, 
        BaseKaiHeiLaClient client, MessageType messageType, string content, RequestOptions options, IQuote quote = null, IUser ephemeralUser = null)
    {
        CreateMessageParams args = new(messageType, channel.Id, content)
        {
            QuotedMessageId = quote?.QuotedMessageId,
            EphemeralUserId = ephemeralUser?.Id
        };
        CreateMessageResponse model = await client.ApiClient.CreateMessageAsync(args, options).ConfigureAwait(false);
        return (model.MessageId, model.MessageTimestamp);
    }

    public static Task DeleteMessageAsync(IMessageChannel channel, Guid messageId, BaseKaiHeiLaClient client,
        RequestOptions options)
        => MessageHelper.DeleteAsync(messageId, client, options);

    public static Task DeleteDirectMessageAsync(IMessageChannel channel, Guid messageId, BaseKaiHeiLaClient client,
        RequestOptions options)
        => MessageHelper.DeleteDirectAsync(messageId, client, options);

    
    public static async Task ModifyMessageAsync(IMessageChannel channel, Guid messageId, Action<MessageProperties> func,
        BaseKaiHeiLaClient client, RequestOptions options)
        => await MessageHelper.ModifyAsync(messageId, client, func, options).ConfigureAwait(false);
    
    #endregion

    #region Direct Messages

    public static async Task<RestMessage> GetDirectMessageAsync(IDMChannel channel, BaseKaiHeiLaClient client,
        Guid id, RequestOptions options)
    {
        var model = await client.ApiClient.GetDirectMessageAsync(id, chatCode: channel.Id, options: options).ConfigureAwait(false);
        if (model == null)
            return null;
        // User userModel = await client.ApiClient.GetUserAsync(model.AuthorId, options);
        // var author = MessageHelper.GetAuthor(client, null, userModel);
        return RestMessage.Create(client, channel, channel.Recipient, model);
    }
    
    public static IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetDirectMessagesAsync(IDMChannel channel, BaseKaiHeiLaClient client,
        Guid? referenceMessageId, Direction dir, int limit, bool includeReferenceMessage, RequestOptions options)
    {
        if (dir == Direction.Around) //  && limit > KaiHeiLaConfig.MaxMessagesPerBatch // Around mode returns error messages from endpoint
        {
            int around = limit / 2;
            if (referenceMessageId.HasValue)
            {
                var messages = GetDirectMessagesAsync(channel, client, referenceMessageId, Direction.Before, around, includeReferenceMessage, options);
                messages = messages.Concat(GetDirectMessagesAsync(channel, client, referenceMessageId, Direction.After, around, false, options));
                return messages;
            }
            else // Shouldn't happen since there's no public overload for Guid? and Direction
                return GetDirectMessagesAsync(channel, client, null, Direction.Before, around + 1, includeReferenceMessage, options);
        }

        return new PagedAsyncEnumerable<RestMessage>(
            KaiHeiLaConfig.MaxMessagesPerBatch,
            async (info, ct) =>
            {
                var models = await client.ApiClient.QueryDirectMessagesAsync(channel.ChatCode, referenceMessageId: info.Position, dir: dir, count: limit, options: options).ConfigureAwait(false);
                var builder = ImmutableArray.CreateBuilder<RestMessage>();
                // Insert the reference message before query results
                if (includeReferenceMessage && info.Position.HasValue && dir == Direction.After)
                {
                    DirectMessage currentMessage = await client.ApiClient.GetDirectMessageAsync(info.Position.Value, chatCode: channel.ChatCode, options: options);
                    builder.Add(RestMessage.Create(client, channel, channel.Recipient, currentMessage));
                }
                foreach (var model in models)
                {
                    builder.Add(RestMessage.Create(client, channel, channel.Recipient, model));
                }
                // Append the reference message after query results
                if (includeReferenceMessage && info.Position.HasValue && dir == Direction.Before)
                {
                    DirectMessage currentMessage = await client.ApiClient.GetDirectMessageAsync(info.Position.Value, chatCode: channel.ChatCode, options: options);
                    builder.Add(RestMessage.Create(client, channel, channel.Recipient, currentMessage));
                }
                return builder.ToImmutable();
            },
            nextPage: (info, lastPage) =>
            {
                if (lastPage.Count != KaiHeiLaConfig.MaxMessagesPerBatch)
                    return false;
                if (dir == Direction.Before)
                    info.Position = lastPage.MinBy(x => x.Timestamp)?.Id;
                else
                    info.Position = lastPage.MaxBy(x => x.Timestamp)?.Id;
                return true;
            },
            start: referenceMessageId,
            count: limit
        );
    }
    public static async Task<(Guid MessageId, DateTimeOffset MessageTimestamp)> SendDirectMessageAsync(IDMChannel channel, 
        BaseKaiHeiLaClient client, MessageType messageType, string content, RequestOptions options, IQuote quote = null)
    {
        CreateDirectMessageParams args = new(messageType, channel.Recipient.Id, content)
        {
            QuotedMessageId = quote?.QuotedMessageId,
        };
        CreateDirectMessageResponse model = await client.ApiClient.CreateDirectMessageAsync(args, options).ConfigureAwait(false);
        return (model.MessageId, model.MessageTimestamp);
    }

    public static async Task ModifyDirectMessageAsync(IDMChannel channel, Guid messageId, Action<MessageProperties> func,
        BaseKaiHeiLaClient client, RequestOptions options)
        => await MessageHelper.ModifyDirectAsync(messageId, client, func, options).ConfigureAwait(false);
    #endregion
}