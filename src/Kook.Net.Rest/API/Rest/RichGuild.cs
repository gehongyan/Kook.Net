using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class RichGuild : ExtendedGuild
{
    [JsonPropertyName("emojis")]
    public required Emoji[] Emojis { get; set; }

    [JsonPropertyName("banner")]
    public required string Banner { get; set; }

    [JsonPropertyName("my_nickname")]
    public required string CurrentUserNickname { get; set; }

    [JsonPropertyName("my_roles")]
    public required uint[] CurrentUserRoles { get; set; }
}
