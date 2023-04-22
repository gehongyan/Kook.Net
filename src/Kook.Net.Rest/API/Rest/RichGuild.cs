using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class RichGuild : ExtendedGuild
{
    [JsonPropertyName("emojis")]
    public Emoji[] Emojis { get; set; }

    [JsonPropertyName("banner")]
    public string Banner { get; set; }
}
