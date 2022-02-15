using KaiHeiLa.Rest;
using Model = KaiHeiLa.API.Gateway.GatewayMessageExtraData;

namespace KaiHeiLa.WebSocket;

internal static class SocketMessageHelper
{
    public static MessageSource GetSource(Model msg)
    {
        if (msg.Author.Bot ?? false)
            return MessageSource.Bot;
        if (msg.Type == MessageType.System)
            return MessageSource.System;
        return MessageSource.User;
    }

}