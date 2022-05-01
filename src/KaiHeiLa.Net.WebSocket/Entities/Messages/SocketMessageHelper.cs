using KaiHeiLa.API.Gateway;
using KaiHeiLa.Rest;

namespace KaiHeiLa.WebSocket;

internal static class SocketMessageHelper
{
    public static MessageSource GetSource(API.Message msg)
    {
        if (msg.Author.Bot ?? false)
            return MessageSource.Bot;
        if (msg.Type == MessageType.System)
            return MessageSource.System;
        return MessageSource.User;
    }
    public static MessageSource GetSource(API.DirectMessage msg, SocketUser user)
    {
        if (user.IsBot ?? false)
            return MessageSource.Bot;
        if (msg.Type == MessageType.System)
            return MessageSource.System;
        return MessageSource.User;
    }
    public static MessageSource GetSource(GatewayGroupMessageExtraData msg)
    {
        if (msg.Author.Bot ?? false)
            return MessageSource.Bot;
        if (msg.Type == MessageType.System)
            return MessageSource.System;
        return MessageSource.User;
    }
    public static MessageSource GetSource(GatewayPersonMessageExtraData msg)
    {
        if (msg.Author.Bot ?? false)
            return MessageSource.Bot;
        if (msg.Type == MessageType.System)
            return MessageSource.System;
        return MessageSource.User;
    }

    public static async Task ReloadAsync(SocketMessage msg, KaiHeiLaSocketClient client, RequestOptions options)
    {
        switch (msg.Channel)
        {
            case SocketTextChannel channel:
                msg.Update(client.State, await client.ApiClient.GetMessageAsync(msg.Id, options: options).ConfigureAwait(false));
                break;
            case SocketDMChannel channel:
                msg.Update(client.State, await client.ApiClient.GetDirectMessageAsync(msg.Id, chatCode: channel.ChatCode, options: options).ConfigureAwait(false));
                break;
            default:
                throw new InvalidOperationException("Cannot reload a message from a non-SocketTextChannel or non-SocketTextChannel.");
        }
    }
}