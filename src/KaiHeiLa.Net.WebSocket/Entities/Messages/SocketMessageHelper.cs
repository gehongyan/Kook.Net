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

}