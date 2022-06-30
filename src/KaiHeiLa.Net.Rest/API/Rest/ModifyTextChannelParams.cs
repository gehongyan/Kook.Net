using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Rest;

internal class ModifyTextChannelParams : ModifyGuildChannelParams
{
    [JsonPropertyName("topic")]
    public string Topic { get; set; }
    [JsonPropertyName("slow_mode")]   
    public int? SlowModeInterval { get; set; }
}