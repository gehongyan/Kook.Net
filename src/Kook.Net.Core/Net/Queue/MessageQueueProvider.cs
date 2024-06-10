namespace Kook.Net.Queue;

/// <summary>
///     Represents a delegate that provides a new <see cref="IMessageQueue"/> instance.
/// </summary>
public delegate IMessageQueue MessageQueueProvider();
