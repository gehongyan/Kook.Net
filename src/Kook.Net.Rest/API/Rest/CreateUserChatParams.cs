using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class CreateUserChatParams
{
    [JsonPropertyName("target_id")]
    public ulong UserId { get; set; }

    public static implicit operator CreateUserChatParams(ulong userId) => new() {UserId = userId};
}