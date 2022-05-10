using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Rest;

internal class ModifyVoiceChannelParams : ModifyGuildChannelParams
{
    [JsonPropertyName("voice_quality")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VoiceQuality? VoiceQuality { get; set; }

    [JsonPropertyName("limit_amount")] 
    public int? UserLimit { get; set; }
}
