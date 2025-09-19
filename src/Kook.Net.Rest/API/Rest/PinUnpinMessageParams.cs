using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class PinUnpinMessageParams
{
    [JsonPropertyName("target_id")]
    public required ulong TargetId { get; set; }

    [JsonPropertyName("msg_id")]
    public required Guid MessageId { get; set; }
}
