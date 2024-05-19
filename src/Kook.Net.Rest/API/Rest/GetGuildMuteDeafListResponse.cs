using System.Text.Json.Serialization;
using Kook.Net.Converters;

namespace Kook.API.Rest;

internal class GetGuildMuteDeafListResponse
{
    [JsonPropertyName("mic")]
    public required MuteOrDeafDetail Muted { get; set; }

    [JsonPropertyName("headset")]
    public required MuteOrDeafDetail Deafened { get; set; }
}

internal class MuteOrDeafDetail
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(MuteOrDeafTypeConverter))]
    public MuteOrDeafType Type { get; set; }

    [JsonPropertyName("user_ids")]
    public required ulong[] UserIds { get; set; }
}
