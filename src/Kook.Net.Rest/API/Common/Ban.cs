using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API;

internal class Ban
{
    [JsonPropertyName("user_id")]
    public required ulong UserId { get; set; }

    [JsonPropertyName("created_time")]
    [JsonConverter(typeof(DateTimeOffsetUnixTimeMillisecondsConverter))]
    public required DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("remark")]
    public required string Reason { get; set; }

    [JsonPropertyName("user")]
    public required User User { get; set; }
}
