using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class ModifyVoiceChannelParams : ModifyTextChannelParams
{
    [JsonPropertyName("voice_quality")]
    [JsonConverter(typeof(NullableVoiceQualityConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VoiceQuality? VoiceQuality { get; set; }

    [JsonPropertyName("limit_amount")]
    public int? UserLimit { get; set; }

    [JsonPropertyName("password")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Password { get; set; }

    // 为 0 时表示同步，因此命名为 OverwriteVoiceRegion
    [JsonPropertyName("sync_guild_region")]
    [JsonConverter(typeof(NumberBooleanWriteNumberConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? OverwriteVoiceRegion { get; set; }

    [JsonPropertyName("region")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? VoiceRegion { get; set; }
}
