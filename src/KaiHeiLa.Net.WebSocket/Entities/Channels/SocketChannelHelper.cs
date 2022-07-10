using System.Collections.Immutable;
using KaiHeiLa.Rest;

namespace KaiHeiLa.WebSocket;

internal static class SocketChannelHelper
{
    public static IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ISocketMessageChannel channel, KaiHeiLaSocketClient kaiHeiLa, MessageCache messages,
        Guid? referenceMessageId, Direction dir, int limit, CacheMode mode, RequestOptions options)
    {
        if (dir == Direction.After && referenceMessageId == null)
            return AsyncEnumerable.Empty<IReadOnlyCollection<IMessage>>();

        var cachedMessages = GetCachedMessages(channel, kaiHeiLa, messages, referenceMessageId, dir, limit);
        var result = ImmutableArray.Create(cachedMessages).ToAsyncEnumerable<IReadOnlyCollection<IMessage>>();

        if (dir == Direction.Before)
        {
            limit -= cachedMessages.Count;
            if (mode == CacheMode.CacheOnly || limit <= 0)
                return result;

            //Download remaining messages
            Guid? minId = cachedMessages.Count > 0 ? cachedMessages.MinBy(x => x.Timestamp)?.Id : referenceMessageId;
            var downloadedMessages = ChannelHelper.GetMessagesAsync(channel, kaiHeiLa, minId, dir, limit, true, options);
            if (cachedMessages.Count != 0)
                return result.Concat(downloadedMessages);
            else
                return downloadedMessages;
        }
        else if (dir == Direction.After)
        {
            limit -= cachedMessages.Count;
            if (mode == CacheMode.CacheOnly || limit <= 0)
                return result;

            //Download remaining messages
            Guid? maxId = cachedMessages.Count > 0 ? cachedMessages.MaxBy(x => x.Timestamp)?.Id : referenceMessageId;
            var downloadedMessages = ChannelHelper.GetMessagesAsync(channel, kaiHeiLa, maxId, dir, limit, true, options);
            if (cachedMessages.Count != 0)
                return result.Concat(downloadedMessages);
            else
                return downloadedMessages;
        }
        else //Direction.Around
        {
            if (mode == CacheMode.CacheOnly || limit <= cachedMessages.Count)
                return result;

            //Cache isn't useful here since KaiHeiLa will send them anyways
            return ChannelHelper.GetMessagesAsync(channel, kaiHeiLa, referenceMessageId, dir, limit, true, options);
        }
    }
    
    public static IReadOnlyCollection<SocketMessage> GetCachedMessages(ISocketMessageChannel channel, KaiHeiLaSocketClient kaiHeiLa, MessageCache messages,
        Guid? referenceMessageId, Direction dir, int limit)
    {
        if (messages != null) //Cache enabled
            return messages.GetMany(referenceMessageId, dir, limit);
        else
            return ImmutableArray.Create<SocketMessage>();
    }
    /// <exception cref="NotSupportedException">Unexpected <see cref="ISocketMessageChannel"/> type.</exception>
    public static void AddMessage(ISocketMessageChannel channel, KaiHeiLaSocketClient kaiHeiLa,
        SocketMessage msg)
    {
        switch (channel)
        {
            case SocketDMChannel dmChannel: dmChannel.AddMessage(msg); break;
            case SocketTextChannel textChannel: textChannel.AddMessage(msg); break;
            default: throw new NotSupportedException($"Unexpected {nameof(ISocketMessageChannel)} type.");
        }
    }   
    /// <exception cref="NotSupportedException">Unexpected <see cref="ISocketMessageChannel"/> type.</exception>
    public static SocketMessage RemoveMessage(ISocketMessageChannel channel, KaiHeiLaSocketClient kaiHeiLa,
        Guid id)
    {
        return channel switch
        {
            SocketDMChannel dmChannel => dmChannel.RemoveMessage(id),
            SocketTextChannel textChannel => textChannel.RemoveMessage(id),
            _ => throw new NotSupportedException($"Unexpected {nameof(ISocketMessageChannel)} type."),
        };
    }
    
    public static async Task UpdateAsync(SocketGuildChannel channel, RequestOptions options = null)
    {
        var model = await channel.KaiHeiLa.ApiClient.GetGuildChannelAsync(channel.Id, options).ConfigureAwait(false);
        channel.Update(channel.KaiHeiLa.State, model);
    }
    
    public static async Task UpdateAsync(SocketDMChannel channel, RequestOptions options = null)
    {
        var model = await channel.KaiHeiLa.ApiClient.GetUserChatAsync(channel.Id, options).ConfigureAwait(false);
        channel.Update(channel.KaiHeiLa.State, model);
    }
}