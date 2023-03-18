using System.Text.Json.Serialization;

namespace Kook.API;

internal class MentionedUser
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("full_name")]
    public string FullName { get; set; }

    [JsonPropertyName("avatar")]
    public string Avatar { get; set; }
}
