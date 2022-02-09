using Model = KaiHeiLa.API.Gateway.GatewayMessageExtraData;

namespace KaiHeiLa.WebSocket;

internal static class MessageHelper
{
    public static MessageSource GetSource(Model msg)
    {
        if (msg.Author.Bot)
            return MessageSource.Bot;
        if (msg.Type == MessageType.System)
            return MessageSource.System;
        return MessageSource.User;
    }

}