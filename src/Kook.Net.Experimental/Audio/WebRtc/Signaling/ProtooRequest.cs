using System.Text.Json;

namespace Kook.Audio.WebRtc.Signaling;

internal sealed class ProtooRequest(long id, string method, JsonElement? data) : ProtooMessage
{
    public long Id { get; } = id;

    public string Method { get; } = method;

    public JsonElement? Data { get; } = data;
}
