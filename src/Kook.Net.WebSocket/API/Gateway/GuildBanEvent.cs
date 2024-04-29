using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class GuildBanEvent
{
    [JsonPropertyName("operator_id")]
    public ulong OperatorUserId { get; set; }

    [JsonPropertyName("remark")]
    public string? Reason { get; set; }

    [JsonPropertyName("user_id")]
    public required ulong[] UserIds { get; set; }
}
