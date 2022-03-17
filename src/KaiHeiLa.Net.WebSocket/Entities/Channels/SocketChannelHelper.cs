namespace KaiHeiLa.WebSocket;

internal static class SocketChannelHelper
{
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
}