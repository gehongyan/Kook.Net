using System.Text.Json;

namespace Kook.Net.Queue;

/// <summary>
///     Represents a delegate that provides a new <see cref="IMessageQueue"/> instance.
/// </summary>
public delegate BaseMessageQueue MessageQueueProvider(Func<JsonElement, Task> eventHandler);
