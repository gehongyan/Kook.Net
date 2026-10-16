using System.Text.Json;

namespace Kook.Audio.WebRtc.Signaling;

internal sealed class ProtooNotification(string method, JsonElement? data) : ProtooMessage
{
    public string Method { get; } = method;

    public JsonElement? Data { get; } = data;
}
