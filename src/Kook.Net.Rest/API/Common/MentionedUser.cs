using System.Text.Json.Serialization;

namespace Kook.API;

internal class MentionedUser
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("username")]
    public required string Username { get; set; }

    [JsonPropertyName("full_name")]
    public required string FullName { get; set; }

    [JsonPropertyName("avatar")]
    public required string Avatar { get; set; }
}
