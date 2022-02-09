using System.Collections.Concurrent;

namespace KaiHeiLa.WebSocket;

internal class MessageCache
{
    private readonly ConcurrentDictionary<Guid, SocketMessage> _messages;
    private readonly ConcurrentQueue<Guid> _orderedMessages;
    private readonly int _size;

    public IReadOnlyCollection<SocketMessage> Messages => _messages.ToReadOnlyCollection();

    public MessageCache(KaiHeiLaSocketClient kaiHeiLa)
    {
        _size = kaiHeiLa.MessageCacheSize;
        _messages = new ConcurrentDictionary<Guid, SocketMessage>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(_size * 1.05));
        _orderedMessages = new ConcurrentQueue<Guid>();
    }

    public void Add(SocketMessage message)
    {
        if (_messages.TryAdd(message.Id, message))
        {
            _orderedMessages.Enqueue(message.Id);

            while (_orderedMessages.Count > _size && _orderedMessages.TryDequeue(out Guid msgId))
                _messages.TryRemove(msgId, out _);
        }
    }

    public SocketMessage Remove(Guid id)
    {
        _messages.TryRemove(id, out SocketMessage msg);
        return msg;
    }

    public SocketMessage Get(Guid id)
    {
        if (_messages.TryGetValue(id, out SocketMessage result))
            return result;
        return null;
    }
}