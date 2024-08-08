using System.Text.Json;

namespace Kook.Net.Queue;

/// <summary>
///     表示一个提供新的 <see cref="IMessageQueue"/> 实例的委托。
/// </summary>
public delegate BaseMessageQueue MessageQueueProvider(Func<JsonElement, Task> eventHandler);
