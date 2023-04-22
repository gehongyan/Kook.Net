using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class RichGuild : ExtendedGuild
{
    [JsonPropertyName("emojis")]
    public Emoji[] Emojis { get; set; }

    [JsonPropertyName("banner")]
    public string Banner { get; set; }

    [JsonPropertyName("my_nickname")]
    public string CurrentUserNickname { get; set; }

    [JsonPropertyName("my_roles")]
    public uint[] CurrentUserRoles { get; set; }
}
