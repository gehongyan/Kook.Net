using Kook.API;
using Kook.API.Gateway;
using Kook.Rest;

namespace Kook.WebSocket;

internal static class SocketMessageHelper
{
    public static MessageSource GetSource(GatewayGroupMessageExtraData msg)
    {
        if (msg.Author.IsSystemUser ?? msg.Author.Id == KookConfig.SystemMessageAuthorID)
            return MessageSource.System;
        if (msg.Author.Bot is true)
            return MessageSource.Bot;
        return MessageSource.User;
    }

    public static MessageSource GetSource(GatewayPersonMessageExtraData msg)
    {
        if (msg.Author.IsSystemUser ?? msg.Author.Id == KookConfig.SystemMessageAuthorID)
            return MessageSource.System;
        if (msg.Author.Bot is true)
            return MessageSource.Bot;
        return MessageSource.User;
    }

    public static async Task UpdateAsync(SocketMessage msg, KookSocketClient client, RequestOptions? options)
    {
        switch (msg.Channel)
        {
            case SocketTextChannel:
            {
                Message model = await client.ApiClient.GetMessageAsync(msg.Id, options).ConfigureAwait(false);
                msg.Update(client.State, model);
            }
                break;
            case SocketDMChannel channel:
            {
                DirectMessage model = await client.ApiClient
                    .GetDirectMessageAsync(msg.Id, channel.ChatCode, options)
                    .ConfigureAwait(false);
                msg.Update(client.State, model);
            }
                break;
            default:
                throw new InvalidOperationException("Cannot reload a message from a channel type that is not a text channel or DM channel.");
        }
    }
}
