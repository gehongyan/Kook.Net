using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Rest;

internal class CreateUserChatParams
{
    [JsonPropertyName("target_id")]
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public ulong UserId { get; set; }

    public static implicit operator CreateUserChatParams(ulong userId) => new() {UserId = userId};
}