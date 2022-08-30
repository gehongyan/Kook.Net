using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class RichGuild : ExtendedGuild
{
    [JsonPropertyName("user_id")]
    public new ulong OwnerId { get; set; }
    
    [JsonPropertyName("emojis")] 
    public Emoji[] Emojis { get; set; }
}