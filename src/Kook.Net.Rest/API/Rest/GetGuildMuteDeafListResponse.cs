using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class GetGuildMuteDeafListResponse
{
    [JsonPropertyName("mic")]
    public MuteOrDeafDetail Muted { get; set; }
    
    [JsonPropertyName("headset")]
    public MuteOrDeafDetail Deafened { get; set; }
}

internal class MuteOrDeafDetail
{
    [JsonPropertyName("type")]
    public MuteOrDeafType Type { get; set; }

    [JsonPropertyName("user_ids")] 
    public ulong[] UserIds { get; set; }
}