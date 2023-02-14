using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class UserUpdateEvent
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("username")] public string Username { get; set; }

    [JsonPropertyName("avatar")] public string Avatar { get; set; }
}
