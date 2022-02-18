using System.Text.Json.Serialization;
using KaiHeiLa.Net.Converters;

namespace KaiHeiLa.API;

internal class Ban
{
    [JsonPropertyName("user_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong Id { get; set; }

    [JsonPropertyName("created_time")]
    [JsonConverter(typeof(DateTimeOffsetConverter))]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("remark")]
    public string Reason { get; set; }

    [JsonPropertyName("user")]
    public User User { get; set; }
}