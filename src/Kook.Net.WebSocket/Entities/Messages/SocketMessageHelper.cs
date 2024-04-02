using Kook.API.Gateway;

namespace Kook.WebSocket;

internal static class SocketMessageHelper
{
    public static MessageSource GetSource(API.Message msg)
    {
        if (msg.Author.Bot ?? false) return MessageSource.Bot;

        if (msg.Author.IsSystemUser ?? msg.Author.Id == KookConfig.SystemMessageAuthorID)
            return MessageSource.System;

        return MessageSource.User;
    }

    public static MessageSource GetSource(API.DirectMessage msg, SocketUser user)
    {
        if (user.IsBot ?? false) return MessageSource.Bot;

        if (user.IsSystemUser ?? msg.AuthorId == KookConfig.SystemMessageAuthorID)
            return MessageSource.System;

        return MessageSource.User;
    }

    public static MessageSource GetSource(GatewayGroupMessageExtraData msg)
    {
        if (msg.Author.Bot ?? false) return MessageSource.Bot;

        if (msg.Author.IsSystemUser ?? msg.Author.Id == KookConfig.SystemMessageAuthorID)
            return MessageSource.System;

        return MessageSource.User;
    }

    public static MessageSource GetSource(GatewayPersonMessageExtraData msg)
    {
        if (msg.Author.Bot ?? false) return MessageSource.Bot;

        if (msg.Author.IsSystemUser ?? msg.Author.Id == KookConfig.SystemMessageAuthorID)
            return MessageSource.System;

        return MessageSource.User;
    }

    public static async Task UpdateAsync(SocketMessage msg, KookSocketClient client, RequestOptions options)
    {
        switch (msg.Channel)
        {
            case SocketTextChannel channel:
                msg.Update(client.State, await client.ApiClient.GetMessageAsync(msg.Id, options).ConfigureAwait(false));
                break;
            case SocketDMChannel channel:
                msg.Update(client.State,
                    await client.ApiClient.GetDirectMessageAsync(msg.Id, channel.ChatCode, options: options).ConfigureAwait(false));
                break;
            default:
                throw new InvalidOperationException("Cannot reload a message from a non-SocketTextChannel or non-SocketTextChannel.");
        }
    }
}
