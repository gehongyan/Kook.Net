using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Kook.WebSocket;

internal class MessageCache
{
    private readonly ConcurrentDictionary<Guid, SocketMessage> _messages;
    private readonly ConcurrentQueue<(Guid MsgId, DateTimeOffset Timestamp)> _orderedMessages;
    private readonly int _size;

    public IReadOnlyCollection<SocketMessage> Messages => _messages.ToReadOnlyCollection();

    public MessageCache(KookSocketClient kook)
    {
        _size = kook.MessageCacheSize;
        _messages = new ConcurrentDictionary<Guid, SocketMessage>(ConcurrentHashSet.DefaultConcurrencyLevel, (int)(_size * 1.05));
        _orderedMessages = new ConcurrentQueue<(Guid MsgId, DateTimeOffset Timestamp)>();
    }

    public void Add(SocketMessage message)
    {
        if (!_messages.TryAdd(message.Id, message))
            return;
        _orderedMessages.Enqueue((message.Id, message.Timestamp));
        while (_orderedMessages.Count > _size
               && _orderedMessages.TryDequeue(out (Guid MsgId, DateTimeOffset Timestamp) msg))
            _messages.TryRemove(msg.MsgId, out _);
    }

    public SocketMessage? Remove(Guid id) =>
        _messages.TryRemove(id, out SocketMessage? msg) ? msg : null;

    public SocketMessage? Get(Guid id) =>
        _messages.TryGetValue(id, out SocketMessage? result) ? result : null;

    /// <exception cref="ArgumentOutOfRangeException"><paramref name="limit"/> is less than 0.</exception>
    public IReadOnlyCollection<SocketMessage> GetMany(Guid? referenceMessageId,
        Direction dir, int limit = KookConfig.MaxMessagesPerBatch)
    {
        if (limit < 0)
            throw new ArgumentOutOfRangeException(nameof(limit));
        if (limit == 0)
            return [];

        SocketMessage referenceMessage = _messages.SingleOrDefault(x => x.Key == referenceMessageId).Value;
        IEnumerable<Guid> cachedMessageIds;
        if (referenceMessageId == null || dir == Direction.Unspecified)
            cachedMessageIds = _orderedMessages.Select(x => x.MsgId);
        else if (dir == Direction.Before)
            cachedMessageIds = _orderedMessages.Where(x => x.Timestamp < referenceMessage.Timestamp).Select(x => x.MsgId);
        else if (dir == Direction.After)
            cachedMessageIds = _orderedMessages.Where(x => x.Timestamp > referenceMessage.Timestamp).Select(x => x.MsgId);
        else //Direction.Around
        {
            if (!_messages.TryGetValue(referenceMessageId.Value, out SocketMessage? msg))
                return [];
            int around = limit / 2;
            IReadOnlyCollection<SocketMessage> before = GetMany(referenceMessageId, Direction.Before, around);
            IEnumerable<SocketMessage> after = GetMany(referenceMessageId, Direction.After, around).Reverse();
            return [..after, msg, ..before];
        }

        if (dir == Direction.Before)
            cachedMessageIds = cachedMessageIds.Reverse();

        //Only happens if referenceMessageId is null, should only get "around" and itself (+1)
        if (dir == Direction.Around)
            limit = limit / 2 + 1;

        return cachedMessageIds
            .Select(x => _messages.TryGetValue(x, out SocketMessage? msg) ? msg : null)
            .OfType<SocketMessage>()
            .Take(limit)
            .ToImmutableArray();
    }
}
