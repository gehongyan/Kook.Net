using System.Text.Json;

namespace Kook.Net.Queue.InMemory;

internal readonly record struct QueueItem(int Sequence, JsonElement Payload);
