using Kook.Rest;
using System.Collections.Immutable;
using Kook.API;

namespace Kook.WebSocket;

internal static class SocketChannelHelper
{
    public static IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ISocketMessageChannel channel, KookSocketClient kook,
        MessageCache messages,
        Guid? referenceMessageId, Direction dir, int limit, CacheMode mode, RequestOptions options)
    {
        if (dir == Direction.After && referenceMessageId == null) return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();

        IReadOnlyCollection<SocketMessage> cachedMessages = GetCachedMessages(channel, kook, messages, referenceMessageId, dir, limit);
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> result = ImmutableArray.Create(cachedMessages)
            .ToAsyncEnumerable<IReadOnlyCollection<IMessage>>();

        if (dir == Direction.Before)
        {
            limit -= cachedMessages.Count;
            if (mode == CacheMode.CacheOnly || limit <= 0) return result;

            //Download remaining messages
            Guid? minId = cachedMessages.Count > 0
#if NET6_0_OR_GREATER
                ? cachedMessages.MinBy(x => x.Timestamp)?.Id
#else
                ? cachedMessages.OrderBy(x => x.Timestamp).FirstOrDefault()?.Id
#endif
                : referenceMessageId;
            IAsyncEnumerable<IReadOnlyCollection<RestMessage>> downloadedMessages =
                ChannelHelper.GetMessagesAsync(channel, kook, minId, dir, limit, true, options);
            if (cachedMessages.Count != 0)
                return result.Concat(downloadedMessages);
            else
                return downloadedMessages;
        }
        else if (dir == Direction.After)
        {
            if (mode == CacheMode.CacheOnly)
                return result;

            bool ignoreCache = false;

            // We can find two cases:
            // 1. referenceMessageId is not null and corresponds to a message that is not in the cache,
            // so we have to make a request and ignore the cache
            if (referenceMessageId.HasValue && messages?.Get(referenceMessageId.Value) == null)
            {
                ignoreCache = true;
            }
            // 2. referenceMessageId is null or already in the cache, so we start from the cache
            else if (cachedMessages.Count > 0)
            {
                referenceMessageId = cachedMessages
#if NET6_0_OR_GREATER
                    .MaxBy(x => x.Timestamp)?.Id;
#else
                    .OrderByDescending(x => x.Timestamp).FirstOrDefault()?.Id;
#endif
                limit -= cachedMessages.Count;

                if (limit <= 0)
                    return result;
            }

            //Download remaining messages
            var downloadedMessages = ChannelHelper.GetMessagesAsync(
                channel, kook, referenceMessageId, dir, limit, true, options);
            if (!ignoreCache && cachedMessages.Count != 0)
                return result.Concat(downloadedMessages);
            else
                return downloadedMessages;
        }
        else //Direction.Around
        {
            if (mode == CacheMode.CacheOnly || limit <= cachedMessages.Count) return result;

            //Cache isn't useful here since Kook will send them anyways
            return ChannelHelper.GetMessagesAsync(channel, kook, referenceMessageId, dir, limit, true, options);
        }
    }

    public static IReadOnlyCollection<SocketMessage> GetCachedMessages(ISocketMessageChannel channel, KookSocketClient kook, MessageCache messages,
        Guid? referenceMessageId, Direction dir, int limit)
    {
        if (messages != null) //Cache enabled
            return messages.GetMany(referenceMessageId, dir, limit);
        else
            return ImmutableArray.Create<SocketMessage>();
    }

    /// <exception cref="NotSupportedException">Unexpected <see cref="ISocketMessageChannel"/> type.</exception>
    public static void AddMessage(ISocketMessageChannel channel, KookSocketClient kook,
        SocketMessage msg)
    {
        switch (channel)
        {
            case SocketDMChannel dmChannel:
                dmChannel.AddMessage(msg);
                break;
            case SocketTextChannel textChannel:
                textChannel.AddMessage(msg);
                break;
            default:
                throw new NotSupportedException($"Unexpected {nameof(ISocketMessageChannel)} type.");
        }
    }

    /// <exception cref="NotSupportedException">Unexpected <see cref="ISocketMessageChannel"/> type.</exception>
    public static SocketMessage RemoveMessage(ISocketMessageChannel channel, KookSocketClient kook, Guid id) =>
        channel switch
        {
            SocketDMChannel dmChannel => dmChannel.RemoveMessage(id),
            SocketTextChannel textChannel => textChannel.RemoveMessage(id),
            _ => throw new NotSupportedException($"Unexpected {nameof(ISocketMessageChannel)} type.")
        };

    public static async Task UpdateAsync(SocketGuildChannel channel, RequestOptions options)
    {
        Channel model = await channel.Kook.ApiClient.GetGuildChannelAsync(channel.Id, options).ConfigureAwait(false);
        channel.Update(channel.Kook.State, model);
    }

    public static async Task UpdateAsync(SocketDMChannel channel, RequestOptions options)
    {
        UserChat model = await channel.Kook.ApiClient.GetUserChatAsync(channel.Id, options).ConfigureAwait(false);
        channel.Update(channel.Kook.State, model);
    }

    public static async Task<IReadOnlyCollection<SocketGuildUser>> GetConnectedUsersAsync(SocketVoiceChannel channel,
        SocketGuild guild, KookSocketClient kook, RequestOptions options)
    {
        IReadOnlyCollection<User> users = await channel.Kook.ApiClient.GetConnectedUsersAsync(channel.Id, options).ConfigureAwait(false);
        return users.Select(x => SocketGuildUser.Create(guild, kook.State, x)).ToImmutableArray();
    }
}
