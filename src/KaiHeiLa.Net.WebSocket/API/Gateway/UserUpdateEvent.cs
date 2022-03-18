using System.Text.Json.Serialization;

namespace KaiHeiLa.API.Gateway;

internal class UserUpdateEvent
{
    [JsonPropertyName("user_id")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public ulong UserId { get; set; }
    
    [JsonPropertyName("username")] public string Username { get; set; }
    
    [JsonPropertyName("avatar")] public string Avatar { get; set; }
}