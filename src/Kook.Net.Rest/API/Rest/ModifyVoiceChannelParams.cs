using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class ModifyVoiceChannelParams : ModifyGuildChannelParams
{
    [JsonPropertyName("voice_quality")]
    [JsonConverter(typeof(NullableVoiceQualityConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VoiceQuality? VoiceQuality { get; set; }

    [JsonPropertyName("limit_amount")] 
    public int? UserLimit { get; set; }
}
