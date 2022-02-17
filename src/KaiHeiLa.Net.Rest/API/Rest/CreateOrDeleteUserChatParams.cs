using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class CreateOrDeleteUserChatParams
{
    [JsonPropertyName("target_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong UserId { get; set; }

    public static implicit operator CreateOrDeleteUserChatParams(ulong userId) => new() {UserId = userId};
}