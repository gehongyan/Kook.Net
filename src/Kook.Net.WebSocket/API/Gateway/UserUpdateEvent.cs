using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class UserUpdateEvent
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("username")]
    public required string Username { get; set; }

    [JsonPropertyName("avatar")]
    public required string Avatar { get; set; }
}
