using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ModifyThreadChannelParams : ModifyGuildChannelParams
{
    [JsonPropertyName("topic")]
    public string? Topic { get; set; }

    [JsonPropertyName("slow_mode")]
    public int? SlowModeInternal { get; set; }

    [JsonPropertyName("slow_mode_reply")]
    public int? SlowModeReply { get; set; }
}
