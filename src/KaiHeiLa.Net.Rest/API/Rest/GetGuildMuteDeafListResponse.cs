using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API.Rest;

internal class GetGuildMuteDeafListResponse
{
    [JsonPropertyName("mic")]
    public MuteOrDeafDetail Mute { get; set; }
    
    [JsonPropertyName("headset")]
    public MuteOrDeafDetail Deaf { get; set; }
}

internal class MuteOrDeafDetail
{
    [JsonPropertyName("type")]
    public MuteOrDeafType Type { get; set; }

    [JsonPropertyName("user_ids")] 
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong[] UserIds { get; set; }
}