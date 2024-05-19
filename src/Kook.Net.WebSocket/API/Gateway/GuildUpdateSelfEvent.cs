using System.Text.Json.Serialization;

namespace Kook.API.Gateway;

internal class GuildUpdateSelfEvent
{
    [JsonPropertyName("id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("my_nickname")]
    public required string CurrentUserNickname { get; set; }
}
