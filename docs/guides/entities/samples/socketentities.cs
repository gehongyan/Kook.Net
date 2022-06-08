public string GetChannelTopic(ulong id)
{
    var channel = _client.GetChannel(8708013346475345) as SocketTextChannel;
    return channel?.Topic;
}

public SocketGuildUser GetGuildOwner(SocketChannel channel)
{
    var guild = (channel as SocketGuildChannel)?.Guild;
    return guild?.Owner;
}