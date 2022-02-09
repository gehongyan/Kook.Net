namespace KaiHeiLa.WebSocket;

internal static class SocketChannelHelper
{
    public static void AddMessage(ISocketMessageChannel channel, KaiHeiLaSocketClient discord,
        SocketMessage msg)
    {
        switch (channel)
        {
            case SocketTextChannel textChannel: textChannel.AddMessage(msg); break;
            default: throw new NotSupportedException($"Unexpected {nameof(ISocketMessageChannel)} type.");
        }
    }    
}