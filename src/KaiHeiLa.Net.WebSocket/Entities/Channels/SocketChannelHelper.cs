namespace KaiHeiLa.WebSocket;

internal static class SocketChannelHelper
{
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
}