using System.Text.Json;

namespace Kook.Audio.WebRtc.Signaling;

internal sealed class ProtooResponse(long id, bool ok, JsonElement? data, int? errorCode, string? errorReason) : ProtooMessage
{
    public long Id { get; } = id;

    public bool Ok { get; } = ok;

    public JsonElement? Data { get; } = data;

    public int? ErrorCode { get; } = errorCode;

    public string? ErrorReason { get; } = errorReason;
}
