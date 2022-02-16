using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class DeleteDMChannelParams
{
    [JsonPropertyName("target_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong UserId { get; set; }

    public static implicit operator DeleteDMChannelParams(ulong userId) => new() {UserId = userId};
}