using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ModifyTextChannelParams : ModifyGuildChannelParams
{
    [JsonPropertyName("topic")]
    public string? Topic { get; set; }

    [JsonPropertyName("slow_mode")]
    public int? SlowModeInterval { get; set; }
}
